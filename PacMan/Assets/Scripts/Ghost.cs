using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Ghost : MonoBehaviour {

	public float speed = 6;
	public GameObject grid;

	public enum AvailableGhostStates {
		Chasing,	//Chasing weak Pac-Man
		Afraid,		//Afraid of strong Pac-Man
		Wandering,	//Wandering as there is currently no-one to chase or be afraid of
		Idle,		//Not moving
		Dead		//Dead
	};

	protected int motionX;
	protected int motionY;
	protected AvailableGhostStates ghostState;
	protected Tile currentTile;

	// Use this for initialization
	void Start () {
		SetGhostState (AvailableGhostStates.Idle);
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
	}

	void Update () {
		SubUpdate ();
	}

	//Sets the ghost's current animation
	protected void SetAnimation() {
		switch(ghostState) {
			case AvailableGhostStates.Chasing:
				SetAnimator (true);
				break;
			case AvailableGhostStates.Afraid:
				SetAnimator (false);
				break;
			case AvailableGhostStates.Wandering:
				SetAnimator (true);
				break;
			case AvailableGhostStates.Idle:
				SetAnimator (false);
				break;
			case AvailableGhostStates.Dead:
				SetAnimator (false);
				break;
		}		
	}

	//Sets the ghost's state
	public void SetGhostState(AvailableGhostStates state) {
		ghostState = state;
		SetAnimation();
	}

	//Sets the animators parameters
	protected void SetAnimator(bool chasing) {
		this.GetComponent<Animator> ().SetBool ("Chasing", chasing);
		this.GetComponent<Animator> ().SetInteger ("MotionX", motionX);
		this.GetComponent<Animator> ().SetInteger ("MotionY", motionY);
	}

	protected void OnConsultingMenu() {
		SetGhostState (AvailableGhostStates.Idle);
	}

	protected void OnGettingReady() {
		SetGhostState (AvailableGhostStates.Idle);
	}

	protected void OnWeakPacMan() {
		SetGhostState (AvailableGhostStates.Chasing);
	}

	protected void OnStrongPacMan() {
		SetGhostState (AvailableGhostStates.Afraid);
	}

	protected void OnNoPacMan() {
		SetGhostState (AvailableGhostStates.Wandering);
	}

	protected void OnPacManWins() {
		SetGhostState (AvailableGhostStates.Dead);
	}

	protected void OnPacManLoses() {
		SetGhostState (AvailableGhostStates.Wandering);
	}

	protected void OnGamePaused() {
		SetGhostState (AvailableGhostStates.Idle);
	}

	public abstract void SubUpdate();

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
}
