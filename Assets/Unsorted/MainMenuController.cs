using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MainMenuController : MonoBehaviour
{
    [SerializeField] string startTrigger = "StartGame";
    [SerializeField] string settingsTrigger = "Settings";
    [SerializeField] string quitTrigger = "Quit";
    [SerializeField] int gameSceneIndex = 1;
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void StartGame()
    {
        anim.SetTrigger(startTrigger);
    }

    public void Settings()
    {
        anim.SetTrigger(settingsTrigger);
    }

    public void QuitGame()
    {
        anim.SetTrigger(quitTrigger);
    }
}
