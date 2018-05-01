using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multimap {
	List<String> keys;
	List<List<String>> values;
	List<List<float>> weights;
	public Multimap(){
		keys = new List<String>();
		values = new List<List<String>>();
		weights = new List<List<float>>();
	}

	public Add(String new_key, List<String> new_values, List<float> new_weights){
		keys.Add(new_key);
		values.Add(new_values);
		weights.Add(new_weights);
	}

	public ContainsKey(String key){
		return keys.Contains(key);
	}

	public ContainsValue(String valuesToCheck){
		return values.Contains(valuesToCheck);
	}

	// Define the indexer to allow client code to use [] notation.
	public String this[String i]
	{
		get 
		{
			int keyIndex = keys.IndexOf(i);
			List<String> myValues = values[keyIndex];
			List<float> myWeights = weights[keyIndex];
			return keys[i];
		}
	}
}
