using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CanvasManager : MonoBehaviour {

	public Canvas gameCanvas;
	public Canvas menuCanvas;
	public GameObject gameCanvasPanel;
	public GameObject menuCanvasMenuPanel;
	public GameObject menuCanvasGamePanel;
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
	}
	
	// Update is called once per frame
	void Update () {

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

	public void OnConsultingMenu() {
		gameCanvasText.text = "PRESS START";
		menuCanvasMenuPanel.SetActive (true);
		menuCanvasGamePanel.SetActive (false);
	}

	public void OnGettingReady() {
		menuCanvasMenuPanel.SetActive (false);
		menuCanvasGamePanel.SetActive (true);
		StartCoroutine(ShowCountDown());
	}

	public void OnWeakPacMan() {
		gameCanvasPanel.SetActive(false);
	}

	public void OnStrongPacMan() {
		gameCanvasPanel.SetActive(false);
	}

	public void OnNoPacMan() {
	}

	public void OnPacManWins() {
		gameCanvasText.text = "YOU WON !";
		gameCanvasPanel.SetActive(true);
	}

	public void OnPacManLoses() {
		gameCanvasText.text = "GAME OVER";
		gameCanvasPanel.SetActive(true);
	}

	public void OnGamePaused() {
		gameCanvasText.text = "GAME PAUSED";
		gameCanvasPanel.SetActive(true);
	}
}
