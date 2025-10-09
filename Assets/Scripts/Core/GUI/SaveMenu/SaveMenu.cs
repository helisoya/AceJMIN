using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [Header("Audio")]
    [SerializeField] private AudioClip openSFX;
    [SerializeField] private AudioClip closeSFX;
    [SerializeField] private AudioClip selectSFX;
    [SerializeField] private AudioClip cantSFX;

    [Header("Event System")]
    [SerializeField] private GameObject objConfirm;
    private int currentButtonIdx;

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
        AudioManager.instance.PlaySFX(closeSFX);

        root.SetActive(false);

        if (isInGame && !isLoading)
        {
            VNGUI.instance.OpenSettings(isInSavingMode);
        }
        else if (!isInGame)
        {
            MainMenuManager.instance.SetActiveMainScreenButton(1);
        }
    }

    /// <summary>
    /// Opens the save menu
    /// </summary>
    /// <param name="isInSavingMode">Is the menu in save mode ? False for load mode</param>
    /// <param name="resetEventSystem">Reset the event system ?</param>
    public void Open(bool isInSavingMode, bool resetEventSystem = true)
    {
        AudioManager.instance.PlaySFX(openSFX);

        this.isInSavingMode = isInSavingMode;

        root.SetActive(true);
        titleText.SetNewKey(isInSavingMode ? "saves_title_save" : "saves_title_load");

        SaveManager.SaveInfo[] infos = GameManager.GetSaveManager().GetAvailableSaves();

        for (int i = 0; i < infos.Length; i++)
        {
            buttons[i].Init(infos[i], i, this);
        }

        if (resetEventSystem)
        {
            currentButtonIdx = 0;
            EventSystem.current.SetSelectedGameObject(buttons[currentButtonIdx].gameObject);
            scrollbar.value = 1;
        }
    }

    void Update()
    {
        scrollbar.size = 0.01f;

        if (!isOpen) return;

        if (AJInput.Instance.GetCancelDown())
        {
            if (confirmRoot.activeInHierarchy)
            {
                Click_Back();
            }
            else
            {
                Close(false);
            }
        }

        if (!confirmRoot.activeInHierarchy)
        {
            int side = (AJInput.Instance.GetMoveDownDown() ? 1 : 0) - (AJInput.Instance.GetMoveUpDown() ? 1 : 0);

            if (side != 0)
            {
                AudioManager.instance.PlaySFX(selectSFX);
                currentButtonIdx = (currentButtonIdx + side + buttons.Length) % buttons.Length;
                scrollbar.value = 1.0f - (currentButtonIdx / 10.0f);
                EventSystem.current.SetSelectedGameObject(buttons[currentButtonIdx].gameObject);
            }
        }


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
            AudioManager.instance.PlaySFX(cantSFX);
        }
        else
        {
            // Can choose
            AudioManager.instance.PlaySFX(openSFX);

            if (isInSavingMode && info == null)
            {
                NovelController.instance.SaveGameFile(slot.ToString());
                Open(isInSavingMode, false);
            }
            else
            {
                confirmRoot.SetActive(true);
                confirmWidget.Init(info, slot, this);
                currentInfo = info;
                currentSlot = slot;

                EventSystem.current.SetSelectedGameObject(objConfirm);
            }

        }
    }


    public void Click_Confirm()
    {
        AudioManager.instance.PlaySFX(selectSFX);
        confirmRoot.SetActive(false);
        if (isInSavingMode)
        {
            NovelController.instance.SaveGameFile(currentSlot.ToString());
            Open(isInSavingMode, false);

            currentButtonIdx = currentSlot;
            scrollbar.value = 1.0f - (currentButtonIdx / 10.0f);
            EventSystem.current.SetSelectedGameObject(buttons[currentSlot].gameObject);
        }
        else
        {
            if (isInGame)
            {
                NovelController.instance.LoadGameFile(currentInfo.saveName);
                VNGUI.instance.SetCoolDownForAction(0.1f);
                Close(true);
            }
            else
            {
                GameManager.instance.SetSaveToLoad(currentInfo.saveName);
                GameManager.instance.SetNextChapter("");
                MainMenuManager.instance.StartTransitionToVN();
                Close(true);
            }

        }
    }

    public void Click_Back()
    {
        AudioManager.instance.PlaySFX(closeSFX);
        confirmRoot.SetActive(false);

        currentButtonIdx = currentSlot;
        scrollbar.value = 1.0f - (currentButtonIdx / 10.0f);
        EventSystem.current.SetSelectedGameObject(buttons[currentSlot].gameObject);
    }

}
