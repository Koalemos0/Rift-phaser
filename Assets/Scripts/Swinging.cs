using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("References")]
    public LayerMask whatIsGrappleable;
    private LineRenderer lr;
    public Transform gunTip, player;
    new public Transform camera;
    public PlayerMovement pm;

    [Header("Swinging")]
    private Vector3 swingPoint;
    private float maxDistance = 25f;
    private SpringJoint joint;
    public float spring, damper, massScale;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereRadius;
    public Transform predictionPoint;

    [Header("Input")]
    public KeyCode startStopGrapple = KeyCode.Mouse1;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(startStopGrapple) && !pm.groundSlam)
        {
            StartSwing();
        }
        else if (Input.GetKeyUp(startStopGrapple))
        {
            StopSwing();
        }
        CheckForSwingPoints();
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    void StartSwing()
    {
        if (predictionHit.point == Vector3.zero) return;

        pm.swinging = true;

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        //The distance grapple will try to keep from grapple point. 
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        //Adjust these values
        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;

    }

    void StopSwing()
    {
        pm.swinging = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return swingPoint;
    }

    public void CheckForSwingPoints()
    {
        if (joint != null) return;
        RaycastHit sphereCastHit;
        Physics.SphereCast(camera.position, predictionSphereRadius, camera.forward, out sphereCastHit, maxDistance, whatIsGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(camera.position, camera.forward, out raycastHit, maxDistance, whatIsGrappleable);

        Vector3 realHitPoint;

        // direct hit
        if (raycastHit.point != Vector3.zero)
        {
            realHitPoint = raycastHit.point;
        }
        // predicted hit (indirect)
        else if (sphereCastHit.point != Vector3.zero)
        {
            realHitPoint = sphereCastHit.point;
        }
        // miss
        else
        {
            realHitPoint = Vector3.zero;
        }

        //real hit point found
        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        //real hit point not found
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

}
