using UnityEngine;

public class MainMenuController : Menu
{
    [Header("Main Menu Fields")]
    [SerializeField] int gameSceneIndex = 1;
    public void StartGame()
    {
        SceneLoader.Instance.LoadScene(gameSceneIndex);
    }
}
