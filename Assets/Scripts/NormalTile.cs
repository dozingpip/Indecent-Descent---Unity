using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTile : MonoBehaviour {

	float timeColliding = 0;

	public delegate void EventHandler();
	public event EventHandler CollisionEnterEvent = () => {};
	public event EventHandler CollisionStayEvent = () => {};
	public event EventHandler CollisionExitEvent = () => {};

	void OnCollisionEnter(Collision collision){
		foreach(ContactPoint contact in collision){
			if(contact.otherCollider.gameObject.CompareTag("Player")){
				Debug.Log("player touch tile, what do?");
				CollisionEnterEvent();
			}
		}
	}

	void OnCollisionStay(Collision collision){
		foreach(ContactPoint contact in collision){
			if(contact.otherCollider.gameObject.CompareTag("Player")){
				Debug.Log("player continuing to touch tile, what do?");
				timeColliding+=Time.deltaTime;
			}
		}
	}

	void OnCollisionExit(Collision collision){
		foreach(ContactPoint contact in collision){
			if(contact.otherCollider.gameObject.CompareTag("Player")){
				Debug.Log("player stopped touching tile, what do?");
				timeColliding = 0;
			}
		}
	}

	void Delete(){
		Destroy(this.gameObject);
	}
}
