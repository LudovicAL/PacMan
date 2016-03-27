using UnityEngine;
using System.Collections;

public class GhostRandomAi : Ghost {

	private int previousMotionX;
	private int previousMotionY;

	// Update is called once per frame
	public override void SubUpdate () {
		switch(ghostState) {
			case AvailableGhostStates.Chasing:
			case AvailableGhostStates.Afraid:
			case AvailableGhostStates.Wandering:
				break;
			case AvailableGhostStates.Idle:
				break;
			case AvailableGhostStates.Dead:
				break;
		}
	}
}
