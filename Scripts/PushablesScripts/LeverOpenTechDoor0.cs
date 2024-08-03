using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverOpenTechDoor0 : MonoBehaviour
{
    public bool Active = false;
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject[] gameObjects;

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
        foreach (GameObject gameObject in gameObjects)
        {
            if (!gameObject.GetComponent<Animator>().IsInTransition(0))
            {
                gameObject.GetComponent<Animator>().SetTrigger("Start");
            }
        }
    }
    public void EventOp()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (!gameObject.GetComponent<Animator>().IsInTransition(0))
            {
                gameObject.GetComponent<Animator>().SetTrigger("End");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        if (other.tag == "Player" || other.tag == "Entity")
        {
            Active = !Active;
            parent.transform.Rotate(Vector3.forward, 180, Space.Self);
            if (Active)
                Event();
            else EventOp();
        }
    }
}

