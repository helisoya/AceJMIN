using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Represents a button on the case file menu
/// </summary>
public class CaseFileButton : MonoBehaviour, ISelectHandler
{
    [Header("Components")]
    [SerializeField] private Image evidenceSprite;
    [SerializeField] private Image backgroundImg;
    [SerializeField] private Button button;

    [Header("Sprites")]
    [SerializeField] private Sprite spriteDisabled;
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteHighlighted;

    private Evidence linkedEvidence;
    private CaseFileMenu parent;
    private int buttonIndex;

    /// <summary>
    /// Initialize the button
    /// </summary>
    /// <param name="evidence">The linked evidence. Null if the button is disabled</param>
    /// <param name="menu">The root menu</param>
    /// <param name="index">The button's index</param>
    public void Init(Evidence evidence, CaseFileMenu menu, int index)
    {
        buttonIndex = index;
        parent = menu;
        linkedEvidence = evidence;
        backgroundImg.sprite = evidence != null ? spriteNormal : spriteDisabled;
        evidenceSprite.sprite = evidence != null ? evidence.GetSprite() : null;
        evidenceSprite.color = evidence != null ? Color.white : Color.clear;
        button.interactable = evidence != null;
    }

    /// <summary>
    /// Sets the button to hightlighted
    /// </summary>
    public void Highlight()
    {
        backgroundImg.sprite = spriteHighlighted;
    }

    /// <summary>
    /// Sets the button to normal
    /// </summary>
    public void Normal()
    {
        backgroundImg.sprite = spriteNormal;
    }

    /// <summary>
    /// OnClick Event
    /// </summary>
    public void Click()
    {
        OnSelect(null);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (linkedEvidence != null)
        {
            parent.SelectEvidence(linkedEvidence);
            parent.HighlightButton(buttonIndex);
        }
    }
}
