using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InstantiateGameShuffle : MonoBehaviour
{
    // Holds all the prefab gameobjects
    public GameObject verticalPrefab;
    public GameObject horizontalPrefab;
    public GameObject playerPrefab;

    public int maxTiles = 15;

    // Lists of all the gameobjects
    private List<DraggableTile> tiles = new List<DraggableTile>();
    private List<Vector3> lastMoveOfTile = new List<Vector3>();
    private GameObject player;
    public Vector3 playerStart = new Vector3(-2, 2, 0);
    private List<int> unmovedTiles = new List<int>();
    private bool created;
    private string state;
    private int loops;
    private int delay;
    private int loopsSinceBlocked;

    // Creates the empty grid and creates a level by calling CreateLevel()
    void Start()
    {
        player = Instantiate(playerPrefab, playerStart, Quaternion.identity);
        player.GetComponent<DraggableTile>().allowHorizontal = false;
        created = false;
        state = "placing";
        // Reset();
        // CreateLevel();
    }

    // Checks if the player is on the grid, checks if the player reached the goal, and checks if the player reset the level by pressing the 'R' key
    void Update()
    {
        if (!created)
        {
            created = CreateLevel(200, maxTiles, 20);
            if (created) {
                ScoreManager.StartTimer();
                player.GetComponent<DraggableTile>().allowHorizontal = true;
            }
        }
        if (player != null && player.transform.position.x == 6)
        {
            ScoreManager.AddWin();
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }
    }

    // Creates a randomly generated level
    bool CreateLevel(int maxLoops, int numTiles, int acceptLoops)
    {
        Debug.Log(state);
        if (state == "resetting") {
            Reset();
            state = "placing";
            return false;
        }
        if (state == "placing") {
            int stop = 0;
            int placed = 0;
            while (placed < numTiles && stop < maxLoops)
            {
                bool wasPlaced = PlaceRandomTile();
                if (wasPlaced) {
                    placed++;
                }
                stop++;
            }
            Debug.Log("placed: " + placed);
            unmovedTiles = new List<int>();
            loops = -1;
            state = "moving";
            return false;
        }
        else if (state == "moving") {
            if (unmovedTiles.Count == 0) {
                loops++;
                if (!ExitClear()) {
                    loopsSinceBlocked++;
                }
                else {
                    loopsSinceBlocked = 0;
                }
                if (loops % 3 == 0) {
                    lastMoveOfTile = new List<Vector3>();
                }
                for (int i = 0; i < tiles.Count; i++)
                {
                    unmovedTiles.Add(i);
                    if (loops % 3 == 0) {
                        lastMoveOfTile.Add(Vector3.zero);
                    }
                }
            }
            int index = Random.Range(0, unmovedTiles.Count);
            bool moved = MoveRandomTile(tiles[unmovedTiles[index]]);
            unmovedTiles.RemoveAt(index);
            if (loops > maxLoops) {
                state = "placing";
            } 
            return loopsSinceBlocked > acceptLoops && !ExitClear();
        }
        return false;
    }

    // randomly picks a vertical or horizontal tile and randomly places it in an open space that doesn't block the exit
    bool PlaceRandomTile()
    {
        bool placed = false;
        GameObject prefab = Random.Range(0, 2) == 0 ? horizontalPrefab : verticalPrefab;

        int stop = 0;
        while (placed == false)
        {
            int ranCol = Random.Range(-1, 6);
            int ranRow = Random.Range(-1, 6);
            placed = CreateTile(ranRow, ranCol, prefab);
            // Stops the loop after a certain amount of times so there isn't an infinte loop
            stop++;
            if (stop > 100)
            {
                break;
            }
        }
        return placed;
    }

    // Moves 1 space in a random direction.
    bool MoveRandomTile(DraggableTile tile)
    {
        List<Vector3> directions = new List<Vector3>();
        directions.Add(new Vector3(1, 0, 0));
        directions.Add(new Vector3(-1, 0, 0));
        directions.Add(new Vector3(0, 1, 0));
        directions.Add(new Vector3(0, -1, 0));
        Vector3 last = lastMoveOfTile[tiles.IndexOf(tile)];
        directions.Remove(last * -1);
        int i = Random.Range(0, directions.Count);
        bool moved = tile.Move(directions[i], false);
        if (moved) {
            lastMoveOfTile[tiles.IndexOf(tile)] = directions[i];
            return true;
        }
        return false;
    }

    // Resets all of the lists
    void Reset()
    {
        foreach (DraggableTile tile in tiles) {
            tile.gameObject.SetActive(false);
            Destroy(tile.gameObject);
        }
        tiles = new List<DraggableTile>();
        lastMoveOfTile = new List<Vector3>();

        player.transform.position = playerStart;
    }

    // Creates a piece on the scene and adds it to the list
    bool CreateTile(int row, int col, GameObject prefab)
    {
        DraggableTile tile = Instantiate(prefab, new Vector3(row, col, 0), Quaternion.identity).GetComponent<DraggableTile>();
        if (tile.CheckCollision(Vector3.zero) || !ExitClear()) {
            tile.gameObject.SetActive(false);
            Destroy(tile.gameObject);
            return false;
        }
        tiles.Add(tile);
        lastMoveOfTile.Add(Vector3.zero);
        return true;
    }

    bool ExitClear()
    {
        float y = 1.5f;
        float xStart = -0.5f;
        float xEnd = 5.5f;

        List<Vector3> visited = new List<Vector3>();
        List<Vector3> toVisit = new List<Vector3>();
        toVisit.Add(new Vector3(xStart, y, 0));


        while (toVisit.Count > 0) {
            Vector3 space = toVisit[toVisit.Count - 1];
            toVisit.RemoveAt(toVisit.Count - 1);
            visited.Add(space);
            Debug.Log(visited.Count);
            Collider[] hits = Physics.OverlapSphere(space, 0);
            if (hits.Length == 0) {
                if (space == new Vector3(xEnd, y, 0)) {
                    return true;
                }
                List<Vector3> toAdd = new List<Vector3>();
                toAdd.Add(new Vector3(space.x + 1, space.y, 0));
                toAdd.Add(new Vector3(space.x - 1, space.y, 0));
                //toAdd.Add(new Vector3(space.x, space.y + 1, 0));
                //toAdd.Add(new Vector3(space.x, space.y - 1, 0));
                foreach (Vector3 v in toAdd) {
                    if (!visited.Contains(v) && !toVisit.Contains(v)) {
                        toVisit.Add(v);
                    }
                }
                
            }
        }
        return false;
    }
}
