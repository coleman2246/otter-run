using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject player;

    public static float movementSpeed = 10f; //units are m/s
    public static float jumpHeight = 3f;
    public static float airTime = 0.1f;
    public static int maxMidAirJumps = 2;

    private Vector3 lastLocation;
    private Vector2 startLocation;
    private Rigidbody2D rigidBody = null;
    private float debounceTime;
    private float pauseDebounceTime;
    private Animator animator;
    private string defaultAnimationState;

    public LevelGeneration levelGen;
    public bool isDead = false;
    public bool isPaused = false;
    public bool isEnded = false;
    public int jumpsRemaining = maxMidAirJumps;
    public float progress = 0; // percent done the level
    public float multiplier = 1;
    public float score = 0;
    public bool isMidAir = false;
    public bool expectingFall = false;
    public bool hasAnim = false;


    void Awake()
    {
        levelGen = GameObject.Find("Procedural Generation").GetComponent<LevelGeneration>();
    }
    void Start()
    {

        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        hasAnim = animator != null;

        rigidBody.velocity = new Vector3(movementSpeed,0,0);

        float y = levelGen.startLocation.y + levelGen.units[0].gridMiddleY + 1;

        // need to start close to 0 for time to line up
        transform.position = new Vector2(levelGen.startLocation.x+1,y+1);
        

        lastLocation = transform.position;
        startLocation = transform.position;
        debounceTime = Time.time;
        pauseDebounceTime = Time.time;

        if(hasAnim)
        {
            // found this here https://forum.unity.com/threads/animator-go-to-default-state-by-script.1189132/
            defaultAnimationState = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        }
    }

    

    void Update()
    {

        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0 && Time.time > debounceTime)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

            float v0 = (jumpHeight / airTime)  - (  (rigidBody.gravityScale * airTime) / 2f);

            rigidBody.velocity = new Vector2(rigidBody.velocity.x, v0);


            if(jumpsRemaining == maxMidAirJumps)
            {
                if(hasAnim)
                {
                    animator.SetTrigger("Jump");
                }

                isMidAir = true;
                expectingFall = true;
            }

            jumpsRemaining -= 1;
            debounceTime = Time.time + .05f;


        }

        if(!isMidAir && expectingFall)
        {

            if(hasAnim)
            {

                animator.SetTrigger("HitFloor");
            }

            expectingFall = false;
        }


        if(Input.GetButton("Cancel") & pauseDebounceTime < Time.realtimeSinceStartup)
        {
            isPaused = !isPaused;
            pauseDebounceTime = Time.realtimeSinceStartup + 0.2f;
        }
        

        if(isPaused)
        {
            StopLevel();
        }
        else
        {
            ContinueLevel();
        }

        if(isDead || isEnded)
        {
            StopLevel();
            if(Song.passedSongInstance != null)
            {
                HighscoreManager.UpdateHighScore(Song.passedSongInstance.md5Hash,score);
            }

        }


        UpdateProgress();
        UpdateScore();

    }


    void StopLevel()
    {
        Time.timeScale = 0;
        levelGen.audioAnal.audioSource.Pause();
    }

    public void ContinueLevel()
    {
        Time.timeScale = 1;
        levelGen.audioAnal.audioSource.UnPause();
    }
    
    
    void FixedUpdate()
    {
        
        rigidBody.velocity = new Vector2(movementSpeed,rigidBody.velocity.y);

    }

    public void KillPlayer()
    {
        isDead = true;
    }



    public void TouchedFloor()
    {
        
        isMidAir = false;
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
        isEnded = true;
    }

    public void RestartLevel()
    {
        //go back to start
        transform.position = startLocation;
        lastLocation = startLocation;

        
        // restart song
        levelGen.audioAnal.audioSource.Stop();
        levelGen.audioAnal.audioSource.Play();

        // reset stats
        multiplier = 1;
        score = 0;


        // clear all game states
        isEnded = false;
        isDead = false;
        isPaused = false; 

        // clear all forces
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0;

        jumpsRemaining = maxMidAirJumps-1;
        isMidAir = false;
        expectingFall = false;

        if(hasAnim)
        {
            animator.SetBool("HitFloor",false);
            animator.SetBool("Jump",false);
            animator.Play(defaultAnimationState);
        }
        
    }

}
