using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverMenuController : Menu
{
    public struct GameOverMenuData
    {
        public GameOverMenuData(string winnerName)
        {
            WinnerName = winnerName;
        }

        /// <summary>
        /// Can be a player index, or player name.
        /// </summary>
        public string WinnerName;
    }

    [Header("Game Over Menu Specific Fields")]
    [SerializeField] int mainMenuSceneBuildIndex = 0;
    [Space]
    [SerializeField] string winnerPrefix = "Player ";
    [SerializeField] string winnerSuffix = " wins!";
    [SerializeField] TMP_Text winnerText;
    GameOverMenuData data;

    public void Init(GameOverMenuData Data)
    {
        data = Data;
        winnerText.text = winnerPrefix + data.WinnerName + winnerSuffix;
    }

    public void PlayAgain()
    {
        // Reload currently active scene
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneLoader.Instance.LoadScene(mainMenuSceneBuildIndex);
    }
}
