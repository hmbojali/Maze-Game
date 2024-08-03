using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class Scraps
    {
        //old rolling func

        //public void Rolling(float speed)
        //{
        //    float forward = Input.GetAxis("Vertical") * speed;
        //    float sideways = Input.GetAxis("Roll") * speed / 2;
        //    float around = Input.GetAxis("Horizontal") * speed / 2;

        //    Vector3 torque = Vector3.forward * -sideways + Vector3.right * forward + Vector3.up * around;

        //    // Calculate the center of mass relative to the object's position and rotation
        //    Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;
        //    Vector3 centerOfMass = worldToLocalMatrix.MultiplyPoint(transform.position + centerOfMassOffset);
        //    rigidbody.centerOfMass = centerOfMass;
        //    n[0] = rigidbody.centerOfMass;
        //    n[1] = rigidbody.worldCenterOfMass;
        //    n[2] = centerOfMass;

        //    //removing rotation constraints
        //    rigidbody.constraints = RigidbodyConstraints.None;
        //    rigidbody.AddRelativeTorque(torque, ForceMode.Acceleration);

        //    //jumping
        //    bool jump = Input.GetKeyDown(KeyCode.Space);
        //    if (jump && isGrounded)
        //    {
        //        gameObject.GetComponent<Rigidbody>().AddForce(jumpVector * jumpForce, ForceMode.Impulse);
        //    }
        //}


        //public void Rolling(float speed)
        //{
        //    float forward = Input.GetAxis("Vertical") * speed;
        //    float sideways = Input.GetAxis("Roll") * speed / 2;
        //    float around = Input.GetAxis("Horizontal") * speed / 2;

        //    //Vector3 torque = Vector3.forward *-sideways  + Vector3.right *forward + Vector3.up*around;


        //    GameObject camFixedT = Instantiate(ThirdPerson.gameObject);
        //    Destroy(camFixedT);
        //    camFixedT.transform.localPosition -= Vector3.up * 3;
        //    camFixedT.transform.LookAt(transform);


        //    Vector3 direction = ThirdPerson.GetComponent<CameraChange>().direction;//fix here



        //    n[0] = ((Vector3)Vector3Int.FloorToInt(camFixedT.transform.position * 10)) / 10;
        //    n[1] = camFixedT.transform.eulerAngles;
        //    Vector3 torque = Vector3.right * sideways + Vector3.forward * forward + Vector3.up * around;//here
        //    torque.Scale(direction);//here, and now go to CameraChange and also fix everything direction.


        //    // Calculate the center of mass relative to the object's position and rotation
        //    Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;
        //    Vector3 centerOfMass = worldToLocalMatrix.MultiplyPoint(transform.position + centerOfMassOffset);
        //    rigidbody.centerOfMass = centerOfMass;
        //    //n[0] = rigidbody.centerOfMass;
        //    //n[1] = rigidbody.worldCenterOfMass;
        //    //n[2] = centerOfMass;

        //    //removing rotation constraints
        //    rigidbody.constraints = RigidbodyConstraints.None;
        //    rigidbody.AddRelativeTorque(torque, ForceMode.Acceleration);

        //    //jumping
        //    bool jump = Input.GetKeyDown(KeyCode.Space);
        //    if (jump && isGrounded)
        //    {
        //        gameObject.GetComponent<Rigidbody>().AddForce(jumpVector * jumpForce, ForceMode.Impulse);
        //    }
        //}


    }
}
