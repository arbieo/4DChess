using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidePanelController : MonoBehaviour {

	public Image whitePlayerIcon;
	public Text whitePlayerName;
	public Image blackPlayerIcon;
	public Text blackPlayerText;

	public Image blackQueenCaptures;
	public Text blackQueenCaptureAmount;
	public Image blackBishopCaptures;
	public Text blackBishopCaptureAmount;
	public Image blackKnightCaptures;
	public Text blackKnightCaptureAmount;
	public Image blackRookCaptures;
	public Text blackRookCaptureAmount;
	public Image blackPawnCaptures;
	public Text blackPawnCaptureAmount;

	public Image whiteQueenCaptures;
	public Text whiteQueenCaptureAmount;
	public Image whiteBishopCaptures;
	public Text whiteBishopCaptureAmount;
	public Image whiteKnightCaptures;
	public Text whiteKnightCaptureAmount;
	public Image whiteRookCaptures;
	public Text whiteRookCaptureAmount;
	public Image whitePawnCaptures;
	public Text whitePawnCaptureAmount;

	public Text undoCounter;

	public GameController2D gameController;

	int undoCount = 0;
	
	// Update is called once per frame
	void Update () {
		undoCounter.text = "Count: " + undoCount;

		int blackQueenCount = 0;
		int blackKnightCount = 0;
		int blackBishopCount = 0;
		int blackRookCount = 0;
		int blackPawnCount = 0;
		int whiteQueenCount = 0;
		int whiteKnightCount = 0;
		int whiteBishopCount = 0;
		int whiteRookCount = 0;
		int whitePawnCount = 0;

		ChessBoard board = gameController.board;
		foreach (ChessBoard.Move move in board.moveHistory)
		{
			if (move.pieceCaptured != null)
			{
				if(move.pieceCaptured.team == ChessPiece.Team.WHITE)
				{
					if (move.pieceCaptured.type == ChessPiece.Type.QUEEN)
					{
						whiteQueenCount ++;
					}
					if (move.pieceCaptured.type == ChessPiece.Type.BISHOP)
					{
						whiteBishopCount ++;
					}
					if (move.pieceCaptured.type == ChessPiece.Type.KNIGHT)
					{
						whiteKnightCount ++;
					}
					if (move.pieceCaptured.type == ChessPiece.Type.ROOK)
					{
						whiteRookCount ++;
					}
					if (move.pieceCaptured.type == ChessPiece.Type.PAWN)
					{
						whitePawnCount ++;
					}
				}

				if(move.pieceCaptured.team == ChessPiece.Team.BLACK)
				{
					if (move.pieceCaptured.type == ChessPiece.Type.QUEEN)
					{
						blackQueenCount ++;
					}
					if (move.pieceCaptured.type == ChessPiece.Type.BISHOP)
					{
						blackBishopCount ++;
					}
					if (move.pieceCaptured.type == ChessPiece.Type.KNIGHT)
					{
						blackKnightCount ++;
					}
					if (move.pieceCaptured.type == ChessPiece.Type.ROOK)
					{
						blackRookCount ++;
					}
					if (move.pieceCaptured.type == ChessPiece.Type.PAWN)
					{
						blackPawnCount ++;
					}
				}
			}
		}

		UpdateCaptures(blackQueenCaptures, blackQueenCaptureAmount, blackQueenCount);
		UpdateCaptures(blackBishopCaptures, blackBishopCaptureAmount, blackBishopCount);
		UpdateCaptures(blackKnightCaptures, blackKnightCaptureAmount, blackKnightCount);
		UpdateCaptures(blackRookCaptures, blackRookCaptureAmount, blackRookCount);
		UpdateCaptures(blackPawnCaptures, blackPawnCaptureAmount, blackPawnCount);

		UpdateCaptures(whiteQueenCaptures, whiteQueenCaptureAmount, whiteQueenCount);
		UpdateCaptures(whiteBishopCaptures, whiteBishopCaptureAmount, whiteBishopCount);
		UpdateCaptures(whiteKnightCaptures, whiteKnightCaptureAmount, whiteKnightCount);
		UpdateCaptures(whiteRookCaptures, whiteRookCaptureAmount, whiteRookCount);
		UpdateCaptures(whitePawnCaptures, whitePawnCaptureAmount, whitePawnCount);
		
	}

	public void UpdateCaptures(Image icon, Text captureAmount, int count)
	{
		if (count > 0)
		{
			icon.color = Color.white;
		}
		else
		{
			icon.color = new Color(1,1,1,0.25f);;
		}
		captureAmount.text = "x"+count;
	}

	public void TurnCubeLeft()
	{
		gameController.TryLeftTurn();
	}
	
	public void TurnCubeRight()
	{
		gameController.TryRightTurn();
	}
	
	public void TurnCubeUp()
	{
		gameController.TryUpTurn();
	}

	public void TurnCubeDown()
	{
		gameController.TryDownTurn();
	}

	public void Undo()
	{
		gameController.Undo();
		undoCount ++;
	}
}
