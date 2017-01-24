using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public abstract class Ghost : MonoBehaviour {

	public GameObject grid;

	public enum AvailableGhostStates {
		ChasingMoving,		//Chasing weak Pac-Man
		ChasingIdle,		//Chasing weak Pac-Man but not moving at the moment
		AfraidMoving,		//Afraid of strong Pac-Man
		AfraidIdle,			//Afraid of strong Pac-Man but not moving at the moment
		WanderingMoving,	//Wandering as there is currently no Pac-Man to chase or be afraid of
		WanderingIdle,		//Wandering as there is currently no Pac-Man to chase or be afraid of but not moving at the moment
		Paused,				//Paused
		DeadMoving,			//Dead and moving
		DeadIdle,			//Dead and idle
		DeadPaused			//Dead and paused
	};
		
	protected float speed;
	protected int direction;
	protected AvailableGhostStates ghostState;
	protected Tile spawningTile;
	protected Tile currentTile;
	private Tile destinationTile;
	protected List<Tile> currentPath;
	protected int progressionInPath;

	// Use this for initialization
	void Start () {
		SetGhostSpeed ();
		spawningTile = currentTile; //As currentTile has been initialized at Awake by the GridManager
		destinationTile = null;
		currentPath = null;
		progressionInPath = 0;
		SetGhostState (AvailableGhostStates.Paused);
		if (grid != null) {
			grid.GetComponent<GameStatesManager> ().ConsultingMenuGameState.AddListener(OnConsultingMenu);
			grid.GetComponent<GameStatesManager> ().GettingReadyGameState.AddListener(OnGettingReady);
			grid.GetComponent<GameStatesManager> ().WeakPacManGameState.AddListener(OnWeakPacMan);
			grid.GetComponent<GameStatesManager> ().StrongPacManGameState.AddListener(OnStrongPacMan);
			grid.GetComponent<GameStatesManager> ().PacManWinsGameState.AddListener(OnPacManWins);
			grid.GetComponent<GameStatesManager> ().PacManLosesGameState.AddListener(OnPacManLoses);
			grid.GetComponent<GameStatesManager> ().PausedGameState.AddListener(OnGamePaused);
		} else {
			Debug.Log("The script component is missing a reference.");
		}
	}

	void Update () {
		if (IsMoving) {
			Move ();
		} else if (ghostState != AvailableGhostStates.Paused && ghostState != AvailableGhostStates.DeadPaused) {
			ChooseNextTile ();
		}
	}

	//Moves the ghost to currentTile
	public void Move() {
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, currentTile.GObject.transform.position, step);
		if (transform.position == currentTile.GObject.transform.position) {
			GoIdle ();
			progressionInPath++;
		}
	}

	//Chooses the next tile to go and prepares the path
	public void ChooseNextTile() {
		if (currentTile == destinationTile) {
			destinationTile = null;
			currentPath = null;
			if (!IsAlive) {
				SetGhostSpeed();
				SetGhostState (AvailableGhostStates.ChasingIdle);
				return;
			}
		}
		if (currentPath == null) {
			currentPath = AStarPathfinding.GeneratePath(currentTile, GetDestinationTile (), grid);
			progressionInPath = 0;
			if (currentPath != null) {
				destinationTile = currentPath.Last();
			}
		}
		if (currentPath != null) {
			GoMoving (currentTile.GetNeighborTileDirection(currentPath.ElementAt(progressionInPath)));
			if (progressionInPath < currentPath.Count) {
				currentTile = currentPath.ElementAt(progressionInPath);
			}
		}
	}

	//Kills the ghost
	public void Die() {
		SetGhostState (AvailableGhostStates.DeadIdle);
		progressionInPath = 0;
		currentPath = null;
		destinationTile = null;
	}

	//Sets the ghost's state
	public void SetGhostState(AvailableGhostStates state, int dir) {
		this.direction = dir;
		ghostState = state;
		SetAnimation();
	}
	public void SetGhostState(AvailableGhostStates state) {
		ghostState = state;
		SetAnimation();
	}

	//Sets the ghost's current animation
	protected void SetAnimation() {
		switch(ghostState) {
			case AvailableGhostStates.ChasingMoving:
			case AvailableGhostStates.WanderingMoving:
			case AvailableGhostStates.ChasingIdle:
			case AvailableGhostStates.WanderingIdle:
				SetAnimator (false, false);
				break;
			case AvailableGhostStates.AfraidMoving:
			case AvailableGhostStates.AfraidIdle:
				SetAnimator (true, false);
				break;
			case AvailableGhostStates.Paused:
				SetAnimator (false, false);
				break;
			case AvailableGhostStates.DeadMoving:
			case AvailableGhostStates.DeadIdle:
			case AvailableGhostStates.DeadPaused:
				SetAnimator (false, true);
				break;
		}		
	}

	//Changes the ghost state to the appropriate kind of idle state
	protected void GoIdle() {
		switch(ghostState) {
			case AvailableGhostStates.ChasingMoving:
				SetGhostState (AvailableGhostStates.ChasingIdle);
				break;
			case AvailableGhostStates.AfraidMoving:
				SetGhostState (AvailableGhostStates.AfraidIdle);
				break;
			case AvailableGhostStates.WanderingMoving:
				SetGhostState (AvailableGhostStates.WanderingIdle);
				break;
			case AvailableGhostStates.DeadMoving:
				SetGhostState (AvailableGhostStates.DeadIdle);
				break;
		}	
	}

	//Changes the ghost state to the appropriate kind of moving state
	protected void GoMoving(int dir) {
		switch(ghostState) {
			case AvailableGhostStates.ChasingIdle:
				SetGhostState (AvailableGhostStates.ChasingMoving, dir);
				break;
			case AvailableGhostStates.AfraidIdle:
				SetGhostState (AvailableGhostStates.AfraidMoving, dir);
				break;
			case AvailableGhostStates.WanderingIdle:
				SetGhostState (AvailableGhostStates.WanderingMoving, dir);
				break;
			case AvailableGhostStates.DeadIdle:
				SetGhostState (AvailableGhostStates.DeadMoving, dir);
				break;
		}	
	}

	//Sets the animators parameters
	protected void SetAnimator(bool afraid, bool dead) {
		this.GetComponent<Animator> ().SetBool ("Afraid", afraid);
		this.GetComponent<Animator> ().SetBool ("Dead", dead);
		this.GetComponent<Animator> ().SetInteger ("Direction", direction);
	}

	protected void OnConsultingMenu() {
		SetGhostState (AvailableGhostStates.Paused);
	}

	protected void OnGettingReady() {
		SetGhostState (AvailableGhostStates.Paused);
	}

	protected void OnWeakPacMan() {
		if (IsAlive) {
			SetGhostState (AvailableGhostStates.ChasingIdle);
		} else {
			SetGhostState (AvailableGhostStates.DeadIdle);
		}
		SetGhostColor (false);
	}

	protected void OnStrongPacMan() {
		if (IsAlive) {
			SetGhostState (AvailableGhostStates.AfraidIdle);
		} else {
			SetGhostState (AvailableGhostStates.DeadIdle);
		}
		SetGhostColor (true);
	}

	protected void OnPacManWins() {
		SetGhostState (AvailableGhostStates.DeadIdle);
	}

	protected void OnPacManLoses() {
		if (IsAlive) {
			SetGhostState (AvailableGhostStates.WanderingIdle);
		} else {
			SetGhostState (AvailableGhostStates.DeadIdle);
		}
	}

	protected void OnGamePaused() {
		if (IsAlive) {
			SetGhostState (AvailableGhostStates.Paused);
		} else {
			SetGhostState (AvailableGhostStates.DeadPaused);
		}
	}

	public abstract void SetGhostColor(bool afraid);
	public abstract void SetGhostSpeed();
	public abstract Tile GetDestinationTile();

	//Proterties
	public Tile CurrentTile {
		get {
			return currentTile;
		}
		set {
			currentTile = value;
		}
	}

	public AvailableGhostStates GhostState {
		get {
			return ghostState;
		}
		set {
			ghostState = value;
		}
	}

	public bool IsAlive {
		get {
			if (ghostState != AvailableGhostStates.DeadMoving && ghostState != AvailableGhostStates.DeadIdle && ghostState != AvailableGhostStates.DeadPaused) {
				return true;
			} else {
				return false;
			}
		}
	}

	public bool IsMoving {
		get {
			if (ghostState == AvailableGhostStates.ChasingMoving || ghostState == AvailableGhostStates.AfraidMoving || ghostState == AvailableGhostStates.WanderingMoving || ghostState == AvailableGhostStates.DeadMoving) {
				return true;
			} else {
				return false;
			}
		}
	}
}
