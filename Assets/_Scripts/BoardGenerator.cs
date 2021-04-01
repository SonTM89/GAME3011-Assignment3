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

                int maxLoopTime = 0;
                while (MatchesAt(i, j, gems[currentGem]) && maxLoopTime < 100)
                {
                    currentGem = Random.Range(0, gems.Length);
                    maxLoopTime++;
                }
                maxLoopTime = 0;
                
                GameObject tempGem = Instantiate(gems[currentGem], tempPos, Quaternion.identity);
                tempGem.transform.parent = this.transform;
                tempGem.name = "( " + i + "," + j + " )";
                board[i, j] = tempGem;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject obj)
    {
        if(column > 1 && row > 1)
        {
            if(board[column - 1, row].tag == obj.tag && board[column - 2, row].tag == obj.tag)
            {
                return true;
            }
            if (board[column, row - 1].tag == obj.tag && board[column, row - 2].tag == obj.tag)
            {
                return true;
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if (board[column, row - 1].tag == obj.tag && board[column, row - 2].tag == obj.tag)
                {
                    return true;
                }
            }           
            else if(column > 1)
            {
                if (board[column - 1, row].tag == obj.tag && board[column - 2, row].tag == obj.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(board[column,row].GetComponent<GemBehaviour>().isMatched)
        {
            Destroy(board[column, row]);
            board[column, row] = null;
        }
    }

    public void DestroyAllMatchesGem()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(board[i,j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }

        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(board[i,j] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    board[i,j].GetComponent<GemBehaviour>().row -= nullCount;
                    board[i,j] = null; 
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(FillBoard());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(board[i,j] == null)
                {
                    Vector3 temPos = new Vector3(i, j, 0);
                    int currentGem = Random.Range(0, gems.Length);
                    GameObject temp = Instantiate(gems[currentGem], temPos, Quaternion.identity);
                    board[i, j] = temp;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(board[i,j] != null)
                {
                    if(board[i,j].GetComponent<GemBehaviour>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);
        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyAllMatchesGem();
        }
    }
}
