using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;

public class InputsMonitor : MonoBehaviour {

	public UnityEvent UpArrowPressed;
	public UnityEvent RightArrowPressed;
	public UnityEvent DownArrowPressed;
	public UnityEvent LeftArrowPressed;
	public UnityEvent SpaceKeyPressed;
	public UnityEvent ReturnKeyPressed;
	public UnityEvent EscapeKeyPressed;

	// Use this for initialization
	void Start () {
		if (UpArrowPressed == null) {
			UpArrowPressed = new UnityEvent();
		}
		if (RightArrowPressed == null) {
			RightArrowPressed = new UnityEvent();
		}
		if (DownArrowPressed == null) {
			DownArrowPressed = new UnityEvent();
		}
		if (LeftArrowPressed == null) {
			LeftArrowPressed = new UnityEvent();
		}
		if (SpaceKeyPressed == null) {
			SpaceKeyPressed = new UnityEvent();
		}
		if (ReturnKeyPressed == null) {
			ReturnKeyPressed = new UnityEvent();
		}
		if (EscapeKeyPressed == null) {
			EscapeKeyPressed = new UnityEvent();
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.anyKey) {
			if (Input.GetKey(KeyCode.UpArrow) == true || Input.GetKey(KeyCode.W) == true) {
				if (UpArrowPressed != null) {
					UpArrowPressed.Invoke();
				}
			}
			if (Input.GetKey(KeyCode.RightArrow) == true || Input.GetKey(KeyCode.D) == true) {
				if (RightArrowPressed != null) {
					RightArrowPressed.Invoke();
				}
			}
			if (Input.GetKey(KeyCode.DownArrow) == true || Input.GetKey(KeyCode.S) == true) {
				if (DownArrowPressed != null) {
					DownArrowPressed.Invoke();
				}
			}
			if (Input.GetKey(KeyCode.LeftArrow) == true || Input.GetKey(KeyCode.A) == true) {
				if (LeftArrowPressed != null) {
					LeftArrowPressed.Invoke();
				}
			}
		}
		if (Input.anyKeyDown) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				if (SpaceKeyPressed != null) {
					SpaceKeyPressed.Invoke();
				}
			}
			if (Input.GetKeyDown(KeyCode.Return)) {
				if (ReturnKeyPressed != null) {
					ReturnKeyPressed.Invoke();
				}
			}
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
				if (EscapeKeyPressed != null) {
					EscapeKeyPressed.Invoke();
				}
			}
		}
	}
}
