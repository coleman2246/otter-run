using System.Collections;
using System.Linq;
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
    public int seed = 100;
    public List<List<BuildBlocks>> grid = new List<List<BuildBlocks>>();
    public const float unitTime = 2f;
    public List<GameObject> prefab;
    private AudioAnalysis audioAnalysis;


    private int worldStartX;
    private int worldStartY;
    public int worldEndX;
    public int worldEndY;

    private int gridHeight;
    private int gridWidth;
    private int gridMiddleX;
    private int gridMiddleY;

    private List<(int,int)> floorCords = new List<(int,int)>();
    private List<(int,int)> obstacleCords = new List<(int,int)>();
    private int maxDeathObstacles;


    public LevelUnit(int worldStartX, int worldStartY, List<GameObject> prefab, AudioAnalysis audioAnalysis)
    {

        this.worldStartX = worldStartX;
        this.worldStartY = worldStartY;
        this.gridWidth = Mathf.CeilToInt(unitTime * Player.movementSpeed);
        this.gridHeight = 20;

        this.gridMiddleX = Mathf.CeilToInt(this.gridWidth / 2f);
        this.gridMiddleY = Mathf.CeilToInt(this.gridHeight / 2f);

        this.worldEndX = worldStartX + gridWidth;
        

        // no more than 2 per sec
        this.maxDeathObstacles = Mathf.FloorToInt(unitTime * 2.0f);;

        this.prefab = prefab;
        this.audioAnalysis = audioAnalysis;

        for(int i = 0; i < gridWidth; i++)
        {
            grid.Add(new List<BuildBlocks>());
            for(int j = 0; j < gridHeight; j++)
            {
                grid[i].Add(BuildBlocks.Air);
            }
        }
    }



    /*
    public LevelUnit(int startX, int startY, List<GameObject> prefab, AudioAnalysis audioAnalysis)
    {

        this.startX = startX;
        this.width = Mathf.CeilToInt(unitTime * Player.movementSpeed);
        this.endX = startX + width;
        this.startY = startY;
        this.endY = startY;
        this.height = 30;
        this.prefab = prefab;
        this.audioAnalysis = audioAnalysis;

        for(int i = 0; i < width; i++)
        {
            grid.Add(new List<BuildBlocks>());
            for(int j = 0; j < height; j++)
            {
                grid[i].Add(BuildBlocks.Air);
            }
        }
    }
    */

    public void GenerateUnit()
    {
        GenerateFloor();
        GenerateFloorSupport();
        GenerateDeathObstacles();
        GenerateBonus();
        InstantiateUnit();

    }

    void GenerateFloorSupport()
    {

        // want to start one bellow the floor
        FloodFill(0,this.gridMiddleY-1);
    }

    // this function is going to flood fill the FloorSupport
    // want to fill everything that bellow floor and still in grid
    void FloodFill(int x, int y)
    {
        // will recurse until out of bounds
        if(x >= this.gridWidth || y >= this.gridHeight || x < 0 || y < 0)
        {
            return;
        }

        if(this.grid[x][y] != BuildBlocks.Air || y >= this.floorCords[x].Item2)
        {
            return;
        }

        this.grid[x][y] = BuildBlocks.FloorSupport; 


        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                FloodFill(x + i, y + j);
            }
        }
    }

    void GenerateBonus()
    {

    }

    void GenerateDeathObstacles()
    {
        List<SoundBands> bands = new List<SoundBands>();
        List<float> powerIncreases = new List<float>();
        List<float> percents = new List<float>();
        

        foreach(SoundBands band in SoundBands.GetValues(typeof(SoundBands)))
        {
                
             float percent = this.audioAnalysis.highestDeltaPercent[band];
             float powerIncrease = this.audioAnalysis.binnedPowerLevelIncreases[band];

             bands.Add(band);
             powerIncreases.Add(powerIncrease);
             percents.Add(percent);

        }
        // sort the lists by powerIncreases in descending order
        var sortedLists = powerIncreases
            .Select((power, index) => new { Power = power, Bands = bands[index], Percents = percents[index] })
            .OrderByDescending(item => item.Power)
            .ToList();


        powerIncreases = sortedLists.Select(item => item.Power).ToList();
        bands = sortedLists.Select(item => item.Bands).ToList();
        percents = sortedLists.Select(item => item.Percents).ToList();


        UnityEngine.Random.InitState(seed + this.worldStartX + (int)powerIncreases[0]);

        int numObstacles = this.maxDeathObstacles;


        for(int i = 0; i < numObstacles; i ++)
        {

            int obstacleGridX = Mathf.FloorToInt(percents[i] * this.floorCords.Count);
            Debug.Log(obstacleGridX);
            // 1 above the foor
            int obstacleGridY = this.floorCords[obstacleGridX].Item2+1;

            int closetsObstacle = this.gridWidth;

            // find closest obstacle in unit to make sure there is not too many to jump over
            foreach(var currentObstacle in this.obstacleCords)
            {
                float distance = Mathf.Abs(obstacleGridX - currentObstacle.Item1);

                if(distance < closetsObstacle)
                {
                    closetsObstacle = Mathf.FloorToInt(distance);
                }
            }

            if(closetsObstacle > 2)
            {
                //bound check
                if(obstacleGridX >= this.gridWidth)
                {
                    continue;
                }
                if(obstacleGridY >= this.gridHeight)
                {
                    continue;
                }

                obstacleCords.Add((obstacleGridX, obstacleGridY));
                grid[obstacleGridX][obstacleGridY] = BuildBlocks.Death; 
            }
        }

    }

    void GenerateFloor()
    {
        int currentHeight = this.gridMiddleY;
        int lastChangeInHeight = 0;


        // loop over width
        // increment or decrement height randomly while making sure there is sufficient distance between deltas

        for(int x = 0; x < gridWidth; x++)
        {

            UnityEngine.Random.InitState(seed + x + this.worldStartX);

            float randomFloat = UnityEngine.Random.Range(-1f, 1f);

            if((x - lastChangeInHeight) > Mathf.Floor((float)gridWidth/3.0f))
            {
                if(randomFloat > 0.6)
                {
                    currentHeight += 1;
                    lastChangeInHeight = x;
                }
                else if (randomFloat < -0.6)
                {
                    currentHeight -= 1;
                    lastChangeInHeight = x;
                }

                if(currentHeight < 0)
                {
                    currentHeight = 0;
                }

                else if (currentHeight >= grid.Count)
                {
                    currentHeight = grid.Count-1;
                }
                

            }

            this.floorCords.Add((x,currentHeight));

            grid[x][currentHeight] = BuildBlocks.Floor; 
            
        }

        this.worldEndY = currentHeight-this.gridMiddleY + this.worldStartY;

    }




    void InstantiateUnit()
    {
        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if(grid[x][y] != BuildBlocks.Air)
                {
                    Vector2 blockPos = new Vector2(worldStartX + x, worldStartY + y);
                    Instantiate(prefab[(int)grid[x][y]], blockPos, Quaternion.identity); 
                }
            }
        }
        //Instantiate();
    }

    /*

    void GenerateFloor()
    {
        float heightAdjustment = 0;
        float currentHeight = 0;
        int lastChange = 0;
        int lastFullHeight = 0;

       
        float[] percents = new float[2];
        float[] powerIncreases = new float[2];
        bool[] done = new bool[2];

        foreach(SoundBands band in SoundBands.GetValues(typeof(SoundBands)))
        {
                
             float percent = this.audioAnalysis.highestDeltaPercent[band];
             float powerIncrease = this.audioAnalysis.binnedPowerLevelIncreases[band];

             if(powerIncrease > powerIncreases[0])
             {
                 powerIncreases[1]  = powerIncreases[0];
                 powerIncreases[0]  = powerIncrease;

                 percents[1] = percents[0];
                 percents[0] = percent;
             }


            //Debug.Log($"Band: {band} {audioAnal.highestDeltaPercent[band]} {audioAnal.binnedPowerLevelIncreases[band]}");
        }

        int lastDeathBlock = 0;
        UnityEngine.Random.InitState(seed);

        for(int x = 0; x < width; x++)
        {
            float xGen =  x + startX + seed;
            float yGen = currentHeight + startY + seed;

            heightAdjustment += UnityEngine.Random.Range(-1f, 0.5f);
            Debug.Log(heightAdjustment);
             
            
            
            
            currentHeight += heightAdjustment;

            if(Mathf.FloorToInt(currentHeight) > lastFullHeight && x-lastChange > 3)
            {

                lastChange = x;
                lastFullHeight += 1*(int)Mathf.Sign(currentHeight);;
                heightAdjustment = 0;
            }
            


            //Debug.Log($"{xGen},{yGen}");
            Debug.Log($"heght {currentHeight} {heightAdjustment} {lastFullHeight}");

            if(currentHeight >= height)
            {
                currentHeight = height-1;
            }

            if(currentHeight < 0)
            {
                currentHeight = 0;
            }

            grid[x][lastFullHeight] = BuildBlocks.Floor; 


            for(int i = 0; i < 2; i++)
            {
                if(!done[i] && ( (float)x / (float) width > percents[i]) && powerIncreases[i] > 8000 && (x - lastDeathBlock) > 3)
                {
                    grid[x][lastFullHeight+1] = BuildBlocks.Death; 
                    done[i] = true;
                    lastDeathBlock  = x;
                }
            }
            

        }
        endY = Mathf.CeilToInt(lastFullHeight) + startY;
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
                else if(grid[x][y] == BuildBlocks.Death)
                {
                    Vector2 blockPos = new Vector2(startX + x, startY + y);
                    Instantiate(prefab[(int)grid[x][y]], blockPos, Quaternion.identity); 
                }


            }
        }
        //Instantiate();
        
    }
    */ 

}
