using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTriggerMiniGameScript : MonoBehaviour
{

    [SerializeField] private GameObject player;
    private PlayerControlMiniGame playerControl;
    // Start is called before the first frame update
    void Start()
    {
        playerControl = ((PlayerControlMiniGame)player.GetComponent<PlayerControlMiniGame>());
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
            playerControl.movingSpeed = 3f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerControl.isGrounded = playerControl.rolling ? playerControl.isGrounded : false;
    }
}
