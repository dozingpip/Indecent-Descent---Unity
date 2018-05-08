using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// maps out what each tile should parse into so far as the grammar and has weights for each possible value
public class Multimap {
	List<char> keys;
	List<List<string>> values;
	List<List<float>> weights;
	public Multimap(){
		keys = new List<char>();
		values = new List<List<string>>();
		weights = new List<List<float>>();
	}

	public void Add(char new_key, List<string> new_values, List<float> new_weights = null){
		keys.Add(new_key);
		values.Add(new_values);
		if (new_weights == null)
		{
			new_weights = evenlyDistributeWeights(new_key);
		}
		weights.Add(new_weights);
	}

	public bool ContainsKey(char key){
		return keys.Contains(key);
	}

	public List<float> evenlyDistributeWeights(char key)
	{
		int keyIndex = keys.IndexOf(key);
		List<string> myValues = values[keyIndex];
		int length = myValues.Count;
		Debug.Log("Length is: " + length);
		List<float> new_weights = new List<float>(length);
		for (int i = 0; i < length; i++)
		{
			new_weights.Add(1 / length);
		}
		return new_weights;
	}

	// A hacky solution to the minimum length issue
	public void addNewValueToKey(char key, string newValue, List<float> new_weights = null)
	{
		int keyIndex = keys.IndexOf(key);
		values[keyIndex].Add(newValue);
		if (new_weights == null)
		{
			new_weights = evenlyDistributeWeights(key);
		}
	}

	// Define the indexer to allow client code to use [] notation.
	// picks a random value from this map key based on value weights
	public string this[char i]
	{
		get 
		{
			int keyIndex = keys.IndexOf(i);
			List<string> myValues = values[keyIndex];
			List<float> myWeights = weights[keyIndex];
			float randomNum = Random.value;
			int index = 0;
			while(index < myWeights.Count - 1)
			{
				if(randomNum < myWeights[index])
				{
					break;
				}
				randomNum -= myWeights[index];
				index++;
			}
			return myValues[index];
		}
	}
}
