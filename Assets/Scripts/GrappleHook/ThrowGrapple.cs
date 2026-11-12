using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowGrapple : MonoBehaviour
{
    public bool hascharge;
    public float range = 12f;
    public float ropespeed = 40f; 
    public float throwdistance = 6f; 
    public float throwtime = 0.4f;
    public float throwheight = 3f; 

    InputAction use;
    LineRenderer line;

    void Start()
    {
        use = GetComponent<PlayerInput>().actions.FindAction("Interact");

        line = new GameObject("Throw Rope").AddComponent<LineRenderer>();
        line.transform.SetParent(transform, false);
        line.positionCount = 2;
        line.startWidth = 0.12f;
        line.endWidth = 0.12f;
        line.sharedMaterial = GetComponent<LineRenderer>().sharedMaterial; 
        line.enabled = false;
    }

    void Update()
    {
        if (hascharge && use.WasPressedThisFrame())
        {
            hascharge = false;
            StartCoroutine(Throw());
        }
    }

    IEnumerator Throw()
    {
        Vector3 forward = Vector3.right * GetComponent<BlobController>().Facing;
        Vector3 tip = transform.position + Vector3.up;
        BlobController caught = null;
        line.enabled = true;

        for (float travelled = 0f; travelled < range && caught == null; travelled += ropespeed * Time.deltaTime)
        {
            tip += forward * ropespeed * Time.deltaTime;
            caught = PlayerAt(tip);
            Rope(tip);
            yield return null;
        }

        if (caught != null)
        {
            Vector3 start = caught.transform.position;
            Vector3 landing = transform.position - forward * throwdistance;

            for (float time = 0f; time < throwtime; time += Time.deltaTime)
            {
                float done = time / throwtime; 
                Vector3 position = Vector3.Lerp(start, landing, done);
                position.y += Mathf.Sin(done * Mathf.PI) * throwheight;

                caught.ResetTo(position);
                Rope(position + Vector3.up);
                yield return null;
            }
            caught.ResetTo(landing);
        }

        line.enabled = false;
    }

    // if another player touching the rope's tip
    BlobController PlayerAt(Vector3 tip)
    {
        foreach (Collider hit in Physics.OverlapSphere(tip, 0.6f, LayerMask.GetMask("Player")))
        {
            BlobController other = hit.GetComponentInParent<BlobController>();
            if (other != null && other.gameObject != gameObject) return other;
        }
        return null;
    }

    void Rope(Vector3 end)
    {
        line.SetPosition(0, transform.position + Vector3.up);
        line.SetPosition(1, end);
    }
}
