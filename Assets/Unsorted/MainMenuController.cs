using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : Menu
{
    [SerializeField] int gameSceneIndex = 1;
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneIndex, LoadSceneMode.Single);
    }
}
