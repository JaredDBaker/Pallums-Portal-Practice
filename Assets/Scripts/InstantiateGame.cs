using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateGame : MonoBehaviour
{
    // Holds all the prefab gameobjects
    public GameObject verticalPrefab;
    public GameObject horizontalPrefab;
    public GameObject playerPrefab;

    // Lists of all the gameobjects
    public List<GameObject> vertical = new List<GameObject>();
    public List<GameObject> horizontal = new List<GameObject>();
    public List<GameObject> player = new List<GameObject>();

    // List for all the grid positions that tell whether or not it is being used
    public List<List<bool>> grid = new List<List<bool>>();

    // Lists to check if a tile is blocking the path to the exit so it cant be moved
    public List<GameObject> dontTouchVer = new List<GameObject>();
    public List<GameObject> dontTouchHor = new List<GameObject>();

    // Creates the empty grid and creates a level by calling CreateLevel()
    void Start()
    {

        for (int i = 0; i < 5; i++)
        {
            grid.Add(new List<bool>());
            for (int j = 0; j < 5; j++)
            {
                grid[i].Add(false);
            }
        }

        CreateLevel();
        ScoreManager.StartTimer();
    }

    // Checks if the player is on the grid, checks if the player reached the goal, and checks if the player reset the level by pressing the 'R' key
    void Update()
    {
        if (player.Count == 0)
        {
            player.Add(Instantiate(playerPrefab, new Vector3(-1, 2, 0), Quaternion.identity));
        }

        if (player[0].transform.position.x == 5)
        {
            Reset();
            CreateLevel();
            ScoreManager.StartTimer();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
            CreateLevel();
        }
    }

    // Creates a randomly generated level
    void CreateLevel()
    {
        bool goodPlacement = false;

        player.Add(Instantiate(playerPrefab, new Vector3(-1, 2, 0), Quaternion.identity));

        // Creates the tiles and loops through until they are laid out in a way to make a good level
        int stop = 0;
        while (goodPlacement == false)
        {
            // Creates 8 tiles (will stop if it cant find a space on the grid after a certain amount of loops)
            for (int i = 0; i < 8; i++)
            {
                CreateTile();
            }

            // Checks if there is at least one horizontal block at the top and bottom of the grid
            bool top = false;
            bool bottom = false;
            for (int i = 0; i < horizontal.Count; i++)
            {
                if ((int)horizontal[i].transform.position.y == 0)
                {
                    bottom = true;
                }
                if ((int)horizontal[i].transform.position.y == 4)
                {
                    top = true;
                }
            }

            // A check to see if there are too many vertical blocks next to each other
            int amountTop = 0;
            int amountBottom = 0;
            for (int i = 0; i < vertical.Count; i++)
            {
                if ((int)vertical[i].transform.position.y == 1)
                {
                    amountBottom++;
                }
                if ((int)vertical[i].transform.position.y == 4)
                {
                    amountTop++;
                }
            }

            // Resets the level if there are too many horizontal or vertical pieces, if there isn't a horizontal piece on the top and bottom, 
            // or if there are too many vertical tiles next to each other
            if (horizontal.Count > 6 || vertical.Count > 6)
            {
                Reset();
            }
            else if (!bottom || !top)
            {
                Reset();
            }
            else if (amountTop > 3 || amountBottom > 3)
            {
                Reset();
            }
            else
            {
                goodPlacement = true;
            }

            // Stops the loop after a certain amount of times so there isn't an infinte loop
            stop++;
            if (stop > 1000)
            {
                break;
            }
        }

        // Move all of the created tiles every time it loops and stops either when there are enough well placed tiles or when it hits the break point
        goodPlacement = false;
        stop = 0;
        while (goodPlacement == false)
        {
            for (int i = 0; i < vertical.Count; i++)
            {
                MoveTile(vertical[i], 2, i);
            }
            for (int i = 0; i < horizontal.Count; i++)
            {
                MoveTile(horizontal[i], 1, i);
            }

            if (dontTouchVer.Count + dontTouchHor.Count == 5)
            {
                goodPlacement = true;
            }

            // Stops the loop after a certain amount of times so there isn't an infinte loop
            stop++;
            if (stop > 10000)
            {
                Debug.Log("it broke at MoveTile!");
                break;
            }
        }
    }

    // randomly picks a vertical or horizontal tile and randomly places it in an open space
    void CreateTile()
    {
        int ranNum = Random.Range(1, 3);
        bool placed = false;

        // 1 = horizontal tile
        if (ranNum == 1)
        {
            int stop = 0;
            // loops through until it places the tile
            while (placed == false)
            {
                int ranCol = Random.Range(0, 5);
                int ranRow = Random.Range(0, 5);

                // so it doesn't block the path to the exit or clip through the wall
                if (ranRow != 4 && ranCol != 2)
                {
                    // checks if the two grid spaces it will take up are empty
                    if (grid[ranRow][ranCol] == false && grid[ranRow + 1][ranCol] == false)
                    {
                        CreateHorizontal(ranRow, ranCol);
                        grid[ranRow][ranCol] = true;
                        grid[ranRow + 1][ranCol] = true;
                        placed = true;
                    }
                }
                // Stops the loop after a certain amount of times so there isn't an infinte loop
                stop++;
                if (stop > 100)
                {
                    break;
                }
            }
        }
        // 2 = vertical tile
        else if (ranNum == 2)
        {
            int stop = 0;
            // loops through until it places the tile
            while (placed == false)
            {
                int ranCol = Random.Range(0, 5);
                int ranRow = Random.Range(0, 5);

                // so it doesn't block the path to the exit or clip through the wall
                if (ranCol != 0 && ranCol != 2 && ranCol != 3)
                {
                    // checks if the two grid spaces it will take up are empty
                    if (grid[ranRow][ranCol] == false && grid[ranRow][ranCol - 1] == false)
                    {
                        CreateVertical(ranRow, ranCol);
                        grid[ranRow][ranCol] = true;
                        grid[ranRow][ranCol - 1] = true;
                        placed = true;
                    }
                }
                // Stops the loop after a certain amount of times so there isn't an infinte loop
                stop++;
                if (stop > 100)
                {
                    break;
                }
            }
        }
    }

    // Moves the given tile piece wherever there is an open space
    void MoveTile(GameObject tilePiece, int type, int index)
    {
        // If the vertical piece is already in a good spot, then it stops the method
        for (int i = 0; i < dontTouchVer.Count; i++)
        {
            if (tilePiece == dontTouchVer[i])
            {
                return;
            }
        }

        // If the horizontal piece is already in a good spot, then it stops the method
        for (int i = 0; i < dontTouchHor.Count; i++)
        {
            if (tilePiece == dontTouchHor[i])
            {
                return;
            }
        }

        // 1 = horizontal tile
        if (type == 1)
        {
            int ranDirection = Random.Range(0, 3);

            // creates variables for x and y coordinates of the tile 
            int xPos = (int)tilePiece.transform.position.x;
            int yPos = (int)tilePiece.transform.position.y;

            // checks to see if it can go left
            if (ranDirection == 1)
            {
                // so it doesn't check outside of grid
                if (xPos > 0)
                {
                    // checks if the grid position to the left is empty
                    if (grid[xPos - 1][yPos] == false)
                    {
                        // changes the tile coordinates and fixes the grid
                        DeleteHorizontal(horizontal[index]);
                        CreateHorizontal(xPos - 1, yPos);
                        grid[xPos - 1][yPos] = true;
                        grid[xPos + 1][yPos] = false;

                        // Checks if the horizontal piece is blocking a vertical piece from either the top of the bottom of it
                        bool dontMove = false;
                        for (int i = 0; i < dontTouchVer.Count; i++)
                        {
                            // if the vertical tile is blocking the goal, check if the horizontal piece is blocking its movement
                            if ((int)dontTouchVer[i].transform.position.y == 2)
                            {
                                if (((int)horizontal[index].transform.position.x == (int)dontTouchVer[i].transform.position.x ||
                                    (int)horizontal[index].transform.position.x + 1 == (int)dontTouchVer[i].transform.position.x) &&
                                    ((int)horizontal[index].transform.position.y == 3 || (int)horizontal[index].transform.position.y == 0))
                                {
                                    dontMove = true;
                                }
                            }
                            else if ((int)dontTouchVer[i].transform.position.y == 3)
                            {
                                if (((int)horizontal[index].transform.position.x == (int)dontTouchVer[i].transform.position.x ||
                                    (int)horizontal[index].transform.position.x + 1 == (int)dontTouchVer[i].transform.position.x) &&
                                    ((int)horizontal[index].transform.position.y == 1 || (int)horizontal[index].transform.position.y == 4))
                                {
                                    dontMove = true;
                                }
                            }
                        }
                        // If the piece blocks a vertical tile, then it is added the dontTouch list and doesn't move anymore
                        if (dontMove)
                        {
                            dontTouchHor.Add(horizontal[index]);
                        }
                    }
                }
                else
                {
                    // checks if the grid position to the right is empty
                    if (grid[xPos + 1][yPos] == false)
                    {
                        DeleteHorizontal(horizontal[index]);
                        CreateHorizontal(xPos + 1, yPos);
                        grid[xPos + 1][yPos] = true;
                        grid[xPos - 1][yPos] = false;

                        // Checks if the horizontal piece is blocking a vertical piece from either the top of the bottom of it
                        bool dontMove = false;
                        for (int i = 0; i < dontTouchVer.Count; i++)
                        {
                            // if the vertical tile is blocking the goal, check if the horizontal piece is blocking its movement
                            if ((int)dontTouchVer[i].transform.position.y == 2)
                            {
                                if (((int)horizontal[index].transform.position.x == (int)dontTouchVer[i].transform.position.x ||
                                    (int)horizontal[index].transform.position.x + 1 == (int)dontTouchVer[i].transform.position.x) &&
                                    ((int)horizontal[index].transform.position.y == 3 || (int)horizontal[index].transform.position.y == 0))
                                {
                                    dontMove = true;
                                }
                            }
                            else if ((int)dontTouchVer[i].transform.position.y == 3)
                            {
                                if (((int)horizontal[index].transform.position.x == (int)dontTouchVer[i].transform.position.x ||
                                    (int)horizontal[index].transform.position.x + 1 == (int)dontTouchVer[i].transform.position.x) &&
                                    ((int)horizontal[index].transform.position.y == 1 || (int)horizontal[index].transform.position.y == 4))
                                {
                                    dontMove = true;
                                }
                            }
                        }
                        // If the piece blocks a vertical tile, then it is added the dontTouch list and doesn't move anymore
                        if (dontMove)
                        {
                            dontTouchHor.Add(horizontal[index]);
                        }
                    }
                }
            }
            // checks to see if it can go right
            else if (ranDirection == 2)
            {
                if (xPos < 3)
                {
                    // checks if the grid position to the right is empty
                    if (grid[xPos + 1][yPos] == false)
                    {
                        DeleteHorizontal(horizontal[index]);
                        CreateHorizontal(xPos + 1, yPos);
                        grid[xPos + 1][yPos] = true;
                        grid[xPos - 1][yPos] = false;

                        // Checks if the horizontal piece is blocking a vertical piece from either the top of the bottom of it
                        bool dontMove = false;
                        for (int i = 0; i < dontTouchVer.Count; i++)
                        {
                            // if the vertical tile is blocking the goal, check if the horizontal piece is blocking its movement
                            if ((int)dontTouchVer[i].transform.position.y == 2)
                            {
                                if (((int)horizontal[index].transform.position.x == (int)dontTouchVer[i].transform.position.x ||
                                    (int)horizontal[index].transform.position.x + 1 == (int)dontTouchVer[i].transform.position.x) &&
                                    ((int)horizontal[index].transform.position.y == 3 || (int)horizontal[index].transform.position.y == 0))
                                {
                                    dontMove = true;
                                }
                            }
                            else if ((int)dontTouchVer[i].transform.position.y == 3)
                            {
                                if (((int)horizontal[index].transform.position.x == (int)dontTouchVer[i].transform.position.x ||
                                    (int)horizontal[index].transform.position.x + 1 == (int)dontTouchVer[i].transform.position.x) &&
                                    ((int)horizontal[index].transform.position.y == 1 || (int)horizontal[index].transform.position.y == 4))
                                {
                                    dontMove = true;
                                }
                            }
                        }
                        // If the piece blocks a vertical tile, then it is added the dontTouch list and doesn't move anymore
                        if (dontMove)
                        {
                            dontTouchHor.Add(horizontal[index]);
                        }
                    }
                }
                else
                {
                    // checks if the grid position to the left is empty
                    if (grid[xPos - 1][yPos] == false)
                    {
                        // changes the tile coordinates and fixes the grid
                        DeleteHorizontal(horizontal[index]);
                        CreateHorizontal(xPos - 1, yPos);
                        grid[xPos - 1][yPos] = true;
                        grid[xPos + 1][yPos] = false;

                        // Checks if the horizontal piece is blocking a vertical piece from either the top of the bottom of it
                        bool dontMove = false;
                        for (int i = 0; i < dontTouchVer.Count; i++)
                        {
                            // if the vertical tile is blocking the goal, check if the horizontal piece is blocking its movement
                            if ((int)dontTouchVer[i].transform.position.y == 2)
                            {
                                if (((int)horizontal[index].transform.position.x == (int)dontTouchVer[i].transform.position.x ||
                                    (int)horizontal[index].transform.position.x + 1 == (int)dontTouchVer[i].transform.position.x) &&
                                    ((int)horizontal[index].transform.position.y == 3 || (int)horizontal[index].transform.position.y == 0))
                                {
                                    dontMove = true;
                                }
                            }
                            else if ((int)dontTouchVer[i].transform.position.y == 3)
                            {
                                if (((int)horizontal[index].transform.position.x == (int)dontTouchVer[i].transform.position.x ||
                                    (int)horizontal[index].transform.position.x + 1 == (int)dontTouchVer[i].transform.position.x) &&
                                    ((int)horizontal[index].transform.position.y == 1 || (int)horizontal[index].transform.position.y == 4))
                                {
                                    dontMove = true;
                                }
                            }
                        }
                        // If the piece blocks a vertical tile, then it is added the dontTouch list and doesn't move anymore
                        if (dontMove)
                        {
                            dontTouchHor.Add(horizontal[index]);
                        }
                    }
                }
            }
        }
        // 2 = vertical tile
        else if (type == 2)
        {
            // creates variables for x and y coordinates of the tile 
            int xPos = (int)tilePiece.transform.position.x;
            int yPos = (int)tilePiece.transform.position.y;

            // checks to see if it can go up
            if (yPos == 1)
            {
                // checks if the grid position above is empty
                if (grid[xPos][yPos + 1] == false)
                {
                    DeleteVertical(vertical[index]);
                    CreateVertical(xPos, yPos + 1);
                    grid[xPos][yPos + 1] = true;
                    grid[xPos][yPos - 1] = false;
                }
            }
            // checks if it can go down
            else if (yPos == 4)
            {
                // checks if the grid position below is empty
                if (grid[xPos][yPos - 1] == false)
                {
                    DeleteVertical(vertical[index]);
                    CreateVertical(xPos, yPos - 1);
                    grid[xPos][yPos - 1] = true;
                    grid[xPos][yPos + 1] = false;
                }
            }

            // If the vertical tile is blocking the path to the goal, then it is added to the dontTouch list and doesn't move anymore 
            if ((int)vertical[index].transform.position.y == 2 || (int)vertical[index].transform.position.y == 3)
            {
                dontTouchVer.Add(vertical[index]);
            }
        }
    }

    // Resets all of the lists
    void Reset()
    {
        while (horizontal.Count > 0)
        {
            DeleteHorizontal(horizontal[0]);
        }

        while (vertical.Count > 0)
        {
            DeleteVertical(vertical[0]);
        }

        GameObject holder;
        while (player.Count > 0)
        {
            holder = player[0];
            player.Remove(holder);
            Destroy(holder);
        }

        while (dontTouchVer.Count > 0)
        {
            dontTouchVer.RemoveAt(0);
        }

        while (dontTouchHor.Count > 0)
        {
            dontTouchHor.RemoveAt(0);
        }

        for (int i = 0; i < 5; i++)
        {
            grid.Add(new List<bool>());
            for (int j = 0; j < 5; j++)
            {
                grid[i][j] = false;
            }
        }
    }

    // Deletes a horizontal piece from the list and the scene
    void DeleteHorizontal(GameObject holder)
    {
        horizontal.Remove(holder);
        Destroy(holder);
    }

    // Deletes a vertical piece from the list and the scene
    void DeleteVertical(GameObject holder)
    {
        vertical.Remove(holder);
        Destroy(holder);
    }

    // Creates a horizontal piece on the scene and adds it to the horizontal list
    void CreateHorizontal(int row, int col)
    {
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(row, col, 0), Quaternion.identity));
    }

    // Creates a vertical piece on the scene and adds it to the vertical list
    void CreateVertical(int row, int col)
    {
        vertical.Add(Instantiate(verticalPrefab, new Vector3(row, col, 0), Quaternion.identity));
    }
}
