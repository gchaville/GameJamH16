﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

	private Events eventManager;

	public static GameManager instance = null;	

	// Ecran Debut, GameOver/ Win
	private float levelStartDelay = 2f;
	private int level = 1;
	private GameObject levelImage;
	private Text levelText;
    private bool once10, once12;

	public Timer timer;
	private MapEditor mapScript;
	public GameObject HUD;
	private GameObject menuCamera;
	string infoEvent;

	private int enemiesMax;
	public int EnemiesMax {
		get {
			return enemiesMax;
		}
		set {
			enemiesMax = value;
		}
	}

	private int notesMax;
	public int NotesMax {
		get {
			return notesMax;
		}
		set {
			notesMax = value;
		}
	}

	private int enemiesMaxDamages;
	public int EnemiesMaxDamages {
		get {
			return enemiesMaxDamages;
		}
		set {
			enemiesMaxDamages = value;
		}
	}

	private Player player;

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);	

		DontDestroyOnLoad(gameObject);

		timer = GetComponent<Timer> ();
		eventManager = GetComponent<Events> ();
		mapScript = GetComponentInChildren<MapEditor> ();
		player = GetComponentInChildren<Player> ();

		// Setup Difficulty (notesMax, enemiesDamage ?)
		menuCamera = GameObject.Find ("MenuCamera");
		setupDifficulty ();
	}
		
	void Start() {
        once10 = false;
        once12 = false;
		levelImage = GameObject.Find ("LevelImage");
		HUD.GetComponent<AudioSource> ().Play ();
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();

		levelText.text = "Exam " + level;
		levelImage.SetActive (true);

		Invoke("HideLevelImage",HUD.GetComponent<AudioSource>().clip.length );

		Debug.Log ("Level 1 Start !");
	}

	void HideLevelImage()
	{
		levelImage.SetActive(false);
		timer.StartTimer ();
		HUD.SetActive (true);
		mapScript.InitializeMap ();
		GetComponent<AudioSource> ().Play ();
		GetComponent<AudioSource> ().loop =true;
		//StartCoroutine(eventLoop());
	}


	// Update is called once per frame
	void Update () {
		if ((timer.Hours == 9 && timer.Minutes == 55) || (timer.Hours == 11 && timer.Minutes == 55) )
			StartCoroutine (playRing ());
		
        if (timer.Hours == 10 && !once10)
        {
            eventManager.applyEventEffect(0);
			infoEvent = "Playtime, everybody out !"; 
            once10 = true;
        }
			
        if (timer.Hours == 12 && !once12 || Input.GetKeyDown(KeyCode.A))
        {
            eventManager.applyEventEffect(1);
			infoEvent = "FoodParty !! Hold a potatoe and Potate them !!!!!";
            once12 = true;
        }
            
		if (timer.Hours == 16)
			timer.StopTimer ();

		if (!player.Alive || !timer.enabled || player.Note == NotesMax)
			LoadExamScreen ();

       /* if(timer.Hours == 14 && timer.Minutes == 0)
        {
            eventManager.applyEventEffect(2, ref enemies);
        }*/
		
	}

	public bool AnswerLostInToilets() {
		Room toilet = GameObject.Find("ZoneToiletsF").GetComponent<Room>();
        if (toilet.HasNote)
        {
            notesMax--;
            return true;
        }
		return false;
	}

	public void GameOver() {
		//Set levelText to display number of levels passed and game over message
		levelText.text = "You failed your Exam " + level + " !!!!";

		//Enable black background image gameObject.
		levelImage.SetActive(true);
		GetComponent<AudioSource> ().Stop ();
		enabled = false;

	}

	public void GameWin() {
		//Set levelText to display number of levels passed and game over message
		levelText.text = "You success your Exam " + level + " !!!!";

		//Enable black background image gameObject.
		levelImage.SetActive (true);
		GetComponent<AudioSource> ().Stop ();
		enabled = false;
	}

	void LoadExamScreen() {
		enabled = false;
		GetComponent<AudioSource> ().Pause ();
		Clear ();

		SceneManager.LoadScene ("Examen");
	}

	void Clear() {
		Destroy (mapScript.gameObject);
	}

	void setupDifficulty() {
		int difficulty = menuCamera.GetComponent<Menu> ().Difficulty;
		switch (difficulty) {
		case 1:
			notesMax = 6;
			enemiesMaxDamages = 2;
			enemiesMax = 20;
			break;
		case 2:
			notesMax = 9;
			enemiesMaxDamages = 4;
			enemiesMax = 30;
			break;
		case 3:
			notesMax = 12;
			enemiesMaxDamages = 8;
			enemiesMax = 60;
			break;
		}

		Destroy (menuCamera);
	}

	IEnumerator playRing() {
		GetComponent<AudioSource> ().volume = 0.3f;
		HUD.GetComponent<AudioSource> ().Play ();

		HUD.GetComponent<HUD> ().roomIndic.text = infoEvent;
		HUD.GetComponent<HUD> ().roomIndic.gameObject.SetActive (true);

		//Debug.Log (HUD.GetComponent<AudioSource> ().isPlaying);
		//Debug.Log (GetComponent<AudioSource> ().isPlaying);
		yield return new WaitForSeconds (HUD.GetComponent<AudioSource> ().clip.length);
		HUD.GetComponent<HUD> ().roomIndic.gameObject.SetActive (false);
		GetComponent<AudioSource> ().volume=1.0f;
	}
		
	/*	IEnumerator eventLoop() {
		while (timer.enabled) {
			if ((timer.Hours == 10 && timer.Minutes == 0) || (timer.Hours == 12 && timer.Minutes == 0) || (timer.Hours == 14 && timer.Minutes == 0)) {
				yield return new WaitUntil (() => Input.GetKeyDown (KeyCode.A));
				int eventNumber = eventManager.getEvent ();

				eventManager.restoreEvents ();
				eventManager.applyEventEffect (eventNumber, ref enemies);
			}
		}
	}*/
}
