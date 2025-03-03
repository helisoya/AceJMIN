using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] private string backgroundName;
    [SerializeField] private Sprite backgroundSprite;


    /// <summary>
    /// Initialize the background
    /// </summary>
    public void Init()
    {

    }

    /// <summary>
    /// Returns the background's sprite
    /// </summary>
    /// <returns>The background's sprite</returns>
    public Sprite GetLinkedSprite()
    {
        return backgroundSprite;
    }

    /// <summary>
    /// Returns the background's name
    /// </summary>
    /// <returns>The background's name</returns>
    public string GetBackgroundName()
    {
        return backgroundName;
    }

    /// <summary>
    /// Unregisters the background's interactables
    /// </summary>
    public void UnregisterInteractables()
    {
        InteractableObject[] interactables = GetComponentsInChildren<InteractableObject>();
        foreach (InteractableObject interactable in interactables)
        {
            interactable.Unregister();
        }
    }

}
