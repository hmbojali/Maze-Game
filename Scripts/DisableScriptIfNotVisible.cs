using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisableScriptIfNotVisible : MonoBehaviour
{
    [SerializeField] private Renderer m_Renderer;
    private List<MonoBehaviour> m_Scripts = new List<MonoBehaviour>();

    void Start()
    {
        // Get the Renderer and MonoBehaviour components attached to this object
        List<MonoBehaviour> scripts = GetComponents<MonoBehaviour>().ToList();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && script!=null)
            {
                m_Scripts.Add(script);
            }
        }

        // Start the CheckVisibility coroutine
        StartCoroutine(CheckVisibility());
    }

    IEnumerator CheckVisibility()
    {
        // This loop will run forever
        while (true)
        {
            // Wait for 2 seconds
            yield return new WaitForSeconds(2);

            // Check if the object is visible
            if (m_Renderer.isVisible)
            {
                // If the object is visible, enable the script components attached to the object
                foreach (MonoBehaviour script in m_Scripts)
                {
                    script.enabled = true;
                }
            }
            else
            {
                // If the object is not visible, disable the script components attached to the object
                foreach (MonoBehaviour script in m_Scripts)
                {
                    script.enabled = false;
                }
            }
        }
    }
}
