using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> audioClip;
    private AudioSource audioSource;
    private int index = 0;
    private bool isPlaying = false;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!isPlaying)
        {
            StartCoroutine(AudioPlay());
        }
    }

    private IEnumerator AudioPlay()
    {
        isPlaying = true;
        for (index = 0; index < audioClip.Count; index++)
        {
            audioSource.clip = audioClip[index];
            audioSource.Play();
            yield return new WaitForSeconds(audioClip[index].length);
        }
        isPlaying = false;
    }
}
