using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    public float bounceHeight = 0.1f;
    public float bounceSpeed = 1;
    public float rotateSpeed = 1;
    public Vector3 bounceDir = new Vector3(0, 1, 0);
    public Vector3 rotateDir = new Vector3(0, 1, 0);
    private float time;
    private Vector3 initialLocal;
    // Start is called before the first frame update
    void Start()
    {
        initialLocal = transform.localPosition;
        time = 0;
        int flippedY = Random.Range(0, 1) == 0 ? -1 : 1;
        transform.localScale = new Vector3 (Random.Range(0, 2) == 0 ? -1 : 1, Random.Range(0, 2) == 0 ? -1 : 1, Random.Range(0, 2) == 0 ? -1 : 1);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        float bounceTime = (time * bounceSpeed) % (Mathf.PI * 2);
        float rotateTime = (time * rotateSpeed) % (Mathf.PI * 2);
        Vector3 bounceVector = bounceDir * Mathf.Sin(bounceTime) * bounceHeight;
        Vector3 rotateEuler = rotateDir * (rotateTime * 180 / Mathf.PI);

        transform.localPosition = initialLocal + bounceVector;
        transform.localRotation = Quaternion.Euler(rotateEuler);
    }
}
