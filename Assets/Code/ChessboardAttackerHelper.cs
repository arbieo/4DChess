using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChessboardAttackerHelper
{
	public List<ChessPiece>[,,,] attackers;
    ChessBoard board;

    public ChessboardAttackerHelper(ChessBoard board)
    {
        this.board = board;

        attackers = new List<ChessPiece>[board.size.x, board.size.y, board.size.z, board.size.w];
        ComputeAttackers();
    }

    public void ComputeAttackers()
    {
        for(int x = 0; x<board.size.x; x++)
		{
			for(int y = 0; y<board.size.y; y++)
			{
				for(int z = 0; z<board.size.z; z++)
				{
					for(int w = 0; w<board.size.w; w++)
					{
						attackers[x,y,z,w] = new List<ChessPiece>();
					}
				}
			}
		}

		foreach (ChessPiece piece in board.pieces)
		{
			if (piece != null)
			{
				foreach (Point4 attackedTile in piece.GetValidMoves(true))
				{
					attackers[(int)attackedTile.x,(int)attackedTile.y, (int)attackedTile.z, (int)attackedTile.w].Add(piece);
				}
			}
		}
    }

	public List<ChessPiece> GetAttackers(Point4 position)
	{
		return attackers[position.x, position.y, position.z, position.w];
	}
}