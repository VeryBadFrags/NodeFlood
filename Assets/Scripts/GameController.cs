using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	
    // To be populated in the inspector
    public Sprite[] sprites; // The 3 node sprites
    public AudioSource sound1, sound2; // Capture sounds for Player 1 and 2
    public GameObject endPopup; // The victory box
    public GameObject[] progressBars;
    public Text[] nodesCount;
    public Text[] clustersCount;
    public GameObject currentPlayerBox;

    public List<GameObject> btns = new List<GameObject>();
	

	// Game state
    private int playerScore;
    private int currPlayer = -1;
	private bool lockGame = false;

    private readonly float waitFactor = 0.08F;

    private void Awake()
    {
        //sprites = Resources.LoadAll<Sprite>("Sprites/Nodes");
    }

    // Use this for initialization
    void Start () {
        InitButtons();
		InitStartingNodes();

		GameModel.players.Clear();

        Player player1 = new Player
        {
            isHuman = true,
            team = 0,
            color = new Color(0f, 0f, 1.0f, 1.0f)
        };
        GameModel.players.Add(player1);

        Player player2 = new Player
        {
            isHuman = GameModel.IS_HUMAN,
            team = 1,
            color = new Color(1f, 0f, 0f, 1.0f),
            aiLevel = GameModel.AI_DIFFICULTY
        };
        GameModel.players.Add(player2);

		Propagate(btns[0], player1.team,0);
		Propagate(btns[btns.Count -1], player2.team,0);

		UpdateScore();

		currPlayer = player1.team;
    }

    void Update()
    {
        if (Input.GetKey("escape"))
            Application.Quit();
    }

    void InitButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");

        for(int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i]);
            btns[i].GetComponent<Button>().image.sprite = sprites[0];

			//Add click listener
			btns[i].GetComponent<Button>().onClick.AddListener(ClickNodeEvent);

			//Shuffle the nodes
			Button btn = btns[i].GetComponent<Button>();
			int randomDirection = (int) Random.Range(0f, 4f);

			btns[i].GetComponent<Node>().direction = randomDirection;
			btn.transform.Rotate(Vector3.forward * randomDirection * -90);
        }
    }

    void InitStartingNodes()
    {
        //Init starting nodes
        // Node 0
        int zeroDirection = btns[0].GetComponent<Node>().direction;
        btns[0].GetComponent<Node>().direction = 0;
        btns[0].transform.Rotate(Vector3.forward * (4- zeroDirection) * -90);

        // Last Node
        int lastDirection = btns[btns.Count -1].GetComponent<Node>().direction;
        btns[btns.Count - 1].GetComponent<Node>().direction = 2;
        btns[btns.Count - 1].transform.Rotate(Vector3.forward * (2 - lastDirection) * -90);
    }

    public void ClickNodeEvent()
    {
        GameObject clicked = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        if (currPlayer == clicked.GetComponent<Node>().owner && GameModel.players[currPlayer].isHuman) {
            ClickNode(clicked);
        }
    }

    void ClickNode(GameObject clicked)
    {
		if (!lockGame) {
			if (currPlayer == clicked.GetComponent<Node> ().owner) {
				lockGame = true;

				int nextDirection = (clicked.GetComponent<Node> ().direction + 1) % 4;
				clicked.GetComponent<Node> ().direction = nextDirection;
				clicked.GetComponent<Button> ().transform.Rotate (Vector3.forward * -90);

                int maxDepth = Propagate(clicked, currPlayer,0);

                StartCoroutine(WaitForAnimationThenContinue(clicked, maxDepth));
			}
		}
    }

    IEnumerator WaitForAnimationThenContinue(GameObject clicked, int maxDepth)
    {
        yield return new WaitForSecondsRealtime((maxDepth-1) * waitFactor);
        UpdateScore();
        CheckVictory();

        currPlayer = (currPlayer + 1) % GameModel.players.Count;

        // Update the current player box
        Text currentPlayerText = currentPlayerBox.GetComponentInChildren<Text>();
        currentPlayerText.text = "Player " + (currPlayer + 1);
        currentPlayerText.color = GameModel.players[currPlayer].color;

        lockGame = false;
        StartCoroutine(PlayAI());
    }

    void UpdateScore() {
		int[] nodesInt = new int[nodesCount.Length];
        int[] clustersInt = new int[clustersCount.Length];

        // Count nodes per player
        foreach (GameObject candidateNode in btns) {
			int currentOwner = candidateNode.GetComponent<Node> ().owner;
			if (currentOwner != -1) {
                nodesInt[currentOwner] = nodesInt[currentOwner] + 1;
			}
		}

        // Update nodes count
        for (int i = 0; i < nodesInt.Length; i++)
        {
            string nodeText = " nodes";
            if(nodesInt[i] <= 1)
            {
                nodeText = " node";
            }
            nodesCount[i].GetComponent<Text>().text = nodesInt[i].ToString() + nodeText;
        }

        // Progress bars
        for (int i = 0; i < progressBars.Length; i++)
        {
            //int parentHeight = progressBars[i].GetComponentInParent<Transform>().height;
            float percent = (float)nodesInt[i] / (float)btns.Count;
            progressBars[i].GetComponent<Slider>().value = percent;
        }

        // Clusters score
		for (int i = 0; i < GameModel.players.Count; i++)
        {
            clustersInt[i] = TilesHelper.CountClusters(i, btns);

            string clusterText = " clusters";
            if (clustersInt[i] <= 1)
            {
                clusterText = " cluster";
            }

            clustersCount[i].GetComponent<Text>().text = clustersInt[i].ToString() + clusterText;
        }

		if (GameModel.IS_HUMAN) {
			playerScore = nodesInt [0] * clustersInt [0];
		} else {
            // Score vs. AI depends on the difficulty
			playerScore = nodesInt [0] * clustersInt [0] * (GameModel.AI_DIFFICULTY + 1);
		}
	}
 
    void CheckVictory()
    {
        // Check that every node is either neutral or belongs to the current player.
        foreach(GameObject obj in btns)
        {
            int currentOwner = obj.GetComponent<Node>().owner;
            if (currentOwner >= 0 && currentOwner != currPlayer)
            {
                return;
            }
        }

		if (GameModel.IS_HUMAN) {
			endPopup.GetComponentsInChildren<Text> () [1].text = "Player " + (currPlayer + 1) + " wins!";
		} else {
			if (currPlayer == 0) {
				endPopup.GetComponentsInChildren<Text>()[1].text = "You win!\nYou have eliminated the virus threat\nScore: " + playerScore;
			} else {
				endPopup.GetComponentsInChildren<Text>()[1].text = "The virus infection has spread\nThe data center is lost\nYou are fired :(";
			}
		}
        endPopup.SetActive(true);
    }

    IEnumerator PlayAI()
    {
		if(!GameModel.players[currPlayer].isHuman)
        {
			GameObject toPlay = GameModel.players[currPlayer].PlaySomething(btns);
            if (toPlay != null) {
                yield return new WaitForSeconds(0.9f);
                ClickNode(toPlay);
            }
        }
    }

	int Propagate(GameObject source, int newOwner, int PreviousConverted)
    {
		int maxDepth = 1;

		//Rotate node
        source.GetComponent<Node>().ChangeOwner(newOwner);

        StartCoroutine(WaitForPlay(source, newOwner, PreviousConverted));

        // Update the target node
        int targetDirection = source.GetComponent<Node>().direction;
        GameObject targetNeighbor = TilesHelper.GetNeighbor(source, targetDirection, btns);
        if(targetNeighbor != null)
        {
            if(targetNeighbor.GetComponent<Node>().owner != newOwner)
            {
                maxDepth = Mathf.Max(maxDepth+1, Propagate(targetNeighbor, newOwner, PreviousConverted + 1));
            }
        }

        // Update neighbors pointing to currentNode
        for(int neighDirection = 0; neighDirection < 4; neighDirection++)
        {
            GameObject neighbor = TilesHelper.GetNeighbor(source, neighDirection, btns);
            if (neighbor != null)
            {
                if (neighbor.GetComponent<Node>().owner != newOwner) { 
                    if (neighbor.GetComponent<Node>().direction == (neighDirection + 2) % 4)
                    {
                        maxDepth = Mathf.Max(maxDepth+1, Propagate(neighbor, newOwner, PreviousConverted + 1));
                    }
                }
            }
        }

		return Mathf.Max(maxDepth, PreviousConverted+1);
    }

    IEnumerator WaitForPlay(GameObject source, int newOwner, int PreviousConverted)
    {
        yield return new WaitForSecondsRealtime(PreviousConverted* waitFactor);
        source.GetComponent<Button>().image.sprite = sprites[newOwner+1];
        // Set arrow color
        //Color playerColorAlpha = new Color(GameModel.players[newOwner].color.r, GameModel.players[newOwner].color.g, GameModel.players[newOwner].color.b*2, 0.5f);
        //source.GetComponentsInChildren<Image>()[1].color = playerColorAlpha;

        float randomPitch = Random.Range(0.8f, 1.2f);
        AudioSource sound;
        if (newOwner == 1)
        {
            sound = sound1;
        } else
        {
            sound = sound2;
        }
        sound.pitch = randomPitch;
        sound.Play();

        // Show particles
        ParticleSystem.MainModule particlesSettings = source.GetComponent<ParticleSystem> ().main;
        //Color playerColor = GameModel.players[newOwner].color;
        //Color particleColor = new Color(playerColor.r, playerColor.g, playerColor.b);
        //particleColor.a = 0.5F;
        //particlesSettings.startColor = new ParticleSystem.MinMaxGradient(particleColor);
		source.GetComponent<ParticleSystem>().Play();
    }

}
