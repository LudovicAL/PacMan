using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GhostRandomAi : Ghost {

	public float minSpeed = 5.0f;
	public float maxSpeed = 6.0f;

	private float speed;
	private int previousDirection;

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
					previousDirection = direction;
					direction = 0;
					GoIdle ();
				}
				break;
			case AvailableGhostStates.ChasingIdle:
			case AvailableGhostStates.AfraidIdle:
			case AvailableGhostStates.WanderingIdle:
			case AvailableGhostStates.DeadIdle:
				TileDirectionPair tileDirectionPair = GetNextRandomTileToGo ();
				if (tileDirectionPair != null) {
					GoMoving (tileDirectionPair.Direction);
					currentTile = tileDirectionPair.Tile;
				}
				break;
		}
	}

	public override void SetGhostColor(bool afraid) {
		
		if (afraid) {
			gameObject.GetComponent<SpriteRenderer> ().color = Color.cyan;
		} else {
			gameObject.GetComponent<SpriteRenderer> ().color = Color.magenta;
		}
	}

	//Sets the ghost speed
	public void SetGhostSpeed() {
		speed = Random.Range (minSpeed, maxSpeed);
	}

	//Returns the next destination tile choosen somewhat randomly
	public TileDirectionPair GetNextRandomTileToGo() {
		TileDirectionPair tileDirectionPair = null;
		List<TileDirectionPair> nonWallNeighborTilesDirectionPair = GetNonWallNeighborTilesDirectionPair ();
		if (nonWallNeighborTilesDirectionPair.Count > 1) {	//If there is more than one way to go
			if (currentTile.NeighborTiles[direction].TileType == Tile.AvailableTileTypes.Wall) {	//If the ghost just met a wall
				tileDirectionPair = nonWallNeighborTilesDirectionPair[Random.Range(0, nonWallNeighborTilesDirectionPair.Count)];
			} else {	//In any other case
				int index = nonWallNeighborTilesDirectionPair.FindIndex(TileDirectionPair => TileDirectionPair.Direction.Equals(GetOppositeDirection()));
				if (index != -1) {
					nonWallNeighborTilesDirectionPair.RemoveAt (index);
				}
				tileDirectionPair = nonWallNeighborTilesDirectionPair[Random.Range(0, nonWallNeighborTilesDirectionPair.Count)];
			}
		} else if (nonWallNeighborTilesDirectionPair.Count == 1) {	//If there is only one way to go
			tileDirectionPair = nonWallNeighborTilesDirectionPair.First ();
		}
		return tileDirectionPair;
	}

	public int GetOppositeDirection() {
		if (previousDirection == 0) {
			return 2;
		} else if (previousDirection == 1) {
			return 3;
		} else if (previousDirection == 2) {
			return 0;
		} else {
			return 1;
		}
	}
}