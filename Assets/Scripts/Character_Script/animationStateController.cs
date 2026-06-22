using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isJoggingHash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        isJoggingHash = Animator.StringToHash("isJogging");
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isJogging = animator.GetBool(isJoggingHash);
        bool forwardPressed = Input.GetKey("w");
        if (!isJogging && forwardPressed)
        {
            //then isJogging activates
            animator.SetBool(isJoggingHash, true);
        }

        if (isJogging && !forwardPressed)
        {
            //then isJogging deactivates
            animator.SetBool(isJoggingHash, false);
        }
    }
}
