using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverEvents : MonoBehaviour
{
    public bool Active = false;
    [SerializeField] private GameObject parent;

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
    public void EventOp()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        if (other.tag == "Player" || other.tag == "Entity")
        {
            Active = !Active;
            parent.transform.Rotate(Vector3.forward, 180, Space.Self);
            if(Active)
            Event();
            else EventOp();
        }
    }
}

