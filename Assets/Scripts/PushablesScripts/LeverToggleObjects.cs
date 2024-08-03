using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverToggleObjects : MonoBehaviour
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
        for(int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(Active);
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
            Event();
        }
    }
}

