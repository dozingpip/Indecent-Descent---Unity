﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            int length = new_values.Count;
            new_weights = new List<float>(length);
            for (int i = 0; i < length; i++)
            {
                new_weights[i] = 1 / length;
            }
        }
		weights.Add(new_weights);
	}

	public bool ContainsKey(char key){
		return keys.Contains(key);
	}

	// Define the indexer to allow client code to use [] notation.
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