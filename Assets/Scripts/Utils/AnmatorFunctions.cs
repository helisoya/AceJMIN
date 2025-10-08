using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Utilities functions for the animator / animation events
/// </summary>
public class AnmatorFunctions : MonoBehaviour
{
    /// <summary>
    /// Plays an SFX
    /// </summary>
    /// <param name="sfxName">The SFX Name</param>
    public void PlaySFX(string sfxName)
    {
        AudioClip clip = Resources.Load("Audio/SFX/" + sfxName) as AudioClip;
        AudioManager.instance.PlaySFX(clip);
    }
}
