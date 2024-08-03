using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInWater : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        rb.drag = 2;
    }

    private void OnTriggerExit(Collider other)
    {
        rb.drag = 0;
    }
}
