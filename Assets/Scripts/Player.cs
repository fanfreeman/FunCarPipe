using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public PipeSystem pipeSystem;
    
    public float floatingHeight = 1f;

    public float steeringForce = 20f;

    //public GameObject vehicleObject;

    [HideInInspector]
    public Pipe currentPipe; // the current pipe the player is traveling in
    [HideInInspector]
    public Pipe prevPipe; // the pipe the player has just traveled through

    private Vector3 centerTrackPointPosition = Vector3.zero;
    private Vector3 centerTrackPointDirection = Vector3.zero;
    private float progress = 0;

    private float progressDelta;

    [HideInInspector]
    public Quaternion accumulatedYRot = Quaternion.identity;

    private Rigidbody avatarRigidbody;

    private const float Gravity = -9.81f;

    private void Awake()
    {
        avatarRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        currentPipe = pipeSystem.SetupInitialPipes();
        prevPipe = pipeSystem.GetVeryFirstPipe();

        currentPipe.GetPlaneOfCurve(
                transform.TransformPoint(transform.position),
                ref centerTrackPointDirection,
                ref centerTrackPointPosition,
                ref progress
        );
    }

    void FixedUpdate()
    {
        currentPipe.GetPlaneOfCurve(
                transform.position,
                ref centerTrackPointDirection,
                ref centerTrackPointPosition,
                ref progress
        );

        if (progress >= 1) currentPipe = pipeSystem.SetupNextPipe();
        //if (progress >= 1) currentPipe = currentPipe.nextPipe;

        // update vehicle direction
        Vector3 upVector = GetUpVector();

        // method 1
        //Quaternion vehicleRotation = Quaternion.LookRotation(centerTrackPointDirection, upVector);

        // method 2
        float vehicleHeadingElevation = Math3d.AngleVectorPlane(centerTrackPointDirection, upVector);
        Quaternion restrictedVehicleRotation = Quaternion.AngleAxis(vehicleHeadingElevation, Vector3.left);

        // method 3
        //Quaternion rotation = Quaternion.FromToRotation(vehicleObject.transform.forward, centerTrackPointDirection);
        //Vector3 vehicleRotationEuler = rotation.eulerAngles;
        //Debug.Log(vehicleRotationEuler.y);
        //vehicleRotationEuler.y = 0;
        //Quaternion restrictedVehicleRotation = Quaternion.Euler(vehicleRotationEuler);

        //
        //Vector3 rotatedForward = restrictedVehicleRotation * vehicleObject.transform.forward;
        //Quaternion vehicleRotation = Quaternion.LookRotation(rotatedForward, upVector);
        //vehicleRotation *= accumulatedYRot;
        //vehicleObject.transform.rotation = Quaternion.RotateTowards(vehicleObject.transform.rotation, vehicleRotation, Time.deltaTime * 300f);

        // apply force to move forward
        //avatarRigidbody.AddForce(centerTrackPointDirection.normalized * 10f, ForceMode.Force);
        //avatarRigidbody.AddForce(vehicleObject.transform.forward.normalized * 10f, ForceMode.Force);

        float currentPipeRadius = currentPipe.GetPipeRadiusByProgress(progress);

        // apply force to make avatar stick to wall
        //float magnitudeModifier = (currentPipeRadius - upVector.magnitude + 1f) * 10f;
        //avatarRigidbody.AddForce(-upVector * 20f / currentPipeRadius, ForceMode.Acceleration);

        //avatarRigidbody.AddForce(upVector.normalized * Gravity, ForceMode.Acceleration);
        //Debug.Log(avatar.GetComponent<Rigidbody>().velocity.magnitude);
        //if (upVector.magnitude < currentPipeRadius) avatarRigidbody.AddForce(new Vector3(0, -9.81f, 0), ForceMode.Acceleration);

        //// hover
        //Ray ray = new Ray(avatar.transform.position, -avatar.transform.up);
        //RaycastHit hit;
        //float hoverHeight = 1.0f;
        //float hoverForce = 20f;
        //if (Physics.Raycast(ray, out hit, hoverHeight))
        //{
        //    float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
        //    Vector3 appliedHoverForce = upVector.normalized * proportionalHeight * hoverForce;
        //    avatar.GetComponent<Rigidbody>().AddForce(appliedHoverForce, ForceMode.Acceleration);
        //}

        // update avatar turning according to user input
        //UpdateAvatarRotation(centerTrackPointDirection, centerTrackPointPosition);
    }
    
    private Vector3 coolVehicleLookAtVelocity;
    private Vector3 coolVehicleLookAtUpVelocity;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            // draw center track point
            Gizmos.DrawSphere(centerTrackPointPosition, 0.3f);

            // draw center track point direction
            Gizmos.DrawLine(transform.position, transform.position + centerTrackPointDirection * 6f);

            // draw up vector
            //Gizmos.DrawLine(avatar.transform.position, avatar.transform.position - GetUpVector() * 6f);

            // draw avatar plane
            //Gizmos.color = Color.blue;
            //Gizmos.DrawLine(centerTrackPointPosition, currentPipe.transform.TransformPoint(Vector3.zero));
            //Gizmos.DrawLine(centerTrackPointPosition, avatar.transform.position);
            //Gizmos.DrawLine(avatar.transform.position, currentPipe.transform.TransformPoint(Vector3.zero));

            // draw spin force vectors
            //Gizmos.color = Color.red;
            //Gizmos.DrawLine(avatar.transform.position, avatar.transform.position + forceL * 4f);
            //Gizmos.DrawLine(avatar.transform.position, avatar.transform.position - forceR * 4f);
        }
    }

    //撞上兔子后的效果
    public void BoomByRabbitEffect()
    {
        Debug.Log("车打兔子");

        //先减速
        avatarRigidbody.velocity = Vector3.zero;
        avatarRigidbody.AddForce(centerTrackPointDirection.normalized * -5f, ForceMode.VelocityChange);
        //推上天
        Vector3 pushUp = GetUpVector().normalized;
        avatarRigidbody.AddForce(pushUp * 2f, ForceMode.VelocityChange);
        //让车失控然后转
        BlockCarMovement(1.5f);
    }

    //让车失控一会儿
    private float blockCarMovement = 0;
    public void BlockCarMovement(float time){
        blockCarMovement = time;
    }

    public Vector3 GetUpVector()
    {
        return centerTrackPointPosition - transform.position;
    }

    private void UpdateAvatarRotation(Vector3 forceDirection,Vector3 curvePoint)
    {
        float rotationInput = 0f;
        float accelerationInput = 0f;
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                Vector3 goAheadVector = transform.position + forceDirection;
                Vector3 normal = new Vector3();
                Vector3 temp = new Vector3();
                //为了让力能垂直于曲线和车 根据3点计算出左右力坐在平面的normal
                Math3d.PlaneFrom3Points(out normal,out temp, goAheadVector, curvePoint, transform.position);

                if (Input.GetTouch(0).position.x < Screen.width * 0.5f)
                {
                    avatarRigidbody.AddForce(-normal * steeringForce, ForceMode.Acceleration);
                }
                else {
                    avatarRigidbody.AddForce(normal * steeringForce, ForceMode.Acceleration);
                }
            }
        }
        else { // not mobile platform
            // left and right movement
            rotationInput = Input.GetAxis("Horizontal");
            Vector3 goAheadVector = transform.position + forceDirection;
            Vector3 normal = new Vector3();
            Vector3 temp = new Vector3();
            //为了让力能垂直于曲线和车 根据3点计算出左右力坐在平面的normal
            Math3d.PlaneFrom3Points(out normal, out temp, goAheadVector, curvePoint, transform.position);
            //if (rotationInput > 0)
            //{
            //    avatarRigidbody.AddForce(-normal * steeringForce, ForceMode.Acceleration);
            //    forceL = -normal;
            //}
            //else if (rotationInput < 0)
            //{
            //    avatarRigidbody.AddForce(normal * steeringForce, ForceMode.Acceleration);
            //    forceR = normal;
            //}

            // acceleration
            accelerationInput = Input.GetAxis("Vertical");
            avatarRigidbody.AddForce(forceDirection * 5f * accelerationInput, ForceMode.Acceleration);

            // turning
            float yRot = Input.GetAxis("Horizontal") * 3f;
            accumulatedYRot = Quaternion.Euler(0f, yRot, 0f);
        }
    }
    private Vector3 forceL = Vector3.one;
    private Vector3 forceR = Vector3.one;

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetCenterTrackHookPosition()
    {
        return centerTrackPointPosition;
    }

    public Vector3 GetCenterTrackPointDirection()
    {
        return centerTrackPointDirection;
    }

    public Vector3 GetAvatarPosition()
    {
        return transform.position;
    }
}
