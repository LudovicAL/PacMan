using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GhostWiseAStarAi : Ghost {

	public float minSpeed = 4.0f;
	public float maxSpeed = 5.0f;

	private Color[] availableColors = new Color[Color.green, Color.gray, Color.magenta];

	public override Tile GetDestinationTile() {
		if (IsAlive) {
			return grid.GetComponent<GridManager>().PacMan.GetComponent<PacManControls>().CurrentTile;
		} else {
			return spawningTile;
		}
	}

	public override void SetGhostColor(bool afraid) {
		if (afraid) {
			gameObject.GetComponent<SpriteRenderer> ().color = Color.cyan;
		} else {
			gameObject.GetComponent<SpriteRenderer> ().color = availableColors[Random.Range(0, availableColors.Length)];
		}
	}

	//Sets the ghost speed
	public override void SetGhostSpeed() {
		speed = Random.Range (minSpeed, maxSpeed);
	}
}
