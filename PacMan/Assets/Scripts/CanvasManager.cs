using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour {

	public Canvas gameCanvas;
	public Canvas menuCanvas;
	public GameObject gameCanvasPanel;
	public GameObject menuCanvasMenuPanel;
	public GameObject menuCanvasGamePanel;
	public Button pauseButton;
	public Button restartButton;
	public Text scoreValueText;
	public Text gameCanvasText;

	// Use this for initialization
	void Start () {
		OnConsultingMenu ();
		this.GetComponent<GameStatesManager> ().ConsultingMenuGameState.AddListener(OnConsultingMenu);
		this.GetComponent<GameStatesManager> ().GettingReadyGameState.AddListener(OnGettingReady);
		this.GetComponent<GameStatesManager> ().WeakPacManGameState.AddListener(OnWeakPacMan);
		this.GetComponent<GameStatesManager> ().StrongPacManGameState.AddListener(OnStrongPacMan);
		this.GetComponent<GameStatesManager> ().NoPacManGameState.AddListener(OnNoPacMan);
		this.GetComponent<GameStatesManager> ().PacManWinsGameState.AddListener(OnPacManWins);
		this.GetComponent<GameStatesManager> ().PacManLosesGameState.AddListener(OnPacManLoses);
		this.GetComponent<GameStatesManager> ().PausedGameState.AddListener(OnGamePaused);
		this.GetComponent<InputsMonitor> ().ReturnKeyPressed.AddListener(OnReturnKeyPressed);
	}
	
	// Update is called once per frame
	void Update () {
		scoreValueText.text = PersistentData.currentScore.ToString ();
	}

	IEnumerator ShowCountDown() {
		gameCanvasText.text = "3";    
		yield return new WaitForSeconds(1.5f);
		gameCanvasText.text = "2";    
		yield return new WaitForSeconds(1.5f);
		gameCanvasText.text = "1";    
		yield return new WaitForSeconds(1.5f);
		gameCanvasText.text = "";
		this.GetComponent<GameStatesManager> ().ChangeGameState (GameStatesManager.AvailableGameStates.WeakPacMan);
	}

	public void OnStartButtonClic() {
		this.GetComponent<GameStatesManager> ().ChangeGameState (GameStatesManager.AvailableGameStates.GettingReady);
	}

	public void OnRestartButtonClic() {
		PersistentData.currentScore = PersistentData.scoreAtBeginingOfLevel;
		SceneManager.LoadScene (SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}

	public void OnPauseButtonClic() {
		this.GetComponent<GameStatesManager> ().OnEscapeKeyPressed();
	}

	public void OnConsultingMenu() {
		gameCanvasText.text = "PRESS START";
		menuCanvasMenuPanel.SetActive (true);
		menuCanvasGamePanel.SetActive (false);
		PersistentData.scoreAtBeginingOfLevel = PersistentData.currentScore;
	}

	public void OnGettingReady() {
		scoreValueText.text = "0";
		pauseButton.enabled = false;
		pauseButton.enabled = false;
		restartButton.enabled = false;
		menuCanvasMenuPanel.SetActive (false);
		menuCanvasGamePanel.SetActive (true);
		StartCoroutine(ShowCountDown());
	}

	public void OnWeakPacMan() {
		pauseButton.GetComponentInChildren<Text> ().text = "PAUSE";
		pauseButton.enabled = true;
		restartButton.enabled = true;
		gameCanvasPanel.SetActive(false);
	}

	public void OnStrongPacMan() {
		pauseButton.GetComponentInChildren<Text> ().text = "PAUSE";
		pauseButton.enabled = true;
		gameCanvasPanel.SetActive(false);
	}

	public void OnNoPacMan() {
	}

	public void OnPacManWins() {
		pauseButton.enabled = false;
		gameCanvasText.text = "YOU WON !";
		gameCanvasPanel.SetActive(true);
	}

	public void OnPacManLoses() {
		pauseButton.enabled = false;
		gameCanvasText.text = "GAME OVER";
		gameCanvasPanel.SetActive(true);
	}

	public void OnGamePaused() {
		pauseButton.GetComponentInChildren<Text> ().text = "UNPAUSE";
		gameCanvasText.text = "GAME PAUSED";
		gameCanvasPanel.SetActive(true);
	}

	public void OnReturnKeyPressed() {
		if (menuCanvasMenuPanel.activeSelf) {
			OnStartButtonClic ();
		}
	}
}
