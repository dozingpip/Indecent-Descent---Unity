using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour {

	Multimap map;
	public GameObject normalTile;
	public GameObject iceTile;
	public GameObject mudTile;
	public GameObject crackedTile;
	public GameObject collectiblePrefab;
	public GameObject wallPrefab;
	public GameObject enemyPrefab;
	public Transform pathBuilder;
	public int minPathLength = 10;
	public int maxPathLength = 20;
	public float initialEnemyChance = 0.05f;
	private float enemyChance;
	public GameObject playerPrefab;
	Stack<string> forbiddenDirections;

	private bool hasPassedMinimum = false;

	public float width = 20, length = 20;

	private float wallHeight;
	int levelQueueHeight = 5;
	int numLevel = 0;
	Queue<GameObject> levelQueue;
	GameObject levels;

	GameObject player;
	Rigidbody playerRB;

	// Use this for initialization
	void Start () {
		enemyChance = initialEnemyChance;
		levels = new GameObject();
		levels.name = "Levels";

		wallHeight = GameManager.wallHeight;

		levelQueue = new Queue<GameObject>();

		GameObject firstLevel = newLevel();
		levelQueue.Enqueue(firstLevel);
		numLevel++;

		for (int i = 0; i<levelQueueHeight; i++){
			levelQueue.Enqueue(newLevel());
			numLevel++;
		}
		Vector3 playerStart = Vector3.zero;
		playerStart.y += wallHeight;
		player = Instantiate(playerPrefab, playerStart, Quaternion.identity);
		playerRB = player.GetComponent<Rigidbody>();
	}

	void Update(){
		if((playerRB.position.y - levelQueue.Peek().transform.position.y) < -(wallHeight * 2)){
			DeleteOneAddOneLevel();
		}
	}

	void DeleteOneAddOneLevel(){
		GameObject dequeued = levelQueue.Dequeue();
		Destroy(dequeued);
		levelQueue.Enqueue(newLevel());
		numLevel++;
	}

	GameObject newLevel(){
		GameObject level = new GameObject();
		level.transform.position = new Vector3(0, -wallHeight*numLevel, 0);
		level.name = "Level "+numLevel;
		forbiddenDirections = new Stack<string>();
		map = new Multimap();
		mapIt();
		string path = createString("S");
		pathBuilder.position = new Vector3(Mathf.Floor(Random.Range(0, width)), -wallHeight*numLevel, Mathf.Floor(Random.Range(0, length)));
		buildPath(path, level.transform);

		Vector3 mid = makeWallsAround(level.transform);
		level.transform.position-=mid;
		level.transform.parent = levels.transform;
		checkWalls();

		spawnEnemies(level.transform);
		enemyChance = initialEnemyChance * Mathf.Sqrt(numLevel);
		return level;
	}

	void mapIt()
	{
		List<string> tileTypes = new List<string>();
		List<float> tileWeights = new List<float>();
		tileTypes.Add("n");
		tileTypes.Add("s");
		tileTypes.Add("i");
		tileTypes.Add("c");

		tileWeights.Add(0.65f);
		tileWeights.Add(0.10f);
		tileWeights.Add(0.10f);
		tileWeights.Add(0.15f);
		map.Add('T', tileTypes, tileWeights);

		List<string> directionOptions = new List<string>();
		List<float> directionWeights = new List<float>();
		directionOptions.Add("u");
		directionOptions.Add("d");
		directionOptions.Add("l");
		directionOptions.Add("r");

		directionWeights.Add(0.33f);
		directionWeights.Add(0.33f);
		directionWeights.Add(0.17f);
		directionWeights.Add(0.17f);
		map.Add('D', directionOptions, directionWeights);

		List<string> pathOptions = new List<string>();
		List<float> pathWeights = new List<float>();
		pathOptions.Add("KDP");
		pathOptions.Add("KJ");
		pathOptions.Add("KGKDP");

		pathWeights.Add(0.4f);
		pathWeights.Add(0.3f);
		pathWeights.Add(0.3f);
		map.Add('P', pathOptions, pathWeights);

		List<string> junctionOptions = new List<string>();
		List<float> junctionWeights = new List<float>();
		junctionOptions.Add("(DP.DP)");
		junctionOptions.Add("(DP.DP.DP)");

		junctionWeights.Add(0.5f);
		junctionWeights.Add(0.5f);
		map.Add('J', junctionOptions, junctionWeights);

		List<string> tileCollectOptions = new List<string>();
		List<float> tileCollectWeights = new List<float>();
		tileCollectOptions.Add("Tx");
		tileCollectOptions.Add("T");

		tileCollectWeights.Add(0.05f);
		tileCollectWeights.Add(0.95f);
		map.Add('K', tileCollectOptions, tileCollectWeights);

		List<string> gapOptions = new List<string>();
		List<float> gapWeights = new List<float>();
		gapOptions.Add("gx");
		gapOptions.Add("g");

		gapWeights.Add(0.20f);
		gapWeights.Add(0.80f);
		map.Add('G', gapOptions, gapWeights);

		List<string> startOptions = new List<string>();
		startOptions.Add("nDP");
		map.Add('S', startOptions);
	}

	string createString(string key)
	{
		int pathLength = 1;
		while(pathLength < maxPathLength && !isKeyFullOfTerminals(key)){
			key = parseIt(key);
			foreach(char c in key){
				if(c == 'P'){
					pathLength++;
				}else if(c == 'G'){
					pathLength+=2;
				}
			}
			if(!hasPassedMinimum && pathLength > minPathLength)
			{
				List<float> newPathWeights = new List<float>();
				newPathWeights.Add(0.3f);
				newPathWeights.Add(0.25f);
				newPathWeights.Add(0.25f);
				newPathWeights.Add(0.2f);
				map.addNewValueToKey('P', "K", newPathWeights);
				hasPassedMinimum = true;
			}
		}

		while(!isKeyFullOfTerminals(key)){
			key = wrapup(key);
		}
		
		Debug.Log("key: "+key+", length: "+pathLength);
		return key;
	}

	bool isKeyFullOfTerminals(string key){
		foreach(char c in key){
			if (!isTerminal(c)){
				return false;
			}
		}
		return true;
	}

	string parseIt(string str){
		string i = "";
		foreach(char c in str){
			if(isTerminal(c)){
				switch(c){
					case 'u':
						if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
						forbiddenDirections.Push("d");
						break;
					case 'd':
						if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
						forbiddenDirections.Push("u");
						break;
					case 'r':
						if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
						forbiddenDirections.Push("l");
						break;
					case 'l':
						if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
						forbiddenDirections.Push("r");
						break;
					default:
						break;
				}
				i += c;
			}else{
				if(c == 'D'){
					string dir = map[c];
					while(forbiddenDirections.Contains(dir)){
						dir = map[c];
					}
					switch(dir){
						case "u":
							if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
							forbiddenDirections.Push("d");
							break;
						case "d":
							if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
							forbiddenDirections.Push("u");
							break;
						case "r":
							if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
							forbiddenDirections.Push("l");
							break;
						case "l":
							if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
							forbiddenDirections.Push("r");
							break;
						default:
							break;
					}
					i+= dir;
				}else{
					string nextPath = map[c];
					if(nextPath == "KJ"){
						string junction = map['J'];
						if(junction.Length <= 7){ // 2 way junction
							string dir0 = map['D'];
							while(forbiddenDirections.Contains(dir0)){
								dir0 = map['D'];
							}
							forbiddenDirections.Push(dir0);

							string dir1 = map['D'];
							while(forbiddenDirections.Contains(dir1)){
								dir1 = map['D'];
							}
							i += "K("+dir0+"P."+dir1+"P)";
							forbiddenDirections.Pop();
							forbiddenDirections.Pop();
						}else{ // 3 way junction
							string dir0 = map['D'];
							while(forbiddenDirections.Contains(dir0)){
								dir0 = map['D'];
							}
							forbiddenDirections.Push(dir0);

							string dir1 = map['D'];
							while(forbiddenDirections.Contains(dir1)){
								dir1 = map['D'];
							}
							forbiddenDirections.Push(dir1);

							string dir2 = map['D'];
							while(forbiddenDirections.Contains(dir2)){
								dir2 = map['D'];
							}
							i += "K("+dir0+"P."+dir1+"P."+dir2+"P)";
							forbiddenDirections.Pop();
							forbiddenDirections.Pop();
							forbiddenDirections.Pop();
						}
					}
					else if(nextPath == "KGKDP")
					{
						string dir = map['D'];
						while (forbiddenDirections.Contains(dir))
						{
							dir = map['D'];
						}
						i += "KGK" + dir + "P";
					}
					else {
						i+=nextPath;
					}
				}
			}
		}
		return i;
	}

	string wrapup(string str){
		string i = "";
		foreach(char c in str){
			if(isTerminal(c))
				i += c;
			else if(c == 'P')
				i += map['K'];
			else if(c != 'J')
				i += map[c];
		}
		return i;
	}

	void buildPath(string fullString, Transform parent){
		Vector3 dir = new Vector3(0, 0, 0);
		Stack<Vector3> pathStack = new Stack<Vector3>();
		for (int i = 0; i < fullString.Length; i++)
		{
			char c = fullString[i];

			if(isTile(c)){
				if(c == 'g')
				{
					pathBuilder.position += dir;
				}
				Vector3 boxScale = new Vector3(0.25f, 0.25f, 0.25f);
				Collider[] hitColliders = Physics.OverlapSphere(pathBuilder.position, 0.25f);
				if(hitColliders.Length<1){
					switch(c){
						case 'n':
							Instantiate(normalTile, pathBuilder.position, normalTile.transform.rotation, parent);
							break;
						case 'c':
							Instantiate(crackedTile, pathBuilder.position, crackedTile.transform.rotation, parent);
							break;
						case 'i':
							Instantiate(iceTile, pathBuilder.position, iceTile.transform.rotation, parent);
							break;
						case 's':
							Instantiate(mudTile, pathBuilder.position, mudTile.transform.rotation, parent);
							break;
						case 'g':
							if (i + 1 < fullString.Length && fullString[i + 1] == 'x')
							{
								Debug.Log("SPAWNING GAP COLLECTIBLE");
								Instantiate(collectiblePrefab, pathBuilder.position + new Vector3(0, 2, 0), collectiblePrefab.transform.rotation, parent);
								pathBuilder.position += dir;
								i++;
							}
							else
							{
								Debug.Log("PLACING GAP");
								pathBuilder.position += dir;
							}
							break;
					}
				}
			}else{
				switch (c){
					case 'u':
						dir = new Vector3(0, 0, 1);
						pathBuilder.position+= dir;
						break;
					case 'd':
						dir = new Vector3(0, 0, -1);
						pathBuilder.position+= dir;
						break;
					case 'l':
						dir = new Vector3(-1, 0, 0);
						pathBuilder.position+= dir;
						break;
					case 'r':
						dir = new Vector3(1, 0, 0);
						pathBuilder.position+= dir;
						break;
					case 'x':
						Instantiate(collectiblePrefab, pathBuilder.position + new Vector3(0, 1, 0), collectiblePrefab.transform.rotation, parent);
						break;
					case '(':
						pathStack.Push(pathBuilder.position);
						break;
					case '.':
						pathBuilder.position = pathStack.Peek();
						break;
					case ')':
						pathStack.Pop();
						break;
				}
			}
		}
	}

	bool isTerminal(char c){
		return (char.IsLower(c) || c == '(' || c == ')' || c == '.');
	}

	bool isTile(char c){
		return (c == 'n' || c == 'i' || c == 's' || c == 'c' || c == 'g');
	}

	Vector3 makeWallsAround(Transform level){
		float smallestX = level.GetChild(0).position.x;
		float smallestZ = level.GetChild(0).position.z;
		float largestX = level.GetChild(0).position.x;
		float largestZ = level.GetChild(0).position.z;

		foreach(Transform child in level){
			if(child.position.x < smallestX){
				smallestX = child.position.x;
			}
			if(child.position.z < smallestZ){
				smallestZ = child.position.z;
			}
			if(child.position.x > largestX){
				largestX = child.position.x;
			}
			if(child.position.z > largestZ){
				largestZ = child.position.z;
			}
		}
		// center the walls around the level, cut off anything that's too far from this calculated center
		float midX = Mathf.Floor((smallestX + largestX)/2.0f);
		smallestX= Mathf.Floor(midX -(width/2.0f));
		largestX= Mathf.Floor(midX +(width/2.0f));

		float midZ = Mathf.Floor((smallestZ + largestZ)/2.0f);
		smallestZ= Mathf.Floor(midZ -(length/2.0f));
		largestZ= Mathf.Floor(midZ +(length/2.0f));

		Vector3 startOfWalls = new Vector3(smallestX, -numLevel*wallHeight, smallestZ);
		Vector3 upperRightEdge = new Vector3(smallestX, -numLevel*wallHeight, largestZ);
		Vector3 upperLeftEdge = new Vector3(largestX, -numLevel*wallHeight, largestZ);
		Vector3 lowerLeftEdge = new Vector3(largestX, -numLevel*wallHeight, smallestZ);

		GameObject walls = makeFourPointBox(startOfWalls, lowerLeftEdge, upperLeftEdge, upperRightEdge);
		walls.transform.parent = level;

//		checkWalls(walls, midX, midZ);
		return new Vector3(midX, 0, midZ);
	}

	void checkWalls(/*GameObject walls, float midX, float midZ*/){
		List<Collider> hitColliders = new List<Collider>();
		hitColliders.AddRange(Physics.OverlapBox(new Vector3(0, -wallHeight * numLevel, Mathf.Floor(length / 2) + 1), new Vector3(width/2, wallHeight/2, 0.5f), Quaternion.identity));
		hitColliders.AddRange(Physics.OverlapBox(new Vector3(0, -wallHeight * numLevel, Mathf.Floor(-length / 2) - 1), new Vector3(width / 2, wallHeight / 2, 0.5f), Quaternion.identity));
		hitColliders.AddRange(Physics.OverlapBox(new Vector3(Mathf.Floor(width / 2) + 1, -wallHeight * numLevel, 0), new Vector3(0.5f, wallHeight / 2, length / 2), Quaternion.identity));
		hitColliders.AddRange(Physics.OverlapBox(new Vector3(Mathf.Floor(-width / 2) - 1, -wallHeight * numLevel, 0), new Vector3(0.5f, wallHeight / 2, length / 2), Quaternion.identity));
		foreach (Collider c in hitColliders)
		{
			if (c.CompareTag("Collectible") || c.tag.Contains("Tile"))
			{
				c.gameObject.SetActive(false);
			}
		}

		//foreach (Transform child in walls.transform){
		//	Vector3 boxScale = new Vector3(child.localScale.x/2.5f, child.localScale.y / 2, child.localScale.z /2.5f);
		//	Vector3 checkPos = new Vector3(midX + child.position.x, child.position.y, midZ + child.position.z);
		//	Collider[] hitColliders = Physics.OverlapBox(child.position, boxScale, child.rotation);
		//	foreach(Collider c in hitColliders){
		//		if(c.CompareTag("Collectible") || c.tag.Contains("Tile")){
		//			c.gameObject.SetActive(false);
		//		}
		//	}
		//}
	}

	GameObject makeCubeBetweenPoints(Vector3 pointA, Vector3 pointB){
		Vector3 between = pointB - pointA;
		GameObject cube = Instantiate(wallPrefab, pointA, Quaternion.identity);
		cube.transform.rotation = Quaternion.LookRotation(between);
		cube.transform.localScale = new Vector3(1, wallHeight, between.magnitude);
		cube.transform.position = pointA + (between/2.0f);
		cube.transform.position+= new Vector3(0, wallHeight/2.0f, 0);
		return cube;
	}

	GameObject makeFourPointBox(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3){
		GameObject walls = new GameObject();
		walls.name = "walls";
		GameObject cube0 = makeCubeBetweenPoints(point0, point1);
		GameObject cube1 = makeCubeBetweenPoints(point1, point2);
		GameObject cube2 = makeCubeBetweenPoints(point2, point3);
		GameObject cube3 = makeCubeBetweenPoints(point0, point3);
		cube0.transform.parent =  walls.transform;
		cube1.transform.parent =  walls.transform;
		cube2.transform.parent =  walls.transform;
		cube3.transform.parent =  walls.transform;
		return walls;
	}

	void spawnEnemies(Transform level)
	{
		float tempEnemyChance = enemyChance;
		float thisRandom = 0;
		while((thisRandom = Random.value) < tempEnemyChance)
		{
			int wall = Random.Range(0, 3);
			float y = (-wallHeight * numLevel) + 1;
			float x = width / 2;
			float z = length / 2;
			switch (wall)
			{
				//North wall
				case 0:
					x = Random.Range(-width / 2, width / 2);
					break;
				//East wall
				case 1:
					z = Random.Range(-width / 2, width / 2);
					break;
				//South wall
				case 2:
					z = -z;
					x = Random.Range(-width / 2, width / 2);
					break;
				//West wall
				case 3:
					x = -x;
					z = Random.Range(-width / 2, width / 2);
					break;
				default:
					break;
			}
			Vector3 enemyPos = new Vector3(x, y, z);
			GameObject enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
			enemy.transform.parent = level;
			tempEnemyChance -= thisRandom;
		}	
	}
}
