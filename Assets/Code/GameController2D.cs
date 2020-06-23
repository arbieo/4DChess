using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DigitalRubyShared;

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

	Vector2 panAmount = new Vector2(0, 0);
	float scaleAmount = 1;

	Point4 selectedTile = Point4.NONE;
	Point4 destinationTile = Point4.NONE;
	Point4 attackedTile = Point4.NONE;

	private PanGestureRecognizer panGesture;
	private ScaleGestureRecognizer scaleGesture;
	private TapGestureRecognizer tapGesture;

	ChessboardAttackerHelper attackers;

	[HideInInspector]
	public ChessBoard board;

	ChessboardController2D[] chessboardControllers;

	public ChessPiece.Team playerColor = ChessPiece.Team.WHITE;

	public GameEndPopup endPopup;

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
		
		CreatePanGesture();
		CreateScaleGesture();
		CreateTapGesture();
	}

	private void PanGestureCallback(GestureRecognizer gesture)
	{
		if (gesture.State == GestureRecognizerState.Executing && !intro && !shifting)
		{
			float deltaX = panGesture.DeltaX / (Screen.dpi / 8);
			float deltaY = panGesture.DeltaY / (Screen.dpi / 8);
			panAmount.x -= deltaX / scaleAmount;
			panAmount.x = Mathf.Clamp(panAmount.x, -4*scaleAmount, 4*scaleAmount);
			panAmount.y -= deltaY / scaleAmount;
			panAmount.y = Mathf.Clamp(panAmount.y, -4*scaleAmount, 4*scaleAmount);
		}
	}

	private void CreatePanGesture()
	{
		panGesture = new PanGestureRecognizer();
		panGesture.MinimumNumberOfTouchesToTrack = 1;
		panGesture.StateUpdated += PanGestureCallback;
		FingersScript.Instance.AddGesture(panGesture);
	}

	private void ScaleGestureCallback(GestureRecognizer gesture)
	{
		if (gesture.State == GestureRecognizerState.Executing && !intro && !shifting)
		{
			//DebugText("Scaled: {0}, Focus: {1}, {2}", scaleGesture.ScaleMultiplier, scaleGesture.FocusX, scaleGesture.FocusY);
			scaleAmount *= scaleGesture.ScaleMultiplier;
			scaleAmount = Mathf.Clamp(scaleAmount, 1, 1.75f);
		}
	}

	private void CreateScaleGesture()
	{
		scaleGesture = new ScaleGestureRecognizer();
		scaleGesture.StateUpdated += ScaleGestureCallback;
		FingersScript.Instance.AddGesture(scaleGesture);
	}

	private void TapGestureCallback(GestureRecognizer gesture)
	{
		if (gesture.State == GestureRecognizerState.Ended && !intro && !shifting)
		{
			float touchX = gesture.FocusX;
			float touchY = gesture.FocusY;
			Vector2 touchPosition = new Vector2(touchX, touchY);
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(touchPosition);
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
	}

	private void CreateTapGesture()
	{
		tapGesture = new TapGestureRecognizer();
		tapGesture.StateUpdated += TapGestureCallback;
		//tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
		FingersScript.Instance.AddGesture(tapGesture);
	}

	public void Restart()
	{
		chessboardControllers = Transform.FindObjectsOfType<ChessboardController2D>();

		board = ChessBoard.ClassicVariantBoard();
		foreach (ChessboardController2D chessboardController in chessboardControllers)
		{
			chessboardController.Initialize(board);
		}

		attackers = new ChessboardAttackerHelper(board);
		attackers.ComputeAttackers();

		selectedTile = Point4.NONE;
		destinationTile = Point4.NONE;
		attackedTile = Point4.NONE;
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
			else if (board.GetPiece(selectedTile) != null && board.GetPiece(selectedTile).IsValidMove(tile))
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

	bool GameIsOver()
	{
		if (board.moveHistory.Count > 0 && board.moveHistory[board.moveHistory.Count - 1].pieceCaptured != null 
			&& board.moveHistory[board.moveHistory.Count - 1].pieceCaptured.type == ChessPiece.Type.KING)
		{
			return true;
		}
		return false;
	}

	ChessPiece.Team GetWinner()
	{
		if (board.moveHistory.Count > 0 && board.moveHistory[board.moveHistory.Count - 1].pieceCaptured.type == ChessPiece.Type.KING 
			&& board.moveHistory[board.moveHistory.Count - 1].pieceCaptured.team == ChessPiece.Team.WHITE)
		{
			return ChessPiece.Team.BLACK;
		}
		return ChessPiece.Team.WHITE;
	}

	void ExecuteMove(Point4 startTile, Point4 endTile)
	{
		
		board.MovePiece(board.GetMove(startTile, endTile));

		if (GameIsOver())
		{
			endPopup.Show(GetWinner(), playerColor);
		}

		attackers.ComputeAttackers();

		UpdateTileHighlights();
		
		if (board.currentMove != playerColor && !GameIsOver())
		{
			StartCoroutine(RunAI());
		}
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
					chessboardController.GetTile(attacker.currentPosition).Attacked();
				}
			}
			else if (selectedTile != Point4.NONE && destinationTile == Point4.NONE)
			{
				chessboardController.GetTile(selectedTile).Select();
				if (board.GetPiece(selectedTile) != null)
				{
					foreach (Point4 move in board.GetPiece(selectedTile).GetValidMoves())
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

			if (board.moveHistory.Count > 0)
			{
				Point4 startPoint = board.moveHistory[board.moveHistory.Count-1].startPosition;
				chessboardController.GetTile(startPoint).Moved();
				Point4 endPoint = board.moveHistory[board.moveHistory.Count-1].endPosition;
				chessboardController.GetTile(endPoint).Moved();
			}

			foreach (ChessPiece piece in board.pieces)
			{
				if (piece != null && piece.type == ChessPiece.Type.KING)
				{
					foreach (ChessPiece attackingPiece in attackers.GetAttackers(piece.currentPosition))
					{
						if (piece.team != attackingPiece.team)
						{
							chessboardController.GetTile(attackingPiece.currentPosition).Check();
							chessboardController.GetTile(piece.currentPosition).Check();
						}
					}
				}
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

	public void TryUndo()
	{
		//because fuck buttons
		Undo();
	}

	public bool Undo()
	{
		if (board.currentMove == playerColor)
		{
			if (board.moveHistory.Count < 2)
			{
				return false;
			}
			board.Undo();
			board.Undo();
		}
		else
		{
			if (board.moveHistory.Count < 1)
			{
				return false;
			}
			board.Undo();
		}
		attackers.ComputeAttackers();
		UpdateTileHighlights();
		endPopup.Hide();
		return true;
	}

	// Update is called once per frame
	void Update () 
	{

		if (Input.GetKey(KeyCode.Mouse1) && !EventSystem.current.IsPointerOverGameObject())
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
		}*/

		/*if (Input.GetKeyDown(KeyCode.Space))
		{
			StartCoroutine(RunAI());
		}

		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			Undo();
		}*/
	}

	IEnumerator RunAI()
	{
		ChessAI AI = new ChessAI(board);
		float coroutineStepStartTime = Time.realtimeSinceStartup;
		while(!AI.isStepResultReady)
		{
			AI.RunAIStep();
			if(Time.realtimeSinceStartup - coroutineStepStartTime > Time.fixedDeltaTime)
			{
				yield return new WaitForFixedUpdate();
				coroutineStepStartTime = Time.realtimeSinceStartup;
			}
		}
		ChessBoard.Move AIMove = AI.ConsumeAIResult();

		board.MovePiece(board.GetMove(AIMove.startPosition, AIMove.endPosition));
		attackers.ComputeAttackers();
		UpdateTileHighlights();

		if (GameIsOver())
		{
			endPopup.Show(GetWinner(), playerColor);
		}
	}

	void FixedUpdate ()
	{
		Quaternion cameraRotation = Quaternion.LookRotation(ViewVectorToView(viewVector));
		float distance = -30;
		float currentScaleAmount = scaleAmount;
		Vector2 currentPanAmount = panAmount;

		if (intro)
		{
			float shiftPercentage = (Time.time - introStartTime) / introTime;
			float vAngle = introVRotation - introVRotation * shiftPercentage;
			float hAngle = introHRotation - introHRotation * shiftPercentage;
			if (shiftPercentage >= 1)
			{
				shiftPercentage = 1;
				intro = false;

				if (board.currentMove != playerColor)
				{
					StartCoroutine(RunAI());
				}
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
				panAmount = new Vector2(0,0);
				scaleAmount = 1;
			}

			cameraRotation = Quaternion.Lerp(Quaternion.LookRotation(ViewVectorToView(oldViewVector)), Quaternion.LookRotation(ViewVectorToView(viewVector)), shiftPercentage);
			distance = -30 - Mathf.Sin(shiftPercentage*Mathf.PI) * 20;

			currentPanAmount = currentPanAmount * (1-shiftPercentage);
			currentScaleAmount = currentScaleAmount * (1-shiftPercentage) + shiftPercentage;
		}

		distance = distance / currentScaleAmount;

		Camera.main.transform.rotation = cameraRotation;
		Camera.main.transform.position = Camera.main.transform.forward * distance + Camera.main.transform.right * -5 
			+ currentPanAmount.x * Camera.main.transform.right + currentPanAmount.y * Camera.main.transform.up;
	}
}
