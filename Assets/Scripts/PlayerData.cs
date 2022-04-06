using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string scene;
    public int checkpointIndex;
    public float health;
    public float mana;
    
    public void Load(SaveData saveData)
    {
        scene = saveData.scene;
        checkpointIndex = saveData.checkpointIndex;
        health = saveData.health;
        mana = saveData.mana;
    }
}
