using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public const string MainMenu = "MainMenu";
    public const string Gameplay = "Level1";

    public static void LoadMainMenu() => SceneManager.LoadScene(MainMenu);
    public static void LoadGameplay() => SceneManager.LoadScene(Gameplay);
}