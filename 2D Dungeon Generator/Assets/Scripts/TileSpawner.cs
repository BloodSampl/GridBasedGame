using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    private DungounManager dungoun;

    void Awake()
    {
        dungoun = FindObjectOfType<DungounManager>();
        GameObject goFloor = Instantiate(dungoun.floorPrefab, transform.position, Quaternion.identity) as GameObject;
        goFloor.name = dungoun.floorPrefab.name;
        goFloor.transform.parent = dungoun.transform;
        if (transform.position.x > dungoun.maxX)
        {
            dungoun.maxX = transform.position.x;
        }

        if (transform.position.x < dungoun.minX)
        {
            dungoun.minX = transform.position.x;
        }

        if (transform.position.y > dungoun.maxY)
        {
            dungoun.maxY = transform.position.y;
        }

        if (transform.position.y < dungoun.minY)
        {
            dungoun.minY = transform.position.y;
        }
    }
    private void Start()
    {
        LayerMask envMask = LayerMask.GetMask("Wall", "Floor");
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2 targetPos = new Vector2(transform.position.x + x, transform.position.y + y);
                Collider2D hit = Physics2D.OverlapBox(targetPos, Vector2.one *.8f,0, envMask);
                if (!hit)
                {
                    GameObject wall = Instantiate(dungoun.wallPrefab, targetPos, Quaternion.identity) as GameObject;
                    wall.name = dungoun.wallPrefab.name;
                    wall.transform.parent = dungoun.transform;
                }
            }
        }
        Destroy(this.gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
