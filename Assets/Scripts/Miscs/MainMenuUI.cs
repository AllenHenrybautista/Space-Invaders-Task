using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnPlayPressed()
    {
        Time.timeScale = 1f;
        SceneLoader.LoadGameplay();
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}