using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonScript : MonoBehaviour {

	public void StartGame() {
		SceneManager.LoadScene("MainBoard", LoadSceneMode.Single);
	}

	public void GoToMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

	public void RestartGame()
	{
		SceneManager.LoadScene("MainBoard", LoadSceneMode.Single);
	}

}
