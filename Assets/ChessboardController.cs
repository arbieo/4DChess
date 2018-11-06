using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessboardController : MonoBehaviour {

	public const int BOARD_SIZE = 4;

	public GameObject[,,,] chessboard = new GameObject[BOARD_SIZE,BOARD_SIZE,BOARD_SIZE,BOARD_SIZE];
	public ChessPiece[,,,] pieces = new ChessPiece[BOARD_SIZE,BOARD_SIZE,BOARD_SIZE,BOARD_SIZE];

	public int cardinalityX = 0;
	public int cardinalityY = 1;
	public int cardinalityZ = 2;
	public int cardinalityW = 3;
	
	public int oldCardinalityX = 0;
	public int oldCardinalityY = 1;
	public int oldCardinalityZ = 2;
	public int oldCardinalityW = 3;

	float shiftStartTime = -100;
	public float timeToShift = 2;
	bool shiftXFirst = false;

	public enum SHIFT_STATE
	{
		LEFT,
		MID,
		RIGHT,
		LEFT_ROTATED,
		MID_ROTATED,
		RIGHT_ROTATED
	}

	public SHIFT_STATE shiftState = SHIFT_STATE.MID;
	public int verticalCardinalityDirection = 1;

	Vector3 gapVector = Vector3.forward * 6;
	Vector3 oldGapVector = Vector3.forward * 6;
	Vector3 tileFacing = Vector3.up;
	Vector3 oldTileFacing = Vector3.up;

	Vector3 targetCameraPosition;
	Quaternion targetCameraRotation;
	Vector3 oldCameraPosition;
	Quaternion oldCameraRotation;


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

	public Material outerBlackMaterial;
	public Material innerBlackMaterial;
	public Material outerWhiteMaterial;
	public Material innerWhiteMaterial;

	public Material outerDarkMidMaterial;
	public Material innerDarkMidMaterial;
	public Material outerLightMidMaterial;
	public Material innerLightMidMaterial;

	public GameObject pawnPrefab;
	public GameObject rookPrefab;
	public GameObject bishopPrefab;
	public GameObject knightPrefab;
	public GameObject queenPrefab;
	public GameObject kingPrefab;

	public enum ChessPiece
	{
		NONE,
		PAWN,
		ROOK,
		BISHOP,
		KNIGHT,
		QUEEN,
		KING
	}

	Dictionary<ChessPiece, GameObject> piecePrefabs = new Dictionary<ChessPiece, GameObject>();

	public float sensitivityX = 5;
	public float sensitivityY = 5;
	public float minimumX = -360F;
	public float maximumX = 360F;
	public float minimumY = -60F;
	public float maximumY = 60F;
	float rotationX = 0F;
	float rotationY = 0F;
	Quaternion originalRotation = Quaternion.identity;
	Vector3 oldMousePosition = Vector2.zero;

	// Use this for initialization
	void Start () {
		piecePrefabs[ChessPiece.PAWN] = pawnPrefab;
		piecePrefabs[ChessPiece.ROOK] = rookPrefab;
		piecePrefabs[ChessPiece.BISHOP] = bishopPrefab;
		piecePrefabs[ChessPiece.KNIGHT] = knightPrefab;
		piecePrefabs[ChessPiece.QUEEN] = queenPrefab;
		piecePrefabs[ChessPiece.KING] = kingPrefab;
		InitializeBoard();

		EnterMidShift();
		Camera.main.transform.position = targetCameraPosition;
		Camera.main.transform.rotation = targetCameraRotation;

		originalRotation = Camera.main.transform.rotation;
	}

	void InitializeBoard()
	{
		pieces[0,0,0,0] = ChessPiece.ROOK;
		pieces[1,0,0,0] = ChessPiece.KNIGHT;
		pieces[2,0,0,0] = ChessPiece.KNIGHT;
		pieces[3,0,0,0] = ChessPiece.ROOK;

		pieces[0,0,1,0] = ChessPiece.PAWN;
		pieces[3,0,1,0] = ChessPiece.PAWN;

		pieces[0,1,0,0] = ChessPiece.BISHOP;
		pieces[1,1,0,0] = ChessPiece.KING;
		pieces[2,1,0,0] = ChessPiece.QUEEN;
		pieces[3,1,0,0] = ChessPiece.BISHOP;

		pieces[1,1,1,0] = ChessPiece.PAWN;
		pieces[2,1,1,0] = ChessPiece.PAWN;

		pieces[0,2,0,0] = ChessPiece.BISHOP;
		pieces[1,2,0,0] = ChessPiece.QUEEN;
		pieces[2,2,0,0] = ChessPiece.QUEEN;
		pieces[3,2,0,0] = ChessPiece.BISHOP;

		pieces[1,2,1,0] = ChessPiece.PAWN;
		pieces[2,2,1,0] = ChessPiece.PAWN;

		pieces[0,3,0,0] = ChessPiece.ROOK;
		pieces[1,3,0,0] = ChessPiece.KNIGHT;
		pieces[2,3,0,0] = ChessPiece.KNIGHT;
		pieces[3,3,0,0] = ChessPiece.ROOK;

		pieces[0,3,1,0] = ChessPiece.PAWN;
		pieces[3,3,1,0] = ChessPiece.PAWN;

		pieces[0,0,0,1] = ChessPiece.PAWN;
		pieces[3,0,0,1] = ChessPiece.PAWN;

		pieces[0,1,1,1] = ChessPiece.PAWN;
		pieces[1,1,1,1] = ChessPiece.PAWN;
		pieces[2,1,1,1] = ChessPiece.PAWN;
		pieces[3,1,1,1] = ChessPiece.PAWN;
		pieces[1,1,0,1] = ChessPiece.PAWN;
		pieces[2,1,0,1] = ChessPiece.PAWN;

		pieces[0,2,1,1] = ChessPiece.PAWN;
		pieces[1,2,1,1] = ChessPiece.PAWN;
		pieces[2,2,1,1] = ChessPiece.PAWN;
		pieces[3,2,1,1] = ChessPiece.PAWN;
		pieces[1,2,0,1] = ChessPiece.PAWN;
		pieces[2,2,0,1] = ChessPiece.PAWN;

		pieces[0,3,0,1] = ChessPiece.PAWN;
		pieces[3,3,0,1] = ChessPiece.PAWN;

		for(int x = 0; x<BOARD_SIZE; x++)
		{
			for(int y = 0; y<BOARD_SIZE; y++)
			{
				for(int z = 0; z<BOARD_SIZE; z++)
				{
					for(int w = 0; w<BOARD_SIZE; w++)
					{
						chessboard[x,y,z,w] = GameObject.Instantiate(tilePrefab);
						Material tileMaterial;
						bool isDark = false;
						if ((x+y+z+w) % 2 == 0)
						{
							isDark = true;
						}

						if (w == 0 || w == 3)
						{
							if (x == 0 || x == 3 || z == 0 || z == 3)
							{
								tileMaterial = isDark ? outerBlackMaterial : outerWhiteMaterial;
							}
							else
							{
								tileMaterial = isDark ? innerBlackMaterial : innerWhiteMaterial;
							}
						}
						else
						{
							if (x == 0 || x == 3 || z == 0 || z == 3)
							{
								tileMaterial = isDark ? outerDarkMidMaterial : outerLightMidMaterial;
							}
							else
							{
								tileMaterial = isDark ? innerDarkMidMaterial : innerLightMidMaterial;
							}
						}

						chessboard[x,y,z,w].transform.Find("Display").GetComponent<MeshRenderer>().material = tileMaterial;

						chessboard[x,y,z,w].transform.position = GetTilePosition(x,y,z,w);
						chessboard[x,y,z,w].transform.rotation = Quaternion.LookRotation(Vector3.up);
						if (pieces[x,y,z,w] != ChessPiece.NONE)
						{
							GameObject chessPiece = GameObject.Instantiate(piecePrefabs[pieces[x,y,z,w]], chessboard[x,y,z,w].transform.position, chessboard[x,y,z,w].transform.rotation);
							chessPiece.transform.SetParent(chessboard[x,y,z,w].transform.Find("PieceContainer"));
						}
					}
				}
			}
		}
	}

	Vector3 GetCardinalityVector(int cardinality)
	{
		if (cardinality == 0)
		{
			return Vector3.right * (verticalCardinalityDirection == 0 ? 2 : 1);
		}
		else if (cardinality == 1)
		{
			return Vector3.up * (verticalCardinalityDirection == 1 ? 2 : 1);
		}
		else if (cardinality == 2)
		{
			return Vector3.forward * (verticalCardinalityDirection == 2 ? 2 : 1);
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
			return Vector3.right * (verticalCardinalityDirection == 0 ? 2 : 1);
		}
		else if (cardinality == 1)
		{
			return Vector3.up * (verticalCardinalityDirection == 1 ? 2 : 1);
		}
		else if (cardinality == 2)
		{
			return Vector3.forward * (verticalCardinalityDirection == 2 ? 2 : 1);
		}
		else
		{
			return oldGapVector;
		}
	}

	Vector3 GetTilePosition(int x, int y, int z, int w)
	{
		Vector3 xComponent = GetCardinalityVector(cardinalityX);
		Vector3 yComponent = GetCardinalityVector(cardinalityY);
		Vector3 zComponent = GetCardinalityVector(cardinalityZ);
		Vector3 wComponent = GetCardinalityVector(cardinalityW);

		return xComponent * (1.5f - x) + yComponent * (1.5f - y) + zComponent * (1.5f - z) + wComponent * (1.5f - w);
	}

	Vector3 GetOldTilePosition(int x, int y, int z, int w)
	{
		Vector3 xComponent = GetOldCardinalityVector(oldCardinalityX);
		Vector3 yComponent = GetOldCardinalityVector(oldCardinalityY);
		Vector3 zComponent = GetOldCardinalityVector(oldCardinalityZ);
		Vector3 wComponent = GetOldCardinalityVector(oldCardinalityW);

		return xComponent * (1.5f - x) + yComponent * (1.5f - y) + zComponent * (1.5f - z) + wComponent * (1.5f - w);
	}

	void EnterRightShift()
	{
		cardinalityX = 3;
		cardinalityY = 1;
		cardinalityZ = 2;
		cardinalityW = 0;

		shiftState = SHIFT_STATE.RIGHT;
		gapVector = Vector3.forward * 4 + Vector3.right * 6;

		tileFacing = Vector3.up;
		verticalCardinalityDirection = 1;

		targetCameraPosition = rightShiftCameraPosition;
		targetCameraRotation = rightShiftCameraRotation;

		isRotated = false;
	}

	void EnterLeftShift()
	{
		cardinalityX = 0;
		cardinalityY = 1;
		cardinalityZ = 3;
		cardinalityW = 2;

		shiftState = SHIFT_STATE.LEFT;
		gapVector = Vector3.forward * 4 + Vector3.left * 6;

		tileFacing = Vector3.up;
		verticalCardinalityDirection = 1;

		targetCameraPosition = leftShiftCameraPosition;
		targetCameraRotation = leftShiftCameraRotation;

		isRotated = false;
	}

	void EnterMidShift()
	{
		cardinalityX = 0;
		cardinalityY = 1;
		cardinalityZ = 2;
		cardinalityW = 3;

		shiftState = SHIFT_STATE.MID;
		gapVector = Vector3.forward * 6;

		tileFacing = Vector3.up;
		verticalCardinalityDirection = 1;

		targetCameraPosition = midShiftCameraPosition;
		targetCameraRotation = midShiftCameraRotation;

		isRotated = false;
	}

	void EnterRightShiftRotated()
	{
		cardinalityX = 3;
		cardinalityY = 1;
		cardinalityZ = 2;
		cardinalityW = 0;

		shiftState = SHIFT_STATE.RIGHT_ROTATED;
		gapVector = Vector3.right * 6;

		tileFacing = Vector3.back;
		verticalCardinalityDirection = 2;

		targetCameraPosition = rightShiftRotatedCameraPosition;
		targetCameraRotation = rightShiftRotatedCameraRotation;

		isRotated = true;
	}

	void EnterLeftShiftRotated()
	{
		cardinalityX = 0;
		cardinalityY = 1;
		cardinalityZ = 3;
		cardinalityW = 2;

		shiftState = SHIFT_STATE.LEFT_ROTATED;
		gapVector = Vector3.left * 6;

		tileFacing = Vector3.forward;
		verticalCardinalityDirection = 2;

		targetCameraPosition = leftShiftRotatedCameraPosition;
		targetCameraRotation = leftShiftRotatedCameraRotation;

		isRotated = true;
	}

	void EnterMidShiftRotated()
	{
		cardinalityX = 0;
		cardinalityY = 1;
		cardinalityZ = 2;
		cardinalityW = 3;

		shiftState = SHIFT_STATE.MID_ROTATED;
		gapVector = Vector3.forward * 6;

		tileFacing = Vector3.right;
		verticalCardinalityDirection = 0;

		targetCameraPosition = midShiftRotatedCameraPosition;
		targetCameraRotation = midShiftRotatedCameraRotation;

		isRotated = true;
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
	}

	 public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}

	void TryShiftLeft()
	{
		if (Time.time - shiftStartTime < timeToShift)
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

	void TryShiftRight()
	{
		if (Time.time - shiftStartTime < timeToShift)
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

	void ToggleRotate()
	{
		if (Time.time - shiftStartTime < timeToShift)
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

	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 50, 50), "<"))
            TryShiftLeft();
		if (GUI.Button(new Rect(70, 10, 50, 50), ">"))
            TryShiftRight();
		if (GUI.Button(new Rect(130, 10, 50, 50), "/"))
            ToggleRotate();
	}

	// Update is called once per frame
	void FixedUpdate () {

		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.A))
		{
			TryShiftLeft();
		}
		else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
		{
			TryShiftRight();
		}

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

		if (Time.time - shiftStartTime > timeToShift)
		{
			float xAxisValue = Input.GetAxis("Horizontal")*0.5f;
			float yAxisValue = Input.GetAxis("Vertical")*0.25f;
			float zAxisValue = Input.GetAxis("Mouse ScrollWheel")*2;
			if(Camera.main != null)
			{
				Camera.main.transform.Translate(new Vector3(xAxisValue, yAxisValue, zAxisValue));
				/*if (Input.GetKey(KeyCode.Mouse0))
				{
					// Read the mouse input axis
					rotationX += (Input.mousePosition - oldMousePosition).x * sensitivityX;
					rotationY += (Input.mousePosition - oldMousePosition).y * sensitivityY;
					rotationX = ClampAngle (rotationX, minimumX, maximumX);
					rotationY = ClampAngle (rotationY, minimumY, maximumY);
					Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
					Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);
					Camera.main.transform.localRotation = originalRotation * xQuaternion * yQuaternion;
				}*/
			}
		}
			
		oldMousePosition = Input.mousePosition;

		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			ToggleRotate();
		}

		if (Time.time - shiftStartTime < timeToShift)
		{
			float shiftPercentage = (Time.time - shiftStartTime) / timeToShift;
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
							if (shiftXFirst) newX = Mathf.Lerp(oldVector.x, newVector.x, stage2Lerp);
							else newX = Mathf.Lerp(newVector.x, oldVector.x, stage1Lerp);
							float newY = Mathf.Lerp(oldVector.y, newVector.y, stage1Lerp);
							float newZ;
							if (!shiftXFirst) newZ = Mathf.Lerp(oldVector.z, newVector.z, stage2Lerp);
							else newZ = Mathf.Lerp(newVector.z, oldVector.z, stage1Lerp);
							chessboard[x,y,z,w].transform.position = new Vector3(newX, newY, newZ);
							
							chessboard[x,y,z,w].transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(oldTileFacing), Quaternion.LookRotation(tileFacing), shiftPercentage);

						}
					}
				}
			}

			Camera.main.transform.position = Vector3.Lerp(oldCameraPosition, targetCameraPosition, shiftPercentage);
			Camera.main.transform.rotation = Quaternion.Lerp(oldCameraRotation, targetCameraRotation, shiftPercentage);
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
							chessboard[x,y,z,w].transform.position = Vector3.MoveTowards(chessboard[x,y,z,w].transform.position, GetTilePosition(x,y,z,w), Time.fixedDeltaTime*10);
							chessboard[x,y,z,w].transform.rotation = Quaternion.RotateTowards(chessboard[x,y,z,w].transform.rotation, Quaternion.LookRotation(tileFacing), Time.fixedDeltaTime*360);
						}
					}
				}
			}
		}
	}
}
