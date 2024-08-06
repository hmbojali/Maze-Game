using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] public int nextSceneIndex;
    [SerializeField] public Animator transition;
    [SerializeField] public float transitionTime = 5;

    [HideInInspector] public bool Activate = false;

    // Start is called before the first frame update

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(Activate)
        {
            StartCoroutine(LoadSceneWithDelay(transitionTime));
            Activate = false;
        }
    }

    IEnumerator LoadSceneWithDelay(float delay)
    {
        transition.ResetTrigger("End");
        transition.SetTrigger("Start");
        transition.SetInteger("Started", 1);

        yield return new WaitForSeconds(delay);

        transition.SetInteger("Started", 1);
        transition.ResetTrigger("Start");
        transition.SetTrigger("End");
        DontDestroyOnLoad(mainCanvas);
        SceneManager.LoadScene(nextSceneIndex);
    }
}
