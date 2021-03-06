﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameTimer : NetworkBehaviour {
	[SyncVar]
	private float gameStartTimer = 20.0f;

	[SyncVar]
	private float gameTimer = 60.0f;

	[SyncVar]
	private bool gameIsStarted = false;

	public int AdditionalSecondsPerPlayer;
	public UnityEngine.UI.Text timerMessage;
	public UnityEngine.UI.Text centerMessage;
	public UnityEngine.UI.Text runnerCount;
	private MultiplayerRoleStarter manager = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(manager == null)
			manager = GameObject.Find ("NetworkManager").GetComponent<MultiplayerRoleStarter> ();
		
		if (!gameIsStarted) {
			timerMessage.text = "";
			centerMessage.text = "Game Starts in " + (int)gameStartTimer; 
			gameStartTimer -= Time.deltaTime;
			if (gameStartTimer <= 0) {
				UpdateMenCount ();
				gameIsStarted = true;
				centerMessage.text = ""; 
				manager.maxConnections = manager.numPlayers;
				manager.SetHelpText("");
				manager.SetIPText ("");
				runnerCount.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			}
		} 

		if(gameIsStarted && gameTimer>0){
			
			if ((((int)gameTimer) % 60) < 10) {
				timerMessage.text = "Time Remaining: " + (((int)gameTimer) / 60) + ":0" + (((int)gameTimer) % 60);
			} else {
				timerMessage.text = "Time Remaining: " + (((int)gameTimer)/60) + ":" + (((int)gameTimer)%60);
			}

			centerMessage.text = "";
			manager.SetHelpText("");
			manager.SetIPText ("");

			gameTimer -= Time.deltaTime;
			if (gameTimer <= 0) {
				EndGame ();
			}
		}

	}

	public void PlayerAdded(){
		gameTimer += AdditionalSecondsPerPlayer;
	}

	public void EndGame(){
		int livingRunners = GetLivingRunners ();
		runnerCount.text = "Men Remaining: " + livingRunners;
		gameTimer = 0;
		if (livingRunners > 0) {
			centerMessage.text = "Game Over: Man wins! (" + livingRunners + " men remain)";
		} else {
			centerMessage.text = "Game Over: God wins! (" + livingRunners + " men remain)";
		}
	}

	int GetLivingRunners(){
		int livingRunners = 0;
		GameObject[] runners = GameObject.FindGameObjectsWithTag ("Runner");
		foreach (GameObject runner in runners) {
			RunnerHealth runnerHealth = runner.GetComponent<RunnerHealth> ();
			if (!runnerHealth.IsDead ()) {
				livingRunners++;
			}
		}
		return livingRunners;
	}

	public bool IsGameStarted(){
		return gameIsStarted;
	}

	public void UpdateMenCount(){
		int livingRunners = GetLivingRunners ();
		runnerCount.text = "Men Remaining: " + livingRunners;

		if (livingRunners <= 0)
			EndGame ();
	}
}
