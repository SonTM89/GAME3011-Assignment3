using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InputValue : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;

    public static Difficulty gameDifficulty;

    [SerializeField] private TextMeshProUGUI difficultyText;

    public static Level gameLevel;

    [SerializeField] private TextMeshProUGUI gameLevelText;

    [SerializeField] private GameObject message;

    // Start is called before the first frame update
    void Start()
    {
        gameDifficulty = Difficulty.NONE;

        gameLevel = Level.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        difficultyText.text = Enum.GetName(typeof(Difficulty), (int)gameDifficulty);

        gameLevelText.text = Enum.GetName(typeof(Level), (int)gameLevel);
    }

    
    // Press Start Button
    public void StartGame()
    {
        gameUI.gameObject.SetActive(true);
    }


    // Press Quit Button
    public void QuitGame()
    {
        gameUI.gameObject.SetActive(false);
        Application.Quit();
    }


    // Press Easy Button
    public void EasySelected()
    {
        gameDifficulty = Difficulty.EASY;
    }


    // Press Medium Button
    public void MediumSelected()
    {
        gameDifficulty = Difficulty.MEDIUM;
    }


    // Press Hard Button
    public void HardSelected()
    {
        gameDifficulty = Difficulty.HARD;
    }


    // Press Easy Button
    public void Level1Selected()
    {
        gameLevel = Level.LEVEL1;
    }


    // Press Medium Button
    public void Level2Selected()
    {
        gameLevel = Level.LEVEL2;
    }


    // Press Hard Button
    public void Level3Selected()
    {
        gameLevel = Level.LEVEL3;
    }


    // Press To Go
    public void OnPressedToGo()
    {
        // Go to lock simulation inside if Difficulty and Skill is selected
        // Otherwise, show alert message
        if (gameDifficulty != Difficulty.NONE && gameLevel != Level.NONE)
        {
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            message.gameObject.SetActive(true);
            StartCoroutine(HideMessage(3.0f, message));
        }
    }


    // Hide message after specific time period
    IEnumerator HideMessage(float delay, GameObject message)
    {
        yield return new WaitForSeconds(delay);
        message.gameObject.SetActive(false);
    }
}
