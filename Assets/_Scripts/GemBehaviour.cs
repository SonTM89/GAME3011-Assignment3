using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBehaviour : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;

    public int targetPosX;
    public int targetPosY;

    private BoardGenerator miniGame;
    public GameObject nextGem;

    private Vector3 firstPos;
    private Vector3 finalPos;

    private FindMatches findMatches;

    public float changedAngle = 0;
    public float changedThreshold = 0.8f;
    public bool isMatched = false;

    [Header("Powerup")]
    public bool isColorClearGem;
    public bool isColumnClearGem;
    public bool isRowClearGem;

    public GameObject columnClearGem;
    public GameObject rowclearGem;
    public GameObject colorClearGem;

    // Start is called before the first frame update
    void Start()
    {
        isColumnClearGem = false;
        isRowClearGem = false;

        miniGame = FindObjectOfType<BoardGenerator>();
        findMatches = FindObjectOfType<FindMatches>();

        //targetPosX = (int)transform.position.x;
        //targetPosY = (int)transform.position.y;

        //row = targetPosY;
        //column = targetPosX;

        //previousRow = row;
       // previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        //FindMatches();

        if(isMatched)
        {
            SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
            sRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.3f, 1.3f, 1.3f), 0.025f);
        }

        if(isColumnClearGem || isRowClearGem)
        {
            SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
            sRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), 0.025f);
        }

        GemMoving();
    }

    void GemMoving()
    {
        targetPosX = column;
        targetPosY = row;

        Vector3 tempPos;
        // Moving up and down
        if (Mathf.Abs(targetPosX - transform.position.x) > 0.1f)
        {
            tempPos = new Vector3(targetPosX, transform.position.y, 0);
            transform.position = Vector3.Lerp(transform.position, tempPos, 0.3f);
            if(miniGame.board[column, row] != this.gameObject)
            {
                miniGame.board[column, row] = this.gameObject;
            }

            //findMatches.StartFinding();
            StartCoroutine(miniGame.FindAllMatches());
        }
        else
        {
            tempPos = new Vector3(targetPosX, transform.position.y, 0);
            transform.position = tempPos;
            miniGame.board[column, row] = this.gameObject;
        }

        // Moving left and right
        if (Mathf.Abs(targetPosY - transform.position.y) > 0.1f)
        {
            tempPos = new Vector3(transform.position.x, targetPosY, 0);
            transform.position = Vector3.Lerp(transform.position, tempPos, 0.3f);
            if (miniGame.board[column, row] != this.gameObject)
            {
                miniGame.board[column, row] = this.gameObject;
            }

            //findMatches.StartFinding();
            StartCoroutine(miniGame.FindAllMatches());
        }
        else
        {
            tempPos = new Vector3(transform.position.x, targetPosY, 0);
            transform.position = tempPos;
            miniGame.board[column, row] = this.gameObject;
        }
    }



    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            isColorClearGem = true;
            GameObject clearGem = Instantiate(colorClearGem, transform.position, Quaternion.identity);
            clearGem.transform.parent = this.transform;
        }
    }


    private void OnMouseDown()
    {
        if(miniGame.currentState == GameState.MOVE)
        {
            Vector3 tempPos = Input.mousePosition;
            firstPos = new Vector3(Camera.main.ScreenToWorldPoint(tempPos).x, Camera.main.ScreenToWorldPoint(tempPos).y, 0);
            //Debug.Log(tempPos);
        }
    }

   
    private void OnMouseUp()
    {
        if (miniGame.currentState == GameState.MOVE)
        {
            Vector3 tempPos = Input.mousePosition;
            finalPos = new Vector3(Camera.main.ScreenToWorldPoint(tempPos).x, Camera.main.ScreenToWorldPoint(tempPos).y, 0);
            //Debug.Log(finalPos);

            CalculateChangedPos();
        }
        
    }

   
    private void CalculateChangedPos()
    {
        if(Mathf.Abs(finalPos.y - firstPos.y) > changedThreshold || Mathf.Abs(finalPos.x - firstPos.x) > changedThreshold)
        {
            changedAngle = Mathf.Atan2(finalPos.y - firstPos.y, finalPos.x - firstPos.x) * 180.0f / Mathf.PI;
            //Debug.Log(changedAngle);

            GemChangingPosition();

            miniGame.currentState = GameState.WAIT;

            miniGame.currentGem = this;
        }
        else
        {
            miniGame.currentState = GameState.MOVE;           
        }
        
    }

   
    private void GemChangingPosition()
    {
        // Right side
        if(changedAngle > -45.0f && changedAngle <= 45.0f && column < miniGame.width - 1)
        {
            nextGem = miniGame.board[column + 1, row];
            previousRow = row;
            previousColumn = column;
            nextGem.GetComponent<GemBehaviour>().column -= 1;
            column += 1;
        } 
        // Up side
        else if (changedAngle > 45.0f && changedAngle <= 135.0f && row < miniGame.height - 1)
        {
            nextGem = miniGame.board[column, row + 1];
            previousRow = row;
            previousColumn = column;
            nextGem.GetComponent<GemBehaviour>().row -= 1;
            row += 1;
        } 
        // Left side
        else if ((changedAngle > 135.0f || changedAngle <= -135.0f) && column > 0)
        {
            nextGem = miniGame.board[column - 1, row];
            previousRow = row;
            previousColumn = column;
            nextGem.GetComponent<GemBehaviour>().column += 1;
            column -= 1;
        }
        // Down side
        else if (changedAngle > -135.0f && changedAngle <= -45.0f && row > 0)
        {
            nextGem = miniGame.board[column, row - 1];
            previousRow = row;
            previousColumn = column;
            nextGem.GetComponent<GemBehaviour>().row += 1;
            row -= 1;
        }

        StartCoroutine(CheckChanging());
    }


    public IEnumerator CheckChanging()
    {
        if(isColorClearGem)
        {
            miniGame.MatchedGemsOfColor(nextGem.tag);
            isMatched = true;
        }
        else if(nextGem.GetComponent<GemBehaviour>().isColumnClearGem)
        {
            miniGame.MatchedGemsOfColor(this.gameObject.tag);
            nextGem.GetComponent<GemBehaviour>().isMatched = true;
        }

        yield return new WaitForSeconds(0.4f);
        if(nextGem != null)
        {
            if(!isMatched && !nextGem.GetComponent<GemBehaviour>().isMatched)
            {
                nextGem.GetComponent<GemBehaviour>().row = row;
                nextGem.GetComponent<GemBehaviour>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                miniGame.currentGem = null;
                miniGame.currentState = GameState.MOVE;
            }
            else
            {
                miniGame.DestroyAllMatchesGem();
                
            }

            nextGem = null;
        }
    }


    private void FindMatches()
    {
        if(column > 0 && column < miniGame.width - 1)
        {
            GameObject leftGem1 = miniGame.board[column - 1, row];
            GameObject rightGem1 = miniGame.board[column + 1, row];
            if(leftGem1 != null && rightGem1 != null)
            {
                if (leftGem1.tag == this.gameObject.tag && rightGem1.tag == this.gameObject.tag)
                {
                    leftGem1.GetComponent<GemBehaviour>().isMatched = true;
                    rightGem1.GetComponent<GemBehaviour>().isMatched = true;
                    isMatched = true;
                }
            }
        }

        if (row > 0 && row < miniGame.height - 1)
        {
            GameObject downGem1 = miniGame.board[column, row - 1];
            GameObject upGem1 = miniGame.board[column, row + 1];

            if (downGem1 != null && upGem1 != null)
            {
                if (downGem1.tag == this.gameObject.tag && upGem1.tag == this.gameObject.tag)
                {
                    downGem1.GetComponent<GemBehaviour>().isMatched = true;
                    upGem1.GetComponent<GemBehaviour>().isMatched = true;
                    isMatched = true;
                }
            }
            
        }
    }

    public void CreateRowClearGem()
    {
        isRowClearGem = true;
        GameObject tempGem = Instantiate(rowclearGem, transform.position, Quaternion.identity);
        tempGem.transform.parent = this.transform;
    }


    public void CreateColumnClearGem()
    {
        isColumnClearGem = true;
        GameObject tempGem = Instantiate(columnClearGem, transform.position, Quaternion.identity);
        tempGem.transform.parent = this.transform;
    }
}
