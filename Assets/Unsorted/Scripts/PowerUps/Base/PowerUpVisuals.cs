using UnityEngine;

public class PowerUpVisuals : MonoBehaviour
{
    [SerializeField] Vector3 baseLocalOffset = new(0.0f, 1.25f, 0.0f);

    PowerUpInstance instance;
    Transform model;

    public virtual void Init(PowerUpInstance Instance)
    {
        instance = Instance;

        model = Instantiate(instance.Data.PickUpModel, instance.Data.PickUpModel.transform.position + transform.TransformPoint(baseLocalOffset), instance.Data.PickUpModel.transform.rotation, transform).transform;
    }

    public void SetVisibility(bool visible)
    {
        if (model != null && model.gameObject.activeSelf != visible) model.gameObject.SetActive(visible);
    }
}
