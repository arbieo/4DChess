using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessboardController2D : MonoBehaviour {

	ChessBoard board;

	public Tile2D[,,,] tiles;

	public int cardinalityX = 0;
	public int cardinalityY = 1;
	public int cardinalityZ = 2;
	public int cardinalityW = 3;
	
	public GameObject tilePrefab;
	public GameObject tileContainer;

	public Text xText;
	public Text yText;
	public Text zText;
	public Text wText;

	/*
	TODO: these should be controlled from outside
	 */
	ChessPiece.Team currentMove = ChessPiece.Team.WHITE;
	Tile2D selectedTile;
	Tile2D destinationTile;
	Tile2D attackedTile;

	public void Initialize(ChessBoard board)
	{
		InitializeBoard(board);

		xText.text = GetHorizontalCardinalityText(cardinalityX);
		yText.text = GetHorizontalCardinalityText(cardinalityY);
		zText.text = GetVerticalCardinalityText(cardinalityZ);
		wText.text = GetVerticalCardinalityText(cardinalityW);
	}

	string GetHorizontalCardinalityText(int cardinality)
	{
		switch(cardinality)
		{
			case 0:
				return "RIGHT  -->";
			case 1:
				return "3D  -->";
			case 2:
				return "FORWARD  -->";
			case 3:
				return "4D  -->";
		}
		return "";
	}

	string GetVerticalCardinalityText(int cardinality)
	{
		switch(cardinality)
		{
			case 0:
				return "R\nI\nG\nH\nT\n \n|\n|\nV";
			case 1:
				return "3\nD \n|\n|\nV";
			case 2:
				return "F\nO\nR\nW\nA\nR\nD\n \n|\n|\nV";
			case 3:
				return "4\nD\n \n|\n|\nV\n";
		}
		return "";
	}

	void InitializeBoard(ChessBoard board)
	{
		this.board = board;

		transform.parent.rotation = Quaternion.LookRotation(transform.parent.forward);

		tiles = new Tile2D[board.size.x, board.size.y, board.size.z, board.size.w];
		for(int x = 0; x<board.size.x; x++)
		{
			for(int y = 0; y<board.size.y; y++)
			{
				for(int z = 0; z<board.size.z; z++)
				{
					for(int w = 0; w<board.size.w; w++)
					{
						tiles[x,y,z,w] = GameObject.Instantiate(tilePrefab).GetComponent<Tile2D>();
						Point4 position = new Point4(x,y,z,w);
						tiles[x,y,z,w].Initialize(position, board);
						tiles[x,y,z,w].transform.parent = tileContainer.transform;

						tiles[x,y,z,w].pieceRenderer.gameObject.layer = gameObject.layer;
						tiles[x,y,z,w].tileRenderer.gameObject.layer = gameObject.layer;

						tiles[x,y,z,w].transform.localPosition = GetTilePosition(position);
						tiles[x,y,z,w].transform.localScale = tileContainer.transform.localScale;
						tiles[x,y,z,w].transform.rotation = tileContainer.transform.rotation;
					}
				}
			}
		}
	}

	public Tile2D GetTile(Point4 position)
	{
		return tiles[position.x, position.y, position.z, position.w];
	}

	Vector3 GetCardinalityVector(int cardinality)
	{
		if (cardinality == 0)
		{
			return Vector3.right;
		}
		else if (cardinality == 1)
		{
			return Vector3.right * 4.5f;
		}
		else if (cardinality == 2)
		{
			return Vector3.up;
		}
		else
		{
			return Vector3.up * 4.5f;
		}
	}

	Vector3 GetTilePosition(Point4 position)
	{
		
		Vector3 xComponent = GetCardinalityVector(cardinalityX);
		Vector3 yComponent = GetCardinalityVector(cardinalityY);
		Vector3 zComponent = GetCardinalityVector(cardinalityZ);
		Vector3 wComponent = GetCardinalityVector(cardinalityW);

		return xComponent * position.x + yComponent * position.y + zComponent * position.z + wComponent * position.w;
	}
}
