using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource defaultAudioSource;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        defaultAudioSource = GetComponent<AudioSource>();

        // DontDestroyOnLoad doesn't work for nested objects.
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    public void SetBackgroundMusic(AudioClip bgm)
    {
        if (defaultAudioSource.clip == bgm) return;
        
        defaultAudioSource.Stop();
        defaultAudioSource.clip = bgm;
        defaultAudioSource.Play();
    }
}
