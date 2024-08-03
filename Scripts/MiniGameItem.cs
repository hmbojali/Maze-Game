using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameItem : MonoBehaviour
{
    [SerializeField] private GameObject SceneLoader;
    [SerializeField] public int nextSceneIndex;
    [SerializeField] private GameObject transitionGO;
    [SerializeField] public float transitionTime = 5;


    private SceneLoader sceneLoader;
    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = SceneLoader.GetComponent<SceneLoader>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        transitionGO.SetActive(true);
        sceneLoader.transition = transitionGO.GetComponent<Animator>();
        sceneLoader.nextSceneIndex = nextSceneIndex;
        sceneLoader.transitionTime = transitionTime;
        sceneLoader.Activate = true;

    }
}
