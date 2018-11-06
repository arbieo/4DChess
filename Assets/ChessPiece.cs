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

    public ChessPiece(Type type, Team team, int x, int y, int z, int w)
    {
        this.type = type;
        this.team = team;
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public bool isInBounds(int x, int y, int z, int w)
    {
        return x >= 0 && x <= 3 && y >=0 && y<=3 && z >=0 && z <= 3 && w >= 0 && w<=3;
    }

    public bool isEnemyPiece(int x, int y, int z, int w)
    {
        return isInBounds(x, y, z, w) && ChessboardController.instance.pieces[x,y,z,w] != null && ChessboardController.instance.pieces[x,y,z,w].team != team;
    }

    HashSet<Vector4> GetDiagonals()
    {
        HashSet<Vector4> validMoves = new HashSet<Vector4>();

        for (int dir1 = 0; dir1<4; dir1++)
        {
            for (int dir2 = dir1+1; dir2<4; dir2++)
            {
                int newX = x;
                int newY = y;
                int newZ = z;
                int newW = w;

                for (int i = 1; i<4; i++)
                {
                    //0 = x, 1 = y, 2 = z, 3 = w
                    if (dir1 == 0 || dir2 == 0)
                    {
                        newX = x + i;
                    }
                    if (dir1 == 1 || dir2 == 1)
                    {
                        newY = y + i;
                    }
                    if (dir1 == 2 || dir2 == 2)
                    {
                        newZ = z + i;
                    }
                    if (dir1 == 3 || dir2 == 3)
                    {
                        newW = w + i;
                    }
                    if (isInBounds(newX, newY, newZ, newW)) 
                    {
                        validMoves.Add(new Vector4(newX, newY, newZ, newW));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return validMoves;
    }

    HashSet<Vector4> GetOrthoginals()
    {
        HashSet<Vector4> validMoves = new HashSet<Vector4>();

        for (int i = 1; i<4; i++)
        {
            if (isInBounds(x+i, y, z, w)) validMoves.Add(new Vector4(x+i, y, z, w));
            if (isInBounds(x, y+i, z, w)) validMoves.Add(new Vector4(x, y+i, z, w));
            if (isInBounds(x, y, z+i, w)) validMoves.Add(new Vector4(x, y, z+i, w));
            if (isInBounds(x, y, z, w+i)) validMoves.Add(new Vector4(x, y, z, w+i));

            if (isInBounds(x-i, y, z, w)) validMoves.Add(new Vector4(x-i, y, z, w));
            if (isInBounds(x, y-i, z, w)) validMoves.Add(new Vector4(x, y-i, z, w));
            if (isInBounds(x, y, z-i, w)) validMoves.Add(new Vector4(x, y, z-i, w));
            if (isInBounds(x, y, z, w-i)) validMoves.Add(new Vector4(x, y, z, w-1));
        }

        return validMoves;
    }

    public HashSet<Vector4> GetPawnAttacks()
    {
        HashSet<Vector4> validMoves = new HashSet<Vector4>();

        if (isInBounds(x+1, y, z+1, w)) validMoves.Add(new Vector4(x+1, y, z+1, w));
        if (isInBounds(x, y+1, z+1, w)) validMoves.Add(new Vector4(x, y+1, z+1, w));
        if (isInBounds(x-1, y, z+1, w)) validMoves.Add(new Vector4(x-1, y, z+1, w));
        if (isInBounds(x, y-1, z+1, w)) validMoves.Add(new Vector4(x, y-1, z+1, w));

        if (isInBounds(x+1, y, z, w+1)) validMoves.Add(new Vector4(x+1, y, z, w+1));
        if (isInBounds(x, y+1, z, w+1)) validMoves.Add(new Vector4(x, y+1, z, w+1));
        if (isInBounds(x-1, y, z, w+1)) validMoves.Add(new Vector4(x-1, y, z, w+1));
        if (isInBounds(x, y-1, z, w+1)) validMoves.Add(new Vector4(x, y-1, z, w+1));
        
        return validMoves;
    }

    public HashSet<Vector4> GetValidMoves()
    {
        HashSet<Vector4> validMoves = new HashSet<Vector4>();

        if (type == Type.PAWN) {
            if (isInBounds(x, y, z+1, w)) validMoves.Add(new Vector4(x, y, z+1, w));
            if (isInBounds(x, y, z, w+1)) validMoves.Add(new Vector4(x, y, z, w+1));

            if (isEnemyPiece(x+1, y, z+1, w)) validMoves.Add(new Vector4(x+1, y, z+1, w));
            if (isEnemyPiece(x, y+1, z+1, w)) validMoves.Add(new Vector4(x, y+1, z+1, w));
            if (isEnemyPiece(x-1, y, z+1, w)) validMoves.Add(new Vector4(x-1, y, z+1, w));
            if (isEnemyPiece(x, y-1, z+1, w)) validMoves.Add(new Vector4(x, y-1, z+1, w));

            if (isEnemyPiece(x+1, y, z, w+1)) validMoves.Add(new Vector4(x+1, y, z, w+1));
            if (isEnemyPiece(x, y+1, z, w+1)) validMoves.Add(new Vector4(x, y+1, z, w+1));
            if (isEnemyPiece(x-1, y, z, w+1)) validMoves.Add(new Vector4(x-1, y, z, w+1));
            if (isEnemyPiece(x, y-1, z, w+1)) validMoves.Add(new Vector4(x, y-1, z, w+1));
        }
        else if (type == Type.KING)
        {
            if (isInBounds(x+1, y, z, w)) validMoves.Add(new Vector4(x+1, y, z, w));
            if (isInBounds(x, y+1, z, w)) validMoves.Add(new Vector4(x, y+1, z, w));
            if (isInBounds(x, y, z+1, w)) validMoves.Add(new Vector4(x, y, z+1, w));
            if (isInBounds(x, y, z, w+1)) validMoves.Add(new Vector4(x, y, z, w+1));

            if (isInBounds(x-1, y, z, w)) validMoves.Add(new Vector4(x-1, y, z, w));
            if (isInBounds(x, y-1, z, w)) validMoves.Add(new Vector4(x, y-1, z, w));
            if (isInBounds(x, y, z-1, w)) validMoves.Add(new Vector4(x, y, z-1, w));
            if (isInBounds(x, y, z, w-1)) validMoves.Add(new Vector4(x, y, z, w-1));
        }
        else if (type == Type.QUEEN)
        {
            validMoves = GetOrthoginals();
            validMoves.UnionWith(GetDiagonals());
        }
        else if (type == Type.ROOK)
        {
            validMoves = GetOrthoginals();
        }
        else if (type == Type.BISHOP)
        {
            validMoves = GetDiagonals();
        }
        else if (type == Type.KNIGHT)
        {
            for (int dir1 = 0; dir1<4; dir1++)
            {
                for (int dir2 = 0; dir2<4; dir2++)
                {
                    int newX = x + (dir1 == 0 ? 1 : 0) + (dir2 == 0 ? 2 : 0);
                    int newY = y + (dir1 == 1 ? 1 : 0) + (dir2 == 1 ? 2 : 0);
                    int newZ = z + (dir1 == 2 ? 1 : 0) + (dir2 == 2 ? 2 : 0);
                    int newW = w + (dir1 == 3 ? 1 : 0) + (dir2 == 3 ? 2 : 0);
                    if (isInBounds(newX, newY, newZ, newW)) 
                    {
                        validMoves.Add(new Vector4(newX, newY, newZ, newW));
                    }
                }
            }
        }

        return validMoves;
    }
}