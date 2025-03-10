using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Handles the game's health bar
/// </summary>
public class HeathbarManager : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] private GameObject root;
    [SerializeField] private Transform partsRoot;
    [SerializeField] private Material healthbarMat;
    private Image[] images;
    public int currentHealth { get; private set; }
    public int numberOfLastGlowing { get; private set; }
    public bool shown { get; private set; }

    void Awake()
    {
        images = new Image[partsRoot.childCount];
        for (int i = 0; i < partsRoot.childCount; i++)
        {
            images[i] = partsRoot.GetChild(i).GetComponent<Image>();
            images[i].material = new Material(healthbarMat);
            images[i].material.SetFloat("_Glow", 0);
        }
        UpdateHealth(10);
    }


    /// <summary>
    /// Shows the healthbar
    /// </summary>
    public void Show()
    {
        shown = true;
        root.SetActive(true);
    }

    /// <summary>
    /// Hides the healthbar
    /// </summary>
    public void Hide()
    {
        shown = false;
        root.SetActive(false);
    }

    /// <summary>
    /// Updates the currentHealth
    /// </summary>
    /// <param name="healthAmount"></param>
    public void UpdateHealth(int healthAmount)
    {
        currentHealth = healthAmount;
        for (int i = 0; i < images.Length; i++)
        {
            images[i].gameObject.SetActive(10 - i <= healthAmount);
        }
    }

    /// <summary>
    /// Sets the glow effect for the last visible X parts
    /// </summary>
    /// <param name="glowingPartsNumber">The number of parts that will be glowing</param>
    public void SetGlowForLast(int glowingPartsNumber)
    {
        numberOfLastGlowing = glowingPartsNumber;

        for (int i = 0; i < images.Length; i++)
        {
            images[i].material.SetFloat("_Glow", 10 - i > currentHealth - numberOfLastGlowing ? 1 : 0);
        }
    }
}
