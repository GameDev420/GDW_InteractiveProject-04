using UnityEngine;
using System.Collections;

public class PlayerVFX : MonoBehaviour
{
    [Header("Land VFX")]
    [SerializeField] GameObject landParticles;

    [Header("Wall Slide VFX")]
    [SerializeField] GameObject wallSlideParticles;
    GameObject wallSlideVFXInstance;
    
    [Header("Speed Trail")]
    [SerializeField] TrailRenderer speedTrail;
    [SerializeField] MaterialColorRandomizer playerColorRandomizer;

    [Header("Power Up VFX")]
    [SerializeField] Vector3 localVFXPositionOffset = Vector3.zero;

    private void Awake()
    {
        wallSlideVFXInstance = Instantiate(wallSlideParticles, transform.TransformPoint(localVFXPositionOffset), wallSlideParticles.transform.rotation, transform);
        wallSlideVFXInstance.SetActive(false);
    }

    private void Start()
    {
        if (playerColorRandomizer != null && speedTrail != null)
        {
            speedTrail.startColor = new Color(playerColorRandomizer.Color.r, playerColorRandomizer.Color.g, playerColorRandomizer.Color.b, speedTrail.startColor.a);
            speedTrail.endColor = new Color(playerColorRandomizer.Color.r, playerColorRandomizer.Color.g, playerColorRandomizer.Color.b, speedTrail.endColor.a);
        }
    }

    public void SpawnVFX(GameObject prefab, float duration, bool unscaledTime = false)
    {
        if (prefab == null) return;

        StartCoroutine(WaitToDestroyVFX(prefab, duration, unscaledTime));
    }

    IEnumerator WaitToDestroyVFX(GameObject prefab, float duration, bool unscaledTime)
    {
        var vfx = Instantiate(prefab, transform.TransformPoint(localVFXPositionOffset), prefab.transform.rotation, transform);

        yield return unscaledTime ? new WaitForSecondsRealtime(duration) : new WaitForSeconds(duration);

        Destroy(vfx);
    }

    public void SpawnLandVFX(bool grounded)
    {
        if (grounded) Instantiate(landParticles, transform.position, landParticles.transform.rotation);
    }

    #region Wall Slide
    public void OnWallAttached()
    {
        wallSlideVFXInstance.SetActive(true);
    }
    public void OnWallDetached()
    {
        wallSlideVFXInstance.SetActive(false);
    }
    #endregion
}
