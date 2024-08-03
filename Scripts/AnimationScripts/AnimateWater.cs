using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.WSA;

public class AnimateWater : MonoBehaviour
{

    [SerializeField] Texture[] frames = new Texture[4];
    private Material material;

    public int index = 1;
    private int increment = 1;

    public float sinInput = pi/2;
    private float sinIncrement = pi*2/121;
    public float frameTime = 0.1f;
    private const float pi = Mathf.PI;




    // Start is called before the first frame update
    void Start()
    {
        sinIncrement = pi * 2 / frames.Length;
        material = GetComponent<MeshRenderer>().material;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        // This loop will run forever
        while (true)
        {
            //goes from 0.15 to 0.05 and back
            frameTime = Mathf.Sin(sinInput) * 0.1f + 0.15f; //squish the drawing of SIN and lift it up 0.1 to make it posititve, then lift it up 0.05 to get wanted result

            sinInput = sinIncrement*index + pi/2;
            // Wait for 2 seconds
            yield return new WaitForSeconds(frameTime);

            material.mainTexture = frames[index];
            if (index == frames.Length - 1 || index==0)
            {
                increment *= -1;
            }
            index += increment;
        }
    }
}
