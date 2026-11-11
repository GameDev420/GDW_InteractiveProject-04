using UnityEngine;

public partial class BlobController
{
    #region always running: settings
    public bool alwaysrun = true; 
    public float runspeed = 5f; 
    public int startdirection = 1; 
    #endregion

    int rundirection; 
    int lasttap;

    bool RunHasStarted()
    {
        if (GameManager.Instance == null) return true;
        return GameManager.Instance.GameStarted;
    }

    void PickRunDirection(float movex)
    {
        if (rundirection == 0 && RunHasStarted()) rundirection = startdirection;

        int tap = 0;
        if (movex > 0f) tap = 1;
        if (movex < 0f) tap = -1;

        if (tap == 0)
        {
            lasttap = 0;
        }
        else if (tap != lasttap && CanSteer() && rundirection != 0)
        {
            lasttap = tap;
            rundirection = tap;
        }

        if (rundirection != 0)
        {
            facing = rundirection;
            heldx = rundirection;
        }
    }

    Vector3 RunStep(float seconds, float race)
    {
        float boost = 1f;
        if (walking != null) boost = walking.SpeedMultiplierNow();

        return Vector3.right * (rundirection * runspeed * boost * race * walkcontrol * seconds);
    }
}
