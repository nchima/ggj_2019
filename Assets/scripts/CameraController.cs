using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	public Character character; // This is a link to the prefab of the character in the scene
	Transform camTransform;
    public float followspeed;
 
 	public bool followPlayer = true;

	void Start () {
		camTransform = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (followPlayer){
			UpdateCamera();
		}
	}

	void UpdateCamera()
	{
		if (character != null){
			Vector3 targetPosition = new Vector3(character.transform.position.x, character.transform.position.y, camTransform.position.z);
			camTransform.position = Vector3.Lerp(camTransform.position, targetPosition, Time.deltaTime * followspeed);
		}
	}
}