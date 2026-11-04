using UnityEngine;

public class PowerUpVisuals : MonoBehaviour
{
    [SerializeField] Vector3 baseLocalOffset = new(0.0f, 1.25f, 0.0f);
    [SerializeField] float verticalMoveSpeed = 1.0f;
    [SerializeField] float verticalMoveHeight = 0.5f;
    [Tooltip("In degrees per second.")]
    [SerializeField] Vector3 localAngularSpeed = new(30.0f, 45.0f, 0.0f);
    float initialY;

    PowerUpInstance instance;
    Transform model;

    public virtual void Init(PowerUpInstance Instance)
    {
        instance = Instance;

        model = Instantiate(instance.Data.PickUpModel, instance.Data.PickUpModel.transform.position + transform.TransformPoint(baseLocalOffset), instance.Data.PickUpModel.transform.rotation, transform).transform;
        initialY = model.localPosition.y;
    }

    private void Update()
    {
        if (model == null) return;

        model.SetLocalPositionAndRotation(
            new Vector3(model.localPosition.x, initialY + Mathf.Sin(Time.time * verticalMoveSpeed) * verticalMoveHeight * 0.5f + verticalMoveHeight * 0.5f, model.localPosition.z),
            model.localRotation * Quaternion.Euler(localAngularSpeed * Time.deltaTime)
        );
    }

    public void SetVisibility(bool visible)
    {
        if (model != null && model.gameObject.activeSelf != visible) model.gameObject.SetActive(visible);
    }
}
