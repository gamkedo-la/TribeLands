using System;

[Serializable]
public class SaveData
{
    public string scene;
    public int checkpointIndex;
    public float health;
    public float mana;
    
    public SaveData()
    {
        scene = "Dungeon";
        checkpointIndex = 0;
        health = 100;
        mana = 100;
    }

    public SaveData(PlayerData playerData)
    {
        scene = playerData.scene;
        checkpointIndex = playerData.checkpointIndex;
        health = playerData.health;
        mana = playerData.mana;
    }
    
    public SaveData(string scene, int checkpointIndex, float health, float mana)
    {
        this.scene = scene;
        this.checkpointIndex = checkpointIndex;
        this.health = health;
        this.mana = mana;
    }
}