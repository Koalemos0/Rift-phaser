using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;
    private Grappling grappling;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    private bool enableSlideOnNextTouch;

    public Vector3 inputDirection;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        grappling = GetComponent<Grappling>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && pm.grounded)
            StartSlide();
        if (Input.GetKeyDown(slideKey) && !pm.grounded)
            StartLateSlide();
        if (Input.GetKeyUp(slideKey) && pm.sliding || Input.GetKeyDown(grappling.grappleKey) || (horizontalInput == 0 && verticalInput == 0))
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
        {
            SlidingMovement();
        }
    }

    private void GetSlideDirection()
    {
        inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    }

    //late slide is when slide input pressed while not grounded => start slide on next collision with ground
    private void StartLateSlide()
    {
        enableSlideOnNextTouch = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableSlideOnNextTouch)
        {
            enableSlideOnNextTouch = false;

            StartSlide();
        }
    }

    private void StartSlide()
    {
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;

        //comment this if slide should not be staight
        //GetSlideDirection();
    }

    private void SlidingMovement()
    {
        //comment this if slide should be straight
        GetSlideDirection();

        // sliding normal
        if (!pm.OnSlope() || rb.linearVelocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        // sliding down a slope
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0 && maxSlideTime != 0)
            StopSlide();
    }

    private void StopSlide()
    {
        pm.sliding = false;

        //revert player model to normal scale
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }
}
