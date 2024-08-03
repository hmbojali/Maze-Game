using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonToggleVisibility : MonoBehaviour
{
    public bool Active = false;
    [SerializeField] public Vector3 buttonDirection;
    [SerializeField] private GameObject[] gameObjects;

    // Start is called before the first frame update
    void Start()
    {
        buttonDirection = buttonDirection.normalized;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Event()
    {
        for(int i=0; i<gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(!gameObjects[i].activeSelf);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled)
            return;

        if (collision.collider.tag == "Player" || collision.collider.tag == "Entity")
        {
            Active = true;
            transform.position = transform.position + buttonDirection * transform.lossyScale.z/2;
            GetComponent<BoxCollider>().center -= buttonDirection * transform.lossyScale.z / 2;

            Event();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!enabled)
            return;

        if (collision.collider.tag == "Player" || collision.collider.tag == "Entity")
        {
            Active = false;
            transform.position = transform.position - buttonDirection * transform.lossyScale.z / 2;
            GetComponent<BoxCollider>().center += buttonDirection * transform.lossyScale.z / 2;

            Event();
        }
    }
}
