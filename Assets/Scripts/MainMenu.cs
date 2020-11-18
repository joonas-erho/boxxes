using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int indexOfFirstLevel;
    public int indexOfLevelSelect;

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(indexOfFirstLevel);
    }

    public void LoadLevelSelecter()
    {
        SceneManager.LoadScene(indexOfLevelSelect);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenInstructions()
    {

    }

    public void OpenSettings()
    {

    }
}
