using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

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

    public SingleTile(int _x, int _y, TileType _type)
    {
        x = _x;
        y = _y;
        tiletype = _type;
    }
}

public class BoardGenerator : MonoBehaviour
{
    public Difficulty gameDifficulty;

    public Level gameLevel;

    [Header("Timer")]
    public float timeRemaining;
    public TextMeshProUGUI minuteText;
    public TextMeshProUGUI secondText;

    [Header("Score")]
    public int score;
    public int winScore;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winScoreText;
    //public int move;
    //public TextMeshProUGUI moveText;
    //public TextMeshProUGUI winMoveText;
    public int gemScore = 10;
    private int streak = 1;

    public float refillDelay = 0.5f;

    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject gameOverText;

    private bool win;
    private bool gameOver;


    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;

    public GameState currentState;

    public GameObject[] gemTypes;

    public SingleTile[] tilePositionList;
    public List<SingleTile> immovableTilelist;
    
    [SerializeField] public bool[,] immovableboard;
    
    public GameObject[,] board;

    public List<GameObject> currentMatchesList = new List<GameObject>();
    public GemBehaviour currentGem;

    public float aspectRatio = 1.1f;
    public float padding = 2;

    public AudioSource matchingSound;


    // Start is called before the first frame update
    void Start()
    {
        win = false;
        gameOver = false;

        score = 0;
        //move = 0;

        immovableTilelist = new List<SingleTile>();

        gameDifficulty = InputValue.gameDifficulty;

        gameLevel = InputValue.gameLevel;

        SettingDifficulty();

        immovableboard = new bool[width, height];
        board = new GameObject[width, height];
        
        SetUpCamera(width, height);

        GenerateBoard();
    }


    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
        //moveText.text = move.ToString();
        winScoreText.text = winScore.ToString();


        if (gameOver == false)
        {
            TimeCounter();

            if(score >= winScore)
            {
                win = true;
                gameOver = true;
            }
        }
        else
        {
            if (win)
            {
                winText.gameObject.SetActive(true);
                StartCoroutine(ShowMessage(2.0f));
            }
            else
            {
                gameOverText.gameObject.SetActive(true);
                StartCoroutine(ShowMessage(2.0f));
            }
        }
    }


    // Show Message after finishing Game and change to Start scene
    IEnumerator ShowMessage(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("StartScene");
    }


    // Setting Difficulty up to Input value
    private void SettingDifficulty()
    {
        if (gameDifficulty == Difficulty.EASY)
        {
            timeRemaining = 60.0f;
            winScore = 1000;
        }
        else if (gameDifficulty == Difficulty.MEDIUM)
        {
            timeRemaining = 90.0f;
            winScore = 1750;
        }
        else if (gameDifficulty == Difficulty.HARD)
        {
            timeRemaining = 120.0f;
            winScore = 2500;
        }
        else
        {
            timeRemaining = 600.0f;
            winScore = 50;
        }
    }


    public int GemTypesToUse()
    {
        int numGemTypes = 0;

        switch(gameDifficulty)
        {
            case Difficulty.HARD:
                numGemTypes = gemTypes.Length;
                break;
            case Difficulty.MEDIUM:
                numGemTypes = gemTypes.Length - 1;
                break;
            case Difficulty.EASY:
                numGemTypes = gemTypes.Length - 2;
                break;
            default:
                numGemTypes = gemTypes.Length - 3;
                break;
        }

        return numGemTypes;
    }



    // Count down the time to set the game over state
    private void TimeCounter()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            int minute = (int)(timeRemaining) / 60;
            int second = (int)timeRemaining - (60 * minute);

            minuteText.text = (minute > 9) ? minute.ToString() : "0" + minute.ToString();
            secondText.text = (second > 9) ? second.ToString() : "0" + second.ToString();
        }
        else
        {
            gameOver = true;
        }
    }



    private void RandomizeImmovablePos()
    {
        int x = UnityEngine.Random.Range(0, width);
        int y = UnityEngine.Random.Range(2, height);

        if (immovableTilelist.Count > 0)
        {
            bool placeable = true;

            for(int i = 0; i < immovableTilelist.Count; i++)
            {

                if(immovableTilelist[i].x == x && immovableTilelist[i].y == y)
                {
                    placeable = false;
                    break;
                }
            }

            if (placeable == true)
            {
                immovableTilelist.Add(new SingleTile(x, y, TileType.IMMOVABLE));
            }
            else
            {
                RandomizeImmovablePos();
            }
        }
        else
        {
            immovableTilelist.Add(new SingleTile(x, y, TileType.IMMOVABLE));
        }
    }


    private void GenerateTileList()
    {
            int immovableTileAmount = 0;

            if (gameLevel != Level.NONE)
            {
                immovableTileAmount = ((int)gameLevel - 1) * 4;
            }
        
        //tilePositionList = new SingleTile[immovableTileAmount];

        for (int i = 0; i < immovableTileAmount; i++)
        {
            RandomizeImmovablePos();
        }

    }


    public void GenerateImmovableTile()
    {
        GenerateTileList();

        //for (int i = 0; i < tilePositionList.Length; i++)
        //{
        //    if (tilePositionList[i].tiletype == TileType.IMMOVABLE)
        //    {
        //        immovableboard[tilePositionList[i].x, tilePositionList[i].y] = true;
        //    }
        //}

        for (int i = 0; i < immovableTilelist.Count; i++)
        {
                immovableboard[immovableTilelist[i].x, immovableTilelist[i].y] = true;
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


                    int currentGem = Random.Range(0, GemTypesToUse());

                    int maxLoopTime = 0;
                    while (MatchesAt(i, j, gemTypes[currentGem]) && maxLoopTime < 100)
                    {
                        currentGem = Random.Range(0, GemTypesToUse());
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
                else
                {
                    Vector3 tempPos = new Vector3(i, j + offSet, 0);
                    GameObject testTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;

                    testTile.transform.parent = this.transform;
                    testTile.name = "( " + i + "," + j + " )";
                    SpriteRenderer sprRen = testTile.GetComponent<SpriteRenderer>();
                    sprRen.enabled = true;
                    testTile.transform.position = new Vector3(i, j, 0);
                }
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


    private bool FiveInColumnOrRow()
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


    private Vector2 RowAndColumn()
    {
        Vector2 rowAndColumn = new Vector2();

        int numberHorizontal = 0;
        int numberVertical = 0;
        GemBehaviour firstGem = currentMatchesList[0].GetComponent<GemBehaviour>();

        if (firstGem != null)
        {
            foreach (GameObject currentGem in currentMatchesList)
            {
                GemBehaviour gem = currentGem.GetComponent<GemBehaviour>();

                if (gem.row == firstGem.row)
                {
                    numberHorizontal++;
                }
                if (gem.column == firstGem.column)
                {
                    numberVertical++;
                }
            }
        }

        rowAndColumn = new Vector2(numberHorizontal, numberVertical);

        return rowAndColumn;
    }


    private void CheckToMakeSpecialGem()
    {
        if (currentMatchesList.Count == 4)
        {
            CheckToSetSpecialClearRowOrColumnGem();
        }
        if (currentMatchesList.Count == 5 || currentMatchesList.Count == 8)
        {
            if(FiveInColumnOrRow())
            {
                CheckToSetSpecialClearColorGem();
            }
            else
            {
                CheckToSetSpecialClearSquareGem();
            }
        }
        else if(currentMatchesList.Count == 6 || currentMatchesList.Count == 7)
        {
            if(RowAndColumn().x == 3 && RowAndColumn().y == 3)
            {

            }
            else if(RowAndColumn().x == 5 || RowAndColumn().y == 5)
            {
                CheckToSetSpecialClearColorGem();
            }
            else
            {
                CheckToSetSpecialClearSquareGem();
            }

        }
    }


    public void CheckToSetSpecialClearColorGem()
    {
        if (currentGem != null)
        {
            if (currentGem.isMatched)
            {
                if (!currentGem.isColorClearGem)
                {
                    currentGem.isMatched = false;
                    currentGem.CreateSpecialGem(SpecialGem.COLOR_CLEAR);
                }
            }
            else
            {
                if (currentGem.nextGem != null)
                {
                    GemBehaviour nextGemBehaviour = currentGem.nextGem.GetComponent<GemBehaviour>();
                    if (nextGemBehaviour.isMatched)
                    {
                        if (!nextGemBehaviour.isColorClearGem)
                        {
                            nextGemBehaviour.isMatched = false;
                            nextGemBehaviour.CreateSpecialGem(SpecialGem.COLOR_CLEAR);
                        }
                    }
                }
            }
        }
    }


    public void CheckToSetSpecialClearSquareGem()
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


    public void CheckToSetSpecialClearRowOrColumnGem()
    {
        if (currentGem != null)
        {
            if (currentGem.isMatched)
            {
                SetSpecialClearRowOrColumnGem(currentGem);
            }
            else if (currentGem.nextGem != null)
            {
                GemBehaviour nextGem = currentGem.nextGem.GetComponent<GemBehaviour>();

                if (nextGem.isMatched)
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

        if (width >= height)
        {
            Camera.main.orthographicSize = (width / 2 + padding) * aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = height / 2 + padding;
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

            if(matchingSound != null)
            {
                matchingSound.Play();
            }

            Destroy(board[column, row]);

            score += gemScore * streak;

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

        yield return  new WaitForSeconds(refillDelay * 0.5f);

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
                    int currentGem = Random.Range(0, GemTypesToUse());
                    int maxIterations = 0;
                    while(MatchesAt(i, j , gemTypes[currentGem]) && maxIterations < 100)
                    {
                        maxIterations++;
                        currentGem = Random.Range(0, GemTypesToUse());
                    }
                    maxIterations = 0;

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

        yield return new WaitForSeconds(refillDelay);

        while(MatchesOnBoard())
        {
            streak ++;
            DestroyAllMatchesGem();

            yield return new WaitForSeconds(2 * refillDelay);   
        }

        currentMatchesList.Clear();
        currentGem = null;

        yield return new WaitForSeconds(refillDelay);

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
                GemBehaviour gemBehaviour = board[column, i].GetComponent<GemBehaviour>();

                if(gemBehaviour.isRowClearGem)
                {
                    gems.Union(MatchedGemsInRow(i)).ToList();
                }

                gems.Add(board[column, i]);
                gemBehaviour.isMatched = true;
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
                GemBehaviour gemBehaviour = board[i, row].GetComponent<GemBehaviour>();

                if (gemBehaviour.isColumnClearGem)
                {
                    gems.Union(MatchedGemsInColumn(i)).ToList();
                }

                gems.Add(board[i, row]);
                gemBehaviour.isMatched = true;
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
                    if(board[i,j] != null)
                    {
                        gems.Add(board[i, j]);
                        board[i, j].GetComponent<GemBehaviour>().isMatched = true;
                    }       
                }
            }
        }

        return gems;
    }


   

    
}
