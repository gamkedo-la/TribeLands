using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepSounds;

    private void AnimationFootfall()
    {
        var index = Random.Range(0, footstepSounds.Length);
        audioSource?.PlayOneShot(footstepSounds[index]);
    }
}
