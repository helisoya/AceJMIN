using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Represents a pause menu's component that can be highlighted
/// </summary>
[System.Serializable]
public class PauseMenuGraphicObject
{
    [SerializeField] private Image linkedImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite highlightedSprite;

    /// <summary>
    /// Highlights the component
    /// </summary>
    public void Highlight()
    {
        linkedImage.sprite = highlightedSprite;
    }

    /// <summary>
    /// Revert the component back to normal
    /// </summary>
    public void Normal()
    {
        linkedImage.sprite = normalSprite;
    }

    /// <summary>
    /// Sets if the graphics is active or not
    /// </summary>
    /// <param name="active">Is the graphics active ?</param>
    public void SetActive(bool active)
    {
        linkedImage.gameObject.SetActive(active);
    }
}
