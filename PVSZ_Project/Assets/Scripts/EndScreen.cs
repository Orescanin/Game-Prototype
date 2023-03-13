using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI winLoseText;
    public TextMeshProUGUI reloadText;
    
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
    
    public void SetWinState(bool won)
    {
        if (won)
        {
            winLoseText.text    = "You WON";
            reloadText.text     = "Again!";
        }
        else 
        {
            winLoseText.text    = "You lost";
            reloadText.text     = "Retry";
        }
    }
}
