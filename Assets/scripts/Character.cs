using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    LevelManager levelManager;
    public Rigidbody m_Rigidbody;
    public Animator animator;
    float jumpUpForce = 7.0f; 
    float jumpForwardForce = 4.1f;
    float runForwardForce = 0.2f;
    float birdBackwardForce = -0.1f;
    float birdUpForce = 1.0f;
    float glideForwardForce = 0.2f;
    float birdUpForceWithBear = 0.15f;

    float xAxisSpeedCapGround = 3.0f;
    float xAxisSpeedCapAir = 5.0f;
    float liftYAxisSpeedCap = 1f;
    float liftXAxisSpeedCap = -2f;
    float birdFlightCap = 4.0f;
    public float desiredBearAudioVolume = 0.0f;
    public float desiredBirdAudioVolume = 0.0f;
    
    public bool canJump = true;
    public bool usingBear = false;
    public bool usingBird = false;

    // Use this for initialization
    void Start () {
		m_Rigidbody = GetComponent<Rigidbody>();
        levelManager = (LevelManager)FindObjectOfType(typeof(LevelManager));
        animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {

        // If the game is not in play state return
        if (levelManager.gameState != "playing")
        {
            return;
        }

        // Bear controls --------------------------------------------------------------------------------------------
        if (Input.GetKey("space"))
        {
            usingBear = true;
            // Only glide if both are held down
            if (usingBird){
                m_Rigidbody.AddForce(new Vector3(glideForwardForce, 0, 0), ForceMode.VelocityChange);
            } else if (canJump){
                m_Rigidbody.AddForce(new Vector3(runForwardForce, 0, 0), ForceMode.VelocityChange);
            }
        }

        if (Input.GetKeyUp("space"))
        {
            // If they release spacebar
            usingBear = false;
            if (usingBird){
                animator.SetTrigger("Birdflying");
            }
            levelManager.bearAudioSource.volume = 0.0f;
            desiredBearAudioVolume = 0.0f;
        }

        if (Input.GetKeyDown("space") )
        {
            // Space is pressed
            usingBear = true;
            if (usingBird){
                animator.SetTrigger("Glide");
                levelManager.bearAudioSource.volume = 1.0f;
                desiredBearAudioVolume = 1.0f;
                levelManager.birdAudioSource.volume = 1.0f;
                desiredBirdAudioVolume = 1.0f;
            } else {
                if (canJump == true){
                    animator.SetTrigger("Jump");
                    m_Rigidbody.AddForce(new Vector3(jumpForwardForce, jumpUpForce, 0), ForceMode.VelocityChange);
                    levelManager.oneShotAudioSource.PlayOneShot(levelManager.jumpSFX);
                    levelManager.bearAudioSource.volume = 0.5f;
                    desiredBearAudioVolume = 0.5f;
                }
            }
			canJump = false;
        }

        // Bird controls --------------------------------------------------------------------------------------------        
        if (Input.GetMouseButtonUp(0))
        {
            usingBird = false;
            if (!canJump){
                animator.SetTrigger("Falling");
            }
            levelManager.birdAudioSource.volume = 0.0f;
            desiredBirdAudioVolume = 0.0f;
            m_Rigidbody.velocity = new Vector3(0,0,0);
        }
        if (Input.GetMouseButtonDown(0))
        {
            usingBird = true;
            if (usingBear){
                animator.SetTrigger("Glide");
                levelManager.birdAudioSource.volume = 1.0f;
                desiredBirdAudioVolume = 1.0f;
                levelManager.bearAudioSource.volume = 1.0f;
                desiredBearAudioVolume = 1.0f;
            } else {
                animator.SetTrigger("Birdflying");
                levelManager.birdAudioSource.volume = 0.5f;
                desiredBirdAudioVolume = 0.5f;
            }
        }
        if (Input.GetMouseButton(0))
        {
            usingBird = true;
            // If the bird isn't too high
            if (this.transform.position.y < birdFlightCap )
            {
                if (!usingBear){
                    m_Rigidbody.AddForce(new Vector3(birdBackwardForce, birdUpForce, 0), ForceMode.VelocityChange);
                } else {
                    m_Rigidbody.AddForce(new Vector3(birdBackwardForce, birdUpForceWithBear, 0), ForceMode.VelocityChange);
                }
                if (m_Rigidbody.velocity.y > liftYAxisSpeedCap)
                {
                    m_Rigidbody.velocity = new Vector3 (m_Rigidbody.velocity.x, liftYAxisSpeedCap, 0);
                }
                if (m_Rigidbody.velocity.x < liftXAxisSpeedCap)
                {
                    m_Rigidbody.velocity = new Vector3 (liftXAxisSpeedCap, m_Rigidbody.velocity.y, 0);
                }
                if (this.transform.position.x < 0.6f)
                {
                    this.transform.position = new Vector3(0.6f, this.transform.position.y, 0);
                    m_Rigidbody.velocity = new Vector3 (0, m_Rigidbody.velocity.y, 0);
                }
            }
        }
    }

    void LateUpdate()
    {
        if (m_Rigidbody.velocity.x > xAxisSpeedCapGround && canJump)
        {
            m_Rigidbody.velocity = new Vector3 (xAxisSpeedCapGround, m_Rigidbody.velocity.y, 0);
        }
        if (m_Rigidbody.velocity.x > xAxisSpeedCapAir && !canJump)
        {
            m_Rigidbody.velocity = new Vector3 (xAxisSpeedCapAir, m_Rigidbody.velocity.y, 0);
        }
        
    }

    void OnCollisionEnter (Collision collider)
    {
        if (collider.gameObject.name == "Ground")
        {
            canJump = true;
            usingBird = false;
            // We only want to go to Idle and turn off bird if we're on ground, not if we hit wall
            levelManager.birdAudioSource.volume = 0.0f;
            desiredBirdAudioVolume = 0.0f;
            if (levelManager.acorns.Count == levelManager.acornsCollected && levelManager.acorns.Count != 0){
                animator.SetTrigger("Dance");
            } else {
                animator.SetTrigger("Idle");
            }
            levelManager.oneShotAudioSource.PlayOneShot(levelManager.landSFX);
        }
    }
}
