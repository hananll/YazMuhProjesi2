using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Panel referans�
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Oyunu kald��� yerden devam ettir
        isPaused = false;
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Oyunu durdur
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Sahne de�i�tirmeden �nce zaman skalas�n� s�f�rla
        SceneManager.LoadScene("MainMenu"); // Sahne ad�n� kendi sahnene g�re de�i�tir
    }
}
