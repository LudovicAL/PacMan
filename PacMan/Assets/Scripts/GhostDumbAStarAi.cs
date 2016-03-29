using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GhostDumbAStarAi : Ghost {

	public float minSpeed = 4.0f;
	public float maxSpeed = 6.0f;

	private Color[] availableColors = new Color[Color.red, Color.white, Color.gray];

	public override Tile GetDestinationTile() {
		if (IsAlive) {
			return grid.GetComponent<GridManager> ().WalkableTileList.ElementAt(Random.Range(0, grid.GetComponent<GridManager> ().WalkableTileList.Count));
		} else {
			return spawningTile;
		}
	}

	public override void SetGhostColor(bool afraid) {
		if (afraid) {
			gameObject.GetComponent<SpriteRenderer> ().color = Color.blue;
		} else {
			gameObject.GetComponent<SpriteRenderer> ().color = availableColors[Random.Range(0, availableColors.Length)];
		}
	}

	//Sets the ghost speed
	public override void SetGhostSpeed() {
		speed = Random.Range (minSpeed, maxSpeed);
	}

	//Returns the next destination tile choosen somewhat randomly
	public TileDirectionPair GetNextTileToGo() {
		TileDirectionPair tileDirectionPair = null;

		return tileDirectionPair;
	}
}
