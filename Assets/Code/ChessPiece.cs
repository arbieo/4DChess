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

    public ChessPiece(Type type, Team team, Point4 position, ChessBoard board)
    {
        this.type = type;
        this.team = team;
        currentPosition = position;
        startPosition = position;
        this.board = board;
    }

    public ChessPiece DeepCopy()
    {
        return (ChessPiece)this.MemberwiseClone();
    }

    public IEnumerable<Point4> GetValidMoves(bool allAttacks = false)
    {
        return new ValidMoves(this, allAttacks);
    }

    public bool IsValidMove(Point4 position)
    {
        foreach(Point4 move in GetValidMoves())
        {
            if (move == position) return true;
        }
        return false;
    }

    class ValidMoves : IEnumerable<Point4>
    {
        bool allAttacks = false;
        ChessPiece piece;

        static Point4 up = new Point4(0, 1, 0, 0);
        static Point4 right = new Point4(1, 0, 0, 0);
        static Point4 forward = new Point4(0, 0, 1, 0);
        static Point4 forward4D = new Point4(0, 0, 0, 1);

        static Point4[] directions = new Point4[8];

        public ValidMoves(ChessPiece piece, bool allAttacks = false)
        {
            this.piece = piece;
            this.allAttacks = allAttacks;

            if (piece.team == Team.WHITE)
            {
                forward = new Point4(0, 0, 1, 0);
                forward4D = new Point4(0, 0, 0, 1);
            }
            else
            {
                forward = new Point4(0, 0, -1, 0);
                forward4D = new Point4(0, 0, 0, -1);
            }

            directions[0] = right;
            directions[1] = up;
            directions[2] = forward;
            directions[3] = forward4D;
            directions[4] = right * -1;
            directions[5] = up * -1;
            directions[6] = forward * -1;
            directions[7] = forward4D * -1;
        }

        public bool isMovableSquare(Point4 pos, bool includeFriendlyAttacks)
        {
            return isInBounds(pos) && (includeFriendlyAttacks || !isFriendlyPiece(pos));
        }

        public bool isInBounds(Point4 pos)
        {
            ChessBoard board = piece.board;
            return pos.x >= 0 && pos.x <= board.size.x-1 && pos.y >=0 && pos.y<=board.size.y-1 
                && pos.z >=0 && pos.z <= board.size.z-1 && pos.w >= 0 && pos.w<=board.size.w-1;
        }

        public bool isEnemyPiece(Point4 pos)
        {
            return isInBounds(pos) && piece.board.GetPiece(pos) != null && piece.board.GetPiece(pos).team != piece.team;
        }

        public bool isFriendlyPiece(Point4 pos)
        {
            return isInBounds(pos) && piece.board.GetPiece(pos) != null && piece.board.GetPiece(pos).team == piece.team;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // Invoke IEnumerator<string> GetEnumerator() above.
            return GetEnumerator();
        }

        public IEnumerator<Point4> GetEnumerator(){
            Point4 currentPosition = piece.currentPosition;
            Point4 scratchPoint;

            if (piece.type == Type.PAWN) {

                if (allAttacks)
                {   
                    scratchPoint = currentPosition + right + forward;
                    if (isInBounds(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition + up + forward;
                    if (isInBounds(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition - right + forward;
                    if (isInBounds(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition - up + forward;
                    if (isInBounds(scratchPoint)) yield return scratchPoint;

                    scratchPoint = currentPosition + right + forward4D;
                    if (isInBounds(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition + up + forward4D;
                    if (isInBounds(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition - right + forward4D;
                    if (isInBounds(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition - up + forward4D;
                    if (isInBounds(scratchPoint)) yield return scratchPoint;
                }
                else
                {
                    scratchPoint = currentPosition + forward;
                    if (isMovableSquare(scratchPoint, false) && !isEnemyPiece(scratchPoint) && !isFriendlyPiece(scratchPoint)) 
                    {
                        yield return scratchPoint;
                        if (piece.board.options.allowPawnDoubleMove && currentPosition == piece.startPosition)
                        {
                            scratchPoint = currentPosition + forward*2;
                            if(isMovableSquare(scratchPoint, false) && !isEnemyPiece(scratchPoint) && !isFriendlyPiece(scratchPoint))
                            {
                                yield return scratchPoint;
                            }
                        }
                    }
                    scratchPoint = currentPosition + forward4D;
                    if (isMovableSquare(scratchPoint, false) && !isEnemyPiece(scratchPoint) && !isFriendlyPiece(scratchPoint)) 
                    {
                        yield return scratchPoint;
                        if (piece.board.options.allowPawnDoubleMove && currentPosition == piece.startPosition)
                        {
                            scratchPoint = currentPosition + forward4D*2;
                            if(isMovableSquare(scratchPoint, false) && !isEnemyPiece(scratchPoint) && !isFriendlyPiece(scratchPoint))
                            {
                                yield return scratchPoint;
                            }
                        }
                    }

                    scratchPoint = currentPosition + right + forward;
                    if (isEnemyPiece(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition + up + forward;
                    if (isEnemyPiece(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition - right + forward;
                    if (isEnemyPiece(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition - up + forward;
                    if (isEnemyPiece(scratchPoint)) yield return scratchPoint;

                    scratchPoint = currentPosition + right + forward4D;
                    if (isEnemyPiece(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition + up + forward4D;
                    if (isEnemyPiece(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition - right + forward4D;
                    if (isEnemyPiece(scratchPoint)) yield return scratchPoint;
                    scratchPoint = currentPosition - up + forward4D;
                    if (isEnemyPiece(scratchPoint)) yield return scratchPoint;
                }

                
            }
            else if (piece.type == Type.KING)
            {
                for (int dir1 = 0; dir1<directions.Length; dir1++)
                {

                    if (!piece.board.options.kingCanMoveW && (dir1 == 3 || dir1 == 7)) continue;
                    if (!piece.board.options.kingCanMoveY && (dir1 == 1 || dir1 == 5)) continue;

                    {
                        scratchPoint = currentPosition + directions[dir1];
                        if (isMovableSquare(scratchPoint, allAttacks)) yield return scratchPoint;
                    }

                    for (int dir2 = dir1+1; dir2<directions.Length; dir2++)
                    {
                        if (dir1 == dir2) continue;
                        if (!piece.board.options.kingCanMoveW && (dir2 == 3 || dir2 == 7)) continue;
                        if (!piece.board.options.kingCanMoveY && (dir2 == 1 || dir2 == 5)) continue;

                        if ((dir1 == 0 && dir2 == 4) || (dir1 == 4 && dir2 == 0)) continue;
                        if ((dir1 == 1 && dir2 == 5) || (dir1 == 5 && dir2 == 1)) continue;
                        if ((dir1 == 2 && dir2 == 6) || (dir1 == 6 && dir2 == 2)) continue;
                        if ((dir1 == 3 && dir2 == 7) || (dir1 == 7 && dir2 == 3)) continue;

                        scratchPoint = currentPosition + directions[dir1] + directions[dir2];
                        if (isMovableSquare(scratchPoint, allAttacks)) yield return scratchPoint;
                    }
                }
            }
            else if (piece.type == Type.QUEEN)
            {
                for (int dir = 0; dir<8; dir++)
                {
                    for (int i = 1; i<=piece.board.maxBoardDimension; i++)
                    {
                        scratchPoint = piece.currentPosition + directions[dir]*i;
                        if (isMovableSquare(scratchPoint, allAttacks)) yield return scratchPoint;
                        if (!isInBounds(scratchPoint) || isFriendlyPiece(scratchPoint) || isEnemyPiece(scratchPoint)) break;
                    }
                }

                for (int dir1 = 0; dir1<8; dir1++)
                {
                    for (int dir2 = dir1+1; dir2<8; dir2++)
                    {
                        if ((dir1 == 0 && dir2 == 4) || (dir1 == 4 && dir2 == 0)) continue;
                        if ((dir1 == 1 && dir2 == 5) || (dir1 == 5 && dir2 == 1)) continue;
                        if ((dir1 == 2 && dir2 == 6) || (dir1 == 6 && dir2 == 2)) continue;
                        if ((dir1 == 3 && dir2 == 7) || (dir1 == 7 && dir2 == 3)) continue;

                        for (int i = 1; i<=piece.board.maxBoardDimension; i++)
                        {   
                            scratchPoint = piece.currentPosition + directions[dir1]*i + directions[dir2]*i;

                            if (isMovableSquare(scratchPoint, allAttacks))  yield return scratchPoint;
                            if (!isInBounds(scratchPoint) || isFriendlyPiece(scratchPoint) || isEnemyPiece(scratchPoint)) break;
                        }
                    }
                }
            }
            else if (piece.type == Type.ROOK)
            {
                for (int dir = 0; dir<8; dir++)
                {
                    for (int i = 1; i<=piece.board.maxBoardDimension; i++)
                    {
                        scratchPoint = piece.currentPosition + directions[dir]*i;
                        if (isMovableSquare(scratchPoint, allAttacks)) yield return scratchPoint;
                        if (!isInBounds(scratchPoint) || isFriendlyPiece(scratchPoint) || isEnemyPiece(scratchPoint)) break;
                    }
                }
            }
            else if (piece.type == Type.BISHOP)
            {
                for (int dir1 = 0; dir1<8; dir1++)
                {
                    for (int dir2 = dir1+1; dir2<8; dir2++)
                    {
                        if ((dir1 == 0 && dir2 == 4) || (dir1 == 4 && dir2 == 0)) continue;
                        if ((dir1 == 1 && dir2 == 5) || (dir1 == 5 && dir2 == 1)) continue;
                        if ((dir1 == 2 && dir2 == 6) || (dir1 == 6 && dir2 == 2)) continue;
                        if ((dir1 == 3 && dir2 == 7) || (dir1 == 7 && dir2 == 3)) continue;

                        for (int i = 1; i<=piece.board.maxBoardDimension; i++)
                        {   
                            scratchPoint = piece.currentPosition + directions[dir1]*i + directions[dir2]*i;

                            if (isMovableSquare(scratchPoint, allAttacks))  yield return scratchPoint;
                            if (!isInBounds(scratchPoint) || isFriendlyPiece(scratchPoint) || isEnemyPiece(scratchPoint)) break;
                        }
                    }
                }
            }
            else if (piece.type == Type.KNIGHT)
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

                        scratchPoint = currentPosition + directions[dir1] + directions[dir2]*2;

                        if (isMovableSquare(scratchPoint, allAttacks)) 
                        {
                            yield return scratchPoint;
                        }
                    }
                }
            }
        }
        
    }
}