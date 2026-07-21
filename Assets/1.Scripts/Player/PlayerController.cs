using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform cam;
    public float moveSpeed = 5f;

    [Range(0.02f, 0.3f)]
    public float turnSmoothTime = 0.08f;

    // Animator-Damping 
    public float locomotionParameterDamping = 0.1f;

    // --- JUMP ---
    [Header("Jump Settings")]
    public float jumpForce = 6f;
    public float gravity = -20f;
    private float verticalVelocity;
    public float airMoveMultiplier = 0.5f;

    // --- Dodge Roll ---
    [Header("Dodge Roll Settings")]
    public float dodgeRollSpeed = 8f;
    public float dodgeRollDuration = 0.5f;
    public float dodgeRollCooldown = 1f;
    private bool isDodging;


    private CharacterController cc;
    private Animator animator;
    private float turnSmoothVelocity;

    // Animator Hashes 

    private int speedHash;
    private int isMovingHash;
    private int jumpHash;
    private int isGroundedHash;
    private int isDodgeRoll;

    //pause Munu



    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        if (!cam && Camera.main) cam = Camera.main.transform;

        speedHash = Animator.StringToHash("speed");
        isMovingHash = Animator.StringToHash("isMoving");
        jumpHash = Animator.StringToHash("Jump");
        isGroundedHash = Animator.StringToHash("isGrounded");
        isDodgeRoll = Animator.StringToHash("isDodgeRoll");
    }

    private void Update()
    {
        // --- GROUND CHECK ---
        bool grounded = cc.isGrounded;

        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        if (animator)
            animator.SetBool(isGroundedHash, grounded);

        // --- JUMP INPUT ---
        if (grounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = jumpForce;
            animator.SetTrigger(jumpHash);
        }

        // --- INPUT  ---
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(x, 0f, z).normalized;
        bool moving = input.sqrMagnitude > 0.01f;

        // --- DODGE ROLL ---
        //dodge roll if i press C while moving on ground
        if (grounded && moving && Input.GetKeyDown(KeyCode.C) && !isDodging)
        {
            StartCoroutine(DodgeRoll(input));
            
        }

        // --- ANIMATOR ---
        if (animator)
        {
            animator.SetBool(isMovingHash, moving);
            animator.SetFloat(
                speedHash,
                Mathf.Clamp01(input.magnitude),
                locomotionParameterDamping,
                Time.deltaTime
            );
        }
       

        if (!moving)
        {
            cc.SimpleMove(Vector3.zero);
        }
        else
        {
            // --- ROTATION + MOVEMENT  ---
            float targetAngle =
                Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                turnSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, cam.eulerAngles.y, 0f) * input;


            if (grounded) cc.SimpleMove(moveDir * moveSpeed);
            else cc.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        // --- GRAVITY ---
        verticalVelocity += gravity * Time.deltaTime;
        cc.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private IEnumerator DodgeRoll(Vector3 input)
    {
        isDodging = true;
        animator.SetBool(isDodgeRoll, true);

        float startTime = Time.time;
        while (Time.time < startTime + dodgeRollDuration)
        {
            Vector3 rollDir = Quaternion.Euler(0f, cam.eulerAngles.y, 0f) * input;
            cc.Move(rollDir * dodgeRollSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool(isDodgeRoll, false);

        yield return new WaitForSeconds(dodgeRollCooldown);
        isDodging = false;
    }


}
