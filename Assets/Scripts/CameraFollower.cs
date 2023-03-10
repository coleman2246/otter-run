using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Camera targetCamera;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        targetCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        targetCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, GetComponent<Camera>().transform.position.z);
    }
}
