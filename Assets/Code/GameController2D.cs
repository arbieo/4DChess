using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController2D : MonoBehaviour {

	public Vector3 oldViewVector = Vector3.forward;
	public Vector3 viewVector = Vector3.forward;

	float shiftStartTime = -100;
	public float timeToShift = 2;
	bool shiftXFirst = false;
	bool shifting = false;

	ChessPiece.Team currentMove = ChessPiece.Team.WHITE;
	Point4 selectedTile = Point4.NONE;
	Point4 attackedTile = Point4.NONE;

	ChessboardAttackerHelper attackers;

	ChessBoard board;

	ChessboardController2D[] chessboardControllers;

	// Use this for initialization
	void Start () {
		chessboardControllers = Transform.FindObjectsOfType<ChessboardController2D>();

		board = ChessBoard.ClassicVariantBoard();
		foreach (ChessboardController2D chessboardController in chessboardControllers)
		{
			chessboardController.Initialize(board);
		}

		attackers = new ChessboardAttackerHelper(board);
		attackers.ComputeAttackers();
	}
	
	void StartShift()
	{
		shiftStartTime = Time.time;
		shifting = true;
	}

	Vector3 ViewVectorToView(Vector3 vector)
	{
		if (vector == Vector3.up)
		{
			return (Vector3.up + Vector3.forward * 0.4f).normalized;
		}
		if (vector == Vector3.down)
		{
			return (Vector3.down + Vector3.forward * 0.4f).normalized;
		}
		if (vector == Vector3.right)
		{
			return (Vector3.right + Vector3.forward * 0.4f).normalized;
		}
		if (vector == Vector3.left)
		{
			return (Vector3.left + Vector3.forward * 0.4f).normalized;
		}
		else
		{
			return vector;
		}
	}

	void ClickTile(Point4 tile)
	{
		if (tile == selectedTile)
		{
			return;
		}

		if (selectedTile == Point4.NONE)
		{
			selectedTile = tile;
		}
		else if (selectedTile != Point4.NONE)
		{
			if (board.GetPiece(selectedTile) != null && board.GetPiece(selectedTile).team == currentMove
				&& board.GetPiece(selectedTile).GetValidMoves().Contains(tile))
			{
				ExecuteMove(selectedTile, tile);

				selectedTile = Point4.NONE;
			}
			else
			{
				selectedTile = tile;
			}
		}

		UpdateTileHighlights();
	}

	void ClickOffTile()
	{
		selectedTile = Point4.NONE;

		UpdateTileHighlights();
	}

	void ExecuteMove(Point4 startTile, Point4 endTile)
	{
		if (board.GetPiece(endTile) != null && board.GetPiece(endTile).type == ChessPiece.Type.KING)
		{
			/*if(currentMove == ChessPiece.Team.WHITE)
			{
				turnText.text = "White wins!!";
			}
			else
			{
				turnText.text = "Black wins!!";
			}
			StartCoroutine(LoadStartScene());*/
		}
		else
		{
			if(currentMove == ChessPiece.Team.WHITE)
			{
				currentMove = ChessPiece.Team.BLACK;
				//turnText.text = "Black's turn";
			}
			else
			{
				currentMove = ChessPiece.Team.WHITE;
				//turnText.text = "White's turn";
			}
		}

		board.MovePiece(board.GetPiece(startTile), endTile);

		attackers.ComputeAttackers();
	}

	void UpdateTileHighlights()
	{
		foreach (ChessboardController2D chessboardController in chessboardControllers)
		{
			foreach (Tile2D boardTile in chessboardController.tiles)
			{
				boardTile.ClearSelection();
			}

			if (attackedTile != Point4.NONE)
			{
				chessboardController.GetTile(attackedTile).Select();
				foreach(ChessPiece attacker in attackers.attackers[attackedTile.x, attackedTile.y, attackedTile.z, attackedTile.w])
				{
					chessboardController.GetTile(attacker.currentPosition).Highlight();
				}
			}
			else if (selectedTile != Point4.NONE)
			{
				chessboardController.GetTile(selectedTile).Select();
				if (board.GetPiece(selectedTile) != null)
				{
					HashSet<Point4> moves = board.GetPiece(selectedTile).GetValidMoves();
					foreach (Point4 move in moves)
					{
						chessboardController.GetTile(move).Highlight();
					}
				}
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ChessboardController2D currentController = null;
			foreach(ChessboardController2D chessboardController in chessboardControllers)
			{
				if (chessboardController.transform.forward == viewVector)
				{
					currentController = chessboardController;
				}
			}
			
			if (Physics.Raycast(ray, out hit, 1000, 1 << currentController.gameObject.layer)) {
				Transform objectHit = hit.transform;
				
				bool clickedTile = false;
				while(objectHit != null)
				{
					if (objectHit.GetComponent<Tile2D>() != null)
					{
						ClickTile(objectHit.GetComponent<Tile2D>().position);
						clickedTile = true;
						break;
					}
					else
					{
						objectHit = objectHit.parent;
					}
				}
				if (!clickedTile)
				{
					ClickOffTile();
				}
			}
			else
			{
				ClickOffTile();
			}
		}

		if (Input.GetKey(KeyCode.Mouse1))
		{
			Point4 newAttackedTile = attackedTile;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ChessboardController2D currentController = null;
			foreach(ChessboardController2D chessboardController in chessboardControllers)
			{
				if (chessboardController.transform.forward == viewVector)
				{
					currentController = chessboardController;
				}
			}

			if (Physics.Raycast(ray, out hit, 1000, 1 << currentController.gameObject.layer)) {
				Transform objectHit = hit.transform;
				
				bool clickedTile = false;
				while(objectHit != null)
				{
					if (objectHit.GetComponent<Tile2D>() != null)
					{
						newAttackedTile = objectHit.GetComponent<Tile2D>().position;
						clickedTile = true;
						break;
					}
					else
					{
						objectHit = objectHit.parent;
					}
				}
				if (!clickedTile)
				{
					newAttackedTile = Point4.NONE;
				}
			}
			else
			{
				newAttackedTile = Point4.NONE;
			}

			if (newAttackedTile != attackedTile)
			{
				attackedTile = newAttackedTile;
				UpdateTileHighlights();
			}
		}
		else
		{
			if(attackedTile != Point4.NONE)
			{
				attackedTile = Point4.NONE;
				UpdateTileHighlights();
			}
		}

		if (!shifting && Input.GetKeyDown(KeyCode.RightArrow))
		{
			oldViewVector = viewVector;
			if (viewVector == Vector3.forward)
			{
				viewVector = Vector3.left;
			}
			else if (viewVector == Vector3.left)
			{
				viewVector = Vector3.back;
			}
			else if (viewVector == Vector3.back)
			{
				viewVector = Vector3.right;
			}
			else if (viewVector == Vector3.right)
			{
				viewVector = Vector3.forward;
			}
			else if (viewVector == Vector3.up)
			{
				viewVector = Vector3.left;
			}
			else if (viewVector == Vector3.down)
			{
				viewVector = Vector3.left;
			}
			StartShift();
		}

		if (!shifting && Input.GetKeyDown(KeyCode.LeftArrow))
		{
			oldViewVector = viewVector;
			if (viewVector == Vector3.forward)
			{
				viewVector = Vector3.right;
			}
			else if (viewVector == Vector3.right)
			{
				viewVector = Vector3.back;
			}
			else if (viewVector == Vector3.back)
			{
				viewVector = Vector3.left;
			}
			else if (viewVector == Vector3.left)
			{
				viewVector = Vector3.forward;
			}
			else if (viewVector == Vector3.up)
			{
				viewVector = Vector3.right;
			}
			else if (viewVector == Vector3.down)
			{
				viewVector = Vector3.right;
			}
			StartShift();
		}

		if (!shifting && Input.GetKeyDown(KeyCode.UpArrow))
		{
			oldViewVector = viewVector;
			if (viewVector == Vector3.up)
			{
				viewVector = Vector3.forward;
			}
			else if (viewVector == Vector3.down)
			{
				viewVector = Vector3.back;
			}
			else 
			{
				viewVector = Vector3.down;
			}
			StartShift();
		}

		if (!shifting && Input.GetKeyDown(KeyCode.DownArrow))
		{
			oldViewVector = viewVector;
			if (viewVector == Vector3.up)
			{
				viewVector = Vector3.back;
			}
			else if (viewVector == Vector3.down)
			{
				viewVector = Vector3.forward;
			}
			else 
			{
				viewVector = Vector3.up;
			}
			StartShift();
		}
	}

	void FixedUpdate ()
	{
		Quaternion cameraRotation = Quaternion.LookRotation(ViewVectorToView(viewVector));
		float distance = -30;
		if (shifting)
		{
			float shiftPercentage = (Time.time - shiftStartTime) / timeToShift;
			if (shiftPercentage >= 1)
			{
				shiftPercentage = 1;
				shifting = false;
			}
			float stage1Lerp = Mathf.Clamp01(1 - (shiftPercentage-0.5f)*2);
			float stage2Lerp = Mathf.Clamp01(shiftPercentage*2);

			cameraRotation = Quaternion.Lerp(Quaternion.LookRotation(ViewVectorToView(oldViewVector)), Quaternion.LookRotation(ViewVectorToView(viewVector)), shiftPercentage);
			distance = -30 - Mathf.Sin(shiftPercentage*Mathf.PI) * 20;
		}

		Camera.main.transform.rotation = cameraRotation;
		Camera.main.transform.position = Camera.main.transform.forward * distance;

		/*Vector3 centerPoint = new Vector3(0,0,0);
		if (Input.GetKey(KeyCode.RightArrow))
		{
			Camera.main.transform.RotateAround(centerPoint, Vector3.up, Time.deltaTime * 300);
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			Camera.main.transform.RotateAround(centerPoint, Vector3.up, -Time.deltaTime * 300);
		}
		
		if (Input.GetKey(KeyCode.UpArrow))
		{
			Camera.main.transform.RotateAround(centerPoint, Vector3.right, -Time.deltaTime * 100);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			Camera.main.transform.RotateAround(centerPoint, Vector3.right, -Time.deltaTime * 100);
		}*/

		/*if (Input.GetKey(KeyCode.UpArrow))
		{
			Camera.main.transform.position += (centerPoint -Camera.main.transform.position).normalized * 60 * Time.fixedDeltaTime;
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			Camera.main.transform.position += (Camera.main.transform.position - centerPoint).normalized * 60 * Time.fixedDeltaTime;
		}*/
	}
}
