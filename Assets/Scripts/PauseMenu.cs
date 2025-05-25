using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Panel referansý
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
        Time.timeScale = 1f; // Oyunu kaldýðý yerden devam ettir
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
        Time.timeScale = 1f; // Sahne deðiþtirmeden önce zaman skalasýný sýfýrla
        SceneManager.LoadScene("MainMenu"); // Sahne adýný kendi sahnene göre deðiþtir
    }
}
