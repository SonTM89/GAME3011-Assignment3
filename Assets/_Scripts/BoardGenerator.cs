using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GameState
{
    WAIT,
    MOVE
}


public enum TileType
{
    NORMAL,
    IMMOVABLE,
    BREAKABLE
}

[System.Serializable]
public class SingleTile
{
    public int x;
    public int y;
    public TileType tiletype;
}

public class BoardGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;

    public GameState currentState;

    public GameObject[] gemTypes;

    public SingleTile[] tilePositionList;
    
    [SerializeField] public bool[,] immovableboard;
    
    public GameObject[,] board;
    private FindMatches findMatches;

    public List<GameObject> currentMatchesList = new List<GameObject>();
    public GemBehaviour currentGem;

    public float aspectRatio = 1.1f;
    public float padding = 2;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        immovableboard = new bool[width, height];
        board = new GameObject[width, height];
        SetUpCamera(width, height);

        GenerateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateImmovableTile()
    {
        for (int i = 0; i < tilePositionList.Length; i++)
        {
            if (tilePositionList[i].tiletype == TileType.IMMOVABLE)
            {
                immovableboard[tilePositionList[i].x, tilePositionList[i].y] = true;
            }
        }
    }

    public void GenerateBoard()
    {
        GenerateImmovableTile();

        for (int i =0; i < width; i++)
        {
            for(int j = 0; j < height; j ++)
            {
                if(!immovableboard[i,j])
                {
                    Vector3 tempPos = new Vector3(i, j + offSet, 0);
                    GameObject testTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;

                    testTile.transform.parent = this.transform;
                    testTile.name = "( " + i + "," + j + " )";


                    int currentGem = Random.Range(0, gemTypes.Length);

                    int maxLoopTime = 0;
                    while (MatchesAt(i, j, gemTypes[currentGem]) && maxLoopTime < 100)
                    {
                        currentGem = Random.Range(0, gemTypes.Length);
                        maxLoopTime++;
                    }
                    maxLoopTime = 0;

                    GameObject tempGem = Instantiate(gemTypes[currentGem], tempPos, Quaternion.identity);
                    tempGem.GetComponent<GemBehaviour>().row = j;
                    tempGem.GetComponent<GemBehaviour>().column = i;

                    tempGem.transform.parent = this.transform;
                    tempGem.name = "( " + i + "," + j + " )";
                    board[i, j] = tempGem;
                } 
                //else
                //{
                //    Vector3 tempPos = new Vector3(i, j + offSet, 0);
                //    GameObject testTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;

                //    testTile.transform.parent = this.transform;
                //    testTile.name = "( " + i + "," + j + " )";
                //    SpriteRenderer sprRen = testTile.GetComponent<SpriteRenderer>();
                //    sprRen.enabled = true;
                //    //testTile.transform.position = new Vector3(i, j, 0);
                //}
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject obj)
    {
        if(column > 1 && row > 1)
        {
            if(board[column - 1, row] != null && board[column - 2, row] != null)
            {
                if (board[column - 1, row].tag == obj.tag && board[column - 2, row].tag == obj.tag)
                {
                    return true;
                }
            }

            if (board[column, row - 1] != null && board[column, row - 2] != null)
            {
                if (board[column, row - 1].tag == obj.tag && board[column, row - 2].tag == obj.tag)
                {
                    return true;
                }
            } 
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if (board[column, row - 1] != null && board[column, row - 2] != null)
                {
                    if (board[column, row - 1].tag == obj.tag && board[column, row - 2].tag == obj.tag)
                    {
                        return true;
                    }
                }         
            }           
            if(column > 1)
            {
                if (board[column - 1, row] != null && board[column - 2, row] != null)
                {
                    if (board[column - 1, row].tag == obj.tag && board[column - 2, row].tag == obj.tag)
                    {
                        return true;
                    }
                }     
            }
        }

        return false;
    }


    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        GemBehaviour firstGem = currentMatchesList[0].GetComponent<GemBehaviour>();

        if(firstGem != null)
        {
            foreach (GameObject currentGem in currentMatchesList)
            {
                GemBehaviour gem = currentGem.GetComponent<GemBehaviour>();

                if(gem.row == firstGem.row)
                {
                    numberHorizontal++;
                }
                if(gem.column == firstGem.column)
                {
                    numberVertical++;
                }
            }
        }

        return (numberVertical == 5 || numberHorizontal == 5);
    }



    private void CheckToMakeSpecialGem()
    {
        if (currentMatchesList.Count == 4 || currentMatchesList.Count == 7)
        {
            CheckToSetSpecialClearRowOrColumnGem();
        }
        if (currentMatchesList.Count == 5 || currentMatchesList.Count == 8)
        {
            if(ColumnOrRow())
            {
                if(currentGem != null)
                {
                    if(currentGem.isMatched)
                    {
                        if(!currentGem.isColorClearGem)
                        {
                            currentGem.isMatched = false;
                            currentGem.CreateSpecialGem(SpecialGem.COLOR_CLEAR);
                        }
                    }
                    else
                    {
                        if(currentGem.nextGem != null)
                        {
                            GemBehaviour nextGemBehaviour = currentGem.nextGem.GetComponent<GemBehaviour>();
                            if(nextGemBehaviour.isMatched)
                            {
                                if(!nextGemBehaviour.isColorClearGem)
                                {
                                    nextGemBehaviour.isMatched = false;
                                    nextGemBehaviour.CreateSpecialGem(SpecialGem.COLOR_CLEAR);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (currentGem != null)
                {
                    if (currentGem.isMatched)
                    {
                        if (!currentGem.isSquareClearGem)
                        {
                            currentGem.isMatched = false;
                            currentGem.CreateSpecialGem(SpecialGem.SQUARE_CLEAR);
                        }
                    }
                    else
                    {
                        if (currentGem.nextGem != null)
                        {
                            GemBehaviour nextGemBehaviour = currentGem.nextGem.GetComponent<GemBehaviour>();
                            if (nextGemBehaviour.isMatched)
                            {
                                if (!nextGemBehaviour.isSquareClearGem)
                                {
                                    nextGemBehaviour.isMatched = false;
                                    nextGemBehaviour.CreateSpecialGem(SpecialGem.SQUARE_CLEAR);
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    private void DestroyMatchesAt(int column, int row)
    {
        if(board[column,row].GetComponent<GemBehaviour>().isMatched)
        {
            
            if(currentMatchesList.Count >= 4)
            {
                CheckToMakeSpecialGem();
                //CheckToSetSpecialClearRowOrColumnGem();
            }

            //currentMatches.Remove(board[column, row]);
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

        currentMatchesList.Clear();
        StartCoroutine(ReconstructBoard());
    }


    private IEnumerator ReconstructBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(!immovableboard[i,j] && board[i,j] == null)
                {
                    for(int k = j+ 1; k < height; k++)
                    {
                        if (board[i, k] != null)
                        {
                            board[i, k].GetComponent<GemBehaviour>().row = j;

                            board[i, k] = null;

                            break;
                        }
                    }
                }
            }
        }

        yield return  new WaitForSeconds(0.5f);

        StartCoroutine(FillAndCheckBoard());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(board[i,j] == null && !immovableboard[i, j])
                {
                    Vector3 temPos = new Vector3(i, j + offSet, 0);
                    int currentGem = Random.Range(0, gemTypes.Length);
                    GameObject temp = Instantiate(gemTypes[currentGem], temPos, Quaternion.identity);
                    board[i, j] = temp;
                    temp.GetComponent<GemBehaviour>().row = j;
                    temp.GetComponent<GemBehaviour>().column = i;
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

    private IEnumerator FillAndCheckBoard()
    {
        RefillBoard();

        yield return new WaitForSeconds(0.4f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.4f);

            DestroyAllMatchesGem();
        }

        currentMatchesList.Clear();
        currentGem = null;

        yield return new WaitForSeconds(0.4f);

        currentState = GameState.MOVE;
    }


    private List<GameObject> CheckMatchedGemsInColumn(GemBehaviour gem1, GemBehaviour gem2, GemBehaviour gem3)
    {
        List<GameObject> currentGems = new List<GameObject>();

        if (gem1.isColumnClearGem)
        {
            currentMatchesList.Union(MatchedGemsInColumn(gem1.column));
        }
        if (gem2.isColumnClearGem)
        {
            currentMatchesList.Union(MatchedGemsInColumn(gem2.column));
        }
        if (gem3.isColumnClearGem)
        {
            currentMatchesList.Union(MatchedGemsInColumn(gem3.column));
        }

        return currentGems;
    }

    private List<GameObject> CheckMatchedGemsInRow(GemBehaviour gem1, GemBehaviour gem2, GemBehaviour gem3)
    {
        List<GameObject> currentGems = new List<GameObject>();

        if (gem1.isRowClearGem)
        {
            currentMatchesList.Union(MatchedGemsInRow(gem1.row));
        }

        if (gem2.isRowClearGem)
        {
            currentMatchesList.Union(MatchedGemsInRow(gem2.row));
        }

        if (gem3.isRowClearGem)
        {
            currentMatchesList.Union(MatchedGemsInRow(gem3.row));
        }

        return currentGems;
    }


    private List<GameObject> CheckMatchedGemsInSquare(GemBehaviour gem1, GemBehaviour gem2, GemBehaviour gem3)
    {
        List<GameObject> currentGems = new List<GameObject>();

        if (gem1.isSquareClearGem)
        {
            currentMatchesList.Union(MatchedGemsInSquare(gem1.column, gem1.row));
        }

        if (gem2.isSquareClearGem)
        {
            currentMatchesList.Union(MatchedGemsInSquare(gem2.column, gem2.row));
        }

        if (gem3.isSquareClearGem)
        {
            currentMatchesList.Union(MatchedGemsInSquare(gem3.column, gem3.row));
        }

        return currentGems;
    }


        private void  AddAndMatchGem(GameObject gem)
    {
        if (!currentMatchesList.Contains(gem))
        {
            currentMatchesList.Add(gem);
        }
        gem.GetComponent<GemBehaviour>().isMatched = true;
    }


    private void AddAndMatchNearbyGemsToList(GameObject gem1, GameObject gem2, GameObject gem3)
    {
        AddAndMatchGem(gem1);
        AddAndMatchGem(gem2);
        AddAndMatchGem(gem3);
    }


    public IEnumerator FindAllMatches()
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject currentGem = board[i, j];
                

                if (currentGem != null)
                {
                    // Checking matches for left and right
                    if (i > 0 && i < width - 1)
                    {
                        ProcessNearbyGems(board[i - 1, j], currentGem, board[i + 1, j]);
                    }

                    // Checking matches for up and down
                    if (j > 0 && j < height - 1)
                    {
                        ProcessNearbyGems(board[i, j + 1], currentGem, board[i, j - 1]);
                    }
                }
            }
        }
    }



    private void ProcessNearbyGems(GameObject gem1, GameObject gem2, GameObject gem3)
    {
        if (gem1 != null && gem3 != null)
        {
            
            GemBehaviour gem1Behaviour = gem1.GetComponent<GemBehaviour>();
            GemBehaviour gem2GemBehaviour = gem2.GetComponent<GemBehaviour>();
            GemBehaviour gem3GemBehaviour = gem3.GetComponent<GemBehaviour>();

            if (gem1.tag == gem2.tag && gem3.tag == gem2.tag)
            {

                currentMatchesList.Union(CheckMatchedGemsInRow(gem1Behaviour, gem2GemBehaviour, gem3GemBehaviour));

                currentMatchesList.Union(CheckMatchedGemsInColumn(gem1Behaviour, gem2GemBehaviour, gem3GemBehaviour));

                currentMatchesList.Union(CheckMatchedGemsInSquare(gem1Behaviour, gem2GemBehaviour, gem3GemBehaviour));

                AddAndMatchNearbyGemsToList(gem1, gem2, gem3);
            }
        }
    }



    public void MatchedGemsOfColor(string color)
    {
        for(int i = 0; i < width; i ++)
        {
            for(int j = 0; j < height; j ++)
            {
                if(board[i,j] != null)
                {
                    if(board[i,j].tag == color)
                    {
                        board[i, j].GetComponent<GemBehaviour>().isMatched = true;
                    }
                }
            }
        }
    }


    List<GameObject> MatchedGemsInColumn(int column)
    {
        List<GameObject> gems = new List<GameObject>();

        for (int i = 0; i < height; i++)
        {
            if (board[column, i] != null)
            {
                gems.Add(board[column, i]);
                board[column, i].GetComponent<GemBehaviour>().isMatched = true;
                }
        }

        return gems;
    }

    List<GameObject> MatchedGemsInRow(int row)
    {
        List<GameObject> gems = new List<GameObject>();

        for (int i = 0; i < width; i++)
        {
            if (board[i, row] != null)
            {
                gems.Add(board[i, row]);
                board[i, row].GetComponent<GemBehaviour>().isMatched = true;
            }
        }

        return gems;
    }


    List<GameObject> MatchedGemsInSquare(int column, int row)
    {
        List<GameObject> gems = new List<GameObject>();

        for(int i = column - 1; i <= column + 1; i++)
        {
            for(int j = row - 1; j <= row + 1; j++)
            {
                if(i >= 0 && i < width && j >=0 && j < height)
                {
                    gems.Add(board[i, j]);
                    board[i, j].GetComponent<GemBehaviour>().isMatched = true;
                }
            }
        }

        return gems;
    }


    public void CheckToSetSpecialClearRowOrColumnGem()
    {
        if(currentGem != null)
        {
            if(currentGem.isMatched)
            {
                SetSpecialClearRowOrColumnGem(currentGem);
            }
            else if(currentGem.nextGem != null)
            {
                GemBehaviour nextGem = currentGem.nextGem.GetComponent<GemBehaviour>();

                if(nextGem.isMatched)
                {
                    SetSpecialClearRowOrColumnGem(nextGem);
                }

                
  
            }
        }
    }


    private void SetSpecialClearRowOrColumnGem(GemBehaviour gem)
    {
        gem.isMatched = false;

        if ((currentGem.changedAngle > -45.0f && currentGem.changedAngle <= 45.0f) || (currentGem.changedAngle <= -135.0f || currentGem.changedAngle > 135.0f))
        {
            gem.CreateSpecialGem(SpecialGem.ROW_CLEAR);
        }
        else
        {
            gem.CreateSpecialGem(SpecialGem.COLUMN_CLEAR);
        }
    }

    private void SetUpCamera(float x, float y)
    {
        Vector3 temPos = new Vector3(x / 2, y / 2, -10.0f);
        Camera.main.transform.position = temPos;

        if(width >= height)
        {
            Camera.main.orthographicSize = (width / 2 + padding) * aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = height / 2 + padding;
        }
    }

    
}
