using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DraggableTile : MonoBehaviour
{
    private bool mouseDown;
    private Vector3 mouseStart;
    public float gridSize = 1;
    public float dragThreshold = 1f;
    public bool allowHorizontal = true;
    public bool allowVertical = true;

    private List<Vector3> gridDirections {
        get {
            List<Vector3> dirs = new List<Vector3>();
            if (allowVertical) {
                dirs.Add(new Vector3(0,1,0));
                dirs.Add(new Vector3(0,-1,0));
            }
            if (allowHorizontal) {
                dirs.Add(new Vector3(1, 0, 0));
                dirs.Add(new Vector3(-1, 0, 0));
            }
            return dirs;
        }
    }
    // Sound
    private AudioClip clipMove;
    private AudioSource moveSrc;
    private Pause pauseScript;

    // Start is called before the first frame update
    void Start()
    {
        // Set sound files up
        clipMove = Resources.Load<AudioClip>("TileMove");
        pauseScript = FindObjectOfType<Pause>();
        moveSrc = pauseScript.AddAudio(clipMove, false, false, 1.0f);

        mouseDown = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Update sound volume
        moveSrc.volume = pauseScript.fxVolume;

        if (Input.GetMouseButtonUp(0)) {
            mouseDown = false;
        }
        if (mouseDown) {
            Vector3 mouseEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mouseDir = mouseEnd - mouseStart;
            Vector3 gridDir = gridDirections[0];
            for (int i = 1; i < gridDirections.Count; i++) {
                Vector3 normalizedDir = mouseDir.normalized;
                float dot = Vector3.Dot(normalizedDir, gridDirections[i]);
                float greatestDot = Vector3.Dot(normalizedDir, gridDir);
                if (dot > greatestDot) {
                    gridDir = gridDirections[i];
                }
            }

            float sqrThreshold = Mathf.Pow((gridSize * dragThreshold), 2);
            if (Vector3.Project(mouseDir, gridDir).sqrMagnitude > sqrThreshold) {
                bool moved = Move(gridDir);
                if (moved) {
                    mouseStart = mouseEnd;
                }
            }
            
        }
    }

    public bool Move(Vector3 direction, bool sound = true) {
        if (gridDirections.Contains(direction) && !CheckCollision(direction) && !Pause.GameIsPaused) {
            transform.position = transform.position + (gridSize * direction);
            // Play Sound
            if (sound) {
                moveSrc.Play();
            }
            return true;
        }
        return false;
    }

    public bool CheckCollision(Vector3 direction) {
        TilePiece[] pieces = GetComponentsInChildren<TilePiece>();
        foreach (TilePiece p in pieces) {
            if (p.CheckCollisions(direction)) {
                return true;
            }
        }
        return false;
    }

    public void DetectClick()
    {
        mouseDown = true;
        mouseStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
