using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedTile : NormalTile {

	public List<Material> crackedLookProgression;
	int crackProgression;

	// Use this for initialization
	void Start () {
		crackProgression = 0;
		base.CollisionEnterEvent += crack;
		//base.CollisionStayEvent += crack;
	}

	void crack(){
		crackProgression++;
		GetComponent<Renderer>().material = crackedLookProgression[crackProgression];
	}
}
