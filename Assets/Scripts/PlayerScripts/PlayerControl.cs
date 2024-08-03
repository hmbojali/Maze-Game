using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float sensitivity;
    [SerializeField] private GameObject GroundTrigger;
    [SerializeField] private Camera ThirdPerson;
    [SerializeField] private Camera POV;
    [SerializeField] private Camera MiniMapCam;           

    private int miniMapDepth = -1;

    private Mesh meshCube;
    private Mesh meshSphere;
    private Mesh meshCapsule;
    [HideInInspector] public Rigidbody rigidbody;

    private int cycleNum = 1;
    [HideInInspector] public bool rolling = false;
    private Vector3 currentVelocity = Vector3.zero;
    private float currentSpeed = 0;
    [SerializeField] private float jumpForce = 7f;
    private Vector3 jumpVector;
    [SerializeField] private float rollingForce = 10f;
    [HideInInspector] public float movingSpeed;
    public float MOVING_SPEED = 3;
    private Vector3 centerOfMassOffset = Vector3.down * 0.5f;
    public bool isGrounded;
    private float Ydirection; //represents the y rotation

    [SerializeField] private Vector3[] n = new Vector3[3];
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;

        movingSpeed = MOVING_SPEED;

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

        initial3rdCamRot = ThirdPerson.transform.parent.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        ShapeShift();
        if (miniMapDepth == -1)
        {
            Cam();
            MiniMapCam.enabled = false;
        }
        else
        {
            MiniMapCam.enabled = true;
            ThirdPerson.enabled = false;
            POV.enabled = false;
        }
        MiniMap();
        Jumping();
        rolling = Input.GetKeyDown(KeyCode.KeypadPlus) ? !rolling : rolling;
        if (!rolling)
        {
            if (!MoveCam())
                Turning();
        }
        else
            Rolling(rollingForce);



        prevMousePos = Input.mousePosition.x * Vector2.right + Input.mousePosition.y * Vector2.up;
    }
    private void FixedUpdate()
    {
        if (!rolling)
        {
            Moving(movingSpeed);
        }
    }

    private void Turning()
    {
        //getting mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * sensitivity;

        

        //rotating the player horizontally
        transform.Rotate(Vector3.up, mouseX);

        //making sure player can only look up and down on the vertical axis
        float delta = Vector3.Angle(transform.up, POV.transform.forward);
        float postDelta = delta + mouseY;

        if (delta < 5 || delta > 175)
        {
            mouseY = 0;
        }
        else if (postDelta < 5 || postDelta > 175)
            mouseY = 0;

        //rotating the POV camera vertically
        POV.transform.Rotate(Vector3.right* mouseY, Space.Self);

        Ydirection = POV.transform.rotation.eulerAngles.y;
    }

    public void Jumping()
    {
        //jumping
        bool jump = Input.GetKeyDown(KeyCode.Space);
        if (jump && isGrounded)
        {
            rigidbody.AddForce(jumpVector * jumpForce, ForceMode.VelocityChange);
        }
    }

    private Vector3 movement;
    private void Moving(float speed)
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        transform.rotation=Quaternion.Euler(0, Ydirection, 0);

        speed *= Time.fixedDeltaTime;

        float forward = Input.GetAxis("Vertical") * speed;
        float sideways = Input.GetAxis("Horizontal") * speed/2;

        movement = (transform.forward * forward + transform.right * sideways);
        movement -= Vector3.Project(movement, collsionNormal);

        //float acceleration = 1;
        //currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);
        //currentVelocity = movement * currentSpeed;

        rigidbody.MovePosition(transform.position + movement);//change movement to currentVelocity
        
        

        jumpVector = Vector3.up;
        //transform.position += currentVelocity;     
    }

    private void Rolling(float force)
    {
        //removing rotation constraints
        rigidbody.constraints = RigidbodyConstraints.None;

        float forward = Input.GetAxis("Vertical") * force;
        float sideways = Input.GetAxis("Roll") * force / 2;
        float around = Input.GetAxis("Horizontal") * force / 2;

        //deciding the rolling forward direction
        Vector3 direction = transform.position - ThirdPerson.transform.position;
        float temp = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized;

        Vector3 torque =
            direction * -sideways
            + Vector3.Cross(direction, Vector3.down) * forward//this gives the vector perpindicular to the plane 
                                                                //created by the desired forward direction and the world down direction
                                                                //- which means this gives the desired RIGHT vector
            + Vector3.up * around; //just made it to spin around the same way regardless of other factors for simplisity

        rigidbody.AddTorque(torque, ForceMode.Acceleration);

        // Calculate the center of mass relative to the object's position and rotation
        Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;
        Vector3 centerOfMass = worldToLocalMatrix.MultiplyPoint(transform.position + centerOfMassOffset);
        rigidbody.centerOfMass = centerOfMass;



        n[0].x = rigidbody.angularVelocity.magnitude;
    }

    private void Cam()
    {
       ThirdPerson.enabled = Input.GetKeyDown(KeyCode.R)? !ThirdPerson.enabled: ThirdPerson.enabled;
       POV.enabled = !ThirdPerson.enabled;
    }


    private Vector2 prevMousePos = Vector2.zero;
    private Vector2 mousePosDelta = Vector2.zero;
    private Quaternion initial3rdCamRot;
    private bool MoveCam()
    {
        //prevMousePos is always updated in the Update function in the last line.
        //no need to do that here again.
        bool mouseDown = Input.GetKey(KeyCode.Mouse0);
        if(ThirdPerson.enabled && !MiniMapCam.enabled)
        {
            if (mouseDown)
            {
                mousePosDelta = prevMousePos - (Input.mousePosition.x * Vector2.right + Input.mousePosition.y * Vector2.up);

                mousePosDelta /= sensitivity*10;
                ThirdPerson.transform.parent.Rotate(mousePosDelta.x * Vector3.down, Space.World);
                ThirdPerson.transform.parent.Rotate(mousePosDelta.y * Vector3.right, Space.Self);
            }
            else
            {
                ThirdPerson.transform.parent.localRotation = Quaternion.Lerp(ThirdPerson.transform.parent.localRotation,initial3rdCamRot, 0.1f); 
            }
        }
        return mouseDown;
    }

    private void MiniMap()
    {
        miniMapDepth = Input.GetKeyDown(KeyCode.Mouse2) ? -miniMapDepth : miniMapDepth;
        if(miniMapDepth > 0)
        {
            Cursor.visible = true;


            MiniMapCam.depth = miniMapDepth;
            MiniMapCam.orthographicSize -= Input.mouseScrollDelta.y * MiniMapCam.orthographicSize / 10;
            if (MiniMapCam.orthographicSize > 250)
                MiniMapCam.orthographicSize = 249;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                mousePosDelta = prevMousePos - (Input.mousePosition.x * Vector2.right + Input.mousePosition.y * Vector2.up);

                MiniMapCam.transform.position += (mousePosDelta.x * Vector3.right + mousePosDelta.y * Vector3.forward) * MiniMapCam.orthographicSize / 275;
            }
        }
        else
            Cursor.visible = true;


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
                rollingForce /= 2;
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
                rollingForce *= 4;
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
                rollingForce /= 2;
                rigidbody.maxAngularVelocity = 14;
            }
            cycleNum++;
        }
    }

    Vector3 collsionNormal;
    private void OnCollisionStay(Collision collision)
    {
        collsionNormal = Vector3.zero;
        foreach(ContactPoint point in collision.contacts)
        {
            collsionNormal += point.normal;
        }
        collsionNormal /= collision.contacts.Length;

        if (rolling)
        {
            isGrounded = true;

            Vector3 avgContactPoint=Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                avgContactPoint += collision.GetContact(i).point;
            }
            avgContactPoint = avgContactPoint / collision.contactCount;
            jumpVector = transform.position - avgContactPoint;
            jumpVector = jumpVector.normalized;
        }
        else if (!isGrounded)
        {
            
            //Vector3 avgContactPoint = Vector3.zero;
            //for (int i = 0; i < collision.contactCount; i++)
            //{
            //    avgContactPoint += collision.GetContact(i).point;
            //}
            //avgContactPoint = avgContactPoint / collision.contactCount;
            //if (avgContactPoint.y > GroundTrigger.transform.position.y + 0.1f)
            //{
            //    //movingSpeed = 0.5f;
            //    transform.position = Vector3.Lerp(transform.position, transform.position+(transform.position-avgContactPoint)/20,0.1f);
                print("Wall");
            //}

            //movingSpeed/=2;
            //rigidbody.AddForce(collsionNormal * collision.impulse.magnitude*0.5f, ForceMode.Force);
            
            //rigidbody.MovePosition(transform.position - Vector3.Project(movement, collsionNormal));
            //rigidbody.MovePosition(transform.position + normal * movingSpeed * Time.fixedDeltaTime*0.5f);

            //float y = rigidbody.velocity.y;
            //print("Vy = " + y);
            //Vector3 velocity = rigidbody.velocity;
            ////velocity = -velocity * Mathf.Sin(Vector3.Angle(collision.GetContact(0).normal, velocity));
            //velocity = velocity.x * Vector3.right + y * Vector3.up + velocity.z * Vector3.forward;
            //rigidbody.velocity = velocity;
            //print("RBVy = " + rigidbody.velocity.y);

        }
        else
        {
            Vector3 avgContactPoint = Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                avgContactPoint += collision.GetContact(i).point;
            }
            avgContactPoint = avgContactPoint / collision.contactCount;
            if (avgContactPoint.y > GroundTrigger.transform.position.y + 0.1f)
            {
                print("WallGround");
                movingSpeed = 1f;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //float y = rigidbody.velocity.y;
        //print("Enter Vy = " + y);
        //Vector3 velocity = rigidbody.velocity;
        //velocity = -velocity * Mathf.Sin(Vector3.Angle(collision.GetContact(0).normal, velocity));
        //velocity = velocity.x * Vector3.right + y * Vector3.up + velocity.z * Vector3.forward;
        //rigidbody.velocity = velocity;

        collsionNormal = Vector3.zero;
        foreach (ContactPoint point in collision.contacts)
        {
            collsionNormal += point.normal;
        }
        collsionNormal /= collision.contacts.Length;
        //print("Enter post Vy = " + y);

        //if (!isGrounded)
        //{
        //    Vector3 avgContactPoint = Vector3.zero;
        //    foreach (ContactPoint contact in collision.contacts)
        //    {
        //        avgContactPoint += contact.point;
        //    }
        //    avgContactPoint = avgContactPoint / collision.contactCount;
        //    if (avgContactPoint.y > GroundTrigger.transform.position.y + 0.1f)
        //    {
        //        movingSpeed = 0;
        //        print("Wall");
        //    }
        //}
    }
    private void OnCollisionExit(Collision collision)
    {
        collsionNormal = Vector3.zero;
        if (rolling)
        {
            isGrounded = false;
            jumpVector = Vector3.zero;
        }
        else
        {
            movingSpeed = MOVING_SPEED;
        }
    }
}
