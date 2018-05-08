using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	//Static instance of GameManager which allows it to be accessed by any other script.
	public static GameManager instance = null;

	public const float wallHeight = 14;

	//number of total scenes
	public int scenes;

	static int points;

	public static int floor;

	static bool inGame;

	string mainSceneName = "levelQuestionMark";

	Text scoreDisplay;

	Text floorDisplay;

	//Awake is always called before any Start functions
	void Awake()
	{
		inGame = false;
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
	}

	// Called when a new scene is loaded
	// initialize variables for all the objects game manager needs to know about
	void stageInit(Scene scene, LoadSceneMode mode)
	{

		GameObject[] scoreDispTemp = GameObject.FindGameObjectsWithTag("scoreDisplay");
		if(scoreDispTemp.Length>0){
			scoreDisplay = scoreDispTemp[0].GetComponent<Text>();
			scoreDisplay.text = "Score: " + points;
		}
		GameObject temp = GameObject.FindGameObjectWithTag("floorDisplay");
		if (temp != null)
		{
			floorDisplay = GameObject.FindGameObjectWithTag("floorDisplay").GetComponent<Text>();
			floorDisplay.text = "Floor: " + 1;
		}
	}

	//load level by scene name instead of index.
	public void loadLevel(String name){
		if(SceneManager.GetSceneByName(name) != null){
			SceneManager.LoadScene(name);
			if(name==mainSceneName){
				points = 0;
				inGame = true;
			}
		}
	}

	public void addPoint(){
		points++;
		if(scoreDisplay != null)
		{
			scoreDisplay.text = "Score: " + points;
		}
	}

	void Update()
	{
		if(inGame && Player.instance != null)
		{
			floor = 2 + (Mathf.FloorToInt(-Player.instance.transform.position.y / wallHeight));
			Debug.Log("Level: " + floor);
			if (floorDisplay != null)
			{
				floorDisplay.text = "Floor: " + floor;
			}
		}
	}

    public void gameOver()
    {
		inGame = false;
        SceneManager.LoadScene("gameOver");
    }
}