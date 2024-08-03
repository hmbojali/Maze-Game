using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class PlayerControlMiniGame : MonoBehaviour
{
    [SerializeField] private float sensitivity;
    [SerializeField] private GameObject GroundTrigger;
    [SerializeField] private Camera POV;
    [SerializeField] private Camera ThirdPerson;
    [SerializeField] private Camera RealCam;
    [SerializeField] private GameObject Moon;

    private int miniMapDepth = -1;
    private Vector3 RealCamPos;

    private Mesh meshCube;
    private Mesh meshSphere;
    private Mesh meshCapsule;
    private Rigidbody rigidbody;

    private int cycleNum = 1;
    [HideInInspector] public bool rolling = false;
    private Vector3 currentVelocity = Vector3.zero;
    private float currentSpeed = 0;
    [SerializeField] private float jumpForce = 7f;
    private Vector3 jumpVector;
    private float rollingSpeed = 10f;
    public float movingSpeed = 3f;
    private Vector3 centerOfMassOffset = Vector3.down * 0.5f;
    [HideInInspector] public bool isGrounded;

    public Vector3[] n = new Vector3[3];
    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        meshCube = temp.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(temp);

        temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        meshSphere = temp.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(temp);

        temp = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        meshCapsule = temp.GetComponent<MeshFilter>().mesh;
        DestroyImmediate(temp);

        //meshCapsule = GetComponentsInChildren<MeshFilter>()[3].mesh;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.centerOfMass = Vector3.down * 0.1f;

        RealCamPos = RealCam.transform.position;
        RealCamPos.z += 20;

    }

    // Update is called once per frame
    void Update()
    {
        ShapeShift();
        if (miniMapDepth == -1)
        {
            Cam();
            RealCam.enabled = false;
        }
        else
        {
            RealCam.enabled = true;
            POV.enabled = false;
        }
        MiniMap();
        RealCamUpdate();
        Jumping();
        rolling = Input.GetKeyDown(KeyCode.KeypadPlus) ? !rolling : rolling;
        if (rolling)
            Rolling(rollingSpeed);

    }
    private void FixedUpdate()
    {
        if (!rolling)
        {
            Moving(movingSpeed);
        }
    }

    private void Jumping()
    {
        //jumping
        bool jump = Input.GetKeyDown(KeyCode.Space);
        if (jump && isGrounded)
        {
            rigidbody.AddForce(jumpVector * jumpForce, ForceMode.VelocityChange);
        }
    }

    private void Moving(float speed)
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;

        float sideways = Input.GetAxis("Horizontal") * speed;

        Vector3 movement = (transform.right * sideways) * Time.deltaTime;

        float acceleration = 1;
        currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);
        currentVelocity = movement * currentSpeed;

        rigidbody.MovePosition(transform.position + currentVelocity);

        jumpVector = Vector3.up;
        //transform.position += currentVelocity;     
    }

    //old rolling func

    public void Rolling(float speed)
    {
        float forward = Input.GetAxis("Vertical") * speed;
        float sideways = Input.GetAxis("Roll") * speed / 2;
        float around = Input.GetAxis("Horizontal") * speed / 2;

        Vector3 torque = Vector3.forward * -sideways + Vector3.right * forward + Vector3.up * around;

        // Calculate the center of mass relative to the object's position and rotation
        Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;
        Vector3 centerOfMass = worldToLocalMatrix.MultiplyPoint(transform.position + centerOfMassOffset);
        rigidbody.centerOfMass = centerOfMass;
        n[0] = rigidbody.centerOfMass;
        n[1] = rigidbody.worldCenterOfMass;
        n[2] = centerOfMass;

        //removing rotation constraints
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        rigidbody.AddRelativeTorque(torque, ForceMode.Acceleration);

        //jumping
        bool jump = Input.GetKeyDown(KeyCode.Space);
        if (jump && isGrounded)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(jumpVector * jumpForce, ForceMode.Impulse);
        }
    }
    private void Cam()
    {
       ThirdPerson.enabled = Input.GetKeyDown(KeyCode.Mouse0)? !ThirdPerson.enabled: ThirdPerson.enabled;
       POV.enabled = !ThirdPerson.enabled;
    }

    private Vector2 prevMousePos = Vector2.zero;
    private Vector2 mousePosDelta = Vector2.zero;
    private void MiniMap()
    {
        miniMapDepth = Input.GetKeyDown(KeyCode.Mouse2) ? -miniMapDepth : miniMapDepth;
        RealCam.depth = miniMapDepth;
        RealCam.orthographicSize -= Input.mouseScrollDelta.y * RealCam.orthographicSize / 10;
        if (RealCam.orthographicSize > 250)
            RealCam.orthographicSize = 249;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            mousePosDelta = prevMousePos - (Input.mousePosition.x * Vector2.right + Input.mousePosition.y * Vector2.up);

            RealCam.transform.position += (mousePosDelta.x * Vector3.right + mousePosDelta.y * Vector3.forward) * RealCam.orthographicSize / 275;
        }
        prevMousePos = Input.mousePosition.x * Vector2.right + Input.mousePosition.y * Vector2.up;
    }

    private void RealCamUpdate()
    {
        Vector3 temp = (transform.position - RealCamPos);//delta
        float deltaY = temp.y < -5 ? temp.y : -5;
        float devider = 1.5f;
        RealCam.transform.position = RealCamPos + (Vector3.right * temp.x)/devider;
        temp = RealCam.transform.position;//current pos
        temp.y = transform.position.y-deltaY/2;
        RealCam.transform.position = temp;
    }

    private void ShapeShift()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            if (cycleNum == 0)
            {
                GetComponent<MeshFilter>().mesh = meshCube;
                GroundTrigger.transform.localPosition += Vector3.up * 0.45f;
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<CapsuleCollider>().enabled = false;

                centerOfMassOffset = Vector3.down * 0.5f;
                rollingSpeed /= 2;
                rigidbody.maxAngularVelocity = 7;
            }
            else if (cycleNum == 1)
            {
                GetComponent<MeshFilter>().mesh = meshSphere;
                GroundTrigger.transform.localPosition -= Vector3.up * 0.05f;
                GetComponent<BoxCollider>().enabled = false;
                GetComponent<SphereCollider>().enabled = true;
                GetComponent<CapsuleCollider>().enabled = false;

                centerOfMassOffset = Vector3.zero;
                rollingSpeed *= 4;
                rigidbody.maxAngularVelocity = 14;
            }
            else
            {
                GetComponent<MeshFilter>().mesh = meshCapsule;
                GroundTrigger.transform.localPosition-=Vector3.up*0.4f;
                GetComponent<BoxCollider>().enabled = false;
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<CapsuleCollider>().enabled = true;
                cycleNum = -1;

                centerOfMassOffset = Vector3.zero;
                rollingSpeed /= 2;
                rigidbody.maxAngularVelocity = 14;
            }
            cycleNum++;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(rolling)
        {
            isGrounded = true;

            Vector3 avgContactPoint=Vector3.zero;
            foreach(ContactPoint contact in collision.contacts)
            {
                avgContactPoint += contact.point;
            }
            avgContactPoint = avgContactPoint / collision.contactCount;
            jumpVector = transform.position - avgContactPoint;
            jumpVector = jumpVector.normalized;
        }
        else if (!isGrounded)
        {
            Vector3 avgContactPoint = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts)
            {
                avgContactPoint += contact.point;
            }
            avgContactPoint = avgContactPoint / collision.contactCount;
            if (avgContactPoint.y > GroundTrigger.transform.position.y + 0.1f)
            {
                movingSpeed = 0.5f;
                print("Wall");
            }
        }
        else
        {
            Vector3 avgContactPoint = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts)
            {
                avgContactPoint += contact.point;
            }
            avgContactPoint = avgContactPoint / collision.contactCount;
            if (avgContactPoint.y > GroundTrigger.transform.position.y + 0.1f)
            {
                print("WallGround");
                movingSpeed = 0.5f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded)
        {
            Vector3 avgContactPoint = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts)
            {
                avgContactPoint += contact.point;
            }
            avgContactPoint = avgContactPoint / collision.contactCount;
            if (avgContactPoint.y > GroundTrigger.transform.position.y + 0.1f)
            {
                movingSpeed = 0;
                print("Wall");
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (rolling)
        {
            isGrounded = false;
            jumpVector = Vector3.zero;
        }
        else
        {
            movingSpeed = 3;
        }
    }
}
