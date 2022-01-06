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

        // Register event listeners for health and energy updates
        var networkAvatar = avatar.GetComponent<NetworkAvatar>();
        networkAvatar.OnHealthChanged.AddListener(UpdateHealth);
        networkAvatar.OnEnergyChanged.AddListener(UpdateEnergy);
        
        // Set initial health and energy values in case they are not 100%
        UpdateEnergy(networkAvatar.Energy, networkAvatar.MaxEnergy);
        UpdateHealth(networkAvatar.Health, networkAvatar.MaxHealth);
    }

    private void UpdateHealth(float healthRemaining, float maxHealth)
    {
        var percentHealthRemaining = healthRemaining / maxHealth;
        healthMeter.fillAmount = percentHealthRemaining;
    }

    private void UpdateEnergy(float energyRemaining, float maxEnergy)
    {
        var percentEnergyRemaining = energyRemaining / maxEnergy;
        specialMeter.fillAmount = percentEnergyRemaining;
    }
}
