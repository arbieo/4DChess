using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	[HideInInspector]
	public int x;
	[HideInInspector]
	public int y;
	[HideInInspector]
	public int z;
	[HideInInspector]
	public int w;

	public GameObject pawnPrefab;
	public GameObject rookPrefab;
	public GameObject bishopPrefab;
	public GameObject knightPrefab;
	public GameObject queenPrefab;
	public GameObject kingPrefab;

	public Material outerBlackMaterial;
	public Material innerBlackMaterial;
	public Material outerWhiteMaterial;
	public Material innerWhiteMaterial;

	public Material outerDarkMidMaterial;
	public Material innerDarkMidMaterial;
	public Material outerLightMidMaterial;
	public Material innerLightMidMaterial;

	public MeshRenderer tileRenderer;
	public GameObject pieceContainer;
	[HideInInspector]
	public GameObject currentPieceGameObject;
	public ChessPiece currentPiece;

	public void Initialize(int x, int y, int z, int w)
	{
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

		tileRenderer.material = tileMaterial;

	}

	public void SetNewPiece(ChessPiece piece)
	{
		if (currentPiece != null)
		{
			//Remove current piece
		}

		currentPiece = piece;
		GameObject piecePrefab = pawnPrefab;
		if (piece.type == ChessPiece.Type.PAWN) piecePrefab = pawnPrefab;
		if (piece.type == ChessPiece.Type.ROOK) piecePrefab = rookPrefab;
		if (piece.type == ChessPiece.Type.BISHOP) piecePrefab = bishopPrefab;
		if (piece.type == ChessPiece.Type.KNIGHT) piecePrefab = knightPrefab;
		if (piece.type == ChessPiece.Type.QUEEN) piecePrefab = queenPrefab;
		if (piece.type == ChessPiece.Type.KING) piecePrefab = kingPrefab;
		currentPieceGameObject = GameObject.Instantiate(piecePrefab, transform.position, transform.rotation);
		currentPieceGameObject.transform.SetParent(pieceContainer.transform);
	}
}
