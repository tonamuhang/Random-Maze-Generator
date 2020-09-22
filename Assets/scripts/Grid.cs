using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject terrainTile;
    public GameObject mazeTile;
    public GameObject mazeWall;
    public GameObject FPSController;
    public GameObject projectile;
    public int gridX, gridZ;
    public float gridSpaceOffset = 1f;
    public Vector3 gridOrigin = Vector3.zero;
    public Texture pathTexture;

    private int startMazeX, startMazeZ, endMazeX, endMazeZ;
    private int startPathX, startPathZ, endPathX, endPathZ;
    private List<Cell> path;
    public Cell[,] grid;
    public bool mazeSolvable = false;
    // Start is called before the first frame update
    void Start()
    {
        grid = new Cell[gridX, gridZ];
        SpawnGrid();
        System.Random rand = new System.Random();
        
        // Set path material
        int proCount = 0;
        int cellCount = 0;
        foreach(Cell cell in path)
        {
            cellCount += 1;
            cell.setMaterial(pathTexture);
            // cell.BecomeBeeg();
            cell.SpawnWall(mazeWall, gridSpaceOffset);


            // Randomly spawn projectiles
            if(rand.Next(10) % 2 == 0)
            {
                cell.SpawnProjectile(projectile, gridSpaceOffset);
                proCount += 1;
            }
            else
            {
                if(path.Count - cellCount + proCount < 8)
                {
                    cell.SpawnProjectile(projectile, gridSpaceOffset);
                    proCount += 1;
                }
            }
            
        }
        
        
        spawnMaze(grid[startMazeX, startMazeZ], grid[endMazeX, endMazeZ]);
        mazeWall.GetComponent<Renderer>().enabled = true;
        for(int x = 0; x < 5; x++)
        {
            for(int z = 6; z < 11; z++)
            {
                grid[x, z].SpawnWall(mazeWall, gridSpaceOffset);
                grid[x, z].visited = false;
            }
        }
        
        
        

    }

    // Update is called once per frame
    void Update()
    {
        
        mazeSolvable = false;
        DFS(grid[startMazeX, startMazeZ]);
        
        // Clear the visited status of all maze cells
        for(int x = 0; x < 5; x++)
        {
            for(int z = 6; z < 11; z++)
            {
                grid[x, z].visited = false;
            }
        }

        if(!mazeSolvable)
        {
            Debug.Log("Game over");
        }
        
    }
            
    
    // Perform a DFS to check if the maze is solvable
    void DFS(Cell entry)
    {
        entry.visited = true;
        
        // If the exit cell is visited, we can still solve the maze
        if(entry == grid[endMazeX, endMazeZ])
        {
            mazeSolvable = true;
            Debug.Log("Visited maze exit " + entry.x + ", " + entry.z);
            return;
        }

        Cell next = null;
        if(entry.up != null)
        {
            next = entry.up;
            if(!next.visited)
            {
                DFS(next);
            }
            
        }
        if(entry.left != null)
        {
            next = entry.left;
            if(!next.visited)
            {
                DFS(next);
            }
        }
        if(entry.right != null)
        {
            next = entry.right;
            if(!next.visited)
            {
                DFS(next);
            }
        }
        if(entry.down != null)
        {
            next = entry.down;
            if(!next.visited)
            {
                DFS(next);
            }
        }

        return;
    }

    void SpawnGrid()
    {
        for(int x = 0; x < gridX; x++)
        {
            for(int z = 0; z < gridZ; z++)
            {
                Vector3 spawnLocation = new Vector3(x * gridSpaceOffset, 0, z * gridSpaceOffset) + gridOrigin;
                GameObject clone = null;
                // Area reserved for maze
                int half = -1;
                if(z > 5 && z <= 10)
                {
                    clone = Instantiate(mazeTile, spawnLocation, Quaternion.identity);
                }
                else
                {
                    clone = Instantiate(terrainTile, spawnLocation, Quaternion.identity);
                }
                
                if(clone != null)
                {
                    
                    if(z <= 5)
                    {
                        half = 0;
                    }
                    else if(z > 10)
                    {
                        half = 1;
                    }
                    grid[x, z] = new Cell(x, z, clone, half);
                }
                else
                {
                    Debug.Log("Tile creation failed");
                }
                
            }
        }
        // randomly choose a starting point for the maze
        System.Random rand = new System.Random();
        int[] side = {6, 10};
        int mazeStartX = rand.Next(0, 5);
        int mazeStartZ = side[rand.Next(2)];
        int mazeEndZ = 16 - mazeStartZ;  // If startZ is 6, end is 10 vice versa
        int mazeEndX = rand.Next(0, 5);

        // Create maze

        // Create path using Wilson's algo
        int pathStartX = rand.Next(0, 5);
        int pathEndX = mazeStartX;
        int pathStartZ;
        int pathEndZ;
        
        // Make sure the starting x position is far from ending x
        while(Mathf.Abs(pathStartX - pathEndX) < 2)
        {
            pathStartX = rand.Next(0, 5);
        }

        if(mazeStartZ == 6)
        {
            pathStartZ = 0;
            pathEndZ = mazeStartZ - 1;
        }
        else
        {
            pathStartZ = 19;
            pathEndZ = mazeStartZ + 1;
        }

        // Create UST
        Cell pathHead = grid[pathStartX, pathStartZ];

        // RandomWalk(grid[pathEndX, pathEndZ], pathHead);
        
        path = RandomWalkTo(grid[pathEndX, pathEndZ], pathHead, 9);

        // Link path to maze
        if(pathEndZ > 10)
        {
            
            grid[pathEndX, pathEndZ].fixedLeft = grid[pathEndX, pathEndZ - 1];
            grid[pathEndX, pathEndZ - 1].fixedRight = grid[pathEndX, pathEndZ];
        }
        else
        {
            grid[pathEndX, pathEndZ].fixedRight = grid[pathEndX, pathEndZ + 1];
            grid[pathEndX, pathEndZ + 1].fixedLeft = grid[pathEndX, pathEndZ];
        }

        // Spawn an FPS controller
        Instantiate(FPSController, new Vector3(pathStartX * gridSpaceOffset, 1, pathStartZ * gridSpaceOffset) + gridOrigin, Quaternion.identity);

        startMazeX = mazeStartX;
        startMazeZ = mazeStartZ;
        endMazeX = mazeEndX;
        endMazeZ = mazeEndZ;

        startPathX = pathStartX;
        startPathZ = pathStartZ;
        endPathX = pathEndX;
        endPathZ = pathEndZ;
        
        
    }

    private void spawnMaze(Cell start, Cell end)
    {

        List<Cell> path = new List<Cell>();

        Debug.Log("Maze from " + start.x + ", " + start.z + " to " + end.x + ", " + end.z);
        // Random walk from the entry to exit
        path = RandomWalkTo(start, end, isMaze: true);
        
        LinkCells(grid[startMazeX, startMazeZ], grid[endPathX, endPathZ]);
        // Link path and maze
        if(endMazeZ == 10)
        {
            LinkCells(end, grid[end.x, end.z + 1]);
            
        }
        else
        {
            LinkCells(end, grid[end.x, end.z - 1]);
            
        }
        start.fixLinks();
        end.fixLinks();
        
        System.Random rand = new System.Random();
        // Check row by row if there are visited cells
        Cell cellInMaze = null;

        // while there are unvisited cells in the maze, keep checking
        
        for(int x = 0; x < gridX; x++)
        {
            for(int z = 6; z < 11; z++)
            {
                // If not visited, randomly connect the cell to one of the cell already visited
                if(!grid[x, z].visited)
                {
                    cellInMaze = path[rand.Next(path.Count)];
                    if(cellInMaze != grid[x, z])
                    {
                        RandomWalkTo(grid[x, z], cellInMaze, isMaze: true);
                    }
                    // reset the loop if an unvisited cell is found
                    x = 0;
                    z = 6;
                }
                // else
                // {
                //     grid[x, z].SpawnProjectile(projectile, gridSpaceOffset);
                // }
            }
        }
            


        
        // foreach(Cell cell in path)
        // {
        //     cell.SpawnWall(mazeWall, gridSpaceOffset);
        // }

    }

    private List<Cell> RandomWalkTo(Cell start, Cell end, int min_path_length = 0, bool isMaze = false)
    {
        Debug.Log("Walking from " + start.x + ", " + start.z + " to " + end.x + ", " + end.z);
        int direction;
        System.Random rand = new System.Random();
        List<Cell> path = new List<Cell>();

        if(isMaze)
        {
            if(end.z < 6 || end.z > 10)
            {
                Debug.Log("Outside maze, return null");
                return null;
            }
        }

        path.Add(start);
        
        while(true)
        {
            while(start != end)
            {
                direction = rand.Next(4);
                
                Cell resultCell = start.WalkToCell(grid, direction, isMaze: isMaze);
                
                // If invalid direction, repick a direction
                if(resultCell == null )
                {
                    continue;
                }

                // If bump into the path, we start over
                if(path.Contains(resultCell))
                {
                    start = path[0];
                    foreach(Cell cell in path)
                    {
                        cell.clearLink();
                        
                    }
                    path.Clear();
                    path.Add(start);
                }
                else
                {
                    
                    LinkCells(start, resultCell);
                    path.Add(resultCell);
                    start = resultCell;

                    // If the result cell is already visited, i.e. the current path bumps into an existing one, the walk is over
                    if(resultCell.visited)
                    {
                        Debug.Log("Bumped into path at " + resultCell.x + ", " + resultCell.z);
                        break;
                    }
                }
            }
            if(min_path_length != 0)
            {
                if(path.Count < min_path_length)
                {
                    Debug.Log("Too short");
                    // If the path is too short, we start over
                    start = path[0];
                    foreach(Cell cell in path)
                    {
                        cell.clearLink();
                    }
                    path.Clear();
                    path.Add(start);
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }            
        }
        
        // Walk is over
        
        foreach(Cell cell in path)
        {
            Debug.Log("Visited " + cell.x + ", " + cell.z);
            cell.visited = true;
            cell.fixLinks();
        }
        

        return path;

    }

    private void LinkCells(Cell start, Cell end)
    {
        // if(start.isMazeCell() && !end.isMazeCell())
        // {
        //     return;
        // }

        if(start.x - 1 == end.x && start.z == end.z)
        {
            start.up = end;
            end.down = start;
        }
        else if(start.x + 1 == end.x && start.z == end.z)
        {
            start.down = end;
            end.up = start;
        }
        else if(start.z - 1 == end.z && start.x == end.x)
        {
            start.left = end;
            end.right = start;
        }
        else if(start.z + 1 == end.z && start.x == end.x)
        {
            start.right = end;
            end.left = start;
        }
        else
        {
            Debug.Log("Invalid linkage");
        }
    }

    private bool CheckGameOver(Cell mazeEntry, Cell mazeExit)
    {
        // Perform a BST/DST on the entry node

        // If every node is visted, and mazeExit is not found, the game is over

        // Keep traversing until every node is visited

        return false;
    }

    
}
public class Cell
{
        // TODO: Add temporary and fixed cell linkage
        public int x, z;
        public Cell up, left, right, down;
        public Cell fixedUp, fixedLeft, fixedRight, fixedDown;
        private GameObject tile;
        public bool visited = false;
        public int half;
        public GameObject projectTile = null;
        public Cell(int x, int z, GameObject tile, int half)
        {
            this.x = x;
            this.z = z;
            this.tile = tile;
            this.half = half;
        }


        public Cell WalkToCell(Cell[,] grid, int direction, bool isMaze=false)
        {
            Cell cell = null;
            if(isMaze)
            {
                if(this.z >= 10 && direction == 2)
                {
                    return null;
                }
                if(this.z <= 6 && direction == 1)
                {
                    return null;
                }
            }

            switch(direction)
            {
                // 0 for up
                case 0:
                    if(this.x - 1 < 0)
                    {
                        break;
                    }
                    cell = grid[x - 1, z];
                    
                    break;
                // 1 for left
                case 1:
                    if(this.z - 1 < 0)
                    {
                        break;
                    }
                    // Constraint on one half
                    if(this.half == 1 && z - 1 <= 10)
                    {
                        break;
                    }
                    cell = grid[x, z - 1];
                    
                    break;
                // 2 for right
                case 2:
                    if(this.z + 1 >= grid.GetLength(1))
                    {
                        break;
                    }
                    if(this.half == 0 && z + 1 > 5)
                    {
                        break;
                    }
                    cell = grid[x, z + 1];
                    
                    break;
                // 3 for down
                case 3:
                    if(this.x + 1 >= grid.GetLength(0))
                    {
                        break;
                    }
                    cell = grid[x + 1, z];
                    
                    break;
                default:
                    Debug.Log("Invalid direction");
                    break;
            }
            if(cell == null)
            {
                Debug.Log("Invalid walk");
                return cell;
            }
            
            Debug.Log("Walked from " + this.x + ", " + this.z + " to " + cell.x + ", " + cell.z);  
            return cell;
            
        }


        public void setMaterial(Texture texture)
        {
            var renderer = this.tile.GetComponent<Renderer>();
            renderer.material.mainTexture = texture;
            Debug.Log("Set cell" + this.x + ", " + this.z);
            
        }

        public void BecomeBeeg()
        {
            this.tile.transform.localScale += new Vector3(0, 10, 0);
        }

        public void SpawnWall(GameObject mazeWall, float gridSpaceOffset)
        {
            if(this.fixedUp == null)
            {

                GameObject.Instantiate(mazeWall, 
                new Vector3(this.x * gridSpaceOffset - 5, 0, this.z * gridSpaceOffset) + Vector3.zero, 
                Quaternion.Euler(new Vector3(0, 90, 0)));
            }
            if(this.fixedDown == null)
            {

                GameObject.Instantiate(mazeWall, 
                new Vector3(this.x * gridSpaceOffset + 5, 0, this.z * gridSpaceOffset) + Vector3.zero, 
                Quaternion.Euler(new Vector3(0, 90, 0)));
            }
            if(this.fixedLeft == null)
            {

                GameObject.Instantiate(mazeWall, 
                new Vector3(this.x * gridSpaceOffset, 0, this.z * gridSpaceOffset - 5) + Vector3.zero, 
                Quaternion.Euler(new Vector3(0, 0, 0)));
            }
            if(this.fixedRight == null)
            {

                GameObject.Instantiate(mazeWall, 
                new Vector3(this.x * gridSpaceOffset, 0, this.z * gridSpaceOffset + 5) + Vector3.zero, 
                Quaternion.Euler(new Vector3(0, 0, 0)));
            }
        }

        // Should only clear temporary links
        public void clearLink()
        {
            this.up = null;
            this.left = null;
            this.right = null;
            this.down = null;
        }


        // fixed* should only be modified by this function
        public void fixLinks()
        {
            this.fixedUp = this.up;
            this.fixedLeft = this.left;
            this.fixedRight = this.right;
            this.fixedDown = this.down;
        }

        public void SpawnProjectile(GameObject projectile, float gridSpaceOffset)
        {
            this.projectTile = GameObject.Instantiate(projectile, 
                new Vector3(this.x * gridSpaceOffset, 3, this.z * gridSpaceOffset) + Vector3.zero, 
                Quaternion.Euler(new Vector3(0, 0, 0)));
        }

        public bool isMazeCell()
        {
            if(this.z > 5 && this.z <= 10)
            {
                return true;
            }
            return false;
            
        }
}

