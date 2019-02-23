using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Adds all the nodes to the gameboard.
 */ 
public class AddButtons : MonoBehaviour {

    // Nodes panel
    [SerializeField]
    private Transform panel;

	// A node button prefab
    [SerializeField]
    private GameObject prefabButton;

    public static int boardWidth = 14;
    public static int boardHeight = 12;

    private void Awake()
    {
        int index = 0;
        for (int j = 0; j < boardHeight; j++) {
            for (int i = 0; i < boardWidth; i++)
            {
                GameObject currButton = Instantiate(prefabButton);
                currButton.name = index.ToString();
                currButton.GetComponent<Node>().x = i;
                currButton.GetComponent<Node>().y = j;
                currButton.GetComponent<Node>().owner = -1;
                currButton.transform.SetParent(panel, false);
                currButton.GetComponentInChildren<Canvas>().overrideSorting = true;
                index++;
            }
        }
    }

}
