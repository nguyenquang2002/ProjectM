using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public void PlayGame()
    {
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Level1");
    }
    public void PlayLevel2()
    {
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Level2");
    }
    public void PlayLevel3()
    {
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Level3");
    }
    public void PlayLevel4()
    {
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Level4");
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }
    public void Continue()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
    public void Home()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    public void LevelSelect()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelect");
    }
    public void Quit()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
}
