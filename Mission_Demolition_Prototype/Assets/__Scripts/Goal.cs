using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	//a static field accessible by code anywhere
	static public bool goalMet = false;

	void OnTriggerEnter(Collider other){
		//When the trigger is hit by something check to see if it's a projectile
		if(other.gameObject.tag == "Projectile"){
			//If so, set goalMet to true
			Goal.goalMet= true;
			//Also set the alpha of the color to higher opacity
			Color c = GetComponent<Renderer>().material.color;
			c.a = 1;
			GetComponent<Renderer> ().material.color = c;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
