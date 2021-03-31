using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private TileScript[,] allTiles;

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new TileScript[width, height];

        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp()
    {
        for (int i =0; i < width; i++)
        {
            for(int j = 0; j < height; j ++)
            {
                Vector3 tempPos = new Vector3(i, j, 0);
                GameObject testTile =  Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;

                testTile.transform.parent = this.transform;
                testTile.name = "( " + i + "," + j + " )";
            }
        }
    }
}
