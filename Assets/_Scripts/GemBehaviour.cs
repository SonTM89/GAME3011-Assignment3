using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBehaviour : MonoBehaviour
{
    public int column;
    public int row;
    public int targetPosX;
    public int targetPosY;

    private BoardGenerator boardGenerator;
    private GameObject nextGem;

    private Vector3 firstPos;
    private Vector3 finalPos;

    public float changedAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        boardGenerator = FindObjectOfType<BoardGenerator>();
        targetPosX = (int)transform.position.x;
        targetPosY = (int)transform.position.y;
        row = targetPosY;
        column = targetPosX;
    }

    // Update is called once per frame
    void Update()
    {
        targetPosX = column;
        targetPosY = row;

        GemMoving();
    }

    void GemMoving()
    {
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

        CalculateAngle();
    }

    private void CalculateAngle()
    {
        changedAngle = Mathf.Atan2(finalPos.y - firstPos.y, finalPos.x - firstPos.x) * 180.0f / Mathf.PI;
        Debug.Log(changedAngle);

        GemChangingPosition();
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
    }
}
