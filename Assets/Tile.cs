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

	public Material whitePieceMaterial;
	public Material blackPieceMaterial;
	public Material selectedPieceMaterial;
	public Material attackedPieceMaterial;

	public Material highlightedTileMaterial;
	public Material selectedTileMaterial;
	public Material attackedTileMaterial;

	public MeshRenderer tileRenderer;
	public GameObject pieceContainer;
	[HideInInspector]
	public GameObject currentPieceGameObject;
	public ChessPiece currentPiece;

	public bool highlighted;
	public bool selected;

	public void Initialize(int x, int y, int z, int w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
		UpdateMaterials();
	}

	public void Select()
	{
		selected = true;
		UpdateMaterials();
	}

	public void Highlight()
	{
		highlighted = true;
		UpdateMaterials();
	}

	public void ClearSelection()
	{
		highlighted = false;
		selected = false;
		UpdateMaterials();
	}

	public void SetPieceMaterial(Material material)
	{
		MeshRenderer[] renderers = currentPieceGameObject.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer renderer in renderers)
		{
			renderer.material = material;
		}
	}

	public void UpdateMaterials()
	{
		if (selected)
		{
			if (currentPiece != null)
			{
				SetPieceMaterial(selectedPieceMaterial);
			}
			tileRenderer.material = selectedTileMaterial;
		}
		else if (highlighted && currentPiece == null)
		{
			tileRenderer.material = highlightedTileMaterial;
		}
		else if (highlighted)
		{
			SetPieceMaterial(attackedPieceMaterial);
			tileRenderer.material = highlightedTileMaterial;
		}
		else
		{
			if (currentPiece != null && currentPiece.team == ChessPiece.Team.WHITE)
			{
				SetPieceMaterial(whitePieceMaterial);
			}
			else if (currentPiece != null && currentPiece.team == ChessPiece.Team.BLACK)
			{
				SetPieceMaterial(blackPieceMaterial);
			}

			bool isDark = false;
			if ((x+y+z+w) % 2 == 0)
			{
				isDark = true;
			}

			if (w == 0 || w == 3)
			{
				if (x == 0 || x == 3 || z == 0 || z == 3)
				{
					tileRenderer.material = isDark ? outerBlackMaterial : outerWhiteMaterial;
				}
				else
				{
					tileRenderer.material = isDark ? innerBlackMaterial : innerWhiteMaterial;
				}
			}
			else
			{
				if (x == 0 || x == 3 || z == 0 || z == 3)
				{
					tileRenderer.material = isDark ? outerDarkMidMaterial : outerLightMidMaterial;
				}
				else
				{
					tileRenderer.material = isDark ? innerDarkMidMaterial : innerLightMidMaterial;
				}
			}
		}
	}

	public void SetPiece(ChessPiece piece)
	{
		if (currentPiece != null)
		{
			GameObject.Destroy(currentPieceGameObject);
			currentPieceGameObject = null;

			currentPiece = null;
		}

		currentPiece = piece;
		if (piece != null)
		{
			GameObject piecePrefab = pawnPrefab;
			if (piece.type == ChessPiece.Type.PAWN) piecePrefab = pawnPrefab;
			if (piece.type == ChessPiece.Type.ROOK) piecePrefab = rookPrefab;
			if (piece.type == ChessPiece.Type.BISHOP) piecePrefab = bishopPrefab;
			if (piece.type == ChessPiece.Type.KNIGHT) piecePrefab = knightPrefab;
			if (piece.type == ChessPiece.Type.QUEEN) piecePrefab = queenPrefab;
			if (piece.type == ChessPiece.Type.KING) piecePrefab = kingPrefab;
			currentPieceGameObject = GameObject.Instantiate(piecePrefab, transform.position, transform.rotation);
			currentPieceGameObject.transform.SetParent(pieceContainer.transform);

			piece.x = x;
			piece.y = y;
			piece.z = z;
			piece.w = w;
		}
		
		UpdateMaterials();
	}
}
