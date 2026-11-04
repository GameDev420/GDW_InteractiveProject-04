using UnityEngine;

public class SFXCleaner : MonoBehaviour
{
    [SerializeField] PlayableSFXObject[] SFX;

    private void Awake()
    {
        for (int i = 0; i < SFX.Length; i++)
            if (SFX[i] != null)
                SFX[i].ResetSFX();
    }
}
