using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChessPiece
{

    public enum Type
	{
		PAWN,
		ROOK,
		BISHOP,
		KNIGHT,
		QUEEN,
		KING
	}

    public enum Team
    {
        WHITE,
        BLACK
    }

    public Type type;
    public Team team;

    public int x;
    public int y;
    public int z;
    public int w;

    public bool hasMoved = false;

    public ChessPiece(Type type, Team team, int x, int y, int z, int w)
    {
        this.type = type;
        this.team = team;
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public bool isMovableSquare(int x, int y, int z, int w, bool includeFriendlyAttacks)
    {
        return isInBounds(x, y, z, w) && (includeFriendlyAttacks || !isFriendlyPiece(x, y, z, w));
    }

    public bool isInBounds(int x, int y, int z, int w)
    {
        return x >= 0 && x <= 3 && y >=0 && y<=3 && z >=0 && z <= 3 && w >= 0 && w<=3;
    }

    public bool isEnemyPiece(int x, int y, int z, int w)
    {
        return isInBounds(x, y, z, w) && ChessboardController.instance.chessboard[x,y,z,w].currentPiece != null && ChessboardController.instance.chessboard[x,y,z,w].currentPiece.team != team;
    }

    public bool isFriendlyPiece(int x, int y, int z, int w)
    {
        return isInBounds(x, y, z, w) && ChessboardController.instance.chessboard[x,y,z,w].currentPiece != null && ChessboardController.instance.chessboard[x,y,z,w].currentPiece.team == team;
    }

    HashSet<Vector4> GetDiagonals(int range, bool includeFriendlyAttacks)
    {
        HashSet<Vector4> validMoves = new HashSet<Vector4>();

        for (int dir1 = 0; dir1<8; dir1++)
        {
            for (int dir2 = dir1+1; dir2<8; dir2++)
            {
                if ((dir1 == 0 && dir2 == 4) || (dir1 == 4 && dir2 == 0)) continue;
                if ((dir1 == 1 && dir2 == 5) || (dir1 == 5 && dir2 == 1)) continue;
                if ((dir1 == 2 && dir2 == 6) || (dir1 == 6 && dir2 == 2)) continue;
                if ((dir1 == 3 && dir2 == 7) || (dir1 == 7 && dir2 == 3)) continue;

                for (int i = 1; i<=range; i++)
                {   
                    int newX = x + (dir1 == 0 || dir2 == 0 ? i : 0) - (dir1 == 4 || dir2 == 4 ? i : 0);
                    int newY = y + (dir1 == 1 || dir2 == 1 ? i : 0) - (dir1 == 5 || dir2 == 5 ? i : 0);
                    int newZ = z + (dir1 == 2 || dir2 == 2 ? i : 0) - (dir1 == 6 || dir2 == 6 ? i : 0);
                    int newW = w + (dir1 == 3 || dir2 == 3 ? i : 0) - (dir1 == 7 || dir2 == 7 ? i : 0);

                    if (isMovableSquare(newX, newY, newZ, newW, includeFriendlyAttacks))  validMoves.Add(new Vector4(newX, newY, newZ, newW));
                    if (!isInBounds(newX, newY, newZ, newW) || isFriendlyPiece(newX, newY, newZ, newW) || isEnemyPiece(newX, newY, newZ, newW)) break;
                }
            }
        }

        return validMoves;
    }

    HashSet<Vector4> GetOrthoginals(int range, bool includeFriendlyAttacks)
    {
        HashSet<Vector4> validMoves = new HashSet<Vector4>();


        for (int dir = 0; dir<8; dir++)
        {
            for (int i = 1; i<=range; i++)
            {
                int newX = x + (dir == 0 ? i : 0) - (dir == 4 ? i : 0);
                int newY = y + (dir == 1 ? i : 0) - (dir == 5 ? i : 0);
                int newZ = z + (dir == 2 ? i : 0) - (dir == 6 ? i : 0);
                int newW = w + (dir == 3 ? i : 0) - (dir == 7 ? i : 0);

                if (isMovableSquare(newX, newY, newZ, newW, includeFriendlyAttacks)) validMoves.Add(new Vector4(newX, newY, newZ, newW));
                if (!isInBounds(newX, newY, newZ, newW) || isFriendlyPiece(newX, newY, newZ, newW) || isEnemyPiece(newX, newY, newZ, newW)) break;
            }
        }

        return validMoves;
    }

    public HashSet<Vector4> GetValidMoves(bool allAttacks = false)
    {
        HashSet<Vector4> validMoves = new HashSet<Vector4>();

        if (type == Type.PAWN) {
            int forward;
            if (team == Team.WHITE)
            {
                forward = 1;
            }
            else
            {
                forward = -1;
            }

            if (allAttacks)
            {   
                if (isInBounds(x+1, y, z+forward, w)) validMoves.Add(new Vector4(x+1, y, z+forward, w));
                if (isInBounds(x, y+1, z+forward, w)) validMoves.Add(new Vector4(x, y+1, z+forward, w));
                if (isInBounds(x-1, y, z+forward, w)) validMoves.Add(new Vector4(x-1, y, z+forward, w));
                if (isInBounds(x, y-1, z+forward, w)) validMoves.Add(new Vector4(x, y-1, z+forward, w));

                if (isInBounds(x+1, y, z, w+forward)) validMoves.Add(new Vector4(x+1, y, z, w+forward));
                if (isInBounds(x, y+1, z, w+forward)) validMoves.Add(new Vector4(x, y+1, z, w+forward));
                if (isInBounds(x-1, y, z, w+forward)) validMoves.Add(new Vector4(x-1, y, z, w+forward));
                if (isInBounds(x, y-1, z, w+forward)) validMoves.Add(new Vector4(x, y-1, z, w+forward));
            }
            else
            {
                if (isMovableSquare(x, y, z+forward, w, false) && !isEnemyPiece(x, y, z+forward, w) && !isFriendlyPiece(x, y, z+forward, w)) 
                {
                    validMoves.Add(new Vector4(x, y, z+forward, w));
                    if (!hasMoved && isMovableSquare(x, y, z+forward*2, w, false) && !isEnemyPiece(x, y, z+forward*2, w) && !isFriendlyPiece(x, y, z+forward*2, w))
                    {
                        //validMoves.Add(new Vector4(x, y, z+forward*2, w));
                    }
                }
                if (isMovableSquare(x, y, z, w+forward, false) && !isEnemyPiece(x, y, z, w+forward) && !isFriendlyPiece(x, y, z, w+forward))
                {
                    validMoves.Add(new Vector4(x, y, z, w+forward));
                    if (!hasMoved && isMovableSquare(x, y, z, w+forward*2, false) && !isEnemyPiece(x, y, z, w+forward) && !isFriendlyPiece(x, y, z, w+forward))
                    {
                        //validMoves.Add(new Vector4(x, y, z, w+forward*2));
                    }
                } 

                if (isEnemyPiece(x+1, y, z+forward, w)) validMoves.Add(new Vector4(x+1, y, z+forward, w));
                if (isEnemyPiece(x, y+1, z+forward, w)) validMoves.Add(new Vector4(x, y+1, z+forward, w));
                if (isEnemyPiece(x-1, y, z+forward, w)) validMoves.Add(new Vector4(x-1, y, z+forward, w));
                if (isEnemyPiece(x, y-1, z+forward, w)) validMoves.Add(new Vector4(x, y-1, z+forward, w));

                if (isEnemyPiece(x+1, y, z, w+forward)) validMoves.Add(new Vector4(x+1, y, z, w+forward));
                if (isEnemyPiece(x, y+1, z, w+forward)) validMoves.Add(new Vector4(x, y+1, z, w+forward));
                if (isEnemyPiece(x-1, y, z, w+forward)) validMoves.Add(new Vector4(x-1, y, z, w+forward));
                if (isEnemyPiece(x, y-1, z, w+forward)) validMoves.Add(new Vector4(x, y-1, z, w+forward));
            }

            
        }
        else if (type == Type.KING)
        {
            validMoves = GetOrthoginals(1, allAttacks);
            validMoves.UnionWith(GetDiagonals(1, allAttacks));
        }
        else if (type == Type.QUEEN)
        {
            validMoves = GetOrthoginals(4, allAttacks);
            validMoves.UnionWith(GetDiagonals(4, allAttacks));
        }
        else if (type == Type.ROOK)
        {
            validMoves = GetOrthoginals(4, allAttacks);
        }
        else if (type == Type.BISHOP)
        {
            validMoves = GetDiagonals(4, allAttacks);
        }
        else if (type == Type.KNIGHT)
        {
            for (int dir1 = 0; dir1<8; dir1++)
            {
                for (int dir2 = 0; dir2<8; dir2++)
                {
                    if (dir1 == dir2) continue;
                    if ((dir1 == 0 && dir2 == 4) || (dir1 == 4 && dir2 == 0))continue;
                    if ((dir1 == 1 && dir2 == 5) || (dir1 == 5 && dir2 == 1)) continue;
                    if ((dir1 == 2 && dir2 == 6) || (dir1 == 6 && dir2 == 2)) continue;
                    if ((dir1 == 3 && dir2 == 7) || (dir1 == 7 && dir2 == 3)) continue;

                    int newX = x + (dir1 == 0 ? 1 : 0) - (dir1 == 4 ? 1 : 0) + (dir2 == 0 ? 2 : 0) - (dir2 == 4 ? 2 : 0);
                    int newY = y + (dir1 == 1 ? 1 : 0) - (dir1 == 5 ? 1 : 0) + (dir2 == 1 ? 2 : 0) - (dir2 == 5 ? 2 : 0);
                    int newZ = z + (dir1 == 2 ? 1 : 0) - (dir1 == 6 ? 1 : 0) + (dir2 == 2 ? 2 : 0) - (dir2 == 6 ? 2 : 0);
                    int newW = w + (dir1 == 3 ? 1 : 0) - (dir1 == 7 ? 1 : 0) + (dir2 == 3 ? 2 : 0) - (dir2 == 7 ? 2 : 0);

                    if (isMovableSquare(newX, newY, newZ, newW, allAttacks)) 
                    {
                        validMoves.Add(new Vector4(newX, newY, newZ, newW));
                    }
                }
            }
        }

        return validMoves;
    }
}