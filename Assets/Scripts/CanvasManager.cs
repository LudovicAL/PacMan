﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CanvasManager : MonoBehaviour {

	public Canvas gameCanvas;
	public Canvas menuCanvas;
	public GameObject gameCanvasPanel;
	public GameObject menuCanvasMenuPanel;
	public GameObject menuCanvasGamePanel;
	public Button gamePanelButton;
	public Text scoreValueText;
	public Text gameCanvasText;

	// Use this for initialization
	void Start () {
		OnConsultingMenu ();
		this.GetComponent<GameStatesManager> ().ConsultingMenuGameState.AddListener(OnConsultingMenu);
		this.GetComponent<GameStatesManager> ().GettingReadyGameState.AddListener(OnGettingReady);
		this.GetComponent<GameStatesManager> ().WeakPacManGameState.AddListener(OnWeakPacMan);
		this.GetComponent<GameStatesManager> ().StrongPacManGameState.AddListener(OnStrongPacMan);
		this.GetComponent<GameStatesManager> ().PacManWinsGameState.AddListener(OnPacManWins);
		this.GetComponent<GameStatesManager> ().PacManLosesGameState.AddListener(OnPacManLoses);
		this.GetComponent<GameStatesManager> ().PausedGameState.AddListener(OnGamePaused);
		this.GetComponent<InputsMonitor> ().ReturnKeyPressed.AddListener(OnReturnKeyPressed);
	}
	
	// Update is called once per frame
	void Update () {
		scoreValueText.text = PersistentData.currentScore.ToString ();
	}

	//Shows a 3-2-1 countdown on the game canvas
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

	//Whenever the player clics the start button
	public void OnStartButtonClic() {
		this.GetComponent<GameStatesManager> ().ChangeGameState (GameStatesManager.AvailableGameStates.GettingReady);
	}

	//Whenever the player clics the quit button
	public void OnQuitButtonClic() {
		Application.Quit ();
	}

	//Whenever the player clics the game custom button
	public void OnGameButtonClic() {
		switch(gameObject.GetComponent<GameStatesManager>().GameState) {
			case GameStatesManager.AvailableGameStates.StrongPacMan:
			case GameStatesManager.AvailableGameStates.WeakPacMan:
			case GameStatesManager.AvailableGameStates.Paused:
				gameObject.GetComponent<GameStatesManager> ().OnEscapeKeyPressed ();
				break;
			case GameStatesManager.AvailableGameStates.PacManLoses:
				gameObject.GetComponent<GridManager> ().LoadLevel (PersistentData.currentLevel);
				this.GetComponent<GameStatesManager> ().ChangeGameState (GameStatesManager.AvailableGameStates.GettingReady);
				break;
			case GameStatesManager.AvailableGameStates.PacManWins:
				if (PersistentData.currentLevel >= gameObject.GetComponent<GridManager> ().MapFile.Length) {
					PersistentData.currentLevel = 0;
				}
				gameObject.GetComponent<GridManager> ().LoadLevel(PersistentData.currentLevel);
				this.GetComponent<GameStatesManager> ().ChangeGameState (GameStatesManager.AvailableGameStates.GettingReady);
				break;
			case GameStatesManager.AvailableGameStates.GettingReady:
			case GameStatesManager.AvailableGameStates.ConsultingMenu:
				//Nothing happens...
				break;
		}
	}

	public void OnConsultingMenu() {
		SetGameCanvas ("PRESS START", true);
		SetGameCanvas ("", true);
		SetMenuCanvasPanels (true);
	}

	public void OnGettingReady() {
		SetGamePanelButton ("", false);
		SetGameCanvas ("", true);
		SetMenuCanvasPanels (false);
		StartCoroutine(ShowCountDown());
		PersistentData.currentScore = PersistentData.scoreAtBeginingOfLevel;
	}

	public void OnWeakPacMan() {
		SetGamePanelButton ("PAUSE", true);
		SetGameCanvas ("", false);
		SetMenuCanvasPanels (false);
	}

	public void OnStrongPacMan() {
		SetGamePanelButton ("PAUSE", true);
		SetGameCanvas ("", false);
		SetMenuCanvasPanels (false);
	}

	public void OnPacManWins() {
		SetGamePanelButton ("NEXT LEVEL", true);
		SetGameCanvas ("YOU WON !", true);
		SetMenuCanvasPanels (false);
		PersistentData.scoreAtBeginingOfLevel = PersistentData.currentScore;
		PersistentData.currentLevel++;
	}

	public void OnPacManLoses() {
		SetGamePanelButton ("RESTART", true);
		SetGameCanvas ("GAME OVER", true);
		SetMenuCanvasPanels (false);
	}

	public void OnGamePaused() {
		SetGamePanelButton ("UNPAUSE", true);
		SetGameCanvas ("GAME PAUSED", true);
		SetMenuCanvasPanels (false);
	}

	public void OnReturnKeyPressed() {
		switch(gameObject.GetComponent<GameStatesManager>().GameState) {
			case GameStatesManager.AvailableGameStates.ConsultingMenu:
				OnStartButtonClic ();
				break;
			case GameStatesManager.AvailableGameStates.StrongPacMan:
			case GameStatesManager.AvailableGameStates.WeakPacMan:
			case GameStatesManager.AvailableGameStates.PacManLoses:
			case GameStatesManager.AvailableGameStates.PacManWins:
			case GameStatesManager.AvailableGameStates.Paused:
				OnGameButtonClic ();				
				break;
			case GameStatesManager.AvailableGameStates.GettingReady:
				//Nothing happens...
				break;
		}
	}

	//Sets the game panel custom button
	public void SetGamePanelButton(string text, bool enabled) {
		gamePanelButton.GetComponentInChildren<Text> ().text = text;
		gamePanelButton.enabled = true;
	}

	//Sets the menu canvas panel
	public void SetMenuCanvasPanels(bool menuPanelEnabled) {
		menuCanvasMenuPanel.SetActive(menuPanelEnabled);
		menuCanvasGamePanel.SetActive(!menuPanelEnabled);
	}

	//Sets the game canvas
	public void SetGameCanvas(string text, bool enabled) {
		gameCanvasText.text = text;
		gameCanvasPanel.SetActive(enabled);
	}
}
