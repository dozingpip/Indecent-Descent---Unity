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
	public int maxPathLength = 5;

	int width = 10, length = 10;

	// Use this for initialization
	void Start () {
		map = new Multimap();
		mapIt();
		parse("S");
		pathBuilder.position = new Vector3(Random.Range(0, width), 0, Random.Range(0, length));
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
		junctionOptions.Add("MM");
		junctionOptions.Add("MMM");

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
		startOptions.Add("nM");
		map.Add('S', startOptions);

		List<string> middleOptions = new List<string>();
		middleOptions.Add("nM");
		map.Add('M', middleOptions);
	}

	void parse(string key)
	{
		int pathLength = 0;
		foreach(char c in key){
			if(char.IsUpper(c) && pathLength < maxPathLength-1){
				string i = map[c];
				parse(i);
			}/*else if(char.IsUpper(c) && pathLength > maxPathLength){
				parse("K");
			}*/else{
				Vector3 dir = new Vector3(0, 0, 0);
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
						pathBuilder.position+= new Vector3(1, 0, 0);
						dir = new Vector3(1, 0, 0);
						break;
					case 'd':
						pathBuilder.position-= new Vector3(1, 0, 0);
						dir = new Vector3(-1, 0, 0);
						break;
					case 'l':
						pathBuilder.position-= new Vector3(0, 0, -1);
						dir = new Vector3(0, 0, -1);
						break;
					case 'r':
						pathBuilder.position+= new Vector3(0, 0, 1);
						dir = new Vector3(0, 0, 1);
						break;
					case 'x':
						Instantiate(collectiblePrefab, pathBuilder.position + new Vector3(0, 1, 0), collectiblePrefab.transform.rotation);
						break;
					case 'g':
						pathBuilder.position+= dir;
						break;
				}
			}
		}
	}
}
