using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;  // Required to work with UI, e.g., Text

public class MapGenerator : MonoBehaviour
{
    public Text variables;
    public GameObject player; 
    public Tilemap layer0;
    public Tilemap layer1;
    public Tilemap layer2;
    public TileBase grass;
    public TileBase wall;
    public TileBase wallTop;
    public float seed;
    public float threshold;

    //xscale and yscale response to how big the obstacles would be
    [Range(0,10)]
    public float yscale;
    [Range(0,10)]
    public float xscale;

    private int[,] cavePoints;

    [Range(0,100)]
    public static int width = 60;

    [Range(0,100)]
    public static int height = 40;

    [Range(0,5)]
    public int smoothCycles;


    //only works when set to 4 for some reasons.
    private int thickness = 4;

    //determine where to spawn the player/enemy
    public int Vertical;
    public int Horizontal;
    void Start()
    {
        Vertical = (int)Camera.main.orthographicSize;
        Horizontal = Vertical * (Screen.width / Screen.height);
        //Debug.Log("Vertical: " + Vertical + ",Horizontal: " + Horizontal);   
        generateNewGrid();
        //List<Coord> temp = GetRegionTiles(100, 100);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        { 
            allGeneration();        
            generateNewGrid();           
        }
        if (Input.GetKeyDown(KeyCode.F))
        { 
            forrestGeneration();        
            generateNewGrid();           
        }
        if (Input.GetKeyDown(KeyCode.Z))
        { 
            mazeGeneration();        
            generateNewGrid();           
        }
        if (Input.GetKeyDown(KeyCode.T))
        { 
            mountainGeneration();        
            generateNewGrid();           
        }
        if (Input.GetKeyDown(KeyCode.O))
        { 
            optimizedGeneration();        
            generateNewGrid();           
        }
    }

//Move player or enemies to cavePoints[x,y] if it's a good place
//return true if move successful
//false otherwise
public bool moveObject(GameObject obj)
{
    for (int y = 5; y < height; ++y)
    {
        for (int x = 5; x < width; ++x)
        {
            
            if  (cavePoints[x,y] == 0)
            {
                //attemp to find the first area  where objects can spawn
                //  - 0 -
                //  0 0 0
                //  - 0 -
                if (((cavePoints[x-1,y] == 0) && (cavePoints[x+1,y] == 0))
                && ((cavePoints[x,y+1] == 0) && (cavePoints[x,y-1] == 0)))
                {
                    //TODO: Remove this junk and make something better.
                    /*Vector3 p = new Vector3(0,0,0);
                    p.y = y - (Vertical - 0.5f);
                    p.x = x - (Horizontal - 0.5f);
                    player.transform.position += p;
                    Debug.Log(x +"  "+ y);*/
                    goto LoopEnd;
                    
                }
            }
        }       
    }
    return false;
    LoopEnd:
        {
            Debug.Log("Found a place!");
            return true;
        }
}


//All possible generation result
public void allGeneration()
{   
    xscale = Random.Range(0,10f);
    yscale = Random.Range(0,10f);
    threshold = Random.Range(0.0f,0.5f);
    smoothCycles = Random.Range(0,5);
}
public void forrestGeneration()
{
    xscale = 4;
    yscale = 4;
    threshold = 0.38f;
    smoothCycles = 1;
}
public void mazeGeneration()
{
    xscale = 3;
    yscale = 3;
    threshold = 0.35f;
    smoothCycles = 0;
}
public void mountainGeneration()
{
    xscale = 10;
    yscale = 10;
    threshold = 0.3f;
    smoothCycles = Random.Range(1,4);
}
public void optimizedGeneration()
{
    xscale = Random.Range(3f,4f);
    yscale = Random.Range(2f,3f);
    threshold = Random.Range(0.37f,0.42f);
    smoothCycles = Random.Range(1,3);
}
public void generateNewGrid()
{
    cavePoints = new int[width, height];
    seed = Random.Range(0,200f);
    variables.text =
                "seed: " + seed + 
                "\nxscale: " + xscale + 
                "\nyscale: " + yscale + 
                "\nthreshold: " + threshold + 
                "\nsmoothCycles: " + smoothCycles;
    //allGeneration();
    //optimizedGeneration();
    //forrestGeneration();
    layer0.ClearAllTiles();
    layer1.ClearAllTiles();
    layer2.ClearAllTiles();
    //Debug.Log("newSeed: " + seed);
    //Debug.Log("newThreshold: " + threshold);

    for (int y = 0; y < height; ++y)
    {
        for (int x = 0; x < width; ++x)
        {
            if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                cavePoints[x, y] = 1;
                }
                else
                {
                    float sample = Mathf.PerlinNoise(x / xscale + seed, y / yscale + seed);               
                
                    if (sample > threshold)
                    {
                        cavePoints[x,y] = 0;
                    } 
                    else
                    {
                        cavePoints[x,y] = 1;
                    }  
                }                    
            
        }
    }

    for (int i = 0; i < smoothCycles; i++)
        {
            //Debug.Log("Smoothcycle: " + smoothCycles );
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int neighboringWalls = GetNeighbors(x, y);

                    if (neighboringWalls > thickness)
                    {
                        cavePoints[x, y] = 1;
                    }else if (neighboringWalls < thickness)
                    {
                        cavePoints[x, y] = 0;
                    }
                }
            }
        }  
    ProcessMap();
    generateTile();      
}

List<List<Coord>> GetRegions(int tileType) {
		List<List<Coord>> regions = new List<List<Coord>> ();
		int[,] mapFlags = new int[width,height];

		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (mapFlags[x,y] == 0 && cavePoints[x,y] == tileType) {
					List<Coord> newRegion = GetRegionTiles(x,y);
					regions.Add(newRegion);

					foreach (Coord tile in newRegion) {
						mapFlags[tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

List<Coord> GetRegionTiles(int startX, int startY) {
		List<Coord> tiles = new List<Coord> ();
		int[,] mapFlags = new int[width,height];
		int tileType = cavePoints [startX, startY];

		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (new Coord (startX, startY));
		mapFlags [startX, startY] = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();
			tiles.Add(tile);

			for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
					if (IsInMapRange(x,y) && (y == tile.tileY || x == tile.tileX)) {
						if (mapFlags[x,y] == 0 && cavePoints[x,y] == tileType) {
							mapFlags[x,y] = 1;
							queue.Enqueue(new Coord(x,y));
						}
					}
				}
			}
		}

		return tiles;
	}

    bool IsInMapRange(int x, int y) {
		return x >= 0 && x < width && y >= 0 && y < height;
	}

    void ProcessMap() {
		List<List<Coord>> wallRegions = GetRegions (1);
		int wallThresholdSize = 5;

		foreach (List<Coord> wallRegion in wallRegions) {
			if (wallRegion.Count < wallThresholdSize) {
				foreach (Coord tile in wallRegion) {
					cavePoints[tile.tileX,tile.tileY] = 0;
				}
			}
		}

		List<List<Coord>> roomRegions = GetRegions (0);
		int roomThresholdSize = 1000;
		
		foreach (List<Coord> roomRegion in roomRegions) {
			if (roomRegion.Count < roomThresholdSize) {
				foreach (Coord tile in roomRegion) {
					cavePoints[tile.tileX,tile.tileY] = 1;
				}
			}
		}
	}


void generateTile()
{
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            if(cavePoints[x,y] == 0)
            {
                setFloor(x,y);
            }
            else if (cavePoints[x,y] == 1)
            {
                setWall(x,y);
            }
            //Check if the tile should be grass (0) or wall (1)
            //Debug.Log("x: " + x + ", y: " + y + ", [x,y] = " + cavePoints[x,y]);
        }
    }
}
void setFloor(int x, int y)
{
    layer0.SetTile(new Vector3Int(x, y, 0), grass);    
}
void setWall(int x,int y)
{
    layer1.SetTile(new Vector3Int(x, y, 0), wall);
    layer2.SetTile(new Vector3Int(x, y + 1, 0), wallTop);
}
private int GetNeighbors(int pointX, int pointY)
    {

        int wallNeighbors = 0;

        for (int x = pointX - 1; x <= pointX + 1; x++)
        {
            for (int y = pointY - 1; y <= pointY + 1; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    if (x != pointX || y!= pointY)
                    {
                        if (cavePoints[x,y] == 1)
                        {
                            wallNeighbors++;
                        }
                    }
                }
                else
                {
                    wallNeighbors++;
                }
            }
        }

        return wallNeighbors;
    }

    struct Coord {
        public int tileX;
        public int tileY;

        public Coord(int x, int y){
            tileX = x;
            tileY = y;
        }
    }
}
