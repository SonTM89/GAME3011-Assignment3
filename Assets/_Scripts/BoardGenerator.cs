using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;

    public GameObject[] gems;
    private TileScript[,] allTiles;
    public GameObject[,] board;

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new TileScript[width, height];
        board = new GameObject[width, height];

        GenerateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateBoard()
    {
        for (int i =0; i < width; i++)
        {
            for(int j = 0; j < height; j ++)
            {
                Vector3 tempPos = new Vector3(i, j, 0);
                GameObject testTile =  Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;

                testTile.transform.parent = this.transform;
                testTile.name = "( " + i + "," + j + " )";

                int currentGem = Random.Range(0, gems.Length);
                GameObject tempGem = Instantiate(gems[currentGem], tempPos, Quaternion.identity);
                tempGem.transform.parent = this.transform;
                tempGem.name = "( " + i + "," + j + " )";
                board[i, j] = tempGem;
            }
        }
    }
}
