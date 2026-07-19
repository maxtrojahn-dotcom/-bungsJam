using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{

    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;


    //variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;
    float rotationFactorPerFrame = 1.0f;

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInput.CharacterControls.Move.started += onMovementInput;

        //cancel movement
        playerInput.CharacterControls.Move.canceled += onMovementInput;

        //update changes
        playerInput.CharacterControls.Move.performed += onMovementInput;
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
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);
        }
    }


    void onMovementInput (InputAction.CallbackContext context)

    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if (isMovementPressed && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }

        else if (!isMovementPressed && isWalking)
        {
            animator.SetBool("isWalking", false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();
        characterController.Move(currentMovement * Time.deltaTime);
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
