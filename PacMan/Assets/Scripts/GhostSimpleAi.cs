using UnityEngine;
using System.Collections;

public class GhostSimpleAi : Ghost {

	public float minSpeed = 4.0f;
	public float maxSpeed = 6.0f;

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
