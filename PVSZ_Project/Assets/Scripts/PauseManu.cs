using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            var state = GameManager.Instance.GameState;
            
            if(state == GameState.Paused)
            {
                Resume();
            } 
            else if (state == GameState.Playing)
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        GameManager.Instance.ResumeGame();
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        GameManager.Instance.PauseGame();
    }
    public void LoadManu()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void Reload()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
