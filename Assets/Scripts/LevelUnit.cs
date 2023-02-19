using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildBlocks
{
    Air,
    FloorSupport,
    Floor,
    RampUp,
    RampDown,
    BonusPoint,
    HealthDown,
    Death,
    LevelEnd
}

public class LevelUnit : MonoBehaviour
{
    public int startX;
    public int endX;
    public int width;
    public int startY;
    public int endY;
    public int height;
    public int seed = 10;
    public float perlinHeightScale = 2;
    public float perlinSmooth = 1;
    public List<List<BuildBlocks>> grid = new List<List<BuildBlocks>>();
    public const float unitTime = 2f;
    public List<GameObject> prefab;

    public LevelUnit(int startX, int startY, List<GameObject> prefab)
    {
        this.startX = startX;
        this.width = Mathf.CeilToInt(unitTime * Player.movementSpeed);
        this.endX = startX + width;
        this.startY = startY;
        this.endY = startY;
        this.height = 20;
        this.prefab = prefab;

        for(int i = 0; i < width; i++)
        {
            grid.Add(new List<BuildBlocks>());
            for(int j = 0; j < height; j++)
            {
                grid[i].Add(BuildBlocks.Air);
            }
        }
    }

    public void GenerateUnit()
    {
        GenerateFloor();
        GenerateDeathObstacles();
        GenerateBonus();
        InstantiateUnit();

    }

    void GenerateBonus()
    {

    }

    void GenerateDeathObstacles()
    {

    }

    void GenerateFloor()
    {
        float heightAdjustment = 0;
        float currentHeight = 0;
        int lastChange = 0;
        int lastFullHeight = 0;

        for(int x = 0; x < width; x++)
        {
            float xGen =  x + startX + seed;
            float yGen =  currentHeight + heightAdjustment + startY + seed;

            heightAdjustment = 2* (Mathf.PerlinNoise(xGen,yGen) - 0.5f);
             
            
            if(Mathf.Floor(currentHeight +heightAdjustment) != lastFullHeight && x-lastChange < 5)
            {
                //currentHeight -= heightAdjustment;
            }
            else if(Mathf.FloorToInt(currentHeight) != lastFullHeight )
            {

                lastChange = x;
                lastFullHeight = Mathf.FloorToInt(currentHeight);
            }
            else 
            {

                currentHeight += heightAdjustment;

            }

            //Debug.Log($"{xGen},{yGen}");
            //Debug.Log(currentHeight);

            if(currentHeight >= height)
            {
                currentHeight = height-1;
            }

            if(currentHeight < 0)
            {
                currentHeight = 0;

            }

            grid[x][lastFullHeight] = BuildBlocks.Floor; 
        }
        endY = Mathf.CeilToInt(currentHeight) + startY;
    }

    void InstantiateUnit()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if(grid[x][y] == BuildBlocks.Floor)
                {
                    Vector2 blockPos = new Vector2(startX + x, startY + y);
                    Instantiate(prefab[(int)grid[x][y]], blockPos, Quaternion.identity); 
                }


            }
        }
        //Instantiate();
        
    }

}
