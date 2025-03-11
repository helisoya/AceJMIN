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
    [SerializeField] private Animator rootAnimator;
    [SerializeField] private Transform partsRoot;
    [SerializeField] private Material healthbarMat;
    [SerializeField] private Animator explosionAnimator;
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
        SetGlowForLast(0);
    }


    /// <summary>
    /// Shows the healthbar
    /// </summary>
    public void Show()
    {
        shown = true;
        rootAnimator.SetBool("Shown", true);
    }

    /// <summary>
    /// Hides the healthbar
    /// </summary>
    public void Hide()
    {
        shown = false;
        rootAnimator.SetBool("Shown", false);
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
    /// Starts an explosion on the last glowing part 
    /// </summary>
    public void ExplodeLastGlowPart()
    {
        int toBlowUp = currentHealth - numberOfLastGlowing + 1;
        if (toBlowUp < 0 || toBlowUp > images.Length) return;

        explosionAnimator.GetComponent<RectTransform>().anchoredPosition = images[10 - toBlowUp].rectTransform.anchoredPosition;
        explosionAnimator.SetTrigger("Explode");
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
