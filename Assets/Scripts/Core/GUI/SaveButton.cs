using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents a button linked to a save file
/// </summary>
public class SaveButton : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private LocalizedText caseNameText;
    [SerializeField] private LocalizedText caseDescText;
    [SerializeField] private TextMeshProUGUI slotText;

    [Header("Sprite")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite noFileSprite;
    [SerializeField] private Image backgroundImg;

    private SaveMenu parent;
    private SaveManager.SaveInfo linkedInfo;
    private int slot;

    /// <summary>
    /// Initialize the button
    /// </summary>
    /// <param name="saveInfo">The linked save</param>
    /// <param name="slot">The linked slot</param>
    /// <param name="parent">The button's parent</param>
    public void Init(SaveManager.SaveInfo saveInfo, int slot, SaveMenu parent)
    {
        linkedInfo = saveInfo;
        this.slot = slot;
        this.parent = parent;

        slotText.text = linkedInfo != null ? linkedInfo.saveName : "";
        caseNameText.SetNewKey(linkedInfo != null ? linkedInfo.caseName : "saves_none");
        caseDescText.SetNewKey(linkedInfo != null ? linkedInfo.caseDesc : null);
        caseNameText.SetColor(linkedInfo != null ? Color.black : Color.white);

        backgroundImg.sprite = linkedInfo != null ? normalSprite : noFileSprite;
    }

    /// <summary>
    /// On Click Event
    /// </summary>
    public void Click()
    {
        parent.ChooseSlot(slot, linkedInfo);
    }
}
