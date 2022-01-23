using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LightFlicker : MonoBehaviour
{
    public float maxReduction = 1000f;
    public float maxIncrease = 1000f;
    public float flickerRate = 0.1f;
    public float flickerStrength = 1.0f;
    public bool stopFlickering = false;
    
    private Light lightSource;
    private float baseIntensity;
    private bool isFlickering;
    
    // Start is called before the first frame update
    void Start()
    {
        lightSource = GetComponentInChildren<Light>();
        if (lightSource == null)
        {
            Debug.LogError("LightFlicker found no Light component");
            return;
        }

        baseIntensity = lightSource.intensity;
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        isFlickering = true;
        while (!stopFlickering)
        {
            lightSource.intensity = Mathf.Lerp(
                lightSource.intensity,
                Random.Range(baseIntensity - maxReduction, baseIntensity + maxIncrease),
                flickerStrength + Time.deltaTime);
            yield return new WaitForSeconds(flickerRate);
        }

        isFlickering = false;
    }

    void Update()
    {
        if (!stopFlickering && !isFlickering)
        {
            StartCoroutine(Flicker());
        }
    }
}
