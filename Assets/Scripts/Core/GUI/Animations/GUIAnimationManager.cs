using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the various GUI Animations
/// </summary>
public class GUIAnimationManager : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    private Dictionary<string, GUIAnimation> animations;

    void Awake()
    {
        animations = new Dictionary<string, GUIAnimation>();
    }

    /// <summary>
    /// Starts a new animation if it doesn't exists already
    /// </summary>
    /// <param name="ID">The animation's ID</param>
    /// <returns>The animation's length</returns>
    public float StartNewAnimation(string ID)
    {
        if (animations.ContainsKey(ID)) return 0;

        GUIAnimation obj = Instantiate(Resources.Load<GUIAnimation>("AnimatedGUI/" + ID), cameraTransform);
        obj.transform.localPosition = new Vector3(0, 0, 10);
        animations.Add(ID, obj);
        return obj.GetAnimationLength();
    }

    /// <summary>
    /// Deletes an animation if it exists
    /// </summary>
    /// <param name="ID">The animation's ID</param>
    public void DeleteAnimation(string ID)
    {
        if (animations.ContainsKey(ID))
        {
            Destroy(animations[ID].gameObject);
            animations.Remove(ID);
        }
    }

    /// <summary>
    /// Destroys all registered animations
    /// </summary>
    public void DeleteAllAnimations()
    {
        foreach (GUIAnimation animation in animations.Values)
        {
            Destroy(animation.gameObject);
        }
        animations.Clear();
    }
}
