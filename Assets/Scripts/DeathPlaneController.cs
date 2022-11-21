using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeathPlaneController : MonoBehaviour
{
    public Transform currentCheckPoint;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Player")
        {
            ReSpawn(other.gameObject);
        }
    }

    public void ReSpawn(GameObject go)
    {
        go.transform.position = currentCheckPoint.position;
    }
}
