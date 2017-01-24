using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GameStatesManager : MonoBehaviour {

	public float boostDurationInSeconds = 8.0f;

	public enum AvailableGameStates {
		ConsultingMenu,	//Player is consulting the menu
		GettingReady,	//Game is about to start
		WeakPacMan,		//Pac-Man is being chased by the ghosts
		StrongPacMan,	//Pac-Man is chasing the ghosts
		PacManLoses,	//Pac-Man lost
		PacManWins,		//Pac-Man won
		Paused			//Game is paused
	};

	public UnityEvent ConsultingMenuGameState;
	public UnityEvent GettingReadyGameState;
	public UnityEvent WeakPacManGameState;
	public UnityEvent StrongPacManGameState;
	public UnityEvent PacManWinsGameState;
	public UnityEvent PacManLosesGameState;
	public UnityEvent PausedGameState;

	private AvailableGameStates gameState;
	private AvailableGameStates previousGameState;

	// Use this for initialization
	void Start () {
		if (ConsultingMenuGameState == null) {
			ConsultingMenuGameState = new UnityEvent();
		}
		if (GettingReadyGameState == null) {
			GettingReadyGameState = new UnityEvent();
		}
		if (WeakPacManGameState == null) {
			WeakPacManGameState = new UnityEvent();
		}
		if (StrongPacManGameState == null) {
			StrongPacManGameState = new UnityEvent();
		}
		if (PacManWinsGameState == null) {
			PacManWinsGameState = new UnityEvent();
		}
		if (PacManLosesGameState == null) {
			PacManLosesGameState = new UnityEvent();
		}
		if (PausedGameState == null) {
			PausedGameState = new UnityEvent();
		}
		ChangeGameState(AvailableGameStates.ConsultingMenu);
		this.GetComponent<InputsMonitor> ().EscapeKeyPressed.AddListener(OnEscapeKeyPressed);
	}

	// Update is called once per frame
	void Update () {
		if (gameState == AvailableGameStates.WeakPacMan || gameState == AvailableGameStates.StrongPacMan) {
			if (GameObject.FindGameObjectWithTag("Dot") == null && GameObject.FindGameObjectWithTag("Boost") == null) {
				ChangeGameState (AvailableGameStates.PacManWins);
			}
		}
	}

	//Call this to change the game state
	public void ChangeGameState(AvailableGameStates desiredState) {
		previousGameState = gameState;
		gameState = desiredState;
		switch(desiredState) {
			case AvailableGameStates.ConsultingMenu:
				ConsultingMenuGameState.Invoke ();
				break;
			case AvailableGameStates.GettingReady:
				GettingReadyGameState.Invoke ();
				break;
			case AvailableGameStates.WeakPacMan:
				WeakPacManGameState.Invoke ();
				break;
			case AvailableGameStates.StrongPacMan:
				StrongPacManGameState.Invoke ();
				StartCoroutine(StrongPacManCountDown());
				break;
			case AvailableGameStates.PacManLoses:
				PacManLosesGameState.Invoke ();
				break;
			case AvailableGameStates.PacManWins:
				PacManWinsGameState.Invoke ();
				break;
			case AvailableGameStates.Paused:
				PausedGameState.Invoke ();
				break;
		}
	}

	//Whenever pacman eat a boost, a countdown is launched to determine when it is the gamestate must return to weak pacman
	IEnumerator StrongPacManCountDown() {
		yield return new WaitForSeconds(boostDurationInSeconds);
		while (gameState == AvailableGameStates.Paused) {
			yield return new WaitForSeconds(1.0f);
		}
		if (gameState == AvailableGameStates.StrongPacMan) {
			ChangeGameState (AvailableGameStates.WeakPacMan);
		}
	}

	//Called when users presses the escape key
	public void OnEscapeKeyPressed() {
		if (gameState == AvailableGameStates.Paused) {
			ChangeGameState (previousGameState);
		} else if (gameState == AvailableGameStates.WeakPacMan || gameState == AvailableGameStates.StrongPacMan) {
			ChangeGameState (AvailableGameStates.Paused);
		}
	}

	//Properties
	public AvailableGameStates GameState {
		get {
			return gameState;
		}
		set {
			gameState = value;
		}
	}
}