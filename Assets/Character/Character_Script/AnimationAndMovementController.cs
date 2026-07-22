using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{

    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    //optimization
    int isWalkingHash;
    int isRunningHash;

    //variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;

    //constants
    float rotationFactorPerFrame = 15.0f;
    float runMultiplier = 3.0f;
    int zero = 0;

    //gravity variable
    float gravity = -9.8f;
    float groundedGravity = -2f;

    //jump variable
    bool isJumpPressed = false;
    float initialJumpVelocity;
    float maxJumpHeight = 4.0f;
    float maxJumpTime = 0.75f;
    bool isJumping = false;
    int isJumpingHash;
    int JumpCountHash;
    bool isJumpAnimating = false;
    int jumpCount = 0;
    Dictionary<int, float> initialJumpVelocities = new Dictionary<int, float>();
    Dictionary<int, float> jumpGravities = new Dictionary<int, float>();
    Coroutine currentJumpResetRoutine = null;

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        JumpCountHash = Animator.StringToHash("jumpCount");

        //start walking
        playerInput.CharacterControls.Move.started += onMovementInput;

        //cancel walking
        playerInput.CharacterControls.Move.canceled += onMovementInput;

        //update changes
        playerInput.CharacterControls.Move.performed += onMovementInput;

        //run
        playerInput.CharacterControls.Run.started += onRun;

        //stop running
        playerInput.CharacterControls.Run.canceled += onRun;

        playerInput.CharacterControls.Jump.started += onJump;

        playerInput.CharacterControls.Jump.canceled += onJump;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2f;

        gravity = (-2f * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2f * maxJumpHeight) / timeToApex;

        float secondJumpGravity = (-2f * (maxJumpHeight + 2f)) / Mathf.Pow(timeToApex * 1.25f, 2);
        float secondJumpInitialVelocity = (2f * (maxJumpHeight + 2f)) / (timeToApex * 1.25f);
        float thirdJumpGravity = (-2f * (maxJumpHeight + 4f)) / Mathf.Pow(timeToApex * 1.5f, 2);
        float thirdJumpInitialVelocity = (2f * (maxJumpHeight + 4f)) / (timeToApex * 1.5f);

        // Store jump velocities
        initialJumpVelocities[1] = initialJumpVelocity;
        initialJumpVelocities[2] = secondJumpInitialVelocity;
        initialJumpVelocities[3] = thirdJumpInitialVelocity;

        // Store gravities
        jumpGravities[0] = gravity;
        jumpGravities[1] = gravity;
        jumpGravities[2] = secondJumpGravity;
        jumpGravities[3] = thirdJumpGravity;
    }


    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            if (jumpCount < 3 && currentJumpResetRoutine != null)
            {
                StopCoroutine(currentJumpResetRoutine);
            }
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            jumpCount += 1; 
            animator.SetInteger(JumpCountHash, jumpCount);
            currentMovement.y = initialJumpVelocities[jumpCount] * .5f;
            currentRunMovement.y = initialJumpVelocities[jumpCount] * .5f;
        }

       else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }

    }

    IEnumerator jumpResetRoutine()
    {
        yield return new WaitForSeconds(.5f);
        jumpCount = 0;
        animator.SetInteger(JumpCountHash, jumpCount);
    }


    void onJump (InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        Debug.Log(isJumpPressed);
    }

    void onRun (InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }


    void onMovementInput (InputAction.CallbackContext context)

    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentRunMovement.x * runMultiplier;
        currentRunMovement.z = currentRunMovement.z * runMultiplier;
        isMovementPressed = currentMovementInput.x != zero || currentMovementInput.y != zero;
    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }

        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool("isWalking", false);
        }

        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }

        else if ((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }

    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed; 
        float fallMultiplier = 2.0f;

        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
            animator.SetBool(isJumpingHash, false);
            isJumpAnimating = false;
            currentJumpResetRoutine = StartCoroutine(jumpResetRoutine());

            if (jumpCount == 3)
            {
             jumpCount = 0;
             animator.SetInteger(JumpCountHash, jumpCount);

                }

            }

            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
            
        }

        else if (isFalling)

        {
            float previouslyYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (jumpGravities[jumpCount] * fallMultiplier * Time.deltaTime);
            float nextYVelocity = Mathf.Max ((previouslyYVelocity + newYVelocity) * .5f, -20f);
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;

        }

        else

        {
            float previouslyYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (jumpGravities[jumpCount] * Time.deltaTime);
            float nextYVelocity = (previouslyYVelocity + newYVelocity) * .5f;
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }

    }


    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();

        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }

        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
        handleGravity();
        handleJump();
       
    }

    void OnEnable()
    {
        //enable character control action map
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        //disable character control action map
        playerInput.CharacterControls.Disable();
    }
}
