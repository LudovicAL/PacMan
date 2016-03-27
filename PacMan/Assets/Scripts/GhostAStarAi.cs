using UnityEngine;
using System.Collections;

public class GhostAStarAi : Ghost {

	public float minSpeed = 4.0f;
	public float maxSpeed = 5.0f;

	private float speed;

	// Use this for initialization
	public override void SubStart () {
		SetGhostSpeed ();
	}
	
	// Update is called once per frame
	public override void SubUpdate () {
		
	}

	//Sets the ghost speed
	public void SetGhostSpeed() {
		speed = Random.Range (minSpeed, maxSpeed);
	}
}
