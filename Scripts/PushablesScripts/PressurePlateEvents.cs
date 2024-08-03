using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateEvents : MonoBehaviour
{

    public bool Active = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Event()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled)
            return;

        if (collision.collider.tag=="Player"|| collision.collider.tag == "Entity")
        {
            Active = true;
            transform.position = transform.position - Vector3.up * 0.02f;
            GetComponent<BoxCollider>().center += Vector3.up * 0.02f;

            Event();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!enabled)
            return;

        if (collision.collider.tag=="Player"|| collision.collider.tag == "Entity")
        {
            Active = false;
            transform.position = transform.position + Vector3.up * 0.02f;
            GetComponent<BoxCollider>().center -= Vector3.up * 0.02f;

            Event();
        }
    }

}
