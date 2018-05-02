using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour {

	Multimap map;

	// Use this for initialization
	void Start () {
		map = new Multimap();
		mapIt();
		parse("S");
		
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
		pathOptions.Add("KD(P)");
		pathOptions.Add("K");
		pathOptions.Add("KJ");
		pathOptions.Add("KGKD(P)");

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
		map.Add('J', pathOptions, pathWeights);

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
		foreach(char c in key){
			if(c.IsUpper()){
				string i = map[c];
				parse(i);
			}else{
				// terminal cases
			}
		}
	}
}
