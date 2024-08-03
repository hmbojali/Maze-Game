using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float elevationForce=1;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!enabled)
            return;
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Entity")
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * elevationForce,ForceMode.Acceleration);
            print("elevate!");
        }
    }
}
