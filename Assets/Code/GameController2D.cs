using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController2D : MonoBehaviour {

	public Vector3 oldViewVector = Vector3.forward;
	public Vector3 viewVector = Vector3.forward;

	float shiftStartTime = -100;
	public float timeToShift = 2;
	bool shifting = false;

	bool intro = true;
	float introStartTime;
	float introTime = 3;
	float introVRotation = 360;
	float introHRotation = 180;

	Point4 selectedTile = Point4.NONE;
	Point4 destinationTile = Point4.NONE;
	Point4 attackedTile = Point4.NONE;

	ChessboardAttackerHelper attackers;

	[HideInInspector]
	public ChessBoard board;

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

		introStartTime = Time.time;
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
			return (Vector3.up + Vector3.forward * 0.3f).normalized;
		}
		if (vector == Vector3.down)
		{
			return (Vector3.down + Vector3.forward * 0.3f).normalized;
		}
		if (vector == Vector3.right)
		{
			return (Vector3.right + Vector3.forward * 0.3f).normalized;
		}
		if (vector == Vector3.left)
		{
			return (Vector3.left + Vector3.forward * 0.3f).normalized;
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
			//Nothing selected, select
			selectedTile = tile;
		}
		else if (selectedTile != Point4.NONE && destinationTile != Point4.NONE)
		{
			//Selected piece and destination
			if (tile == destinationTile)
			{
				//Move piece
				ExecuteMove(selectedTile, tile);

				selectedTile = Point4.NONE;
				destinationTile = Point4.NONE;
			}
			else
			{
				destinationTile = Point4.NONE;
			}
		}
		else if (selectedTile != Point4.NONE)
		{
			//Selected piece
			if (board.GetPiece(selectedTile) == null || board.GetPiece(selectedTile).team != board.currentMove)
			{
				selectedTile = tile;
			}
			else if (board.GetPiece(selectedTile) != null && board.GetPiece(selectedTile).GetValidMoves().Contains(tile))
			{
				destinationTile = tile;
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
		if (selectedTile != Point4.NONE && destinationTile == Point4.NONE)
		{
			selectedTile = Point4.NONE;
		}
		else if (selectedTile != Point4.NONE && destinationTile != Point4.NONE)
		{
			destinationTile = Point4.NONE;
		}

		UpdateTileHighlights();
	}

	void ExecuteMove(Point4 startTile, Point4 endTile)
	{
		board.MovePiece(board.GetMove(startTile, endTile));

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
			else if (selectedTile != Point4.NONE && destinationTile == Point4.NONE)
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
			else if (selectedTile != Point4.NONE && destinationTile != Point4.NONE)
			{
				chessboardController.GetTile(selectedTile).Select();
				chessboardController.GetTile(destinationTile).Select();
			}
		}
	}

	public void TryRightTurn()
	{
		if (!shifting && !intro)
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
	}

	public void TryLeftTurn()
	{
		if (!shifting && !intro)
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
	}

	public void TryUpTurn()
	{
		if (!shifting && !intro)
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
	}

	public void TryDownTurn()
	{
		if (!shifting && !intro)
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

	public void Undo()
	{
		board.Undo();
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

		/*if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			TryRightTurn();
		}
		
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			TryLeftTurn();
		}
		
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			TryUpTurn();
		}
		
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			TryDownTurn();
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			ChessAI AI = new ChessAI(board);
			ChessBoard.Move AIMove = AI.DoTurn();
			board.MovePiece(board.GetMove(AIMove.startPosition, AIMove.endPosition));
			attackers.ComputeAttackers();
		}

		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			Undo();
		}*/
	}

	void FixedUpdate ()
	{
		Quaternion cameraRotation = Quaternion.LookRotation(ViewVectorToView(viewVector));
		float distance = -30;

		if (intro)
		{
			float shiftPercentage = (Time.time - introStartTime) / introTime;
			float vAngle = introVRotation - introVRotation * shiftPercentage;
			float hAngle = introHRotation - introHRotation * shiftPercentage;
			if (shiftPercentage >= 1)
			{
				shiftPercentage = 1;
				intro = false;
			}

			cameraRotation = Quaternion.AngleAxis(vAngle, Vector3.back) * Quaternion.AngleAxis(hAngle, Vector3.up) * cameraRotation;
			distance = -30 - (1- shiftPercentage) * 150;
		}
		else if (shifting)
		{
			float shiftPercentage = (Time.time - shiftStartTime) / timeToShift;
			if (shiftPercentage >= 1)
			{
				shiftPercentage = 1;
				shifting = false;
			}

			cameraRotation = Quaternion.Lerp(Quaternion.LookRotation(ViewVectorToView(oldViewVector)), Quaternion.LookRotation(ViewVectorToView(viewVector)), shiftPercentage);
			distance = -30 - Mathf.Sin(shiftPercentage*Mathf.PI) * 20;
		}

		Camera.main.transform.rotation = cameraRotation;
		Camera.main.transform.position = Camera.main.transform.forward * distance + Camera.main.transform.right * -5;
	}
}
