using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]

public class PlayerSpeaker : MonoBehaviour
{
    [SerializeField] float pauseBetweenSentance;

    private AudioSource audioSource;
    private List<AudioClip> playerSentance;

    // Use this for initialization
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        playerSentance = new List<AudioClip>();
    }

    public void giveSentance(AudioClip sentance)
    {
        if (sentance != null)
        playerSentance.Add(sentance);
        if (!audioSource.isPlaying && playerSentance.Count > 0)
        {
            playNewSentance();
        }
    }

    private void playNewSentance()
    {
        audioSource.clip = playerSentance[0];
        audioSource.Play();
        Invoke("SoundEnded", playerSentance[0].length + pauseBetweenSentance);
        playerSentance.RemoveAt(0);
    }

    private void SoundEnded()
    {
        if (playerSentance.Count > 0)
        {
            playNewSentance();
        }
    }
}
