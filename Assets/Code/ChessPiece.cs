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

    public Point4 startPosition;
    public Point4 currentPosition;

    public Point4 forwardZ {
        get {
            if (team == ChessPiece.Team.WHITE)
            {
                return new Point4(0, 0, 1, 0);
            }
            else 
            {
                return new Point4(0, 0, -1, 0);
            }
        }
    }
    public Point4 forwardW {
        get {
            if (team == ChessPiece.Team.WHITE)
            {
                return new Point4(0, 0, 0, 1);
            }
            else 
            {
                return new Point4(0, 0, 0, -1);
            }
        }
    }

    public int x {
        get {
            return currentPosition.x;
        }
    }
    public int y {
        get {
            return currentPosition.y;
        }
    }
    public int z {
        get {
            return currentPosition.z;
        }
    }
    public int w {
        get {
            return currentPosition.w;
        }
    }

    public ChessBoard board;

    public ChessPiece(Type type, Team team, int x, int y, int z, int w, ChessBoard board)
    {
        this.type = type;
        this.team = team;
        currentPosition = new Point4(x, y, z, w);
        startPosition = new Point4(x, y, z, w);
        this.board = board;
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
        return isInBounds(x, y, z, w) && board.pieces[x,y,z,w] != null && board.pieces[x,y,z,w].team != team;
    }

    public bool isFriendlyPiece(int x, int y, int z, int w)
    {
        return isInBounds(x, y, z, w) && board.pieces[x,y,z,w] != null && board.pieces[x,y,z,w].team == team;
    }

    HashSet<Point4> GetDiagonals(int range, bool includeFriendlyAttacks)
    {
        HashSet<Point4> validMoves = new HashSet<Point4>();

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

                    if (isMovableSquare(newX, newY, newZ, newW, includeFriendlyAttacks))  validMoves.Add(new Point4(newX, newY, newZ, newW));
                    if (!isInBounds(newX, newY, newZ, newW) || isFriendlyPiece(newX, newY, newZ, newW) || isEnemyPiece(newX, newY, newZ, newW)) break;
                }
            }
        }

        return validMoves;
    }

    HashSet<Point4> GetOrthoginals(int range, bool includeFriendlyAttacks)
    {
        HashSet<Point4> validMoves = new HashSet<Point4>();


        for (int dir = 0; dir<8; dir++)
        {
            for (int i = 1; i<=range; i++)
            {
                int newX = x + (dir == 0 ? i : 0) - (dir == 4 ? i : 0);
                int newY = y + (dir == 1 ? i : 0) - (dir == 5 ? i : 0);
                int newZ = z + (dir == 2 ? i : 0) - (dir == 6 ? i : 0);
                int newW = w + (dir == 3 ? i : 0) - (dir == 7 ? i : 0);

                if (isMovableSquare(newX, newY, newZ, newW, includeFriendlyAttacks)) validMoves.Add(new Point4(newX, newY, newZ, newW));
                if (!isInBounds(newX, newY, newZ, newW) || isFriendlyPiece(newX, newY, newZ, newW) || isEnemyPiece(newX, newY, newZ, newW)) break;
            }
        }

        return validMoves;
    }

    public HashSet<Point4> GetValidMoves(bool allAttacks = false)
    {
        HashSet<Point4> validMoves = new HashSet<Point4>();

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

            //Fucking en passant
            /*if (board. lastMovedPiece != null && board.lastMovedPiece.type == ChessPiece.Type.PAWN && board.lastMovedPieceLocation == board.lastMovedPiece.startPosition && 
			    (newPosition == board.lastMovedPiece.currentPosition - board.lastMovedPiece.forwardW || newPosition == board.lastMovedPiece.currentPosition - board.lastMovedPiece.forwardZ))
            {
                //En passant kill
                //Fuuck i need listeners to update this shit
                validMoves.Add(newPosition);
            }*/

            if (allAttacks)
            {   
                if (isInBounds(x+1, y, z+forward, w)) validMoves.Add(new Point4(x+1, y, z+forward, w));
                if (isInBounds(x, y+1, z+forward, w)) validMoves.Add(new Point4(x, y+1, z+forward, w));
                if (isInBounds(x-1, y, z+forward, w)) validMoves.Add(new Point4(x-1, y, z+forward, w));
                if (isInBounds(x, y-1, z+forward, w)) validMoves.Add(new Point4(x, y-1, z+forward, w));

                if (isInBounds(x+1, y, z, w+forward)) validMoves.Add(new Point4(x+1, y, z, w+forward));
                if (isInBounds(x, y+1, z, w+forward)) validMoves.Add(new Point4(x, y+1, z, w+forward));
                if (isInBounds(x-1, y, z, w+forward)) validMoves.Add(new Point4(x-1, y, z, w+forward));
                if (isInBounds(x, y-1, z, w+forward)) validMoves.Add(new Point4(x, y-1, z, w+forward));
            }
            else
            {
                if (isMovableSquare(x, y, z+forward, w, false) && !isEnemyPiece(x, y, z+forward, w) && !isFriendlyPiece(x, y, z+forward, w)) 
                {
                    validMoves.Add(new Point4(x, y, z+forward, w));
                    if (board.options.allowPawnDoubleMove && currentPosition == startPosition && isMovableSquare(x, y, z+forward*2, w, false) && !isEnemyPiece(x, y, z+forward*2, w) && !isFriendlyPiece(x, y, z+forward*2, w))
                    {
                        validMoves.Add(new Point4(x, y, z+forward*2, w));
                    }
                }
                if (isMovableSquare(x, y, z, w+forward, false) && !isEnemyPiece(x, y, z, w+forward) && !isFriendlyPiece(x, y, z, w+forward))
                {
                    validMoves.Add(new Point4(x, y, z, w+forward));
                    if (board.options.allowPawnDoubleMove && currentPosition == startPosition && isMovableSquare(x, y, z, w+forward*2, false) && !isEnemyPiece(x, y, z, w+forward) && !isFriendlyPiece(x, y, z, w+forward))
                    {
                        validMoves.Add(new Point4(x, y, z, w+forward*2));
                    }
                } 

                if (isEnemyPiece(x+1, y, z+forward, w)) validMoves.Add(new Point4(x+1, y, z+forward, w));
                if (isEnemyPiece(x, y+1, z+forward, w)) validMoves.Add(new Point4(x, y+1, z+forward, w));
                if (isEnemyPiece(x-1, y, z+forward, w)) validMoves.Add(new Point4(x-1, y, z+forward, w));
                if (isEnemyPiece(x, y-1, z+forward, w)) validMoves.Add(new Point4(x, y-1, z+forward, w));

                if (isEnemyPiece(x+1, y, z, w+forward)) validMoves.Add(new Point4(x+1, y, z, w+forward));
                if (isEnemyPiece(x, y+1, z, w+forward)) validMoves.Add(new Point4(x, y+1, z, w+forward));
                if (isEnemyPiece(x-1, y, z, w+forward)) validMoves.Add(new Point4(x-1, y, z, w+forward));
                if (isEnemyPiece(x, y-1, z, w+forward)) validMoves.Add(new Point4(x, y-1, z, w+forward));
            }

            
        }
        else if (type == Type.KING)
        {
            for (int dir1 = 0; dir1<8; dir1++)
            {
                if (!board.options.kingCanMoveW && (dir1 == 3 || dir1 == 7)) continue;
                if (!board.options.kingCanMoveY && (dir1 == 1 || dir1 == 5)) continue;

                {
                    int newX = x + (dir1 == 0 ? 1 : 0) - (dir1 == 4 ? 1 : 0);
                    int newY = y + (dir1 == 1 ? 1 : 0) - (dir1 == 5 ? 1 : 0);
                    int newZ = z + (dir1 == 2 ? 1 : 0) - (dir1 == 6 ? 1 : 0);
                    int newW = w + (dir1 == 3 ? 1 : 0) - (dir1 == 7 ? 1 : 0);

                    if (isMovableSquare(newX, newY, newZ, newW, allAttacks)) validMoves.Add(new Point4(newX, newY, newZ, newW));
                }

                for (int dir2 = dir1+1; dir2<8; dir2++)
                {
                    if (!board.options.kingCanMoveW && (dir2 == 3 || dir2 == 7)) continue;
                    if (!board.options.kingCanMoveY && (dir2 == 1 || dir2 == 5)) continue;

                    if ((dir1 == 0 && dir2 == 4) || (dir1 == 4 && dir2 == 0)) continue;
                    if ((dir1 == 1 && dir2 == 5) || (dir1 == 5 && dir2 == 1)) continue;
                    if ((dir1 == 2 && dir2 == 6) || (dir1 == 6 && dir2 == 2)) continue;
                    if ((dir1 == 3 && dir2 == 7) || (dir1 == 7 && dir2 == 3)) continue;

                    int newX = x + (dir1 == 0 || dir2 == 0 ? 1 : 0) - (dir1 == 4 || dir2 == 4 ? 1 : 0);
                    int newY = y + (dir1 == 1 || dir2 == 1 ? 1 : 0) - (dir1 == 5 || dir2 == 5 ? 1 : 0);
                    int newZ = z + (dir1 == 2 || dir2 == 2 ? 1 : 0) - (dir1 == 6 || dir2 == 6 ? 1 : 0);
                    int newW = w + (dir1 == 3 || dir2 == 3 ? 1 : 0) - (dir1 == 7 || dir2 == 7 ? 1 : 0);

                    if (isMovableSquare(newX, newY, newZ, newW, allAttacks))  validMoves.Add(new Point4(newX, newY, newZ, newW));
                }
            }
            validMoves.UnionWith(GetDiagonals(1, allAttacks));
        }
        else if (type == Type.QUEEN)
        {
            validMoves = GetOrthoginals(board.maxBoardDimension, allAttacks);
            validMoves.UnionWith(GetDiagonals(board.maxBoardDimension, allAttacks));
        }
        else if (type == Type.ROOK)
        {
            validMoves = GetOrthoginals(board.maxBoardDimension, allAttacks);
        }
        else if (type == Type.BISHOP)
        {
            validMoves = GetDiagonals(board.maxBoardDimension, allAttacks);
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
                        validMoves.Add(new Point4(newX, newY, newZ, newW));
                    }
                }
            }
        }

        return validMoves;
    }
}