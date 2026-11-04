using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;

/// <summary>
/// Loads scenes with a loading screen transition.
/// </summary>
[DefaultExecutionOrder(-105)]
public class SceneLoader : MonoBehaviour
{
    #region Variables

    private const char DotChar = '.';
    private const char PercentageChar = '%';

    private static readonly bool StopTimeDuringOpenAnim = true;

    public static SceneLoader Instance;

    public bool IsLoading { get; private set; }
    
    [Header("General Config")]
    [SerializeField] private bool waitForAnimations = false;

    [Header("Animation")]
    [SerializeField] private AnimationClip openClip;
    [SerializeField] private AnimationClip closeClip;
    [SerializeField] private Animator loadingScreen;

    // Events
    public delegate void BeforeSceneLoadedCallback();
    public event BeforeSceneLoadedCallback BeforeSceneLoaded;

    // Currently only used to run logic right after loading screen finishes closing (good for initial animations, etc.).
    public delegate void OnSceneLoadedCallback();
    public event OnSceneLoadedCallback OnSceneLoaded;

    public event BeforeSceneLoadedCallback BeforeDifferentSceneLoaded;

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            // Got annoying while testing scene transitions, as I don't typically start from the main menu (where only one instance would be needed, but currently there's one in every scene).
            //Debug.LogWarning("Two SceneLoader object instances found. Destroying latest one.", gameObject);
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        if (openClip == null && closeClip == null) Debug.LogWarning("Both the open and close animation clips on this ScreenLoader are null. Loading screen may not function correctly.", this);
        else if (openClip == null) Debug.LogWarning("The open animation clip on this ScreenLoader is null. Loading screen may not function correctly.", this);
        else if (closeClip == null) Debug.LogWarning("The close animation clip on this ScreenLoader is null. Loading screen may not function correctly.", this);

        if (loadingScreen == null && !TryGetComponent(out loadingScreen))
        {
            loadingScreen = GetComponentInChildren<Animator>();
            if (loadingScreen == null) Debug.LogWarning("Animator component reference not set on this SceneLoader. Loading screen may not function correctly.", this);
        }

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Scene Methods (public)

    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single) => StartCoroutine(WaitToLoadScene(sceneName, mode));
    public void LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single) => StartCoroutine(WaitToLoadScene(sceneBuildIndex, mode));

    #endregion

    #region Scene Coroutines (private)

    /// <summary>
    /// Waits delay before actually starting to load the new scene (and running the WaitForSceneLoad coroutine)
    /// </summary>
    private IEnumerator WaitToLoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        loadingScreen.updateMode = AnimatorUpdateMode.UnscaledTime;
        StopTime();

        if (!waitForAnimations || openClip == null)
        {
            StartCoroutine(WaitForSceneLoad(SceneManager.LoadSceneAsync(sceneName, mode), false));
            yield break;
        }

        loadingScreen.Play(openClip.name);
        yield return new WaitForSecondsRealtime(openClip.length);

        // Call 'BeforeDifferentSceneLoaded' first since car position may be reset when entering the main menu
        // (otherwise, car pos would be reset when return to menu callback triggers, then immediately be set to current scene state when BeforeDiffSceneLoaded is invoked)
        if (sceneName != SceneManager.GetActiveScene().name && BeforeDifferentSceneLoaded != null) BeforeDifferentSceneLoaded.Invoke();
        StartCoroutine(WaitForSceneLoad(SceneManager.LoadSceneAsync(sceneName, mode), true));
    }
    private IEnumerator WaitToLoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        loadingScreen.updateMode = AnimatorUpdateMode.UnscaledTime;
        StopTime();

        Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;

        if (!waitForAnimations || openClip == null)
        {
            StartCoroutine(WaitForSceneLoad(SceneManager.LoadSceneAsync(sceneBuildIndex, mode), true));
            yield break;
        }

        loadingScreen.Play(openClip.name);
        yield return new WaitForSecondsRealtime(openClip.length);

        if (sceneBuildIndex != SceneManager.GetActiveScene().buildIndex && BeforeDifferentSceneLoaded != null) BeforeDifferentSceneLoaded.Invoke();
        StartCoroutine(WaitForSceneLoad(SceneManager.LoadSceneAsync(sceneBuildIndex, mode), false));
    }

    /// <summary>
    /// Waits for a scene to finish loading once the process has already begun.
    /// This primarily just updates the loading screen while this occurs.
    /// </summary>
    private IEnumerator WaitForSceneLoad(AsyncOperation op, bool isAnimPlaying)
    {
        if (BeforeSceneLoaded != null) BeforeSceneLoaded.Invoke();
        
        op.allowSceneActivation = false;

        IsLoading = true;

        var timeDoneLoading = -1f; // used in while loop
        
        if (isAnimPlaying) loadingScreen.Play(openClip.name);

        while (!op.isDone || (waitForAnimations && timeDoneLoading > closeClip.length) )
        {
            // Cache the Time.realtimeSinceStartup when operation finished
            if (op.isDone && timeDoneLoading < 0f) timeDoneLoading = Time.realtimeSinceStartup;

            if (op.progress >= 0.85f)
            {
                op.allowSceneActivation = true;
                Application.backgroundLoadingPriority = ThreadPriority.Normal;
            }

            yield return null;
        }

        loadingScreen.updateMode = AnimatorUpdateMode.Normal;
        loadingScreen.Play(closeClip.name);

        IsLoading = false;

        ResumeTime();

        yield return new WaitForSeconds(closeClip.length / loadingScreen.GetCurrentAnimatorStateInfo(0).speed);

        if (OnSceneLoaded != null) OnSceneLoaded.Invoke();
    }

    #endregion

    #region Time Helper Methods

    private void StopTime()
    {
        if (!StopTimeDuringOpenAnim) return;
        Time.timeScale = 0.0f;
    }
    private void ResumeTime()
    {
        if (!StopTimeDuringOpenAnim) return;
        
        Time.timeScale = 1.0f;
    }

    #endregion
}
