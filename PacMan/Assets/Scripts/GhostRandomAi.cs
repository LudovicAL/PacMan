using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GhostRandomAi : Ghost {

	public float minSpeed = 5.0f;
	public float maxSpeed = 6.0f;

	private float speed;
	private int previousMotionX;
	private int previousMotionY;

	public override void  SubStart() {
		SetGhostSpeed ();
	}

	// Update is called once per frame
	public override void SubUpdate () {
		switch(ghostState) {
			case AvailableGhostStates.ChasingMoving:
			case AvailableGhostStates.AfraidMoving:
			case AvailableGhostStates.WanderingMoving:
			case AvailableGhostStates.DeadMoving:
				float step = speed * Time.deltaTime;
				transform.position = Vector3.MoveTowards (transform.position, currentTile.GObject.transform.position, step);
				if (transform.position == currentTile.GObject.transform.position) {
					previousMotionX = motionX;
					previousMotionY = motionY;
					motionX = 0;
					motionY = 0;
					GoIdle ();
				}
				break;
			case AvailableGhostStates.ChasingIdle:
			case AvailableGhostStates.AfraidIdle:
			case AvailableGhostStates.WanderingIdle:
			case AvailableGhostStates.DeadIdle:
				Tile tile = GetNextRandomTileToGo ();
				if (tile != null) {
					GoMoving (tile.CoordX - currentTile.CoordX, tile.CoordY - currentTile.CoordY);
					currentTile = tile;
				}
				break;
		}
	}

	//Sets the ghost speed
	public void SetGhostSpeed() {
		speed = Random.Range (minSpeed, maxSpeed);
	}

	//Returns the next destination tile choosen somewhat randomly
	public Tile GetNextRandomTileToGo() {
		Tile tile = null;
		List<Tile> nonWallNeighborTiles = GetNonWallNeighborTiles ();
		if (nonWallNeighborTiles.Count > 0) {
			if (nonWallNeighborTiles.Count == 1) {	//If there is only one way to go
				tile = nonWallNeighborTiles.First ();
			} else if (nonWallNeighborTiles.Count > 1) {	//If there is more than one way to go
				if (previousMotionY == 0 && previousMotionX == 0) {	//If Pac-Man hasn't started moving yet
					tile = nonWallNeighborTiles [Random.Range (0, nonWallNeighborTiles.Count)];
				} else if (previousMotionY == 1 && currentTile.NeighborTiles[0].TileType == Tile.AvailableTileTypes.Wall) {	//If Pac-Man, going up, just hitted a wall
					tile = nonWallNeighborTiles [Random.Range (0, nonWallNeighborTiles.Count)];
				} else if (previousMotionX == 1 && currentTile.NeighborTiles[1].TileType == Tile.AvailableTileTypes.Wall) {	//If Pac-Man, going right, just hitted a wall
					tile = nonWallNeighborTiles [Random.Range (0, nonWallNeighborTiles.Count)];
				} else if (previousMotionY == -1 && currentTile.NeighborTiles[2].TileType == Tile.AvailableTileTypes.Wall) {	//If Pac-Man, going down, just hitted a wall
					tile = nonWallNeighborTiles [Random.Range (0, nonWallNeighborTiles.Count)];
				} else if (previousMotionX == -1 && currentTile.NeighborTiles[3].TileType == Tile.AvailableTileTypes.Wall) {	//If Pac-Man, going left, just hitted a wall
					tile = nonWallNeighborTiles [Random.Range (0, nonWallNeighborTiles.Count)];
				} else {	//If Pac-Man, meets any other intersection
					
				}
			}
		}
		return tile;
	}
	
	public int GetRandomTileExcluding(Tile excluded) {
		int i = -1;

		return i;
	}
}
