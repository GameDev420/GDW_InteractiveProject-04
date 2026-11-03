using UnityEngine;

public class PowerUpVisual : MonoBehaviour
{
    [SerializeField] float pingPongSpeed = 1.0f;
    [SerializeField] float pingPongHeight = 0.5f;
    [Tooltip("In degrees per second.")]
    [SerializeField] Vector3 localAngularSpeed = Vector3.one;
    float initialY;
    PowerUpData data;
    GameObject model;

    public void Init(PowerUpData Data)
    {
        initialY = transform.localPosition.y;
        data = Data;

        model = Instantiate(Data.PickUpModel, transform.position, data.PickUpModel.transform.rotation, transform);
    }

    private void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, initialY + Mathf.PingPong(Time.time * pingPongSpeed, pingPongHeight), transform.localPosition.z);
        transform.Rotate(localAngularSpeed * Time.deltaTime, Space.Self);
    }
}
