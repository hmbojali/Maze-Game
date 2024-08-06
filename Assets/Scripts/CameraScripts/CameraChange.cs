using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    public GameObject player;
    private PlayerControl playerControl;
    private Rigidbody rigidbody;
    private Transform parent;
    private Vector3 preAdd;
    private Vector3 notRollingPos;
    private Quaternion notRollingRot;

    public Vector3 direction = Vector3.forward;

    public Vector3[] n=new Vector3[4];

    // Start is called before the first frame update
    void Start()
    {
        playerControl = ((PlayerControl)player.GetComponent<PlayerControl>());
        rigidbody = GetComponent<Rigidbody>();
        notRollingPos = transform.localPosition;
        notRollingRot = transform.localRotation;
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (playerControl.rolling)
            {
                transform.parent = null;
                Vector3 velocity = player.GetComponent<Rigidbody>().velocity;

                if (velocity.sqrMagnitude > 4 || player.GetComponent<Rigidbody>().angularVelocity.sqrMagnitude > 30)
                {
                    Vector3 add, awayZone, newPos = player.transform.position;


                    float ADD_MAGNITUDE = 7f;

                    add = Vector3.zero;

                    awayZone = velocity.normalized;
                    awayZone.y = 0;//making it x and z only
                    direction = awayZone;
                    n[0] = awayZone;
                    n[2] = Vector3Int.FloorToInt(new Vector3(Mathf.Abs(awayZone.x), 0, Mathf.Abs(awayZone.z)) * 1.5f);

                    if (Vector3.zero == Vector3Int.FloorToInt(new Vector3(Mathf.Abs(awayZone.x), 0, Mathf.Abs(awayZone.z)) * 1.5f))
                    {
                        awayZone = preAdd.normalized * ADD_MAGNITUDE;
                        n[1].y = 0;
                    }
                    else
                    {
                        awayZone = awayZone * -ADD_MAGNITUDE;
                        awayZone += Vector3.up * 3;
                        n[1].y = 1;
                    }
                    //awayZone = Vector3.zero == Vector3Int.FloorToInt(awayZone) ? awayZone.normalized*ADD_MAGNITUDE : awayZone * -ADD_MAGNITUDE;

                    //add = ((Vector3)(Vector3Int.FloorToInt(velocity - Vector3.up * (velocity.y / 2)) / 2)).normalized;
                    //add *= -ADD_MAGNITUDE;

                    add += awayZone;
                    newPos += add;
                    preAdd = add;
                    n[3].x = (player.transform.position - transform.position).sqrMagnitude;

                    float lerpValue = 0.01f;
                    if ((player.transform.position - transform.position).sqrMagnitude > 49)
                        lerpValue = 0.02f;

                    transform.position = Vector3.Lerp(transform.position, newPos, 0.2f);

                    //eleminating weird jitter when rolling, thats caused by camera being too close.
                    //removed by not rotating instantly rather a bit slower.
                    Quaternion rot = transform.rotation;
                    transform.LookAt(player.transform.position);
                    transform.rotation = Quaternion.Lerp(rot, transform.rotation, 0.1f);

                }
                else
                {
                    Vector3 newPos = player.transform.position;


                    newPos += preAdd;

                    transform.position = Vector3.Lerp(transform.position, newPos, 0.01f);
                    transform.LookAt(player.transform.position);

                }
            }
            else
            {
                transform.parent = parent;
                transform.localPosition = notRollingPos;
                transform.localRotation = notRollingRot;
            }
        }
        catch { }
        }
        
}
