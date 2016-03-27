using UnityEngine;
using System.Collections;

public class WallSpriteAssignator : MonoBehaviour {

	public Sprite fourWaysWall;
	public Sprite[] threeWaysWall;
	public Sprite[] twoWaysWall;
	public Sprite[] oneWayWall;
	public Sprite zeroWayWall;
	public Sprite[] straightWall;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Determines the best suited sprite for a tile
	public Sprite GetWallSprite(Tile tile) {
		Sprite sprite = null;
		int numberOfWallNeighbors = GetNumberOfWallNeighbors (tile);
		//[?, ?, ?, ?]
		switch (numberOfWallNeighbors) {
			case 0: //[O, O, O, O]
				sprite = zeroWayWall;
				break;
			case 1:	//[WOOO]
				sprite = oneWayWall [GetPositionOfWallNeighbor(tile, 1)];
				break;
			case 2: //[WWOO]
				sprite = GetAppropriateTwoWaysWall (tile);
				break;
			case 3: //[WWWO]
				sprite = GetAppropriateThreeWaysWall (tile);
				break;
			case 4: //[W, W, W, W]
				sprite = fourWaysWall;
				break;
		}
		return sprite;
	}

	public Sprite GetAppropriateTwoWaysWall(Tile tile) {
		Sprite sprite = null;
		int positionOfFirstNeighborOfType = GetPositionOfWallNeighbor(tile, 1);
		int positionOfLastNeighborOfType = GetPositionOfWallNeighbor(tile, 2);
		switch (positionOfFirstNeighborOfType) {
			case 0: //[W, ?, ?, ?]
				switch (positionOfLastNeighborOfType) {
					case 1: //[W, W, O, O]
						sprite = twoWaysWall[0];
						break;
					case 2: //[W, O, W, O]
						sprite = straightWall[0];
						break;
					case 3: //[W, O, O, W]
						sprite = twoWaysWall[3];
						break;
				}			
				break;
			case 1: //[O, W, ?, ?]
				switch (positionOfLastNeighborOfType) {
					case 2: //[O, W, W, O]
						sprite = twoWaysWall[1];
						break;
					case 3: //[O, W, O, W]
						sprite = straightWall[1];
						break;
				}	
				break;
			case 2: //[O, O, W, W]
				sprite = twoWaysWall[2];
				break;
		}
		return sprite;
	}

	public Sprite GetAppropriateThreeWaysWall(Tile tile) {
		Sprite sprite = null;
		int positionOfFirstNeighborOfType = GetPositionOfWallNeighbor(tile, 1);
		switch (positionOfFirstNeighborOfType) {
			case 0: //[W, ?, ?, ?]
				int positionOfSecondNeighborOfType = GetPositionOfWallNeighbor(tile, 2);
				switch (positionOfSecondNeighborOfType) {
					case 1: //[W, W, ?, ?]
						int positionOfLastNeighborOfType = GetPositionOfWallNeighbor(tile, 3);
						switch (positionOfLastNeighborOfType) {
							case 2: //[W, W, W, O]
								sprite = threeWaysWall[0];
								break;
							case 3: //[W, W, O, W]
								sprite = threeWaysWall[3];
								break;
						}
						break;
					case 2: //[W, O, W, W]
						sprite = threeWaysWall[2];
						break;
				}
				break;
			case 1: //[O, W, W, W]
				sprite = threeWaysWall[1];
				break;
		}
		return sprite;
	}

	//Return the number of wall neighbors
	private int GetNumberOfWallNeighbors(Tile tile) {
		int n = 0;
		foreach (Tile tileTempo in tile.NeighborTiles) {
			if (tileTempo != null) {
				if (tileTempo.TileType == Tile.AvailableTileTypes.Wall) {
					n++;
				}
			}
		}
		return n;
	}

	//Returns the position of a specified wall neighbor
	private int GetPositionOfWallNeighbor(Tile tile, int number) {
		int n = 0;
		for (int i = 0, max = tile.NeighborTiles.Length; i < max; i++) {
			if (tile.NeighborTiles[i] != null) {
				if (tile.NeighborTiles[i].TileType == Tile.AvailableTileTypes.Wall) {
					n++;
					if (n == number) {
						return i;
					}
				}
			}
		}
		return -1;
	}
}
