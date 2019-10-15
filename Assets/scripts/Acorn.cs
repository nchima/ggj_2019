using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acorn : MonoBehaviour {

	LevelManager levelManager;

	// Use this for initialization
	void Start () {
		levelManager = (LevelManager)FindObjectOfType(typeof(LevelManager));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	private void OnTriggerEnter(Collider other)
	{
		// Only go forward if this is the character
		if (!other.name.Contains("CharacterCollision")){
			return;
		}
		// They got the acorn!
		print ("You got an acorn!");
		levelManager.acornCollected();
		levelManager.oneShotAudioSource.PlayOneShot(levelManager.celebrationSFX);
		Destroy(gameObject);
	}
}
