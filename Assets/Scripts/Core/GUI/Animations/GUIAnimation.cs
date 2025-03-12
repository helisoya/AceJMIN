using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an animation on the GUI (Hammer, Testimony, ...)
/// </summary>
public class GUIAnimation : MonoBehaviour
{
    [SerializeField] private AnimationClip animationClip;

    /// <summary>
    /// Gets the animation's length
    /// </summary>
    /// <returns></returns>
    public float GetAnimationLength()
    {
        return animationClip.length;
    }
}
