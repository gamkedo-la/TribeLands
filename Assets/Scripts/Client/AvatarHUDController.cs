using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarHUDController : MonoBehaviour
{
    [SerializeField] private string avatarTag;
    [SerializeField] private string avatarName;
    [SerializeField] private Image healthMeter;
    [SerializeField] private Image specialMeter;
    
    private void Start()
    {
        GameObject avatar = null;
        var allAvatars = GameObject.FindGameObjectsWithTag(avatarTag);

        foreach (var a in allAvatars)    
        {
            if (a.name.Contains(avatarName))
            {
                avatar = a;
                break;
            }
        }
        
        if (avatar == null)
        {
            Debug.LogError($"Unable to find avatar with name `{avatarName}`");
            return;
        }

        var networkAvatar = avatar.GetComponent<NetworkAvatar>();
        networkAvatar.OnDamaged.AddListener(OnAvatarDamaged);
    }

    private void OnAvatarDamaged(float healthRemaining, float maxHealth)
    {
        var percentHealthRemaining = healthRemaining / maxHealth;
        healthMeter.fillAmount = percentHealthRemaining;
    }
}
