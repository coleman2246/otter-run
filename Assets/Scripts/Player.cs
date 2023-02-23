using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] LevelGeneration levelGen;

    public static float movementSpeed = 10f; //units are m/s
    public static float jumpHeight = 2.5f;
    public static int maxMidAirJumps = 2;

    private Vector3 lastLocation;
    private Rigidbody2D rigidBody = null;
    private float debounceTime;
    private float pauseDebounceTime;


    public bool isDead = false;
    public bool isPaused = false;
    public int jumpsRemaining = maxMidAirJumps;
    public float progress = 0; // percent done the level
    public float multiplier = 1;
    public float score = 0;



    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector3(movementSpeed,0,0);
        lastLocation = transform.position;
        debounceTime = Time.time;
        pauseDebounceTime = Time.time;
    }

    

    void Update()
    {
        if(Input.GetButton("Cancel") & pauseDebounceTime < Time.realtimeSinceStartup)
        {
            isPaused = !isPaused;
            pauseDebounceTime = Time.realtimeSinceStartup + 0.2f;
        }
        

        if(isPaused)
        {
            Time.timeScale = 0;
            levelGen.audioAnal.audioSource.Pause();
        }
        else
        {
            Time.timeScale = 1;
            levelGen.audioAnal.audioSource.UnPause();
        }


        UpdateProgress();
        UpdateScore();

    }
    
    
    void FixedUpdate()
    {
        
        if(isDead)
        {
            rigidBody.velocity = new Vector2(0,0);
            return;
        }

        rigidBody.velocity = new Vector2(movementSpeed,rigidBody.velocity.y);


        if (Input.GetButton("Jump") && jumpsRemaining > 0 && Time.time > debounceTime)
        {
            float requiredForce = rigidBody.mass * rigidBody.gravityScale * jumpHeight;
            rigidBody.AddForce(new Vector2(0,requiredForce),ForceMode2D.Impulse);
            jumpsRemaining -= 1;
            debounceTime = Time.time + .1f;
        }



    }

    public void KillPlayer()
    {
        //isDead = true;
    }



    public void TouchedFloor()
    {
        jumpsRemaining = maxMidAirJumps;
    }

    public void UpdateProgress()
    {
        //startLocation.x - 
        float levelLen = Mathf.Abs(levelGen.startLocation.x - levelGen.endLocation.x);
        float percent =  Mathf.Abs(transform.position.x - levelGen.endLocation.x) / levelLen;

        if(percent > 1.0f)
        {
            this.progress = 100;
            return;
        }

        this.progress = (1-percent) * 100;
    }

    void UpdateScore()
    {
        score += multiplier * (transform.position.x - lastLocation.x);
        lastLocation = transform.position;
    }

    public void IncrementMultiplier()
    {
        this.multiplier += .25f;
    }

    public void EndLevel()
    {
        Debug.Log("Level Ended");
    }

}
