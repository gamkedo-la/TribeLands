using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SaveData", menuName = "Data/SaveData")]
public class SaveData : ScriptableObject
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

    public SaveData(string scene, int checkpointIndex, float health, float mana)
    {
        this.scene = scene;
        this.checkpointIndex = checkpointIndex;
        this.health = health;
        this.mana = mana;
    }
}
