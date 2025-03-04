using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Represents a row in the pause menu
/// </summary>
[System.Serializable]
public class PauseMenuRow
{
    [Header("Row")]
    [SerializeField] private string descriptionID;
    [SerializeField] private PauseMenuGraphicObject[] objects;
    [SerializeField] private TextMeshProUGUI[] texts;

    /// <summary>
    /// Gets the row's description ID
    /// </summary>
    /// <returns>The row's description ID</returns>
    public string GetDescriptionID()
    {
        return descriptionID;
    }

    /// <summary>
    /// Highlights the row
    /// </summary>
    public void Highlight()
    {
        foreach (PauseMenuGraphicObject obj in objects)
        {
            obj.Highlight();
        }

        foreach (TextMeshProUGUI text in texts)
        {
            text.color = Color.black;
        }
    }

    /// <summary>
    /// Revert the row back to normal
    /// </summary>
    public void Normal()
    {
        foreach (PauseMenuGraphicObject obj in objects)
        {
            obj.Normal();
        }

        foreach (TextMeshProUGUI text in texts)
        {
            text.color = Color.white;
        }
    }
}
