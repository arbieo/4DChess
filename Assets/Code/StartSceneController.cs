using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour {

	public void StartGame()
	{
		SceneManager.LoadScene("Chess2D");
	}
}
