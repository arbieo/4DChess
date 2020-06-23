using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile2D : MonoBehaviour {

	public Point4 position;

	public Sprite whitePawnSprite;
	public Sprite whiteRookSprite;
	public Sprite whiteBishopSprite;
	public Sprite whiteKnightSprite;
	public Sprite whiteQueenSprite;
	public Sprite whiteKingSprite;

	public Sprite blackPawnSprite;
	public Sprite blackRookSprite;
	public Sprite blackBishopSprite;
	public Sprite blackKnightSprite;
	public Sprite blackQueenSprite;
	public Sprite blackKingSprite;

	public Color whiteTileColor;
	public Color blackTileColor;

	public Color highlightedTileColor;
	public Color selectedTileColor;
	public Color attackedTileColor;
	public Color movedTileColor;
	public Color checkTileColor;

	public SpriteRenderer tileRenderer;
	public SpriteRenderer pieceRenderer;
	ChessBoard board;

	public bool highlighted;
	public bool selected;
	public bool attacked;
	public bool moved;
	public bool check;

	public void Initialize(Point4 position, ChessBoard board)
	{
		this.position = position;
		this.board = board;

		UpdatePiece();
		board.RegisterTileUpdateCallback(position, UpdatePiece);
	}

	public void Select()
	{
		selected = true;
		UpdatePiece();
	}

	public void Highlight()
	{
		highlighted = true;
		UpdatePiece();
	}

	public void Attacked()
	{
		attacked = true;
		UpdatePiece();
	}

	public void Moved()
	{
		moved = true;
		UpdatePiece();
	}

	public void Check()
	{
		check = true;
		UpdatePiece();
	}

	public void ClearSelection()
	{
		highlighted = false;
		selected = false;
		attacked = false;
		moved = false;
		check = false;
		UpdatePiece();
	}

	public void UpdatePiece()
	{
		ChessPiece currentPiece = board.GetPiece(position);
		
		Sprite pieceSprite = null;
		if (currentPiece != null)
		{
			if (currentPiece.team == ChessPiece.Team.WHITE)
			{
				if (currentPiece.type == ChessPiece.Type.PAWN) pieceSprite = whitePawnSprite;
				if (currentPiece.type == ChessPiece.Type.ROOK) pieceSprite = whiteRookSprite;
				if (currentPiece.type == ChessPiece.Type.BISHOP) pieceSprite = whiteBishopSprite;
				if (currentPiece.type == ChessPiece.Type.KNIGHT) pieceSprite = whiteKnightSprite;
				if (currentPiece.type == ChessPiece.Type.QUEEN) pieceSprite = whiteQueenSprite;
				if (currentPiece.type == ChessPiece.Type.KING) pieceSprite = whiteKingSprite;
			}
			else{
				if (currentPiece.type == ChessPiece.Type.PAWN) pieceSprite = blackPawnSprite;
				if (currentPiece.type == ChessPiece.Type.ROOK) pieceSprite = blackRookSprite;
				if (currentPiece.type == ChessPiece.Type.BISHOP) pieceSprite = blackBishopSprite;
				if (currentPiece.type == ChessPiece.Type.KNIGHT) pieceSprite = blackKnightSprite;
				if (currentPiece.type == ChessPiece.Type.QUEEN) pieceSprite = blackQueenSprite;
				if (currentPiece.type == ChessPiece.Type.KING) pieceSprite = blackKingSprite;
			}
		}
		pieceRenderer.sprite = pieceSprite;

		
		if (selected)
		{
			tileRenderer.color = selectedTileColor;
		}
		else if (check)
		{
			tileRenderer.color = checkTileColor;
		}
		else if (attacked)
		{
			tileRenderer.color = attackedTileColor;
		}
		else if (highlighted)
		{
			tileRenderer.color = highlightedTileColor;
		}
		else if (moved)
		{
			tileRenderer.color = movedTileColor;
		}
		else
		{
			if ((position.x+position.y+position.z+position.w) % 2 == 0)
			{
				tileRenderer.color = whiteTileColor;
			}
			else
			{
				tileRenderer.color = blackTileColor;
			}

		}
	}
}
