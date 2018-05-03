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
	public Transform pathBuilder;
	public int maxPathLength = 20;
	int doublemax = 50;

	int width = 10, length = 10;

	// Use this for initialization
	void Start () {
		map = new Multimap();
		mapIt();
		string path = createString("S");
		//Debug.Log(parseIt("nM"));
		pathBuilder.position = new Vector3(Random.Range(0, width), 0, Random.Range(0, length));
		buildPath(path);
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
		pathOptions.Add("K");
		pathOptions.Add("KJ");
		pathOptions.Add("KGKDP");

		pathWeights.Add(0.25f);
		pathWeights.Add(0.25f);
		pathWeights.Add(0.25f);
		pathWeights.Add(0.25f);
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
		string path = "";
		int pathLength = 1;
		int counter = 0;
		while(pathLength < maxPathLength /*&& counter<doublemax */&& key.Length>0){
			Debug.Log("count "+ counter);
			counter++;
			key = parseIt(key);
			foreach(char c in key){
				if(c == 'P'){
					pathLength++;
				}else if(c == 'G'){
					pathLength+=2;
				}
			}

			while(key.Length>0 && isTerminal(key[0])){
				path = path + key[0];
				key = key.Remove(0, 1);
			}
		}

		while(key.Length>0){
			key = wrapup(key);
			while(key.Length>0 && isTerminal(key[0])){
				path = path + key[0];
				key = key.Remove(0, 1);
			}
		}
		
		Debug.Log("path: "+path+", length: "+pathLength);
		return path;
	}

	string parseIt(string str){
		string i = "";
		foreach(char c in str){
			if(isTerminal(c))
				i += c;
			else
				i += map[c];
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
		Debug.Log("wrapup "+i);
		return i;
	}

	void buildPath(string fullString){
		Vector3 dir = new Vector3(0, 0, 0);
		Stack<Vector3> pathStack = new Stack<Vector3>();
		foreach(char c in fullString){
			switch(c){
				case 'n':
					Instantiate(normalTile, pathBuilder.position, normalTile.transform.rotation);
					break;
				case 'c':
					Instantiate(crackedTile, pathBuilder.position, crackedTile.transform.rotation);
					break;
				case 'i':
					Instantiate(iceTile, pathBuilder.position, iceTile.transform.rotation);
					break;
				case 's':
					Instantiate(mudTile, pathBuilder.position, mudTile.transform.rotation);
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
					Instantiate(collectiblePrefab, pathBuilder.position + new Vector3(0, 1, 0), collectiblePrefab.transform.rotation);
					break;
				case 'g':
					pathBuilder.position+= 2*dir;
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
}
