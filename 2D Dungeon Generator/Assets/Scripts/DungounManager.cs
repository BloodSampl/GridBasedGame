using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;



public enum DungounType
{
    Caverns,
    Rooms
}
public class DungounManager : MonoBehaviour
{
    public GameObject[] randomItems;
    public GameObject[] randomEnemies;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject TilePrefab;
    public GameObject ExitPrefab;
    public DungounType dungounType;
    [Range(50,5000)] public int totalFloorCount;
    [Range(0,100)] public int itemSpawnPercentage;
    [Range(0,100)] public int enemySpawnPercentage;
    [HideInInspector] public float minX, maxX, minY, maxY;
    
    List<Vector3> floorList = new List<Vector3>();
    LayerMask floorMask;
    LayerMask wallMask;

    void Start()
    {
        floorMask = LayerMask.GetMask("Floor");
        wallMask = LayerMask.GetMask("Wall");
        switch (dungounType)
        {
            case DungounType.Caverns:
                RandomWalker();
                break;
            case DungounType.Rooms:
                RoomWalker();
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void RandomWalker()
    {
        Vector3 curPos = Vector3.zero; 
        floorList.Add(curPos);
        while (floorList.Count < totalFloorCount)
        {
            curPos += RandomDirection();
            if (!inFloorList(curPos))
            {
                floorList.Add(curPos);
            }
        }
        StartCoroutine(DelayProgress());
    }

    void RoomWalker()
    {
        Vector3 curPos = Vector3.zero; 
        floorList.Add(curPos);
        while (floorList.Count < totalFloorCount)
        {
            Vector3 walkDir = RandomDirection();
            int walkLength = Random.Range(9, 18);
            for (int i = 0; i < walkLength; i++)
            {
                if (!inFloorList(curPos + walkDir))
                {
                    floorList.Add(curPos + walkDir);
                }
                curPos += walkDir;
            }
            // create room at end of long walk
            int width = Random.Range(1, 5);
            int height = Random.Range(1, 5);
            for (int w = -width; w <= width; w++)
            {
                for (int h = -height; h <= height; h++)
                {
                    Vector3 offset = new Vector3(w, h, 0);
                    if (!inFloorList(curPos + offset))
                    {
                        floorList.Add(curPos + offset);
                    }
                }
            }
        }
        StartCoroutine(DelayProgress());
    }

    bool inFloorList(Vector3 pos)
    {
        for (int i = 0; i < floorList.Count; i++)
        {
            if (Vector3.Equals(pos, floorList[i]))
            {
                return true;
            }
        }
        return false;
    }

    Vector3 RandomDirection()
    {
        switch (Random.Range(1, 5))
        {
            case 1:
                return Vector3.up;
            case 2:
                return Vector3.right;
            case 3:
                return Vector3.down;
            case 4:
                return Vector3.left;
        }
        return Vector3.zero;
    }

    IEnumerator DelayProgress()
    {
        for (int i = 0; i < floorList.Count; i++)
        {
            GameObject goTile = Instantiate(TilePrefab, floorList[i], Quaternion.identity);
            goTile.name = TilePrefab.name;
            goTile.transform.SetParent(transform);
        }
        while (FindObjectsOfType<TileSpawner>().Length > 0)
        {
            yield return null;
        }
        ExitDoorway();
        Vector2 hitSize = Vector2.one * .8f;
        for (int x = (int)minX - 2; x <= (int)maxX + 2; x++)
        {
            for (int y = (int)minY - 2; y <= (int)maxY + 2; y++)
            {
                Collider2D hitFloor = Physics2D.OverlapBox(new Vector2(x, y), hitSize,0,floorMask);
                if (hitFloor)
                {
                    if (!Vector2.Equals(hitFloor.transform.position, floorList[floorList.Count - 1]))
                    {
                        Collider2D hitTop = Physics2D.OverlapBox(new Vector2(x, y + 1),hitSize,0,wallMask);
                        Collider2D hitRight = Physics2D.OverlapBox(new Vector2(x + 1, y), hitSize,0,wallMask);
                        Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(x, y - 1), hitSize,0,wallMask);
                        Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(x - 1, y), hitSize,0,wallMask);
                        RandomItems(hitFloor, hitTop, hitRight, hitBottom, hitLeft);
                        RandomEnemies(hitFloor, hitTop, hitRight, hitBottom, hitLeft);
                    }
                }
            }
        }
    }

    void RandomEnemies(Collider2D hitFloor, Collider2D hitTop, Collider2D hitRight, Collider2D hitBottom, Collider2D hitLeft)
    {
        if (!hitTop && !hitRight && !hitBottom && !hitLeft)
        {
            int roll = Random.Range(1, 101);
            if (roll <= enemySpawnPercentage)
            {
                int enemyIndex = Random.Range(0, randomEnemies.Length);
                GameObject goEnemy = Instantiate(randomEnemies[enemyIndex], hitFloor.transform.position, Quaternion.identity);
                goEnemy.name = randomEnemies[enemyIndex].name;
                goEnemy.transform.SetParent(hitFloor.transform);
            }
        }
    }
    void RandomItems(Collider2D hitFloor, Collider2D hitTop, Collider2D hitRight, Collider2D hitBottom, Collider2D hitLeft)
    {
        if ((hitTop || hitBottom || hitLeft || hitRight) && !(hitTop && hitBottom) &&
            !(hitLeft && hitRight))
        {
            int roll = Random.Range(1, 101);
            if (roll <= itemSpawnPercentage)
            {
                int itemIndex = Random.Range(0, randomItems.Length);
                GameObject goItem = Instantiate(randomItems[itemIndex], hitFloor.transform.position, Quaternion.identity);
                goItem.name = randomItems[itemIndex].name;
                goItem.transform.SetParent(hitFloor.transform);
            }
        }
    }
    void ExitDoorway()
    {
        Vector3 DoorPos = floorList[floorList.Count - 1];
        GameObject goExit = Instantiate(ExitPrefab, DoorPos, Quaternion.identity);
        goExit.name = ExitPrefab.name;
        goExit.transform.SetParent(transform);
    }
}
