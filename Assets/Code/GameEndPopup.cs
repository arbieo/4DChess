using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameEndPopup : MonoBehaviour {

	public Text title;
	public Text body;
	public Button undoButton;
	public Button restartButton;

	// Use this for initialization
	public void Show(ChessPiece.Team winner, ChessPiece.Team player){
		gameObject.SetActive(true);
		if (winner == player)
		{
			undoButton.gameObject.SetActive(false);
			restartButton.gameObject.SetActive(true);
		}
		else
		{
			undoButton.gameObject.SetActive(true);
			restartButton.gameObject.SetActive(false);
		}

		string winnerName = "";
		if (winner == ChessPiece.Team.WHITE)
		{
			winnerName = "White";
		}
		else
		{
			winnerName = "Black";
		}

		title.text = winnerName + " wins!";
		body.text = winnerName + " took the enemy king";
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene("StartScene");
	}
}
