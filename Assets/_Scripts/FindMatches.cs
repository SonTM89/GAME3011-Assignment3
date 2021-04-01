using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private BoardGenerator miniGame;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        miniGame = FindObjectOfType<BoardGenerator>();
    }

    public void StartFinding()
    {
        StartCoroutine(FindAllMatches());
    }

    private IEnumerator FindAllMatches()
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < miniGame.width; i++)
        {
            for (int j = 0; j < miniGame.height; j++)
            {
                GameObject currentGem = miniGame.board[i, j];

                if(currentGem != null)
                {
                    // Checking matches for left and right
                    if (i > 0 && i < miniGame.width - 1)
                    {
                        GameObject leftGem = miniGame.board[i - 1, j];
                        GameObject rightGem = miniGame.board[i + 1, j];

                        if(leftGem != null && rightGem != null)
                        {
                            if(leftGem.tag == currentGem.tag && rightGem.tag == currentGem.tag)
                            {
                                if(!currentMatches.Contains(leftGem))
                                {
                                    currentMatches.Add(leftGem);
                                }
                                leftGem.GetComponent<GemBehaviour>().isMatched = true;

                                if (!currentMatches.Contains(rightGem))
                                {
                                    currentMatches.Add(rightGem);
                                }
                                rightGem.GetComponent<GemBehaviour>().isMatched = true;

                                if (!currentMatches.Contains(currentGem))
                                {
                                    currentMatches.Add(currentGem);
                                }
                                currentGem.GetComponent<GemBehaviour>().isMatched = true;
                            }
                        }
                    }

                    // Checking matches for up and down
                    if (j > 0 && j < miniGame.height - 1)
                    {
                        GameObject upGem = miniGame.board[i, j + 1];
                        GameObject downGem = miniGame.board[i, j - 1];

                        if (upGem != null && downGem != null)
                        {
                            if (upGem.tag == currentGem.tag && downGem.tag == currentGem.tag)
                            {
                                if (!currentMatches.Contains(upGem))
                                {
                                    currentMatches.Add(upGem);
                                }
                                upGem.GetComponent<GemBehaviour>().isMatched = true;
                                
                                if (!currentMatches.Contains(downGem))
                                {
                                    currentMatches.Add(downGem);
                                }
                                downGem.GetComponent<GemBehaviour>().isMatched = true;

                                if (!currentMatches.Contains(currentGem))
                                {
                                    currentMatches.Add(currentGem);
                                }
                                currentGem.GetComponent<GemBehaviour>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }


    //    List<GameObject> GetColumnGems(int column)
    //    {
    //        List<GameObject> gems = new List<GameObject>();

    //        for(int i = 0; i < miniGame.height; i++)
    //        {
    //            if(miniGame.board[column, i]!= null)
    //            {
    //                gems.Add(miniGame.board[column, i]);
    //                miniGame.
    //            }
    //        }

    //        return gems;
    //    }
}
