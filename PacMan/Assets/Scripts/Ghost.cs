using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

	protected int direction;
	protected AvailableGhostStates ghostState;
	protected Tile currentTile;

	// Use this for initialization
	void Start () {
		SetGhostState (AvailableGhostStates.Paused);
		if (grid != null) {
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
		SubStart ();
	}

	void Update () {
		SubUpdate ();
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

	protected void OnNoPacMan() {
		if (IsAlive) {
			SetGhostState (AvailableGhostStates.WanderingIdle);
		} else {
			SetGhostState (AvailableGhostStates.DeadIdle);
		}
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


	//Returns a list of all the neighbor tiles that are not walls
	protected List<TileDirectionPair> GetNonWallNeighborTilesDirectionPair() {
		List<TileDirectionPair> nonWallNeighborTilesDirectionPair = new List<TileDirectionPair> ();
		for (int i = 0; i < 4; i++) {
			if (currentTile.NeighborTiles[i] != null && currentTile.NeighborTiles[i].TileType != Tile.AvailableTileTypes.Wall) {
				nonWallNeighborTilesDirectionPair.Add (new TileDirectionPair(i, currentTile.NeighborTiles[i]));
			}
		}
		return nonWallNeighborTilesDirectionPair;
	}

	public abstract void SubStart();
	public abstract void SubUpdate();
	public abstract void SetGhostColor(bool afraid);

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
