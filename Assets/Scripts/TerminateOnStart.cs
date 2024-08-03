using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class TerminateOnStart : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            EditorApplication.ExitPlaymode();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}