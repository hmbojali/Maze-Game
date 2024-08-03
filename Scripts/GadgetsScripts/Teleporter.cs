using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private GameObject otherTeleporter;
    [SerializeField] private ParticleSystem particleSystemUp;
    [SerializeField] private ParticleSystem particleSystemDown;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Color color;
    [SerializeField] private MeshRenderer innerMeshRenderer;

    private Material material;

    private float time;

    public bool justTeleported = false;

    // Start is called before the first frame update
    void Start()
    {
        material = Material.Instantiate(innerMeshRenderer.material);
        material.color = color;
        material.SetColor("_EmissionColor", color);
        innerMeshRenderer.material = material;

        var colorBySpeedUp = particleSystemUp.colorBySpeed;
        var colorBySpeedDown = particleSystemDown.colorBySpeed;
        gradient = particleSystemUp.colorBySpeed.color.gradient;
        Color midColor = new Color();
        for (int i = 0; i < 3; i++)
        {
            if (color[i] == 0)
            {
                midColor[i] = 185;
                Color additive = ((Color)((Vector4)color).normalized * 225);
                additive.a = 1;
                midColor += additive;
                print("done added color");
                break;
            }
        }
        GradientColorKey mid = gradient.colorKeys[1];
        GradientColorKey last = gradient.colorKeys[2];
        mid.color = midColor;
        last.color = color;
        gradient.colorKeys = new GradientColorKey[]{ gradient.colorKeys[0], mid, last };
        gradient.mode = GradientMode.Blend;
        Gradient reversedGradient=new Gradient();
        last.time = 0;
        mid.time = 0.75f;
        reversedGradient.colorKeys = new GradientColorKey[] { last, mid, new GradientColorKey(gradient.colorKeys[0].color,1) };
        reversedGradient.alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(gradient.alphaKeys[1].alpha, 0), new GradientAlphaKey(gradient.alphaKeys[0].alpha, 1) };

        colorBySpeedUp.color = reversedGradient;
        colorBySpeedDown.color = gradient;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;
        time = Time.time;

        material.color = ((Vector4)color).normalized;
        material.SetColor("_EmissionColor", ((Vector4)color).normalized);
        innerMeshRenderer.material = material;

        if (justTeleported)
        {
            var emissionUp = particleSystemUp.emission;
            var mainUp = particleSystemUp.main;
            emissionUp.rateOverTime = 2;
            mainUp.simulationSpeed = 2;
        }
        else
        {
            var emissionDown = particleSystemDown.emission;
            var mainDown = particleSystemDown.main;
            emissionDown.rateOverTime = 2;
            mainDown.simulationSpeed = 2;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!enabled)
            return;

        if (Time.time-time>2&&!justTeleported)
        {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Entity")
            {
                otherTeleporter.GetComponentInChildren<Teleporter>().justTeleported = true;
                other.transform.position = otherTeleporter.transform.position + Vector3.up * 0.5f;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!enabled)
            return;

        time = float.MaxValue;

        material.color = color;
        material.SetColor("_EmissionColor", color);
        var emissionUp = particleSystemUp.emission;
        var mainUp = particleSystemUp.main;
        emissionUp.rateOverTime = 0.5f;
        mainUp.simulationSpeed = 1f;
        var emissionDown = particleSystemDown.emission;
        var mainDown = particleSystemDown.main;
        emissionDown.rateOverTime = 0.5f;
        mainDown.simulationSpeed = 1;

        justTeleported = false;
    }
}
