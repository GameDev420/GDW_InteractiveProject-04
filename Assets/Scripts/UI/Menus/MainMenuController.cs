using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] int gameSceneIndex = 1;
    [SerializeField] PreGameDataWriter preGameWriter;

    public void CloseGame() => Application.Quit();

    public void StartGame()
    {
        if (preGameWriter != null) preGameWriter.WritePreGameData();

        if (SceneLoader.Instance != null) SceneLoader.Instance.LoadScene(gameSceneIndex);
        else UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneIndex);
    }
}
