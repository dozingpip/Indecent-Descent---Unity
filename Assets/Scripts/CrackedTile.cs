using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedTile : NormalTile {

	public List<Material> crackedLookProgression;
	public float breakTimer = 1;

	private bool cracking;

	int crackProgression;

	// Use this for initialization
	void Start () {
		cracking = false;
		crackProgression = 0;
//		base.CollisionEnterEvent += crack;
		//base.CollisionStayEvent += crack;
	}

	void FixedUpdate()
	{
		if (cracking)
		{
			breakTimer -= Time.fixedDeltaTime;
			if(breakTimer <= 0)
			{
				gameObject.SetActive(false);
//				Player.instance.checkFooting();
			}
		}
	}

	public void steppedOn(){
		cracking = true;
		crackProgression++;
		GetComponent<Renderer>().material = crackedLookProgression[crackProgression];
	}

	public bool isCracking()
	{
		return cracking;
	}
}
