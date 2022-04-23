using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip powerAttackSound;

    private void AnimationAttack()
    {
        audioSource?.PlayOneShot(attackSound);
    }

    private void AnimationPowerAttack()
    {
        audioSource?.PlayOneShot(powerAttackSound);
    }

    private void AnimationFootfall()
    {
        var index = Random.Range(0, footstepSounds.Length);
        audioSource?.PlayOneShot(footstepSounds[index]);
    }
}
