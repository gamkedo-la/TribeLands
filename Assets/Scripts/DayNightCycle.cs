using System;
using UnityEngine;

[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light sun;
    [SerializeField] private Light moon;

    [SerializeField] private float highSunStrength;
    [SerializeField] private float lowSunStrength;

    [Range(0, 360)] [SerializeField] private float time = 0;
    [Range(0, 20)] [SerializeField] private float timeScale = 1;

    // Update is called once per frame
    void Update()
    {
        time = time + (Time.deltaTime * timeScale) % 360;
        
        var sunPos = time;
        var moonPos = (time + 180f) % 360;

        var sunStrength = Mathf.Sin(sunPos * Mathf.Deg2Rad);
        var sunIntensity = Mathf.Lerp(lowSunStrength, highSunStrength, sunStrength);
        sun.intensity = sunIntensity;

        // Change shadowcaster to moon at night.
        if (sunStrength > 0)
        {
            sun.shadows = LightShadows.Soft;
            moon.shadows = LightShadows.None;
        }
        else
        {
            sun.shadows = LightShadows.None;
            moon.shadows = LightShadows.Soft;
        }
        
        sun.transform.rotation = Quaternion.AngleAxis(sunPos, Vector3.right);
        moon.transform.rotation = Quaternion.AngleAxis(moonPos, Vector3.right);
    }
}
