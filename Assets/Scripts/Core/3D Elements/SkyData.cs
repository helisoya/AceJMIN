using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a skybox, with a certain weather
/// </summary>
[CreateAssetMenu(fileName = "SkyData", menuName = "HQ/SkyData", order = 0)]
public class SkyData : ScriptableObject
{
    public Material skybox;
    public Color sunColor;
    public Wheather wheather;
}

/// <summary>
/// A possible type of wheather
/// </summary>
public enum Wheather
{
    NORMAL,
    RAIN,
    SNOW
}