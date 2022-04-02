using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    public string SceneName;
    public string NextScene;
    public AudioClip BackgroundMusic;
}
