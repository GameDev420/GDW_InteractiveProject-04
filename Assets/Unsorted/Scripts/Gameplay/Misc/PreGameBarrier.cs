using UnityEngine;

public class PreGameBarrier : MonoBehaviour
{
    [SerializeField] bool enableBarriers = true;
    [SerializeField] Collider[] barriers = new Collider[0];

    private void Awake()
    {
        SetBarrierEnabled(enableBarriers);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameStart += OnGameStart;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnGameStart -= OnGameStart;
    }

    void OnGameStart()
    {
        if (enableBarriers) SetBarrierEnabled(false);
    }

    void SetBarrierEnabled(bool status)
    {
        for (int i = 0; i < barriers.Length; i++)
        {
            if (barriers[i] != null) barriers[i].enabled = status;
        }
    }
}
