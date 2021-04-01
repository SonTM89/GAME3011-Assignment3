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

    private BoardGenerator boardGenerator;
    private GameObject nextGem;

    private Vector3 firstPos;
    private Vector3 finalPos;

    public float changedAngle = 0;
    public float changedThreshold = 0.8f;
    public bool isMatched = false;

    // Start is called before the first frame update
    void Start()
    {
        boardGenerator = FindObjectOfType<BoardGenerator>();

        targetPosX = (int)transform.position.x;
        targetPosY = (int)transform.position.y;

        row = targetPosY;
        column = targetPosX;

        previousRow = row;
        previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();

        if(isMatched)
        {
            SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
            sRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
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
            transform.position = Vector3.Lerp(transform.position, tempPos, 0.1f);
        }
        else
        {
            tempPos = new Vector3(targetPosX, transform.position.y, 0);
            transform.position = tempPos;
            boardGenerator.board[column, row] = this.gameObject;
        }

        // Moving left and right
        if (Mathf.Abs(targetPosY - transform.position.y) > 0.1f)
        {
            tempPos = new Vector3(transform.position.x, targetPosY, 0);
            transform.position = Vector3.Lerp(transform.position, tempPos, 0.1f);
        }
        else
        {
            tempPos = new Vector3(transform.position.x, targetPosY, 0);
            transform.position = tempPos;
            boardGenerator.board[column, row] = this.gameObject;
        }
    }

    
    private void OnMouseDown()
    {
        Vector3 tempPos = Input.mousePosition;
        firstPos = new Vector3(Camera.main.ScreenToWorldPoint(tempPos).x, Camera.main.ScreenToWorldPoint(tempPos).y, 0);
        //Debug.Log(tempPos);
    }

   
    private void OnMouseUp()
    {
        Vector3 tempPos = Input.mousePosition;
        finalPos = new Vector3(Camera.main.ScreenToWorldPoint(tempPos).x, Camera.main.ScreenToWorldPoint(tempPos).y, 0);
        //Debug.Log(finalPos);

        CalculateChangedPos();
    }

   
    private void CalculateChangedPos()
    {
        if(Mathf.Abs(finalPos.y - firstPos.y) > changedThreshold || Mathf.Abs(finalPos.x - firstPos.x) > changedThreshold)
        {
            changedAngle = Mathf.Atan2(finalPos.y - firstPos.y, finalPos.x - firstPos.x) * 180.0f / Mathf.PI;
            //Debug.Log(changedAngle);

            GemChangingPosition();
        }

        
    }

   
    private void GemChangingPosition()
    {
        // Right side
        if(changedAngle > -45.0f && changedAngle <= 45.0f && column < boardGenerator.width - 1)
        {
            nextGem = boardGenerator.board[column + 1, row];
            nextGem.GetComponent<GemBehaviour>().column -= 1;
            column += 1;
        } 
        // Up side
        else if (changedAngle > 45.0f && changedAngle <= 135.0f && row < boardGenerator.height - 1)
        {
            nextGem = boardGenerator.board[column, row + 1];
            nextGem.GetComponent<GemBehaviour>().row -= 1;
            row += 1;
        } 
        // Left side
        else if ((changedAngle > 135.0f || changedAngle <= -135.0f) && column > 0)
        {
            nextGem = boardGenerator.board[column - 1, row];
            nextGem.GetComponent<GemBehaviour>().column += 1;
            column -= 1;
        }
        // Down side
        else if (changedAngle > -135.0f && changedAngle <= -45.0f && row > 0)
        {
            nextGem = boardGenerator.board[column, row - 1];
            nextGem.GetComponent<GemBehaviour>().row += 1;
            row -= 1;
        }

        StartCoroutine(CheckChanging());
    }


    public IEnumerator CheckChanging()
    {
        yield return new WaitForSeconds(0.4f);
        if(nextGem != null)
        {
            if(!isMatched && !nextGem.GetComponent<GemBehaviour>().isMatched)
            {
                nextGem.GetComponent<GemBehaviour>().row = row;
                nextGem.GetComponent<GemBehaviour>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            else
            {
                boardGenerator.DestroyAllMatchesGem();
            }

            nextGem = null;
        }
    }


    private void FindMatches()
    {
        if(column > 0 && column < boardGenerator.width - 1)
        {
            GameObject leftGem1 = boardGenerator.board[column - 1, row];
            GameObject rightGem1 = boardGenerator.board[column + 1, row];
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

        if (row > 0 && row < boardGenerator.height - 1)
        {
            GameObject downGem1 = boardGenerator.board[column, row - 1];
            GameObject upGem1 = boardGenerator.board[column, row + 1];

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
}
