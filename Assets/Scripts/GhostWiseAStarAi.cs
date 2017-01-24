using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GhostWiseAStarAi : Ghost {

	public float minSpeed = 4.0f;
	public float maxSpeed = 5.0f;

	private Color[] chasingColors = new Color[] {Color.green, Color.magenta};
	private Color[] afraidColors = new Color[] {Color.cyan, Color.gray};

	//Returns the next destination tile
	public override Tile GetDestinationTile() {
		if (IsAlive) {
			return grid.GetComponent<GridManager>().PacMan.GetComponent<PacManControls>().CurrentTile;
		} else {
			return spawningTile;
		}
	}

	//Sets the ghost's color
	public override void SetGhostColor(bool afraid) {
		if (afraid) {
			gameObject.GetComponent<SpriteRenderer> ().color = afraidColors[Random.Range(0, afraidColors.Length)];
		} else {
			gameObject.GetComponent<SpriteRenderer> ().color = chasingColors[Random.Range(0, chasingColors.Length)];
		}
	}

	//Sets the ghost's speed
	public override void SetGhostSpeed() {
		speed = Random.Range (minSpeed, maxSpeed);
	}
}
