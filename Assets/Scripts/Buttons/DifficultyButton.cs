using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{

    public int playerId;
    public GameObject playerType;
    public GameObject difficultyButton;

    private readonly int difficultyLevels = 4;

    void Start()
    {
        SetGameType();
        SetDifficultyText();
    }

    public void ChangePlayerType()
    {
        GameModel.IS_HUMAN = !GameModel.IS_HUMAN;

        SetGameType();
        SetDifficultyText();
    }

    void SetGameType()
    {
        if (!GameModel.IS_HUMAN)
        {
            playerType.GetComponentInChildren<Text>().text = "1 Player";
        }
        else
        {
            playerType.GetComponentInChildren<Text>().text = "2 Players";
        }
        difficultyButton.SetActive(!GameModel.IS_HUMAN);
    }


    public void ChangeDifficulty()
    {
        GameModel.AI_DIFFICULTY = (GameModel.AI_DIFFICULTY + 1) % difficultyLevels;
        SetDifficultyText();
    }

    void SetDifficultyText()
    {
        difficultyButton.SetActive(!GameModel.IS_HUMAN);

        if (GameModel.IS_HUMAN)
        {
            difficultyButton.GetComponentInChildren<Text>().text = "1v1";
        }
        else
        {
            string baseText = "Difficulty\n";
            switch (GameModel.AI_DIFFICULTY)
            {
                case 0:
                    difficultyButton.GetComponentInChildren<Text>().text = baseText + "Easy";
                    break;
                case 1:
                    difficultyButton.GetComponentInChildren<Text>().text = baseText + "Medium";
                    break;
                case 2:
                    difficultyButton.GetComponentInChildren<Text>().text = baseText + "Hard";
                    break;
                case 3:
                    difficultyButton.GetComponentInChildren<Text>().text = baseText + "Very Hard";
                    break;
            }
        }
    }

}
