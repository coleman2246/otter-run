using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusOscillation : MonoBehaviour
{
    private Vector2 startLocation;
    public float frequency = 1;
    public float phase;

    void Start()
    {
        startLocation =  transform.position;
        phase = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
    }

    void Update()
    {
        transform.position = new Vector2(startLocation.x , (startLocation.y) + Mathf.Sin(2f * Mathf.PI * frequency * Time.time + phase));
    }

}
