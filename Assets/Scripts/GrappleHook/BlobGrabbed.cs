using UnityEngine;

public partial class BlobController
{
    float grabbedtimer;

    public bool IsGrabbed 
    { 
        get 
        { 
            return grabbedtimer > 0f; 
        }
    }

    public void Grab(float seconds)
    {
        grabbedtimer = Mathf.Max(grabbedtimer, seconds);
        velocity = Vector3.zero;
        hook.Cancel();
    }

    //  while grabbed counts the grab down
    bool StillGrabbed(float seconds)
    {
        if (grabbedtimer <= 0f) return false;

        grabbedtimer -= seconds;
        lastposition = transform.position;
        return true;
    }
}
