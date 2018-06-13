using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footstep_sound : MonoBehaviour {

    [SerializeField]
    private AudioClip[] Ssounds;

    private AudioSource FSaudiosource;

    private void Start()
    {
        FSaudiosource = GetComponent<AudioSource>();
    }

    private void step()
    {
        AudioClip Ssound = GetRandomClip();
        FSaudiosource.PlayOneShot(Ssound);
    }

    private AudioClip GetRandomClip()
    {
        return Ssounds[UnityEngine.Random.Range(0, Ssounds.Length)];
    }
}
