using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject player;

    public static float movementSpeed = 10f; //units are m/s
    public static float jumpHeight = 3;
    private float displacement = 0;
    private Vector3 startLocation;
    [SerializeField] private Rigidbody2D rigidBody = null;
    public float score = 0;
    private float nextJumpTime;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector3(movementSpeed,0,0);
        startLocation = player.transform.position;
        nextJumpTime = Time.time + .2f;
    }

    

    void Update()
    {

    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided");

    }

    void FixedUpdate()
    {
        float dp = movementSpeed * Time.deltaTime;
        //player.transform.Translate(Vector3.right * dp);
        //score += dp;

        rigidBody.velocity = new Vector2(movementSpeed,rigidBody.velocity.y);


        if (Input.GetButton("Jump") && nextJumpTime < Time.time)
        {
            float requiredForce = rigidBody.mass * rigidBody.gravityScale * jumpHeight;
            rigidBody.AddForce(new Vector2(0,requiredForce),ForceMode2D.Impulse);
            nextJumpTime = Time.time+.1;

            Debug.Log(Input.GetButton("Jump"));
        }



    }

    void Jump()
    {

    }

}
