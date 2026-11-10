using UnityEngine;

public class OneShotAudioInitializer : MonoBehaviour
{
    private static OneShotAudioInitializer Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(this);

            static void InitAudioSourceInstance(AudioSource obj)
            {
                obj.transform.SetParent(null);
                obj.playOnAwake = false;
                DontDestroyOnLoad(obj);
            }

            OneShotAudioManager.Initialize(InitAudioSourceInstance);
            Instance = this;
        }
        else Destroy(gameObject);
    }
}
