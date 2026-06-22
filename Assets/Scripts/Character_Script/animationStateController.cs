using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    int isJoggingHash;
    int isRunningHash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        isJoggingHash = Animator.StringToHash("isJogging");
        isRunningHash = Animator.StringToHash("isRunning");

    }

    // Update is called once per frame
    void Update()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isJogging = animator.GetBool(isJoggingHash);
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");

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


        //press shift to run
        if (!isRunning && (forwardPressed && runPressed))
        {
            animator.SetBool(isRunningHash, true);
        }

        if (isRunning && (!forwardPressed || !runPressed))
        {
            animator.SetBool(isRunningHash, false);
        }
    }
}