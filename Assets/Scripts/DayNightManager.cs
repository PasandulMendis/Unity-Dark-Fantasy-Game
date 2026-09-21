using UnityEngine;

using UnityEngine.Rendering.Universal;

public class DayNightManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float timeMultiplier = 10f;
    [Range(0, 24)]
    public float currentTime = 8f;

    [Header("Lighting Components")]
    public Light2D globalLight;

    [Header("Lighting Colors")]
    public Color dayColor = new Color(1f, 1f, 1f);
    public Color sunsetColor = new Color(1f, 0.6f, 0.3f);
    public Color nightColor = new Color(0.2f, 0.2f, 0.4f);

    [Header("Lighting Intensities")]
    public float dayIntensity = 1f;
    public float nightIntensity = 0.3f;

    void Update()
    {
        currentTime += (Time.deltaTime / 60f) * timeMultiplier;

        if (currentTime >= 24f)
        {
            currentTime %= 24f;
        }
        UpdateLighting();
    }

    private void UpdateLighting()
    {
        if (globalLight == null) return;

        if (currentTime >= 6f && currentTime < 8f)
        {
            float percentage = (currentTime - 6f) / 2f;
            globalLight.color = Color.Lerp(nightColor, dayColor, percentage);
            globalLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, percentage);
        }
        else if (currentTime >= 8f && currentTime < 18f)
        {
            globalLight.color = dayColor;
            globalLight.intensity = dayIntensity;
        }
        else if (currentTime >= 18f && currentTime < 20f)
        {
            float percentage = (currentTime - 18f) / 2f;
            globalLight.color = Color.Lerp(dayColor, sunsetColor, percentage);
            globalLight.intensity = Mathf.Lerp(dayIntensity, 0.8f, percentage);
        }
        else if (currentTime >= 20f && currentTime < 22f)
        {
            float percentage = (currentTime - 20f) / 2f;
            globalLight.color = Color.Lerp(sunsetColor, nightColor, percentage);
            globalLight.intensity = Mathf.Lerp(0.8f, nightIntensity, percentage);
        }
        else
        {
            globalLight.color = nightColor;
            globalLight.intensity = nightIntensity;
        }
    }
}