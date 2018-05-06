using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if(other.gameObject.CompareTag("Player")){
			GameManager.instance.addPoint();
			gameObject.SetActive(false);
		}
	}
}
