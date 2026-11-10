using UnityEngine;

public interface ISmoothedValue<T>
{
    SmoothedValueConfig Config { get; set; }
    T SmoothedValue { get; set; }
    T TargetValue { get; set; }
    T ReferenceSmoothVelocity { protected get; set; }

    void Smooth(float deltaTime);
}

/// <summary>
/// Data structure for configuring a smoothed value.
/// This can be used to set up the smoothing parameters for different types of values (e.g., float, Vector2, Vector3) in a consistent manner.
/// </summary>
[System.Serializable]
public struct SmoothedValueConfig
{
    public float SmoothTime;
    public float MaxSmoothSpeed;
}

[System.Serializable]
public class SmoothedFloat : ISmoothedValue<float>
{
    public SmoothedFloat()
    {

    }
    public SmoothedFloat(SmoothedValueConfig Config)
    {
        config = Config;
    }

    [SerializeField] private bool isAngle;
    public bool IsAngle
    {
        get => isAngle;
        private set => isAngle = value;
    }

    [SerializeField] SmoothedValueConfig config;
    public SmoothedValueConfig Config
    {
        get => config;
        set => config = value;
    }
    public float SmoothedValue { get; set; }
    public float TargetValue { get; set; }

    float ReferenceSmoothVelocity;
    float ISmoothedValue<float>.ReferenceSmoothVelocity { get => ReferenceSmoothVelocity; set => ReferenceSmoothVelocity = value; }


    public void Smooth(float deltaTime)
    {
        if (IsAngle)
            SmoothedValue = Mathf.SmoothDampAngle(SmoothedValue, TargetValue, ref ReferenceSmoothVelocity, Config.SmoothTime, Config.MaxSmoothSpeed, deltaTime);
        else
            SmoothedValue = Mathf.SmoothDamp(SmoothedValue, TargetValue, ref ReferenceSmoothVelocity, Config.SmoothTime, Config.MaxSmoothSpeed, deltaTime);
    }

    public void SetIsAngle(bool isAngle)
    {
        IsAngle = isAngle;
    }
}

[System.Serializable]
public class SmoothedVector2 : ISmoothedValue<Vector2>
{
    public SmoothedVector2() { }
    public SmoothedVector2(SmoothedValueConfig config) => Config = config;

    public SmoothedValueConfig Config { get; set; }
    public Vector2 SmoothedValue { get; set; }
    public Vector2 TargetValue { get; set; }

    Vector2 ReferenceSmoothVelocity;
    Vector2 ISmoothedValue<Vector2>.ReferenceSmoothVelocity { get => ReferenceSmoothVelocity; set => ReferenceSmoothVelocity = value; }


    public void Smooth(float deltaTime)
    {
        SmoothedValue = Vector2.SmoothDamp(SmoothedValue, TargetValue, ref ReferenceSmoothVelocity, Config.SmoothTime, Config.MaxSmoothSpeed, deltaTime);
    }
}

[System.Serializable]
public class SmoothedVector3 : ISmoothedValue<Vector3>
{
    public SmoothedVector3() { }
    public SmoothedVector3(SmoothedValueConfig config) => Config = config;


    public SmoothedValueConfig Config { get; set; }
    public Vector3 SmoothedValue { get; set; }
    public Vector3 TargetValue { get; set; }

    Vector3 ReferenceSmoothVelocity;
    Vector3 ISmoothedValue<Vector3>.ReferenceSmoothVelocity { get => ReferenceSmoothVelocity; set => ReferenceSmoothVelocity = value; }


    public void Smooth(float deltaTime)
    {
        SmoothedValue = Vector3.SmoothDamp(SmoothedValue, TargetValue, ref ReferenceSmoothVelocity, Config.SmoothTime, Config.MaxSmoothSpeed, deltaTime);
    }
}

[System.Serializable]
public class SmoothedQuaternion : ISmoothedValue<Quaternion>
{
    public SmoothedQuaternion() { }
    public SmoothedQuaternion(SmoothedValueConfig config) => Config = config;

    public SmoothedValueConfig Config { get; set; }
    public Quaternion SmoothedValue { get; set; }
    public Quaternion TargetValue { get; set; }

    Quaternion ReferenceSmoothVelocity;
    Quaternion ISmoothedValue<Quaternion>.ReferenceSmoothVelocity { get => ReferenceSmoothVelocity; set => ReferenceSmoothVelocity = value; }

    public void Smooth(float deltaTime)
    {
        // First time trying this with Quaternions, unsure if it'll work.
        SmoothedValue = new Quaternion(
            Mathf.SmoothDamp(SmoothedValue.x, TargetValue.x, ref ReferenceSmoothVelocity.x, Config.SmoothTime, Config.MaxSmoothSpeed, deltaTime),
            Mathf.SmoothDamp(SmoothedValue.y, TargetValue.y, ref ReferenceSmoothVelocity.y, Config.SmoothTime, Config.MaxSmoothSpeed, deltaTime),
            Mathf.SmoothDamp(SmoothedValue.z, TargetValue.z, ref ReferenceSmoothVelocity.z, Config.SmoothTime, Config.MaxSmoothSpeed, deltaTime),
            Mathf.SmoothDamp(SmoothedValue.w, TargetValue.w, ref ReferenceSmoothVelocity.w, Config.SmoothTime, Config.MaxSmoothSpeed, deltaTime)
        );

        // Normalize to ensure the result is actually a rotation
        SmoothedValue.Normalize();
    }
}
