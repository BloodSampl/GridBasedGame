using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungounManager : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject TilePrefab;
    public int totalFloorCount;
    [HideInInspector] public float minX, maxX, minY, maxY;
    
    List<Vector3> floorList = new List<Vector3>();

    void Start()
    {
        RandomWalker();
    }

    void RandomWalker()
    {
        Vector3 curPos = Vector3.zero;
        while (floorList.Count < totalFloorCount)
        {
            floorList.Add(curPos);
            switch (Random.Range(1, 5))
            {
                case 1:
                    curPos += Vector3.up;
                    break;
                case 2:
                    curPos += Vector3.right;
                    break;
                case 3:
                    curPos += Vector3.down;
                    break;
                case 4:
                    curPos += Vector3.left;
                    break;
            }

            bool inFloorList = false;
            for (int i = 0; i < floorList.Count; i++)
            {
                if (Vector3.Equals(curPos, floorList[i]))
                {
                    inFloorList = true;
                    break;
                }
            }
            if (!inFloorList)
            {
                floorList.Add(curPos);
            }
        }

        for (int i = 0; i < floorList.Count; i++)
        {
            GameObject goTile = Instantiate(TilePrefab, floorList[i], Quaternion.identity);
            goTile.name = TilePrefab.name;
            goTile.transform.SetParent(transform);
        }
    }
}
