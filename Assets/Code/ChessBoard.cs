using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChessBoard
{
	public int maxBoardDimension = 4;
	public ChessPiece[,,,] pieces;

	public Point4 size;

	public ChessVariantOptions options;

    public struct Move
    {
        public Point4 startPosition;
        public Point4 endPosition;
		public ChessPiece pieceMoved;
		public ChessPiece pieceCaptured;
    }
	public List<Move> moveHistory = new List<Move>();

	public ChessPiece.Team currentMove = ChessPiece.Team.WHITE;

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

	public ChessBoard DeepCopy()
	{
		//Replace with serialization
		ChessBoard copy = new ChessBoard(size, options);

		copy.moveHistory = new List<Move>(moveHistory);
		copy.currentMove = currentMove;

		foreach (ChessPiece piece in pieces)
		{
			if (piece != null)
			{
				ChessPiece newPiece = piece.DeepCopy();
				newPiece.board = copy;
				copy.pieces[piece.x, piece.y, piece.z, piece.w] = newPiece;
			}
		}

		return copy;
	}

	public Move GetMove(Point4 start, Point4 end)
	{
		Move move = new Move();
		move.startPosition = start;
		move.endPosition = end;
		move.pieceMoved = GetPiece(start);
		move.pieceCaptured = GetPiece(end);
		return move;
	}

	public ChessPiece GetPiece(Point4 position)
	{
		return pieces[position.x, position.y, position.z, position.w];
	}

	public void SetPiece(Point4 position, ChessPiece piece)
	{
		pieces[position.x, position.y, position.z, position.w] = piece;
		if (piece != null)
		{
			piece.currentPosition = position;
		}
		OnTileUpdate(position);
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
		Point4 position = new Point4(x, y, z, w);
		SetPiece(position, new ChessPiece(type, team, position, this));
	}

	public void Undo()
	{
		Move lastMove = moveHistory[moveHistory.Count -1];
		SetPiece(lastMove.startPosition, lastMove.pieceMoved);

		//Be explicit because en passant and promotion
		SetPiece(lastMove.endPosition, null);
		if (lastMove.pieceCaptured != null)
		{
			SetPiece(lastMove.pieceCaptured.currentPosition, lastMove.pieceCaptured);
		}

		moveHistory.RemoveAt(moveHistory.Count -1);

		if(currentMove == ChessPiece.Team.WHITE)
		{
			currentMove = ChessPiece.Team.BLACK;
		}
		else
		{
			currentMove = ChessPiece.Team.WHITE;
		}
	}

	public void MovePiece(Move move)
	{
		ChessPiece piece = move.pieceMoved;
		Point4 newPosition = move.endPosition;
		ChessPiece capturedPiece = pieces[piece.x, piece.y, piece.z, piece.w];
		Point4 movedFrom = move.startPosition;

		SetPiece(newPosition, piece);
		SetPiece(movedFrom, null);

		/*if (moveHistory.Count > 0)
		{
			lastMove = moveHistory[moveHistory.Count - 1];
		}

		if (lastMove != null && lastMove.pieceMoved.type == ChessPiece.Type.PAWN && 
			piece.type == ChessPiece.Type.PAWN && lastMove.startPosition == lastMove.pieceMoved.startPosition && 
			(newPosition == lastMove.pieceMoved.currentPosition - lastMove.pieceMoved.forwardW 
				|| newPosition == lastMove.pieceMoved.currentPosition - lastMove.pieceMoved.forwardZ))
		{
			SetPiece(lastMove.pieceMoved.currentPosition, null);
		}*/
		
		if (piece.type == ChessPiece.Type.PAWN)
		{
			if ((piece.team == ChessPiece.Team.WHITE && piece.z == size.z - 1 && piece.w == size.w - 1)
				|| (piece.team == ChessPiece.Team.BLACK && piece.z == 0 && piece.w == 0))
			{
				ChessPiece promotedPiece = piece.DeepCopy();
				promotedPiece.type = ChessPiece.Type.QUEEN;
				SetPiece(newPosition, promotedPiece);
			}
		}

		moveHistory.Add(move);

		/*if (capturedPiece.type == ChessPiece.Type.KING)
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
		}*/

		if(currentMove == ChessPiece.Team.WHITE)
		{
			currentMove = ChessPiece.Team.BLACK;
		}
		else
		{
			currentMove = ChessPiece.Team.WHITE;
		}

		
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