using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTriggerScript : MonoBehaviour
{

    [SerializeField] private GameObject player;
    private PlayerControl playerControl;
    // Start is called before the first frame update
    void Start()
    {
        playerControl = ((PlayerControl)player.GetComponent<PlayerControl>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
             print("hit");
   
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger)
        {
            playerControl.isGrounded = playerControl.rolling ? playerControl.isGrounded : true;
            playerControl.movingSpeed = playerControl.MOVING_SPEED;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerControl.isGrounded = playerControl.rolling ? playerControl.isGrounded : false;
    }
}
