using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave : MonoBehaviour {

	LevelManager levelManager;

	public GameObject front;
	public ParticleSystem successParticles;

	public float fadeXThreshold = 12.0f;
	// Use this for initialization
	void Start () {
		levelManager = (LevelManager)FindObjectOfType(typeof(LevelManager));
	}
	
	// Update is called once per frame
	void Update () {
		if (levelManager.character!=null)
		{
			float distanceAway = gameObject.transform.position.x - levelManager.character.transform.position.x;
			if (distanceAway < fadeXThreshold)
			{
				levelManager.chordsAudioSource.volume = (distanceAway/fadeXThreshold)*0.5f;
				levelManager.idleAudioSource.volume = (distanceAway/fadeXThreshold)*0.5f;
				levelManager.bearAudioSource.volume = (distanceAway/fadeXThreshold)*levelManager.character.desiredBearAudioVolume;
				levelManager.birdAudioSource.volume = (distanceAway/fadeXThreshold)*levelManager.character.desiredBirdAudioVolume;
			}
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		// Only go forward if this is the character
		if (!other.name.Contains("CharacterCollision")){
			return;
		}
		// They got there!
		print ("You got home!");
		successParticles.Play();
		levelManager.oneShotAudioSource.PlayOneShot(levelManager.victorySFX);
		levelManager.character.m_Rigidbody.isKinematic = false;
		levelManager.character.m_Rigidbody.velocity = new Vector3(0,0,0);
		levelManager.character.m_Rigidbody.angularVelocity = new Vector3(0,0,0);
		levelManager.character.gameObject.SetActive(false);
		levelManager.finishedLevel(this);

	}
}
