using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for the backgrounds
/// </summary>
public class BackgroundManager : MonoBehaviour
{
    [Header("Background Manager")]
    [SerializeField] private Transform cameraMarkers;
    [SerializeField] private Transform characterMarkers;
    [SerializeField] private SpriteRenderer[] bgRenderers;
    private Background[] currentBackgrounds;
    private string pathToBackgrounds = "Backgrounds/";
    public static BackgroundManager instance;

    void Awake()
    {
        instance = this;
        currentBackgrounds = new Background[2];
    }

    /// <summary>
    /// Gets the name of a background
    /// </summary>
    /// <param name="slot">The background's slot</param>
    /// <returns>The background's name if it exists</returns>
    public string GetBackgroundName(int slot)
    {
        if (currentBackgrounds[slot] != null) return currentBackgrounds[slot].GetBackgroundName();
        return null;
    }

    /// <summary>
    /// Replaces the current background
    /// </summary>
    /// <param name="slot">The background's slot</param>
    /// <param name="newBackground">The new background's name</param>
    public void ReplaceBackground(int slot, string newBackground)
    {
        if (currentBackgrounds[slot] != null && currentBackgrounds[slot].GetBackgroundName().Equals(newBackground)) return;

        GameObject obj = Resources.Load<GameObject>(pathToBackgrounds + newBackground);
        if (!obj) return;

        if (currentBackgrounds[slot])
        {
            currentBackgrounds[slot].UnregisterInteractables();
            Destroy(currentBackgrounds[slot].gameObject);
        }

        currentBackgrounds[slot] = Instantiate(obj, bgRenderers[slot].transform).GetComponent<Background>();
        bgRenderers[slot].sprite = currentBackgrounds[slot].GetLinkedSprite();
        currentBackgrounds[slot].Init();
    }



    /// <summary>
    /// Finds a camera's marker
    /// </summary>
    /// <param name="ID">The marker's ID</param>
    /// <returns>The marker's position</returns>
    public Vector3 GetCameraPosition(string ID)
    {
        foreach (Transform child in cameraMarkers)
        {
            if (child.name == ID) return child.position;
        }
        return Vector3.zero;
    }


    /// <summary>
    /// Finds a character's marker
    /// </summary>
    /// <param name="ID">The marker's ID</param>
    /// <returns>The marker's position</returns>
    public Vector3 GetCharacterPosition(string ID)
    {
        foreach (Transform child in characterMarkers)
        {
            if (child.name == ID) return child.position;
        }
        return Vector3.zero;
    }
}
