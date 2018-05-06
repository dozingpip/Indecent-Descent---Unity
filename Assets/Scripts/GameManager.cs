using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	//Static instance of GameManager which allows it to be accessed by any other script.
	public static GameManager instance = null;

	// represents the index of the current scene
	private int sceneIndex;
	//number of total scenes
	public int scenes;

	static int points;

	string mainSceneName = "levelQuestionMark";

	Text scoreDisplay;

	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
				//if not, set instance to this
				instance = this;

		//If instance already exists and it's not this:
		else if (instance != this)
				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
				Destroy(gameObject);

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);

		scenes = SceneManager.sceneCountInBuildSettings;

		//Call the InitGame function to initialize the first level
		InitGame();

	}

	//Initializes the game for each level.
	void InitGame()
	{
		SceneManager.sceneLoaded += stageInit;
		sceneIndex = SceneManager.GetActiveScene().buildIndex;
	}

	// Called when a new scene is loaded
	// initialize variables for all the objects game manager needs to know about
	void stageInit(Scene scene, LoadSceneMode mode)
	{
		sceneIndex = SceneManager.GetActiveScene().buildIndex;

		GameObject[] scoreDispTemp = GameObject.FindGameObjectsWithTag("scoreDisplay");
		if(scoreDispTemp.Length>0){
			scoreDisplay = scoreDispTemp[0].GetComponent<Text>();
			scoreDisplay.text = "Score: " + points;
		}
	}

	//load level by scene name instead of index.
	public void loadLevel(String name){
		if(SceneManager.GetSceneByName(name) != null){
			SceneManager.LoadScene(name);
			if(name==mainSceneName){
				points = 0;
			}
		}
	}

	public void addPoint(){
		points++;
		scoreDisplay.text = "Score: " + points;
	}

    public void gameOver()
    {
        SceneManager.LoadScene("mainMenu");
    }
}