using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the save menu
/// </summary>
public class SaveMenu : MonoBehaviour
{
    [Header("Save Menu")]
    [SerializeField] private GameObject root;
    [SerializeField] private LocalizedText titleText;
    [SerializeField] private SaveButton[] buttons;
    [SerializeField] private bool isInGame = true;
    [SerializeField] private Scrollbar scrollbar;

    [Header("Confirm choice")]
    [SerializeField] private GameObject confirmRoot;
    [SerializeField] private SaveButton confirmWidget;

    private int currentSlot;
    private SaveManager.SaveInfo currentInfo;

    private bool isInSavingMode;
    public bool isOpen { get { return root.activeInHierarchy; } }

    /// <summary>
    /// Closes the save menu
    /// </summary>
    /// <param name="isLoading">Is the menu loading a new save ?</param>
    public void Close(bool isLoading = false)
    {
        root.SetActive(false);

        if (isInGame && !isLoading)
        {
            VNGUI.instance.OpenSettings();
        }
    }

    /// <summary>
    /// Opens the save menu
    /// </summary>
    /// <param name="isInSavingMode">Is the menu in save mode ? False for load mode</param>
    public void Open(bool isInSavingMode)
    {
        this.isInSavingMode = isInSavingMode;

        root.SetActive(true);
        titleText.SetNewKey(isInSavingMode ? "saves_title_save" : "saves_title_load");

        SaveManager.SaveInfo[] infos = GameManager.GetSaveManager().GetAvailableSaves();

        for (int i = 0; i < infos.Length; i++)
        {
            buttons[i].Init(infos[i], i, this);
        }
    }

    void Update()
    {
        scrollbar.size = 0.01f;
    }

    /// <summary>
    /// Chooses a slot to save / load
    /// </summary>
    /// <param name="slot">The slot index</param>
    /// <param name="info">The slot's indo</param>
    public void ChooseSlot(int slot, SaveManager.SaveInfo info)
    {
        if ((!isInSavingMode && info == null) || (isInSavingMode && slot == 0))
        {
            // Can't choose this
            // Put Animation here
        }
        else
        {
            // Can choose

            if (isInSavingMode && info == null)
            {
                NovelController.instance.SaveGameFile(slot.ToString());
                Open(isInSavingMode);
            }
            else
            {
                confirmRoot.SetActive(true);
                confirmWidget.Init(info, slot, this);
                currentInfo = info;
                currentSlot = slot;
            }

        }
    }


    public void Click_Confirm()
    {
        confirmRoot.SetActive(false);
        if (isInSavingMode)
        {
            NovelController.instance.SaveGameFile(currentSlot.ToString());
            Open(isInSavingMode);
        }
        else
        {
            NovelController.instance.LoadGameFile(currentInfo.saveName);
            Close(true);
        }
    }

    public void Click_Back()
    {
        confirmRoot.SetActive(false);
    }

}
