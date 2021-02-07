using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InstantiateTutorial : MonoBehaviour
{
    public GameObject verticalPrefab;
    public GameObject horizontalPrefab;
    public GameObject playerPrefab;

    public List<GameObject> vertical = new List<GameObject>();
    public List<GameObject> horizontal = new List<GameObject>();
    public List<GameObject> player = new List<GameObject>();

    int level = 0;
    bool makeLevel = false;
    // Start is called before the first frame update
    void Start()
    {
        // for finding another script if needed
        GameObject manager = GameObject.Find("ScriptManager");
        Level1();
    }

    // Update is called once per frame
    void Update()
    {
        if (player[0].transform.position.x > 1)
        {
            makeLevel = true;
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            makeLevel = true;
            level--;
        }

        switch (level)
        {
            case 0:
                if (makeLevel)
                {
                    Level1();
                }
                break;
            case 1:
                if (makeLevel)
                {
                    Level2();
                }
                break;
            case 2:
                if (makeLevel)
                {
                    Level3();
                }
                break;
            case 3:
                if (makeLevel)
                {
                    Level4();
                }
                break;
            case 4:
                if (makeLevel)
                {
                    Level5();
                }
                break;
            case 5:
                if (makeLevel)
                {
                    SceneManager.LoadScene("Menu");
                }
                break;
        }
    }

    void Level1()
    {
        RestartLevel();

        vertical.Add(Instantiate(verticalPrefab, new Vector3(1, 2, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(2, 2, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(1, 0, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(2, 0, 0), Quaternion.identity));
        
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(1, -2, 0), Quaternion.identity));

        player.Add(Instantiate(playerPrefab, new Vector3(-3, 0, 0), Quaternion.identity));
    }

    void Level2()
    {
        RestartLevel();

        vertical.Add(Instantiate(verticalPrefab, new Vector3(1, 0, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(0, -1, 0), Quaternion.identity));
        
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(-1, 1, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(1, 1, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(1, -2, 0), Quaternion.identity));

        player.Add(Instantiate(playerPrefab, new Vector3(-3, 0, 0), Quaternion.identity));
    }

    void Level3()
    {
        RestartLevel();

        vertical.Add(Instantiate(verticalPrefab, new Vector3(0, 2, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(1, 1, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(2, 1, 0), Quaternion.identity));

        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(1, 2, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(1, -1, 0), Quaternion.identity));

        player.Add(Instantiate(playerPrefab, new Vector3(-3, 0, 0), Quaternion.identity));
    }

    void Level4()
    {
        RestartLevel();

        vertical.Add(Instantiate(verticalPrefab, new Vector3(2, 2, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(1, 1, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(0, -1, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(-2, -1, 0), Quaternion.identity));

        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(-2, 2, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(0, 2, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(-1, 1, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(1, -1, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(1, -2, 0), Quaternion.identity));

        player.Add(Instantiate(playerPrefab, new Vector3(-3, 0, 0), Quaternion.identity));
    }

    void Level5()
    {
        RestartLevel();

        vertical.Add(Instantiate(verticalPrefab, new Vector3(2, 2, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(1, 1, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        vertical.Add(Instantiate(verticalPrefab, new Vector3(-2, -1, 0), Quaternion.identity));

        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(-2, 2, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(0, 2, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(-1, 1, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(1, -1, 0), Quaternion.identity));
        horizontal.Add(Instantiate(horizontalPrefab, new Vector3(0, -2, 0), Quaternion.identity));

        player.Add(Instantiate(playerPrefab, new Vector3(-3, 0, 0), Quaternion.identity));
    }

    void RestartLevel()
    {
        GameObject holder;
        while (vertical.Count > 0)
        {
            holder = vertical[0];
            vertical.Remove(holder);
            Destroy(holder);
        }
        while (horizontal.Count > 0)
        {
            holder = horizontal[0];
            horizontal.Remove(holder);
            Destroy(holder);
        }
        while (player.Count > 0)
        {
            holder = player[0];
            player.Remove(holder);
            Destroy(holder);
        }

        makeLevel = false;

        level++;
    }
}
