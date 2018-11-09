using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChessBoard
{
	public int maxBoardDimension = 4;
	public ChessPiece[,,,] pieces;

	public Point4 size;

	public ChessVariantOptions options;

	public ChessPiece lastMovedPiece;
	public Point4 lastMovedPieceLocation;
	public ChessPiece lastMoveCapturedPiece;

	Dictionary<Point4, HashSet<TileUpdateCallback>> tileUpdateCallbacks = new Dictionary<Point4, HashSet<TileUpdateCallback>>();
	HashSet<PromotionRequiredCallback> promotionRequiredCallbacks = new HashSet<PromotionRequiredCallback>();

	public delegate void TileUpdateCallback();
	public delegate void PromotionRequiredCallback(Point4 location);

	public ChessBoard(Point4 size, ChessVariantOptions options)
	{
		this.size = size;

		pieces = new ChessPiece[size.x,size.y,size.z,size.w];
		maxBoardDimension = Mathf.Max(size.x, size.y, size.z, size.w);
		this.options = options;
	}

	public ChessPiece GetPiece(Point4 position)
	{
		return pieces[position.x, position.y, position.z, position.w];
	}

	public static ChessBoard ClassicVariantBoard()
	{
		ChessVariantOptions options = new ChessVariantOptions();
		Point4 size = new Point4(4, 4, 4, 4);
		ChessBoard board = new ChessBoard(size, options);
		
		/*
		WHITE TEAM
		 */

		board.AddChessPiece(0,1,0,0,ChessPiece.Type.BISHOP, ChessPiece.Team.WHITE);
		board.AddChessPiece(1,1,0,0,ChessPiece.Type.KING, ChessPiece.Team.WHITE);
		board.AddChessPiece(2,1,0,0,ChessPiece.Type.QUEEN, ChessPiece.Team.WHITE);
		board.AddChessPiece(3,1,0,0,ChessPiece.Type.BISHOP, ChessPiece.Team.WHITE);

		board.AddChessPiece(1,1,1,0,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(2,1,1,0,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);

		board.AddChessPiece(0,2,0,0,ChessPiece.Type.ROOK, ChessPiece.Team.WHITE);
		board.AddChessPiece(1,2,0,0,ChessPiece.Type.KNIGHT, ChessPiece.Team.WHITE);
		board.AddChessPiece(2,2,0,0,ChessPiece.Type.KNIGHT, ChessPiece.Team.WHITE);
		board.AddChessPiece(3,2,0,0,ChessPiece.Type.ROOK, ChessPiece.Team.WHITE);

		board.AddChessPiece(1,2,1,0,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(2,2,1,0,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);

		board.AddChessPiece(0,2,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(3,2,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);

		board.AddChessPiece(0,1,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(1,1,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(2,1,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(3,1,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(1,1,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(2,1,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);

		board.AddChessPiece(0,2,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(3,2,1,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(1,2,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);
		board.AddChessPiece(2,2,0,1,ChessPiece.Type.PAWN, ChessPiece.Team.WHITE);

		/*
		BLACK TEAM
		 */

		board.AddChessPiece(0,1,3,3,ChessPiece.Type.BISHOP, ChessPiece.Team.BLACK);
		board.AddChessPiece(1,1,3,3,ChessPiece.Type.KING, ChessPiece.Team.BLACK);
		board.AddChessPiece(2,1,3,3,ChessPiece.Type.QUEEN, ChessPiece.Team.BLACK);
		board.AddChessPiece(3,1,3,3,ChessPiece.Type.BISHOP, ChessPiece.Team.BLACK);

		board.AddChessPiece(1,1,2,3,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(2,1,2,3,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);

		board.AddChessPiece(0,2,3,3,ChessPiece.Type.ROOK, ChessPiece.Team.BLACK);
		board.AddChessPiece(1,2,3,3,ChessPiece.Type.KNIGHT, ChessPiece.Team.BLACK);
		board.AddChessPiece(2,2,3,3,ChessPiece.Type.KNIGHT, ChessPiece.Team.BLACK);
		board.AddChessPiece(3,2,3,3,ChessPiece.Type.ROOK, ChessPiece.Team.BLACK);

		board.AddChessPiece(1,2,2,3,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(2,2,2,3,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);

		board.AddChessPiece(0,2,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(3,2,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);

		board.AddChessPiece(0,1,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(1,1,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(2,1,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(3,1,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(1,1,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(2,1,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);

		board.AddChessPiece(0,2,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(3,2,2,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(1,2,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);
		board.AddChessPiece(2,2,3,2,ChessPiece.Type.PAWN, ChessPiece.Team.BLACK);

		return board;
	}

	public void AddChessPiece(int x, int y, int z, int w, ChessPiece.Type type, ChessPiece.Team team)
	{
		pieces[x,y,z,w] = new ChessPiece(type, team, x, y, z, w, this);
		OnTileUpdate(new Point4(x, y, z, w));
	}

	public void MovePiece(ChessPiece piece, Point4 newPosition)
	{
		ChessPiece capturedPiece = pieces[piece.x, piece.y, piece.z, piece.w];
		Point4 movedFrom = new Point4(piece.x, piece.y, piece.z, piece.w);

		pieces[newPosition.x, newPosition.y, newPosition.z, newPosition.w] = piece;
		OnTileUpdate(newPosition);
		pieces[piece.x, piece.y, piece.z, piece.w] = null;
		OnTileUpdate(piece.currentPosition);
		piece.currentPosition = newPosition;

		if (lastMovedPiece != null && lastMovedPiece.type == ChessPiece.Type.PAWN && 
			piece.type == ChessPiece.Type.PAWN && lastMovedPieceLocation == lastMovedPiece.startPosition && 
			(newPosition == lastMovedPiece.currentPosition - lastMovedPiece.forwardW 
				|| newPosition == lastMovedPiece.currentPosition - lastMovedPiece.forwardZ))
		{
			pieces[lastMovedPiece.x, lastMovedPiece.y, lastMovedPiece.z, lastMovedPiece.w] = null;
			OnTileUpdate(lastMovedPiece.currentPosition);
		}
		
		if (piece.type == ChessPiece.Type.PAWN)
		{
			if ((piece.team == ChessPiece.Team.WHITE && piece.z == size.z - 1 && piece.w == size.w - 1)
				|| (piece.team == ChessPiece.Team.BLACK && piece.z == 0 && piece.w == 0))
			{
				piece.type = ChessPiece.Type.QUEEN;
			}
		}

		lastMovedPiece = piece;
		lastMovedPieceLocation = movedFrom;
		lastMoveCapturedPiece = capturedPiece;
	}

	public void RegisterTileUpdateCallback(Point4 location, TileUpdateCallback callback)
	{
		if (!tileUpdateCallbacks.ContainsKey(location))
		{
			tileUpdateCallbacks[location] = new HashSet<TileUpdateCallback>();
		}
		tileUpdateCallbacks[location].Add(callback);
	}

	public void RemoveTileUpdateCallback(Point4 location, TileUpdateCallback callback)
	{
		if (tileUpdateCallbacks.ContainsKey(location))
		{
			tileUpdateCallbacks[location].Remove(callback);
		}
	}

	void OnTileUpdate(Point4 location)
	{
		if (tileUpdateCallbacks.ContainsKey(location))
		{
			foreach (TileUpdateCallback callback in tileUpdateCallbacks[location])
			{
				callback();
			}
		}
	}

	public void RegisterPromotionRequiredCallback(PromotionRequiredCallback callback)
	{
		promotionRequiredCallbacks.Add(callback);
	}

	public void RemovePromotionRequiredCallback(PromotionRequiredCallback callback)
	{
		promotionRequiredCallbacks.Remove(callback);
	}

	void OnPromotionRequired(Point4 location)
	{
		foreach (PromotionRequiredCallback callback in promotionRequiredCallbacks)
		{
			callback(location);
		}
	}
}