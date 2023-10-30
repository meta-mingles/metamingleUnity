using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceZoneController : MonoBehaviour
{
    public GameObject voice;//∫∏¿ÃΩ∫
    // Start is called before the first frame update
    void Awake()
    {
        voice.SetActive(false);
    }
    private void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(voice!=null)
            {
                voice.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(voice!=null)
            {
                voice.SetActive(false);
            }
        }
    }

}
