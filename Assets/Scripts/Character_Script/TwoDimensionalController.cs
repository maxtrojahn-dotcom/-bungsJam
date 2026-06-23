using UnityEngine;

public class TwoDimensionalController : MonoBehaviour
{
    Animator animator;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey("w");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");
        bool runPressed = Input.GetKey("left shift");

        if (forwardPressed)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        if (leftPressed)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        if (rightPressed)
        {
            velocityX += Time.deltaTime * acceleration;
        }
    }
}
