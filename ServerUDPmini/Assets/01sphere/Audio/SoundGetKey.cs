using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundGetKey : MonoBehaviour
{

    AudioSource source;


    void Start()
    {

        source = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            Debug.Log("push");
            source.Play();
        }
    }
}
