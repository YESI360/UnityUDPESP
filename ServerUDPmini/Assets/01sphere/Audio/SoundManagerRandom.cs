using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerRandom : MonoBehaviour
{
    public AudioSource source;
    [SerializeField] AudioClip[] clipsArray;
     
    public AudioClip clip;
    public float volume = 0.5f;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            source.PlayOneShot(RandomClip());
        }

    }

    AudioClip RandomClip()
    {
        return clipsArray[Random.Range(0, clipsArray.Length)];
    }
}
