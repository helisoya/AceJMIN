using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the game's skybox
/// </summary>
[RequireComponent(typeof(Light))]
public class LightingManager : MonoBehaviour
{
    [Header("Lighting Manager")]
    [SerializeField] private SkyData defaultSky;
    [SerializeField] private GameObject snowEffect;
    [SerializeField] private GameObject rainEffect;
    private SkyData currentData;

    public static LightingManager instance;

    void Awake()
    {
        instance = this;
        SetDataToDefault();
    }

    /// <summary>
    /// Changes the current skybox to the default one
    /// </summary>
    public void SetDataToDefault()
    {
        ChangeData(defaultSky);
    }

    /// <summary>
    /// Changes the current skybox
    /// </summary>
    /// <param name="skyDataName">The new sky data's name</param>
    public void ChangeData(string skyDataName)
    {
        ChangeData(Resources.Load<SkyData>("Skyboxes/" + skyDataName));
    }

    /// <summary>
    /// Changes the current skybox
    /// </summary>
    /// <param name="skyData">The new sky data</param>
    public void ChangeData(SkyData skyData)
    {
        currentData = skyData;

        RenderSettings.skybox = skyData.skybox;
        GetComponent<Light>().color = skyData.sunColor;
        snowEffect.SetActive(skyData.wheather == Wheather.SNOW);
        rainEffect.SetActive(skyData.wheather == Wheather.RAIN);
    }

    /// <summary>
    /// Returns the current data's name
    /// </summary>
    /// <returns>The current data's name</returns>
    public string GetCurrentDataName()
    {
        if (currentData == null) return defaultSky.name;
        return currentData.name;
    }
}
