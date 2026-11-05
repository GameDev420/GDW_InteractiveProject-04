using UnityEngine;
using System.Collections;

public class PlayerVFX : MonoBehaviour
{
    [Header("Land VFX")]
    [SerializeField] GameObject landParticles;

    [Header("Power Up VFX")]
    [SerializeField] Vector3 localVFXPositionOffset = Vector3.zero;

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
}
