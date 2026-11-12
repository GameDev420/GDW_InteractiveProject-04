using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TempMovement : MonoBehaviour
{
    struct SpeedMultiplierData
    {
        public float Multiplier;
        public float EndTime; // using scaled time

        public SpeedMultiplierData(float _Multiplier, float _EndTime)
        {
            Multiplier = _Multiplier;
            EndTime = _EndTime;
        }
    }

    [SerializeField] bool horizontalOnly = true;
    [SerializeField] float moveSpeed = 5.0f;
    Vector2 moveInput;

    readonly List<SpeedMultiplierData> speedMultipliers = new();

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        var multiplier = 1.0f;

        if (speedMultipliers.Count > 0)
        {
            var avg = 0.0f;
            for (int i = 0; i < speedMultipliers.Count; i++)
            {
                if (speedMultipliers[i].EndTime - Time.time <= 0.0f) speedMultipliers.RemoveAt(i);
                else avg += speedMultipliers[i].Multiplier;
            }

            if (speedMultipliers.Count > 0) multiplier = avg / speedMultipliers.Count;
        }

        //var movement = Camera.main.transform.TransformDirection(moveInput.x, 0.0f, horizontalOnly ? 0.0f : moveInput.y);
        //movement = moveSpeed * multiplier * Time.deltaTime * Vector3.ProjectOnPlane(movement, Vector3.up).normalized;
        var movement = moveSpeed * multiplier * Time.deltaTime * new Vector3(moveInput.x, 0.0f, horizontalOnly ? 0.0f : moveInput.y).normalized;
        transform.Translate(movement);
    }

    public void SetSpeedMultiplier(float multiplier, float duration)
    {
        speedMultipliers.Add(new(multiplier, Time.time + duration));
    }

    #region lets BlobController use speed boosts while this script is switched off
    // same averaging as Update above, and it clears out boosts that have run out
    public float SpeedMultiplierNow()
    {
        float total = 0f;
        int count = 0;

        for (int i = speedMultipliers.Count - 1; i >= 0; i--)
        {
            if (speedMultipliers[i].EndTime <= Time.time) speedMultipliers.RemoveAt(i);
            else
            {
                total += speedMultipliers[i].Multiplier;
                count++;
            }
        }

        if (count == 0) return 1f;
        return total / count;
    }
    #endregion
}
