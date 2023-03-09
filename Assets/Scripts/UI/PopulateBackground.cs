using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateBackground : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] GameObject backgroundObject;
    private List<GameObject> bgObjects = new List<GameObject>();
    private Vector2 startScreen;
    private Vector2 prevScreen;

    void Start()
    {
        Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        for(int i = Mathf.FloorToInt(bottomLeft.x); i < Mathf.CeilToInt(topRight.x)+1; i++)
        {
            for(int j = Mathf.FloorToInt(bottomLeft.y);  j < Mathf.CeilToInt(topRight.y)+1; j++)
            {
                Vector2 pos = new Vector2(i, j);
                bgObjects.Add(Instantiate(backgroundObject, pos, Quaternion.identity));
            }
        }

        startScreen = new Vector2(Screen.width, Screen.height);
        prevScreen = new Vector2(Screen.width, Screen.height);
        
    }

    void Update()
    {
        if (prevScreen.x != Screen.width || prevScreen.y != Screen.height)
        {
            foreach(GameObject currObj in bgObjects)
            {
                currObj.transform.localScale = new Vector3(Screen.width / startScreen.x , Screen.height / prevScreen.y);

            }
            prevScreen = new Vector2(Screen.width, Screen.height);
        } 
    }

}
