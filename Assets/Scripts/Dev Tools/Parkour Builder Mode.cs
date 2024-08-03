using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

public class ParkourBuilderMode : MonoBehaviour
{
    [SerializeField] private GameObject[] objects = new GameObject[10];
    [SerializeField] private GameObject selectedObject;

    public bool active = false;

    private GameObject Parent;
    private PlayerControl playerControl;
    private Vector3 pastObjectPosition;
    private Vector3 selectedObjectSize;

    // Start is called before the first frame update
    void Start()
    {
        Parent = new GameObject();
        Parent.name = "Parkour Creation";
        selectedObject = objects[0];
        selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;

        pastObjectPosition = -(Vector3.up * GetComponent<Collider>().bounds.extents.y);
        playerControl = GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        Activate();
        if (active)
        {
            ChangeSelectedObjects();
        }
    }
    private void FixedUpdate()
    {
        if (active)
        {
            Mechanics();
        }
    }

    void Activate()
    {
        if(Input.GetKeyDown(KeyCode.F12))
        {
            active = !active;
        }

    } 

    void Mechanics()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerControl.rigidbody.velocity = Vector3.right * playerControl.rigidbody.velocity.x + Vector3.forward * playerControl.rigidbody.velocity.z;
            //playerControl.isGrounded = false;
            Instantiate(selectedObject,
                (transform.position) + (Vector3.down * (GetComponent<Collider>().bounds.extents.y+selectedObjectSize.y/2)),
                selectedObject.transform.rotation,
                Parent.transform);
            playerControl.isGrounded = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
                Vector3 delta = (transform.position - pastObjectPosition + (Vector3.up * GetComponent<Collider>().bounds.extents.y));
                float absX, absZ;
                absX = Mathf.Abs(delta.x);
                absZ = Mathf.Abs(delta.z);
            delta =
                  (absX > absZ ? Vector3.right * Mathf.Sign(delta.x) : Vector3.forward * Mathf.Sign(delta.z));
            delta = delta.x * selectedObjectSize.x * Vector3.right + delta.z * selectedObjectSize.z * Vector3.forward;
            delta += (Vector3.down * GetComponent<Collider>().bounds.extents.y);
                pastObjectPosition = Instantiate(selectedObject,
                    pastObjectPosition + delta,
                    selectedObject.transform.rotation,
                    Parent.transform).transform.position;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            RaycastHit hit;
            if (!Physics.Raycast(transform.position+playerControl.rigidbody.velocity.normalized*5, Vector3.down, out hit, GetComponent<Collider>().bounds.extents.y+0.1f))
            {

                    Vector3 delta = (transform.position - pastObjectPosition + (Vector3.up * GetComponent<Collider>().bounds.extents.y));
                    float absX, absZ;
                    absX = Mathf.Abs(delta.x);
                    absZ = Mathf.Abs(delta.z);
                delta =
                      (absX > absZ ? Vector3.right * Mathf.Sign(delta.x) : Vector3.forward * Mathf.Sign(delta.z));
                    delta = delta.x * selectedObjectSize.x * Vector3.right + delta.z * selectedObjectSize.z * Vector3.forward;
                    pastObjectPosition = Instantiate(selectedObject,
                        pastObjectPosition + delta,
                        selectedObject.transform.rotation,
                        Parent.transform).transform.position;
                
            }
        }
        else
        {
            pastObjectPosition =  transform.position - (Vector3.up * GetComponent<Collider>().bounds.extents.y); ;

        }
    }

    void ChangeSelectedObjects()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedObject = objects[0];
            selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedObject = objects[1];
            selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedObject = objects[2];
            selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedObject = objects[3];
            selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedObject = objects[4];
            selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectedObject = objects[5];
            selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectedObject = objects[6];
            selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            selectedObject = objects[7];
            selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            selectedObject = objects[8];
            selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            selectedObject = objects[9];
            selectedObjectSize = selectedObject.GetComponent<Collider>().bounds.size;
        }
    }

}
