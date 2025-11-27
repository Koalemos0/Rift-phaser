using UnityEngine;

public class GroundSlam : MonoBehaviour
{
    [Header("References")]
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Ground Slam")]
    public float slamStrength;

    public bool slamWasStarted;

    public float minDistance;
    private RaycastHit heightCheck;
    public float slamDelayTime;

    [Header("Input")]
    public KeyCode groundSlamKey = KeyCode.LeftControl;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(groundSlamKey) && ReadyToStartGroundSlam() && !pm.groundSlam)
        {
            StartGroundSlam();
        }
    }

    private void FixedUpdate()
    {
        if (pm.groundSlam)
        {
            GroundSlamAddForce();
        }
    }

    private bool ReadyToStartGroundSlam()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out heightCheck, minDistance)) // does a raycast and checks if player is above a certain height, return false if not
            return false;
        else
            return true;
    }

    private void StartGroundSlam()
    {
        pm.freeze = true;

        slamWasStarted = true;

        Invoke(nameof(ExecuteSlamAfterDelay), slamDelayTime);
    }

    private void ExecuteSlamAfterDelay()
    {
        pm.groundSlam = true;

        pm.freeze = false;
    }

    private void GroundSlamAddForce()
    {
        rb.AddForce(Vector3.down.normalized * slamStrength, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (slamWasStarted)
        {
            slamWasStarted = false;

            StopGroundSlam();
        }
    }

    private void StopGroundSlam()
    {
        pm.groundSlam = false;

        pm.freeze = false;
    }
}
