using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TilePiece : MonoBehaviour
{
    public float gridSize = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    public bool CheckCollisions(Vector3 direction) {
        Vector3 newPos = transform.position + (gridSize * direction.normalized);
        Collider[] hits = Physics.OverlapSphere(newPos, 0);
        Collider[] siblings = transform.parent.GetComponentsInChildren<Collider>();
        foreach (Collider hit in hits) {
            bool isSibling = Array.Exists(siblings, element => element==hit);
            if (!isSibling) {
                return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    void OnMouseDown()
    {
        transform.parent.GetComponent<DraggableTile>().DetectClick();
    }
}
