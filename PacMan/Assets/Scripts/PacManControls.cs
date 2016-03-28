using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PacManControls : MonoBehaviour {

	public float speed = 6;
	public GameObject grid;

	private enum AvailablePacManStates {
		ChasingMoving,	//Actively chasing ghosts
		ChasingIdle,	//Chasing ghosts, but immobile at the moment
		AfraidMoving,	//Actively escaping ghosts
		AfraidIdle,		//Escaping ghosts, but immobile at the moment
		Idle,			//Not moving
		Dead,			//Dead
		Reviving		//Reviving
	};

	private int motionX;
	private int motionY;
	private AvailablePacManStates pacManState;
	private Tile currentTile;

	// Use this for initialization
	void Start () {
		OnConsultingMenu ();
		if (grid != null) {
			grid.GetComponent<InputsMonitor> ().UpArrowPressed.AddListener(OnUpArrowPressed);
			grid.GetComponent<InputsMonitor> ().RightArrowPressed.AddListener(OnRightArrowPressed);
			grid.GetComponent<InputsMonitor> ().DownArrowPressed.AddListener(OnDownArrowPressed);
			grid.GetComponent<InputsMonitor> ().LeftArrowPressed.AddListener(OnLeftArrowPressed);
			grid.GetComponent<GameStatesManager> ().ConsultingMenuGameState.AddListener(OnConsultingMenu);
			grid.GetComponent<GameStatesManager> ().GettingReadyGameState.AddListener(OnGettingReady);
			grid.GetComponent<GameStatesManager> ().WeakPacManGameState.AddListener(OnWeakPacMan);
			grid.GetComponent<GameStatesManager> ().StrongPacManGameState.AddListener(OnStrongPacMan);
			grid.GetComponent<GameStatesManager> ().NoPacManGameState.AddListener(OnNoPacMan);
			grid.GetComponent<GameStatesManager> ().PacManWinsGameState.AddListener(OnPacManWins);
			grid.GetComponent<GameStatesManager> ().PacManLosesGameState.AddListener(OnPacManLoses);
			grid.GetComponent<GameStatesManager> ().PausedGameState.AddListener(OnGamePaused);
		} else {
			Debug.Log("The script component is missing a reference.");
		}
	}

	// Update is called once per frame
	void Update () {
		switch (pacManState) {
			case AvailablePacManStates.ChasingMoving:
			case AvailablePacManStates.AfraidMoving:
				float step = speed * Time.deltaTime;
				transform.position = Vector3.MoveTowards (transform.position, currentTile.GObject.transform.position, step);
				if (transform.position == currentTile.GObject.transform.position) {
					SetStateIdle();
				}
				break;
			case AvailablePacManStates.ChasingIdle:
				break;
			case AvailablePacManStates.AfraidIdle:
				break;
			case AvailablePacManStates.Idle:
				break;
			case AvailablePacManStates.Dead:
				break;
		}
	}

	//Triggered whenever PacMan collides with another collider
	void OnTriggerEnter2D(Collider2D other) {
		switch(other.tag) {
			case "Dot":
				grid.GetComponent<AudioManager> ().PlaySound ("PacManEatDot");
				Destroy (other.gameObject);
				break;
			case "Boost":
				grid.GetComponent<AudioManager> ().PlaySound ("PacManEatBoost");
				Destroy (other.gameObject);
				grid.GetComponent<GameStatesManager> ().ChangeGameState (GameStatesManager.AvailableGameStates.StrongPacMan);
				break;
			case "Fruit":
				grid.GetComponent<AudioManager> ().PlaySound ("PacManEatFruit");
				Destroy (other.gameObject);
				break;
			case "Ghost":
				if (other.GetComponent<Ghost>().IsAlive) {
					if (grid.GetComponent<GameStatesManager>().GameState == GameStatesManager.AvailableGameStates.StrongPacMan) {
						grid.GetComponent<AudioManager> ().PlaySound ("PacManEatGhost");
						other.gameObject.GetComponent<Ghost> ().SetGhostState(Ghost.AvailableGhostStates.DeadIdle);
					} else if (grid.GetComponent<GameStatesManager>().GameState == GameStatesManager.AvailableGameStates.WeakPacMan) {
						grid.GetComponent<AudioManager> ().PlaySound ("PacManDead");
						SetPacManState (AvailablePacManStates.Dead);
						grid.GetComponent<GameStatesManager> ().ChangeGameState (GameStatesManager.AvailableGameStates.PacManLoses);
					}
				}
				break;
		}
	}

	//Sets Pac-Man's current animation
	private void SetAnimation() {
		switch(pacManState) {
			case AvailablePacManStates.ChasingMoving:
				SetAnimator (true);
				break;
			case AvailablePacManStates.ChasingIdle:
				SetAnimator (false);
				break;
			case AvailablePacManStates.AfraidMoving:
				SetAnimator (true);
				break;
			case AvailablePacManStates.AfraidIdle:
				SetAnimator (false);
				break;
			case AvailablePacManStates.Idle:
				SetAnimator (false);
				break;
			case AvailablePacManStates.Dead:
				SetAnimator (false);
				break;
		}		
	}

	//Sets Pac-Man's state
	private void SetPacManState(AvailablePacManStates state, int dirX, int dirY) {
		this.motionX = dirX;
		this.motionY = dirY;
		pacManState = state;
		SetAnimation();
	}
	private void SetPacManState(AvailablePacManStates state) {
		pacManState = state;
		SetAnimation();
	}

	//Changes Pac-Man's state to the appropriate kind of moving
	private void SetStateMoving (int dirX, int dirY) {
		if (pacManState == AvailablePacManStates.ChasingIdle) {
			SetPacManState (AvailablePacManStates.ChasingMoving, dirX, dirY);
		} else if (pacManState == AvailablePacManStates.AfraidIdle) {
			SetPacManState (AvailablePacManStates.AfraidMoving, dirX, dirY);
		}
	}

	//Changes Pac-Man's state to the appropriate kind of idle
	private void SetStateIdle () {
		if (pacManState == AvailablePacManStates.ChasingMoving) {
			SetPacManState (AvailablePacManStates.ChasingIdle);
		} else if (pacManState == AvailablePacManStates.AfraidMoving) {
			SetPacManState (AvailablePacManStates.AfraidIdle);
		}
	}

	//Sets the animators parameters
	private void SetAnimator(bool moving) {
		this.GetComponent<Animator> ().SetBool ("Moving", moving);
		this.GetComponent<Animator> ().SetInteger ("MotionX", motionX);
		this.GetComponent<Animator> ().SetInteger ("MotionY", motionY);
	}

	//Called when gamestates changes to ConsultingMenu
	public void OnConsultingMenu() {
		SetPacManState (AvailablePacManStates.Idle, 1, 0);
	}

	//Called when gamestates changes to GettingReady
	public void OnGettingReady() {
		SetPacManState (AvailablePacManStates.Idle, 1, 0);
	}

	//Called when gamestates changes to WeakPacMan
	public void OnWeakPacMan() {
		if (Moving) {
			SetPacManState (AvailablePacManStates.AfraidMoving);
		} else {
			SetPacManState (AvailablePacManStates.AfraidIdle);
		}
	}

	//Called when gamestates changes to StrongPacMan
	public void OnStrongPacMan() {
		if (Moving) {
			SetPacManState (AvailablePacManStates.ChasingMoving);
		} else {
			SetPacManState (AvailablePacManStates.ChasingIdle);
		}
	}

	//Called when gamestates changes to NoPacMan
	public void OnNoPacMan() {
		Destroy (this.gameObject);
	}

	//Called when gamestates changes to PacManWins
	public void OnPacManWins() {
		SetPacManState (AvailablePacManStates.Idle);
	}

	//Called when gamestates changes to PacManLoses
	public void OnPacManLoses() {
		SetPacManState(AvailablePacManStates.Dead, 0, 0);
	}

	//Called when gamestates changes to Paused
	public void OnGamePaused() {
		SetPacManState (AvailablePacManStates.Idle);
	}

	//Called when players presses the up arrow
	public void OnUpArrowPressed() {
		if (CanMove
		    && currentTile.NeighborTiles [0]!= null
		    && currentTile.NeighborTiles [0].TileType != Tile.AvailableTileTypes.Wall) {
			currentTile = currentTile.NeighborTiles [0];
			SetStateMoving (0, 1);
		}
	}

	//Called when players presses the right arrow
	public void OnRightArrowPressed() {
		if (CanMove
		    && currentTile.NeighborTiles [1]!= null
		    && currentTile.NeighborTiles [1].TileType != Tile.AvailableTileTypes.Wall) {
			currentTile = currentTile.NeighborTiles [1];
			SetStateMoving (1, 0);
		}
	}

	//Called when players presses the down arrow
	public void OnDownArrowPressed() {
		if (CanMove
		    && currentTile.NeighborTiles [2]!= null
		    && currentTile.NeighborTiles [2].TileType != Tile.AvailableTileTypes.Wall) {
			currentTile = currentTile.NeighborTiles [2];
			SetStateMoving (0, -1);
		}
	}

	//Called when players presses the left arrow
	public void OnLeftArrowPressed() {
		if (CanMove
		    && currentTile.NeighborTiles [3]!= null
		    && currentTile.NeighborTiles [3].TileType != Tile.AvailableTileTypes.Wall) {
			currentTile = currentTile.NeighborTiles [3];
			SetStateMoving (-1, 0);
		}
	}

	//Properties
	public bool Moving {
		get {
			if (pacManState == AvailablePacManStates.ChasingMoving || pacManState == AvailablePacManStates.AfraidMoving) {
				return true;
			} else {
				return false;
			}
		}
	}

	public bool CanMove {
		get {
			if (pacManState == AvailablePacManStates.ChasingIdle || pacManState == AvailablePacManStates.AfraidIdle) {
				return true;
			} else {
				return false;
			}
		}
	}

	public Tile CurrentTile {
		get {
			return currentTile;
		}
		set {
			currentTile = value;
		}
	}
}
