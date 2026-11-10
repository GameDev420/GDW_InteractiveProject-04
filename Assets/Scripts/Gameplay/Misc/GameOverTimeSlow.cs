using UnityEngine;

public class GameOverTimeSlow : MonoBehaviour
{
    const float SlowedTimeScale = 0.0f;

    const float MarginOfError = 0.03f;

    [SerializeField] SmoothedValueConfig timeScaleSmoothConfig;
    SmoothedFloat smoothedTimeScale;
    bool stopTime;

    private void Awake()
    {
        smoothedTimeScale = new(timeScaleSmoothConfig);
        stopTime = false;
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnGameOver += OnGameOver;
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnGameOver -= OnGameOver;
    }

    private void Update()
    {
        if (stopTime)
        {
            smoothedTimeScale.Smooth(Time.unscaledDeltaTime);
            Time.timeScale = smoothedTimeScale.SmoothedValue;

            if (Mathf.Abs(Time.timeScale - SlowedTimeScale) <= MarginOfError)
            {
                Time.timeScale = smoothedTimeScale.SmoothedValue = smoothedTimeScale.TargetValue = SlowedTimeScale;
                stopTime = false;
            }
        }
    }

    void OnGameOver(PlayerController winner)
    {
        smoothedTimeScale.SmoothedValue = Time.timeScale;
        smoothedTimeScale.TargetValue = Mathf.Max(SlowedTimeScale, 0.0f);

        stopTime = true;
    }
}
