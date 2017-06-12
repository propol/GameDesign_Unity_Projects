using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour {

	static public int score = 1000;

	// Use this for initialization
	void Start () {
		
	}

	void Awake(){
		//If the ApplePickerHighScore already exists, read it
		if(PlayerPrefs.HasKey("ApplePickerHighScore")){
			score = PlayerPrefs.GetInt ("ApplePickerHighScore");
		}
		//Assign the highscore to ApplePickerHighScore
		PlayerPrefs.SetInt("ApplePickerHighScore", score);
	}

	// Update is called once per frame
	void Update () {
		Text gt = this.GetComponent<Text> ();
		gt.text = "High Score:" + score;
		//Update ApplePickerHighScore if necessary
		PlayerPrefs.SetInt ("ApplePickerHighScore", score);
	}
}
