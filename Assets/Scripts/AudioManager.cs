using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	public GameObject pacManEatDot;
	public GameObject pacManEatBoost;
	public GameObject pacManEatFruit;
	public GameObject pacManEatGhost;
	public GameObject pacManDead;
	public GameObject ghostSiren;
	public GameObject music;
	public AudioClip intermissionClip;
	public AudioClip openingSongClip;

	private Dictionary<string, GameObject> clipsDictionary;

	// Use this for initialization
	void Start () {
		clipsDictionary = new Dictionary<string, GameObject>();
		clipsDictionary.Add ("PacManEatDot", pacManEatDot);
		clipsDictionary.Add ("PacManEatBoost", pacManEatBoost);
		clipsDictionary.Add ("PacManEatFruit", pacManEatFruit);
		clipsDictionary.Add ("PacManEatGhost", pacManEatGhost);
		clipsDictionary.Add ("PacManDead", pacManDead);
		gameObject.GetComponent<GameStatesManager> ().ConsultingMenuGameState.AddListener(OnConsultingMenu);
		gameObject.GetComponent<GameStatesManager> ().GettingReadyGameState.AddListener(OnGettingReady);
		gameObject.GetComponent<GameStatesManager> ().WeakPacManGameState.AddListener(OnWeakPacMan);
		gameObject.GetComponent<GameStatesManager> ().StrongPacManGameState.AddListener(OnStrongPacMan);
		gameObject.GetComponent<GameStatesManager> ().PacManWinsGameState.AddListener(OnPacManWins);
		gameObject.GetComponent<GameStatesManager> ().PacManLosesGameState.AddListener(OnPacManLoses);
		gameObject.GetComponent<GameStatesManager> ().PausedGameState.AddListener(OnGamePaused);
	}
	
	// Update is called once per frame
	void Update () {

	}

	//Plays a sound if it is present in the dictionary
	public void PlaySound(string sound) {
		if (clipsDictionary.ContainsKey(sound)) {
			if (clipsDictionary[sound] != null) {
				if (!clipsDictionary[sound].GetComponent<AudioSource>().isPlaying) {
					clipsDictionary [sound].GetComponent<AudioSource> ().Play ();
				}
			}
		} else {
			Debug.Log ("This sound isn't listed in the dictionary.");
		}
	}

	public void OnConsultingMenu() {
		music.GetComponent<AudioSource> ().clip = openingSongClip;
		music.GetComponent<AudioSource> ().Play ();
	}

	public void OnGettingReady() {
		music.GetComponent<AudioSource> ().clip = intermissionClip;
		music.GetComponent<AudioSource> ().Play ();
	}

	public void OnWeakPacMan() {
		pacManEatDot.GetComponent<AudioSource> ().UnPause ();
		pacManEatBoost.GetComponent<AudioSource> ().UnPause ();
		pacManEatFruit.GetComponent<AudioSource> ().UnPause ();
		pacManEatGhost.GetComponent<AudioSource> ().UnPause ();
		pacManDead.GetComponent<AudioSource> ().UnPause ();
		ghostSiren.GetComponent<AudioSource> ().Play ();
		music.GetComponent<AudioSource> ().UnPause ();
	}

	public void OnStrongPacMan() {
		pacManEatDot.GetComponent<AudioSource> ().UnPause ();
		pacManEatBoost.GetComponent<AudioSource> ().UnPause ();
		pacManEatFruit.GetComponent<AudioSource> ().UnPause ();
		pacManEatGhost.GetComponent<AudioSource> ().UnPause ();
		pacManDead.GetComponent<AudioSource> ().UnPause ();
		ghostSiren.GetComponent<AudioSource> ().Stop();
		music.GetComponent<AudioSource> ().UnPause ();
	}

	public void OnNoPacMan() {
	}

	public void OnPacManWins() {
		music.GetComponent<AudioSource> ().clip = openingSongClip;
		music.GetComponent<AudioSource> ().Play ();
	}

	public void OnPacManLoses() {
	}

	public void OnGamePaused() {
		pacManEatDot.GetComponent<AudioSource> ().Pause ();
		pacManEatBoost.GetComponent<AudioSource> ().Pause ();
		pacManEatFruit.GetComponent<AudioSource> ().Pause ();
		pacManEatGhost.GetComponent<AudioSource> ().Pause ();
		pacManDead.GetComponent<AudioSource> ().Pause ();
		ghostSiren.GetComponent<AudioSource> ().Pause ();
		music.GetComponent<AudioSource> ().Pause ();
	}
}
