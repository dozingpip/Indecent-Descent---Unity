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
	public Transform pathBuilder;
    public int minPathLength = 10;
    public int maxPathLength = 20;
	Stack<string> forbiddenDirections;


	float width = 10, length = 10, wallHeight = 6;
	GameObject level;

	// Use this for initialization
	void Start () {
		level = new GameObject();
		level.name = "Level 0";
		forbiddenDirections = new Stack<string>();
		map = new Multimap();
		mapIt();
		string path = createString("S");
		pathBuilder.position = new Vector3(Mathf.Floor(Random.Range(0, width)), 0, Mathf.Floor(Random.Range(0, length)));
		buildPath(path, level.transform);

		makeWallsAround(level.transform);
	}

	void mapIt()
	{
		List<string> tileTypes = new List<string>();
		List<float> tileWeights = new List<float>();
		tileTypes.Add("n");
		tileTypes.Add("s");
		tileTypes.Add("i");
		tileTypes.Add("c");

		tileWeights.Add(0.25f);
		tileWeights.Add(0.25f);
		tileWeights.Add(0.25f);
		tileWeights.Add(0.25f);
		map.Add('T', tileTypes, tileWeights);

		List<string> directionOptions = new List<string>();
		List<float> directionWeights = new List<float>();
		directionOptions.Add("u");
		directionOptions.Add("d");
		directionOptions.Add("l");
		directionOptions.Add("r");

		directionWeights.Add(0.25f);
		directionWeights.Add(0.25f);
		directionWeights.Add(0.25f);
		directionWeights.Add(0.25f);
		map.Add('D', directionOptions, directionWeights);

		List<string> pathOptions = new List<string>();
		List<float> pathWeights = new List<float>();
		pathOptions.Add("KDP");
		pathOptions.Add("KJ");
		pathOptions.Add("KGKDP");

		pathWeights.Add(0.34f);
		pathWeights.Add(0.33f);
		pathWeights.Add(0.33f);
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

		tileCollectWeights.Add(0.5f);
		tileCollectWeights.Add(0.5f);
		map.Add('K', tileCollectOptions, tileCollectWeights);

		List<string> gapOptions = new List<string>();
		List<float> gapWeights = new List<float>();
		gapOptions.Add("gx");
		gapOptions.Add("g");

		gapWeights.Add(0.5f);
		gapWeights.Add(0.5f);
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
            if(pathLength > minPathLength)
            {
                minimumReached();
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
				Debug.Log(c);
				switch(c){
					case 'u':
						Debug.Log("bad directions stack " + forbiddenDirections.Count + " on up");
						if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
						forbiddenDirections.Push("d");
						break;
					case 'd':
						Debug.Log("bad directions stack " + forbiddenDirections.Count + " on down");
						if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
						forbiddenDirections.Push("u");
						break;
					case 'r':
						Debug.Log("bad directions stack " + forbiddenDirections.Count + " on right");
						if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
						forbiddenDirections.Push("l");
						break;
					case 'l':
						Debug.Log("bad directions stack " + forbiddenDirections.Count + " on left");
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
							Debug.Log("bad directions stack " + forbiddenDirections.Count + " on up");
							if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
							forbiddenDirections.Push("d");
							break;
						case "d":
							Debug.Log("bad directions stack " + forbiddenDirections.Count + " on down");
							if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
							forbiddenDirections.Push("u");
							break;
						case "r":
							Debug.Log("bad directions stack " + forbiddenDirections.Count + " on right");
							if(forbiddenDirections.Count>0) forbiddenDirections.Pop();
							forbiddenDirections.Push("l");
							break;
						case "l":
							Debug.Log("bad directions stack " + forbiddenDirections.Count + " on left");
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
							Debug.Log("bad directions stack " + forbiddenDirections.Count + " junction dir 0");
							string dir0 = map['D'];
							while(forbiddenDirections.Contains(dir0)){
								dir0 = map['D'];
							}
							forbiddenDirections.Push(dir0);
							Debug.Log("bad directions stack " + forbiddenDirections.Count);
							string dir1 = map['D'];
							while(forbiddenDirections.Contains(dir1)){
								dir1 = map['D'];
							}
							i += "K("+dir0+"P."+dir1+"P)";
							forbiddenDirections.Pop();
							forbiddenDirections.Pop();
							Debug.Log("bad directions stack " + forbiddenDirections.Count + " after pops");
						}else{ // 3 way junction
							Debug.Log("bad directions stack " + forbiddenDirections.Count + " junction dir 0");
							string dir0 = map['D'];
							while(forbiddenDirections.Contains(dir0)){
								dir0 = map['D'];
							}
							forbiddenDirections.Push(dir0);

							Debug.Log("bad directions stack " + forbiddenDirections.Count + " junction dir 1");
							string dir1 = map['D'];
							while(forbiddenDirections.Contains(dir1)){
								dir1 = map['D'];
							}
							forbiddenDirections.Push(dir1);
							Debug.Log("bad directions stack " + forbiddenDirections.Count + " junction dir 2");

							string dir2 = map['D'];
							while(forbiddenDirections.Contains(dir2)){
								dir2 = map['D'];
							}
							i += "K("+dir0+"P."+dir1+"P."+dir2+"P)";
							forbiddenDirections.Pop();
							forbiddenDirections.Pop();
							forbiddenDirections.Pop();
							Debug.Log("bad directions stack " + forbiddenDirections.Count + " after pops");
						}
					}else{
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

    void minimumReached()
    {
        map.addNewValueToKey('P', "K");
    }

    void buildPath(string fullString, Transform parent){
		Vector3 dir = new Vector3(0, 0, 0);
		Stack<Vector3> pathStack = new Stack<Vector3>();
        for (int i = 0; i < fullString.Length; i++)
        {
            char c = fullString[i];
            switch (c){
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
				case 'u':
					dir = new Vector3(1, 0, 0);
					pathBuilder.position+= dir;
					break;
				case 'd':
					dir = new Vector3(-1, 0, 0);
					pathBuilder.position+= dir;
					break;
				case 'l':
					dir = new Vector3(0, 0, 1);
					pathBuilder.position+= dir;
					break;
				case 'r':
					dir = new Vector3(0, 0, -1);
					pathBuilder.position+= dir;
					break;
				case 'x':
					Instantiate(collectiblePrefab, pathBuilder.position + new Vector3(0, 1, 0), collectiblePrefab.transform.rotation, parent);
					break;
                case 'g':
                    if (i + 1 < fullString.Length && fullString[i + 1] == 'x')
                    {
                        pathBuilder.position += dir;
                        Instantiate(collectiblePrefab, pathBuilder.position + new Vector3(0, 1, 0), collectiblePrefab.transform.rotation, parent);
                        pathBuilder.position += dir;
                        i++;
                    }
                    else
                    {
                        pathBuilder.position += 2 * dir;
                    }
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

	bool isTerminal(char c){
		return (char.IsLower(c) || c == '(' || c == ')' || c == '.');
	}

	bool isTile(char c){
		return (c == 'n' || c == 'i' || c == 's' || c == 'c' || c == 'g');
	}

	void makeWallsAround(Transform level){
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

		
		// so the limits aren't right on top of any tiles before they should be
		smallestX--;
		smallestZ--;
		largestX++;
		largestZ++;
		// center the walls around the level, cut off anything that's too far from this calculated center
		float midX = (smallestX + largestX)/2.0f;
		smallestX=midX-(width/2.0f);
		largestX=midX+(width/2.0f);

		float midY = (smallestZ + largestZ)/2.0f;
		smallestZ=midY-(length/2.0f);
		largestZ=midY+(length/2.0f);

		Vector3 startOfWalls = new Vector3(smallestX, 0, smallestZ);
		Vector3 upperRightEdge = new Vector3(smallestX, 0, largestZ);
		Vector3 upperLeftEdge = new Vector3(largestX, 0, largestZ);
		Vector3 lowerLeftEdge = new Vector3(largestX, 0, smallestZ);

		makeFourPointBox(startOfWalls, lowerLeftEdge, upperLeftEdge, upperRightEdge);
	}

	void makeCubeBetweenPoints(Vector3 pointA, Vector3 pointB){
		Vector3 between = pointB - pointA;
		GameObject cube = Instantiate(wallPrefab, pointA, Quaternion.identity);
		cube.transform.rotation = Quaternion.LookRotation(between);
		cube.transform.localScale = new Vector3(1, wallHeight, between.magnitude);
		cube.transform.position = pointA + (between/2.0f);
		cube.transform.position+= new Vector3(0, wallHeight/2.0f, 0);
	}

	void makeFourPointBox(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3){
		makeCubeBetweenPoints(point0, point1);
		makeCubeBetweenPoints(point1, point2);
		makeCubeBetweenPoints(point2, point3);
		makeCubeBetweenPoints(point0, point3);
	}
}
