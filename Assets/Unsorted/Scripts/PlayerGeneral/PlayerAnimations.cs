using UnityEngine;

[RequireComponent(typeof(BlobController))]
public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] float referenceMaxSpeed = 5.0f;
    [SerializeField] string moveSpeedParameterName = "MoveSpeed";
    [SerializeField] string airborneParameterName = "IsInAir";
    [SerializeField] Animator anim;

    BlobController blob;
    Vector3 prevPos;

    int moveSpeedParameterHash;
    int airborneParameterHash;

    private void Awake()
    {
        blob = GetComponent<BlobController>();

        moveSpeedParameterHash = Animator.StringToHash(moveSpeedParameterName);
        airborneParameterHash = Animator.StringToHash(airborneParameterName);
    }

    private void Update()
    {
        var posDelta = transform.position - prevPos;

        anim.SetFloat(moveSpeedParameterHash, Mathf.Abs(posDelta.x) / (Time.deltaTime * referenceMaxSpeed));
        anim.SetBool(airborneParameterHash, !blob.IsGrounded);

        prevPos = transform.position;
    }
}
