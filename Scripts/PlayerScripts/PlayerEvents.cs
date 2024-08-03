using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class PlayerEvents : MonoBehaviour
{
    [SerializeField] private Image UICoinGreen;
    [SerializeField] private Image UICoinRed;
    [SerializeField] private Image UICoinYellow;
    public bool CollectedCoinGreen = false;
    public bool CollectedCoinRed = false;
    public bool CollectedCoinYellow = false;


    // Start is called before the first frame update
    void Start()
    {
        UICoinGreen = GameObject.Find("MainCanvas").transform.GetChild(0).gameObject.GetComponent<Image>();
        UICoinRed = GameObject.Find("MainCanvas").transform.GetChild(1).gameObject.GetComponent<Image>();
        UICoinYellow = GameObject.Find("MainCanvas").transform.GetChild(2).gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        
        {
            GameObject coin = other.gameObject;
            if (coin.name == "coinGreen")
            {
                UICoinGreen.color = new Color(0, 255, 0);
                CollectedCoinGreen = true;
                coin.SetActive(false);
            }
            else if (coin.name == "coinRed")
            {
                UICoinRed.color = new Color(255, 0, 0);
                CollectedCoinRed = true;
                coin.SetActive(false);

            }
            else if (coin.name == "coinYellow")
            {
                UICoinYellow.color = new Color(255, 255, 0);
                CollectedCoinYellow = true;
                coin.SetActive(false);

            }
        }
  
    }
}
