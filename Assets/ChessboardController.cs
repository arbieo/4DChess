using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChessboardController : MonoBehaviour {

	public const int BOARD_SIZE = 4;

	public Tile[,,,] chessboard = new Tile[BOARD_SIZE,BOARD_SIZE,BOARD_SIZE,BOARD_SIZE];

	public List<ChessPiece>[,,,] attackers = new List<ChessPiece>[BOARD_SIZE,BOARD_SIZE,BOARD_SIZE,BOARD_SIZE];

	public static ChessboardController instance;

	public int cardinalityX = 0;
	public int cardinalityY = 1;
	public int cardinalityZ = 2;
	public int cardinalityW = 3;

	public bool flat = false;
	
	int oldCardinalityX = 0;
	int oldCardinalityY = 1;
	int oldCardinalityZ = 2;
	int oldCardinalityW = 3;

	float shiftStartTime = -100;
	public float timeToShift = 2;
	bool shiftXFirst = false;
	bool shifting = false;

	public enum SHIFT_STATE
	{
		LEFT,
		MID,
		RIGHT,
		LEFT_ROTATED,
		MID_ROTATED,
		RIGHT_ROTATED
	}

	public enum SHIFT_STATE_COORD
	{
		XY,
		XZ,
		YZ,
		XW,
		YW,
		WZ
	}

	public SHIFT_STATE shiftState = SHIFT_STATE.MID;
	public int verticalCardinalityDirection = 1;

	public Text turnText;
	public Text warningText;

	public Button upButton;
	public Button rightButton;
	public Button leftButton;
	public Button downButton;

	Vector3 gapVector = Vector3.forward * 5;
	Vector3 oldGapVector = Vector3.forward * 5;
	Vector3 tileFacing = Vector3.up;
	Vector3 oldTileFacing = Vector3.up;

	Vector3 targetCameraPosition;
	Quaternion targetCameraRotation;
	Vector3 oldCameraPosition;
	Quaternion oldCameraRotation;


	public Vector3 flatCameraPosition;
	public Quaternion flatCameraRotation;

	public Vector3 rightShiftCameraPosition;
	public Quaternion rightShiftCameraRotation;

	public Vector3 midShiftCameraPosition;
	public Quaternion midShiftCameraRotation;

	public Vector3 leftShiftCameraPosition;
	public Quaternion leftShiftCameraRotation;

	public Vector3 rightShiftRotatedCameraPosition;
	public Quaternion rightShiftRotatedCameraRotation;

	public Vector3 midShiftRotatedCameraPosition;
	public Quaternion midShiftRotatedCameraRotation;

	public Vector3 leftShiftRotatedCameraPosition;
	public Quaternion leftShiftRotatedCameraRotation;

	bool isRotated = false;
	
	public GameObject tilePrefab;
	public GameObject tileContainer;

	ChessPiece.Team currentMove = ChessPiece.Team.WHITE;
	Tile selectedTile;
	Tile destinationTile;
	Tile attackedTile;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {
		InitializeBoard();

		EnterMidShift();
		//Camera.main.transform.position = targetCameraPosition;
		//Camera.main.transform.rotation = targetCameraRotation;
		CopyOldValues();
		oldCameraPosition = Camera.main.transform.position;
		oldCameraRotation = Camera.main.transform.rotation;
		StartShift();
	}

	void AddChessPiece(int x, int y, int z, int w, ChessPiece.Type type, ChessPiece.Team team)
	{
		ChessPiece newPiece = new ChessPiece(type, team, x, y, z, w);
		chessboard[x,y,z,w].SetPiece(newPiece);
	}

	void ComputeAttackers()
	{
		for(int x = 0; x<BOARD_SIZE; x++)
		{
			for(int y = 0; y<BOARD_SIZE; y++)
			{
				for(int z = 0; z<BOARD_SIZE; z++)
				{
					for(int w = 0; w<BOARD_SIZE; w++)
					{
						attackers[x,y,z,w] = new List<ChessPiece>();
					}
				}
			}
		}

		foreach (Tile tile in chessboard)
		{
			if (tile.currentPiece != null)
			{
				foreach (Vector4 attackedTile in tile.currentPiece.GetValidMoves(true))
				{
					attackers[(int)attackedTile.x,(int)attackedTile.y, (int)attackedTile.z, (int)attackedTile.w].Add(tile.currentPiece);
				}
			}
		}
	}

	void InitializeBoard()
	{
		for(int x = 0; x<BOARD_SIZE; x++)
		{
			for(int y = 0; y<BOARD_SIZE; y++)
			{
				for(int z = 0; z<BOARD_SIZE; z++)
				{
					for(int w = 0; w<BOARD_SIZE; w++)
					{
						chessboard[x,y,z,w] = GameObject.Instantiate(tilePrefab).GetComponent<Tile>();
						chessboard[x,y,z,w].Initialize(x, y, z, w);
						chessboard[x,y,z,w].transform.parent = tileContainer.transform;

						chessboard[x,y,z,w].transform.position = GetTilePosition(x,y,z,w);
						chessboard[x,y,z,w].transform.rotation = Quaternion.LookRotation(Vector3.up);

						attackers[x,y,z,w] = new List<ChessPiece>();
					}
				}
			}
		}

		/*
		WHITE TEAM
		 */

		AddChessPiece(0,0,0,0,ChessPiece.Type.ROOK, ChessPiece.Team.WHITE);
		AddChessPiece(3,0,0,0,ChessPiece.Type.ROOK, ChessPiece.Team.WHITE);

		AddChessPiece(0,1,0,0,ChessPiece.Type.BISHOP, ChessPiece.Team.WHITE);
		AddChessPiece(1,1,0,0,ChessPiece.Type.KING, ChessPiece.Team.WHITE);
		AddChessPiece(2,1,0,0,ChessPiece.Type.QUEEN, ChessPiece.Team.WHITE);
		AddChessPiece(3,1,0,0,ChessPiece.Type.BISHOP, ChessPiece.Team.WHITE);

		AddChessPiece(1,1,1,0,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(2,1,1,0,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);

		AddChessPiece(0,2,0,0,ChessPiece.Type.BISHOP, ChessPiece.Team.WHITE);
		AddChessPiece(1,2,0,0,ChessPiece.Type.KNIGHT, ChessPiece.Team.WHITE);
		AddChessPiece(2,2,0,0,ChessPiece.Type.KNIGHT, ChessPiece.Team.WHITE);
		AddChessPiece(3,2,0,0,ChessPiece.Type.BISHOP, ChessPiece.Team.WHITE);

		AddChessPiece(1,2,1,0,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(2,2,1,0,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);

		AddChessPiece(0,0,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(3,0,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);

		AddChessPiece(0,1,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(1,1,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(2,1,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(3,1,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(1,1,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(2,1,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);

		AddChessPiece(0,2,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(3,2,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(1,2,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		AddChessPiece(2,2,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);

		/*
		BLACK TEAM
		 */

		 AddChessPiece(0,0,3,3,ChessPiece.Type.ROOK, ChessPiece.Team.BLACK);
		AddChessPiece(3,0,3,3,ChessPiece.Type.ROOK, ChessPiece.Team.BLACK);

		AddChessPiece(0,1,3,3,ChessPiece.Type.BISHOP, ChessPiece.Team.BLACK);
		AddChessPiece(1,1,3,3,ChessPiece.Type.KING, ChessPiece.Team.BLACK);
		AddChessPiece(2,1,3,3,ChessPiece.Type.KING, ChessPiece.Team.BLACK);
		AddChessPiece(3,1,3,3,ChessPiece.Type.BISHOP, ChessPiece.Team.BLACK);

		AddChessPiece(1,1,2,3,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(2,1,2,3,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);

		AddChessPiece(0,2,3,3,ChessPiece.Type.BISHOP, ChessPiece.Team.BLACK);
		AddChessPiece(1,2,3,3,ChessPiece.Type.KNIGHT, ChessPiece.Team.BLACK);
		AddChessPiece(2,2,3,3,ChessPiece.Type.KNIGHT, ChessPiece.Team.BLACK);
		AddChessPiece(3,2,3,3,ChessPiece.Type.BISHOP, ChessPiece.Team.BLACK);

		AddChessPiece(1,2,2,3,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(2,2,2,3,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);

		AddChessPiece(0,0,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(3,0,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);

		AddChessPiece(0,1,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(1,1,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(2,1,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(3,1,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(1,1,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(2,1,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);

		AddChessPiece(0,2,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(3,2,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(1,2,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		AddChessPiece(2,2,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);

		ComputeAttackers();
	}

	Vector3 GetFlatCardinalityVector(int cardinality)
	{
		if (cardinality == 0)
		{
			return Vector3.right;
		}
		else if (cardinality == 1)
		{
			return Vector3.right * 5;
		}
		else if (cardinality == 2)
		{
			return Vector3.forward;
		}
		else
		{
			return Vector3.forward * 5;
		}
	}

	Vector3 GetCardinalityVector(int cardinality)
	{
		if (cardinality == 0)
		{
			return Vector3.right * (verticalCardinalityDirection == 0 ? 2.5f : 1);
		}
		else if (cardinality == 1)
		{
			return Vector3.up * (verticalCardinalityDirection == 1 ? 2.5f : 1);
		}
		else if (cardinality == 2)
		{
			return Vector3.forward * (verticalCardinalityDirection == 2 ? 2.5f : 1);
		}
		else
		{
			return gapVector;
		}
	}

	Vector3 GetOldCardinalityVector(int cardinality)
	{
		if (cardinality == 0)
		{
			return Vector3.right * (verticalCardinalityDirection == 0 ? 2.5f : 1);
		}
		else if (cardinality == 1)
		{
			return Vector3.up * (verticalCardinalityDirection == 1 ? 2.5f : 1);
		}
		else if (cardinality == 2)
		{
			return Vector3.forward * (verticalCardinalityDirection == 2 ? 2.5f : 1);
		}
		else
		{
			return oldGapVector;
		}
	}

	Vector3 GetTilePosition(int x, int y, int z, int w)
	{
		if (flat)
		{
			Vector3 xComponent = GetFlatCardinalityVector(cardinalityX);
			Vector3 yComponent = GetFlatCardinalityVector(cardinalityY);
			Vector3 zComponent = GetFlatCardinalityVector(cardinalityZ);
			Vector3 wComponent = GetFlatCardinalityVector(cardinalityW);

			return xComponent * (1.5f - x) + yComponent * (1.5f - y) + zComponent * (1.5f - z) + wComponent * (1.5f - w);
		}
		else
		{
			Vector3 xComponent = GetCardinalityVector(cardinalityX);
			Vector3 yComponent = GetCardinalityVector(cardinalityY);
			Vector3 zComponent = GetCardinalityVector(cardinalityZ);
			Vector3 wComponent = GetCardinalityVector(cardinalityW);

			return xComponent * (1.5f - x) + yComponent * (1.5f - y) + zComponent * (1.5f - z) + wComponent * (1.5f - w);
		}
	}

	Vector3 GetOldTilePosition(int x, int y, int z, int w)
	{
		if (flat)
		{
			Vector3 xComponent = GetFlatCardinalityVector(oldCardinalityX);
			Vector3 yComponent = GetFlatCardinalityVector(oldCardinalityY);
			Vector3 zComponent = GetFlatCardinalityVector(oldCardinalityZ);
			Vector3 wComponent = GetFlatCardinalityVector(oldCardinalityW);

			return xComponent * (1.5f - x) + yComponent * (1.5f - y) + zComponent * (1.5f - z) + wComponent * (1.5f - w);
		}
		else
		{
			Vector3 xComponent = GetOldCardinalityVector(oldCardinalityX);
			Vector3 yComponent = GetOldCardinalityVector(oldCardinalityY);
			Vector3 zComponent = GetOldCardinalityVector(oldCardinalityZ);
			Vector3 wComponent = GetOldCardinalityVector(oldCardinalityW);

			return xComponent * (1.5f - x) + yComponent * (1.5f - y) + zComponent * (1.5f - z) + wComponent * (1.5f - w);
		}
	}

	void EnterWZ()
	{
		EnterRightShift();
	}

	void EnterRightShift()
	{
		cardinalityX = 3;
		cardinalityY = 1;
		cardinalityZ = 2;
		cardinalityW = 0;

		shiftState = SHIFT_STATE.RIGHT;
		gapVector = Vector3.forward * 4 + Vector3.right * 5;

		tileFacing = Vector3.up;
		verticalCardinalityDirection = 1;

		targetCameraPosition = rightShiftCameraPosition;
		targetCameraRotation = rightShiftCameraRotation;

		isRotated = false;
	}

	void EnterXW()
	{
		EnterLeftShift();
	}

	void EnterLeftShift()
	{
		cardinalityX = 0;
		cardinalityY = 1;
		cardinalityZ = 3;
		cardinalityW = 2;

		shiftState = SHIFT_STATE.LEFT;
		gapVector = Vector3.forward * 4 + Vector3.left * 5;

		tileFacing = Vector3.up;
		verticalCardinalityDirection = 1;

		targetCameraPosition = leftShiftCameraPosition;
		targetCameraRotation = leftShiftCameraRotation;

		isRotated = false;
	}

	void EnterXZ()
	{
		EnterMidShift();
	}

	void EnterMidShift()
	{
		cardinalityX = 0;
		cardinalityY = 1;
		cardinalityZ = 2;
		cardinalityW = 3;

		shiftState = SHIFT_STATE.MID;
		gapVector = Vector3.forward * 5;

		tileFacing = Vector3.up;
		verticalCardinalityDirection = 1;

		targetCameraPosition = midShiftCameraPosition;
		targetCameraRotation = midShiftCameraRotation;

		isRotated = false;
	}

	void EnterXY()
	{
		EnterRightShiftRotated();
	}

	void EnterRightShiftRotated()
	{
		if(flat)
		{
			cardinalityX = 0;
			cardinalityY = 2;
			cardinalityZ = 3;
			cardinalityW = 1;
		}
		else
		{
			cardinalityX = 3;
			cardinalityY = 1;
			cardinalityZ = 2;
			cardinalityW = 0;
		}

		shiftState = SHIFT_STATE.RIGHT_ROTATED;
		gapVector = Vector3.right * 5;

		tileFacing = Vector3.back;
		verticalCardinalityDirection = 2;

		targetCameraPosition = rightShiftRotatedCameraPosition;
		targetCameraRotation = rightShiftRotatedCameraRotation;

		isRotated = true;
	}

	void EnterYW()
	{
		EnterLeftShiftRotated();
	}

	void EnterLeftShiftRotated()
	{
		if(flat)
		{
			cardinalityX = 1;
			cardinalityY = 0;
			cardinalityZ = 3;
			cardinalityW = 2;
		}
		else
		{
			cardinalityX = 0;
			cardinalityY = 1;
			cardinalityZ = 3;
			cardinalityW = 2;
		}

		shiftState = SHIFT_STATE.LEFT_ROTATED;
		gapVector = Vector3.left * 5;

		tileFacing = Vector3.forward;
		verticalCardinalityDirection = 2;

		targetCameraPosition = leftShiftRotatedCameraPosition;
		targetCameraRotation = leftShiftRotatedCameraRotation;

		isRotated = true;
	}

	void EnterYZ()
	{
		EnterMidShiftRotated();
	}

	void EnterMidShiftRotated()
	{
		if(flat)
		{
			cardinalityX = 1;
			cardinalityY = 0;
			cardinalityZ = 2;
			cardinalityW = 3;
		}
		else
		{
			cardinalityX = 0;
			cardinalityY = 1;
			cardinalityZ = 2;
			cardinalityW = 3;
		}

		shiftState = SHIFT_STATE.MID_ROTATED;
		gapVector = Vector3.forward * 5;

		tileFacing = Vector3.right;
		verticalCardinalityDirection = 0;

		targetCameraPosition = midShiftRotatedCameraPosition;
		targetCameraRotation = midShiftRotatedCameraRotation;

		isRotated = true;
	}

	SHIFT_STATE_COORD CurrentShiftCoord()
	{
		switch (shiftState)
		{
			case SHIFT_STATE.LEFT:
				return SHIFT_STATE_COORD.XW;
			case SHIFT_STATE.RIGHT:
				return SHIFT_STATE_COORD.WZ;
			case SHIFT_STATE.MID:
				return SHIFT_STATE_COORD.XZ;
			case SHIFT_STATE.LEFT_ROTATED:
				return SHIFT_STATE_COORD.YW;
			case SHIFT_STATE.RIGHT_ROTATED:
				return SHIFT_STATE_COORD.XY;
			case SHIFT_STATE.MID_ROTATED:
				return SHIFT_STATE_COORD.YZ;
		}
		return SHIFT_STATE_COORD.XW;
	}

	void CopyOldValues()
	{
		oldCardinalityX = cardinalityX;
		oldCardinalityY = cardinalityY;
		oldCardinalityZ = cardinalityZ;
		oldCardinalityW = cardinalityW;
		oldGapVector = gapVector;
		oldTileFacing = tileFacing;

		oldCameraPosition = targetCameraPosition;
		oldCameraRotation = targetCameraRotation;
	}
	
	void StartShift()
	{
		shiftStartTime = Time.time;
		shifting = true;
	}

	 public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}

	public void TryShiftLeft()
	{
		if (shifting)
		{
			return;
		}

		if (shiftState == SHIFT_STATE.MID)
		{
			CopyOldValues();
			EnterLeftShift();
			shiftXFirst = true;
			StartShift();
		}
		else if (shiftState == SHIFT_STATE.RIGHT)
		{
			CopyOldValues();
			EnterMidShift();
			shiftXFirst = false;
			StartShift();
		}
		else if (shiftState == SHIFT_STATE.RIGHT_ROTATED)
		{
			CopyOldValues();
			EnterMidShiftRotated();
			shiftXFirst = false;
			StartShift();
		}
		else if (shiftState == SHIFT_STATE.MID_ROTATED)
		{
			CopyOldValues();
			EnterLeftShiftRotated();
			shiftXFirst = true;
			StartShift();
		}
	}

	public void TryShiftRight()
	{
		if (shifting)
		{
			return;
		}

		if (shiftState == SHIFT_STATE.LEFT)
		{
			CopyOldValues();
			EnterMidShift();
			shiftXFirst = false;
			StartShift();
		}
		else if (shiftState == SHIFT_STATE.MID)
		{
			CopyOldValues();
			EnterRightShift();
			shiftXFirst = true;
			StartShift();
		}
		else if (shiftState == SHIFT_STATE.LEFT_ROTATED)
		{
			CopyOldValues();
			EnterMidShiftRotated();
			shiftXFirst = false;
			StartShift();
		}
		else if (shiftState == SHIFT_STATE.MID_ROTATED)
		{
			CopyOldValues();
			EnterRightShiftRotated();
			shiftXFirst = true;
			StartShift();
		}
	}

	public void ToggleRotate()
	{
		if (shifting)
		{
			return;
		}

		CopyOldValues();
		if (shiftState == SHIFT_STATE.LEFT)
		{
			EnterLeftShiftRotated();
		}
		else if (shiftState == SHIFT_STATE.MID)
		{
			EnterMidShiftRotated();
		}
		else if (shiftState == SHIFT_STATE.RIGHT)
		{
			EnterRightShiftRotated();
		}
		else if (shiftState == SHIFT_STATE.LEFT_ROTATED)
		{
			EnterLeftShift();
		}
		else if (shiftState == SHIFT_STATE.MID_ROTATED)
		{
			EnterMidShift();
		}
		else if (shiftState == SHIFT_STATE.RIGHT_ROTATED)
		{
			EnterRightShift();
		}
		shiftXFirst = true;
		StartShift();
	}

	void ClickOffTile()
	{
		if (selectedTile != null && destinationTile == null)
		{
			selectedTile = null;
		}
		else if (selectedTile != null && destinationTile != null)
		{
			destinationTile = null;
		}

		UpdateTileHighlights();
	}

	IEnumerator LoadStartScene()
	{
		yield return new WaitForSeconds(2);
		SceneManager.LoadScene("StartScene");
	}

	void ExecuteMove(Tile startTile, Tile endTile)
	{
		startTile.currentPiece.hasMoved = true;

		if (endTile.currentPiece != null && endTile.currentPiece.type == ChessPiece.Type.KING)
		{
			if(currentMove == ChessPiece.Team.WHITE)
			{
				turnText.text = "White wins!!";
			}
			else
			{
				turnText.text = "Black wins!!";
			}
			StartCoroutine(LoadStartScene());
		}
		else
		{
			if(currentMove == ChessPiece.Team.WHITE)
			{
				currentMove = ChessPiece.Team.BLACK;
				turnText.text = "Black's turn";
			}
			else
			{
				currentMove = ChessPiece.Team.WHITE;
				turnText.text = "White's turn";
			}
		}

		endTile.SetPiece(startTile.currentPiece);
		startTile.SetPiece(null);

		if (endTile.currentPiece.type == ChessPiece.Type.PAWN)
		{
			if ((endTile.currentPiece.team == ChessPiece.Team.WHITE 
				&& endTile.currentPiece.z == BOARD_SIZE-1 && endTile.currentPiece.w == BOARD_SIZE-1)
				|| (endTile.currentPiece.team == ChessPiece.Team.BLACK 
				   && endTile.currentPiece.z == 0 && endTile.currentPiece.w == 0))
			{
				endTile.currentPiece.type = ChessPiece.Type.QUEEN;
				endTile.SetPiece(endTile.currentPiece);
			}
		}

		ComputeAttackers();
	}

	void ClickTile(Tile tile)
	{
		warningText.text = "";
		if (tile == selectedTile)
		{
			return;
		}

		if (selectedTile == null)
		{
			selectedTile = tile;
		}
		if (selectedTile != null && tile == destinationTile)
		{
			//Move piece
			ExecuteMove(selectedTile, destinationTile);

			selectedTile = null;
			destinationTile = null;
		}
		else if (selectedTile != null && selectedTile.currentPiece != null && destinationTile == null)
		{
			if (selectedTile.currentPiece.team == currentMove &&
				selectedTile.currentPiece.GetValidMoves().Contains(new Vector4(tile.x, tile.y, tile.z, tile.w)))
			{
				destinationTile = tile;
			}
			else
			{
				selectedTile = tile;
			}
		}
		else if (selectedTile != null && selectedTile.currentPiece == null && destinationTile == null)
		{
			selectedTile = tile;
		}
		else if (selectedTile != null && selectedTile.currentPiece != null && destinationTile != null && tile != destinationTile)
		{
			destinationTile = null;
		}

		UpdateTileHighlights();
	}

	void UpdateTileHighlights()
	{
		foreach (Tile boardTile in chessboard)
		{
			boardTile.ClearSelection();
		}

		if (attackedTile != null)
		{
			attackedTile.Select();
			foreach(ChessPiece attacker in attackers[attackedTile.x, attackedTile.y, attackedTile.z, attackedTile.w])
			{
				chessboard[attacker.x, attacker.y, attacker.z, attacker.w].Highlight();
			}
		}
		else if (selectedTile != null && destinationTile == null)
		{
			selectedTile.Select();
			if (selectedTile.currentPiece != null)
			{
				HashSet<Vector4> moves = selectedTile.currentPiece.GetValidMoves();
				foreach (Vector4 move in moves)
				{
					chessboard[(int)move.x, (int)move.y, (int)move.z, (int)move.w].Highlight();
				}
			}
		}
		else if (selectedTile != null && destinationTile != null)
		{
			selectedTile.Select();
			destinationTile.Select();
		}
	}

	void UpdateButtons()
	{
		if (shifting)
		{
			upButton.gameObject.SetActive(false);
			rightButton.gameObject.SetActive(false);
			downButton.gameObject.SetActive(false);
			leftButton.gameObject.SetActive(false);
			return;
		}

		if (shiftState == SHIFT_STATE.LEFT || shiftState == SHIFT_STATE.MID || shiftState == SHIFT_STATE.LEFT_ROTATED || shiftState == SHIFT_STATE.MID_ROTATED)
		{
			rightButton.gameObject.SetActive(true);
		}
		else
		{
			rightButton.gameObject.SetActive(false);
		}

		if (shiftState == SHIFT_STATE.RIGHT || shiftState == SHIFT_STATE.MID || shiftState == SHIFT_STATE.RIGHT_ROTATED || shiftState == SHIFT_STATE.MID_ROTATED)
		{
			leftButton.gameObject.SetActive(true);
		}
		else
		{
			leftButton.gameObject.SetActive(false);
		}

		if (shiftState == SHIFT_STATE.LEFT || shiftState == SHIFT_STATE.MID || shiftState == SHIFT_STATE.RIGHT)
		{
			upButton.gameObject.SetActive(true);
		}
		else
		{
			upButton.gameObject.SetActive(false);
		}

		if (shiftState == SHIFT_STATE.LEFT_ROTATED || shiftState == SHIFT_STATE.MID_ROTATED || shiftState == SHIFT_STATE.RIGHT_ROTATED)
		{
			downButton.gameObject.SetActive(true);
		}
		else
		{
			downButton.gameObject.SetActive(false);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast(ray, out hit)) {
				Transform objectHit = hit.transform;
				
				bool clickedTile = false;
				while(objectHit != null)
				{
					if (objectHit.GetComponent<Tile>() != null)
					{
						ClickTile(objectHit.GetComponent<Tile>());
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
			Tile newAttackedTile = attackedTile;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {
				Transform objectHit = hit.transform;
				
				bool clickedTile = false;
				while(objectHit != null)
				{
					if (objectHit.GetComponent<Tile>() != null)
					{
						newAttackedTile = objectHit.GetComponent<Tile>();
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
					newAttackedTile = null;
				}
			}
			else
			{
				newAttackedTile = null;
			}

			if (newAttackedTile != attackedTile)
			{
				Debug.Log("Update " + attackedTile + " : " + newAttackedTile);
				attackedTile = newAttackedTile;
				UpdateTileHighlights();
			}
		}
		else
		{
			if(attackedTile != null)
			{
				attackedTile = null;
				UpdateTileHighlights();
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate () {

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			TryShiftLeft();
		} 
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			TryShiftRight();
		} 
		else if (isRotated && Input.GetKey(KeyCode.DownArrow))
		{
			ToggleRotate();
		} 
		else if (!isRotated && Input.GetKey(KeyCode.UpArrow))
		{
			ToggleRotate();
		}

		/*if (!shifting && Input.GetKeyDown(KeyCode.LeftArrow))
		{
			CopyOldValues();
			switch(CurrentShiftCoord())
			{
				case SHIFT_STATE_COORD.WZ:
					EnterYZ();
				break;
				case SHIFT_STATE_COORD.XW:
					EnterWZ();
				break;
				case SHIFT_STATE_COORD.XY:
					EnterXW();
				break;
				case SHIFT_STATE_COORD.XZ:
					EnterYZ();
				break;
				case SHIFT_STATE_COORD.YW:
					EnterXW();
				break;
				case SHIFT_STATE_COORD.YZ:
					EnterXY();
				break;
			}
			StartShift();
		} 
		else if (!shifting && Input.GetKeyDown(KeyCode.RightArrow))
		{
			CopyOldValues();
			switch(CurrentShiftCoord())
			{
				case SHIFT_STATE_COORD.XW:
					EnterXY();
				break;
				case SHIFT_STATE_COORD.XZ:
					EnterXW();
				break;
				case SHIFT_STATE_COORD.YZ:
					EnterWZ();
				break;
				case SHIFT_STATE_COORD.YW:
					EnterYZ();
				break;
				case SHIFT_STATE_COORD.XY:
					EnterYZ();
				break;
				case SHIFT_STATE_COORD.WZ:
					EnterXW();
				break;
			}
			StartShift();
		} 
		else if (!shifting && Input.GetKeyDown(KeyCode.DownArrow))
		{
			CopyOldValues();
			switch(CurrentShiftCoord())
			{
				case SHIFT_STATE_COORD.XY:
					EnterXZ();
				break;
				case SHIFT_STATE_COORD.WZ:
					EnterYW();
				break;
				default:
					EnterWZ();
				break;
			}
			StartShift();
		} 
		else if (!shifting && Input.GetKeyDown(KeyCode.UpArrow))
		{
			CopyOldValues();
			switch(CurrentShiftCoord())
			{
				case SHIFT_STATE_COORD.XY:
					EnterYW();
				break;
				case SHIFT_STATE_COORD.WZ:
					EnterXZ();
				break;
				default:
					EnterXY();
				break;
			}
			StartShift();
		}*/

		if (Input.GetKey(KeyCode.Space) && !shifting)
		{
			flat = !flat;
			StartShift();
		}

		if (!shifting)
		{
			float xAxisValue = Input.GetAxis("Horizontal")*0.5f;
			float yAxisValue = Input.GetAxis("Vertical")*0.25f;
			float zAxisValue = Input.GetAxis("Mouse ScrollWheel")*2;
			Camera.main.transform.Translate(new Vector3(xAxisValue, yAxisValue, zAxisValue));
		}

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
			for(int x = 0; x<BOARD_SIZE; x++)
			{
				for(int y = 0; y<BOARD_SIZE; y++)
				{
					for(int z = 0; z<BOARD_SIZE; z++)
					{
						for(int w = 0; w<BOARD_SIZE; w++)
						{
							Vector3 newVector = GetTilePosition(x,y,z,w);
							Vector3 oldVector = GetOldTilePosition(x,y,z,w);

							float newX;
							float newZ;
							float newY;
							if (flat)
							{
								float intermeidiateY = (newVector.x + newVector.z - oldVector.x - oldVector.z)/4;
								if (shiftPercentage > 0.5f)
								{
									newY = Mathf.Lerp(oldVector.y, intermeidiateY, stage1Lerp);
								}
								else
								{
									newY = Mathf.Lerp(newVector.y, intermeidiateY, stage2Lerp);
								}
								newX = Mathf.Lerp(oldVector.x, newVector.x, shiftPercentage);
								newZ = Mathf.Lerp(oldVector.z, newVector.z, shiftPercentage);
								chessboard[x,y,z,w].transform.position = new Vector3(newX, newY, newZ);
							}
							else
							{
								newY = Mathf.Lerp(oldVector.y, newVector.y, stage1Lerp);
								if (shiftXFirst) newX = Mathf.Lerp(oldVector.x, newVector.x, stage2Lerp);
								else newX = Mathf.Lerp(newVector.x, oldVector.x, stage1Lerp);
								if (!shiftXFirst) newZ = Mathf.Lerp(oldVector.z, newVector.z, stage2Lerp);
								else newZ = Mathf.Lerp(newVector.z, oldVector.z, stage1Lerp);
								chessboard[x,y,z,w].transform.position = new Vector3(newX, newY, newZ);
							}
							
							if (flat)
							{
								chessboard[x,y,z,w].transform.rotation = Quaternion.LookRotation(Vector3.up);
							}
							else
							{
								chessboard[x,y,z,w].transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(oldTileFacing), Quaternion.LookRotation(tileFacing), shiftPercentage);
							}

						}
					}
				}
			}

			if (flat)
			{
				Camera.main.transform.position = flatCameraPosition;
				Camera.main.transform.rotation = flatCameraRotation;
			}
			else
			{
				Camera.main.transform.position = Vector3.Lerp(oldCameraPosition, targetCameraPosition, shiftPercentage);
				Camera.main.transform.rotation = Quaternion.Lerp(oldCameraRotation, targetCameraRotation, shiftPercentage);
			}
		}
		else
		{
			for(int x = 0; x<BOARD_SIZE; x++)
			{
				for(int y = 0; y<BOARD_SIZE; y++)
				{
					for(int z = 0; z<BOARD_SIZE; z++)
					{
						for(int w = 0; w<BOARD_SIZE; w++)
						{
							//chessboard[x,y,z,w].transform.position = Vector3.MoveTowards(chessboard[x,y,z,w].transform.position, GetTilePosition(x,y,z,w), Time.fixedDeltaTime*10);
							//chessboard[x,y,z,w].transform.rotation = Quaternion.RotateTowards(chessboard[x,y,z,w].transform.rotation, Quaternion.LookRotation(tileFacing), Time.fixedDeltaTime*360);
						}
					}
				}
			}
		}

		UpdateButtons();
	}
}
