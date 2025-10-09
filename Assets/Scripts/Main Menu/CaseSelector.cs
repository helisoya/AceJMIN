using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents the case selection menu on the main menu
/// </summary>
public class CaseSelector : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] private GameObject root;
    [SerializeField] private MenuCase[] cases;
    [SerializeField] private Sprite unknownCaseSprite;
    [SerializeField] private RectTransform casesRoot;
    [SerializeField] private LocalizedText caseTitle;

    [Header("Audio")]
    [SerializeField] private AudioClip moveSFX;
    [SerializeField] private AudioClip selectSFX;
    [SerializeField] private AudioClip cancelSFX;


    [Header("Control")]
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    private int currentCase;

    /// <summary>
    /// Event for openning the case selection menu
    /// </summary>
    public void Event_Open()
    {
        for (int i = 0; i < cases.Length; i++)
        {
            if (i > Settings.GetCaseProgress()) cases[i].spriteRenderer.sprite = unknownCaseSprite;
        }

        currentCase = 0;
        root.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        IncrementCase(0);
    }

    /// <summary>
    /// Event for openning the case selection menu
    /// </summary>
    public void Event_Close()
    {
        AudioManager.instance.PlaySFX(cancelSFX);
        root.SetActive(false);
        MainMenuManager.instance.SetActiveMainScreenButton(0);
    }

    /// <summary>
    /// Event for choosing a case
    /// </summary>
    public void Event_ChooseCase()
    {
        AudioManager.instance.PlaySFX(selectSFX);
        GameManager.instance.SetSaveToLoad(null);
        GameManager.instance.SetNextChapter(cases[currentCase].entryPoint);
        GameManager.GetSaveManager().ResetItems();
        AudioManager.instance.PlaySong(null);
        MainMenuManager.instance.StartTransitionToVN();
    }

    /// <summary>
    /// Increments the current case
    /// </summary>
    /// <param name="side">The side of the incrementation</param>
    public void IncrementCase(int side)
    {
        AudioManager.instance.PlaySFX(moveSFX);
        currentCase += side;

        casesRoot.anchoredPosition = new Vector2(-500 * currentCase, 0);
        caseTitle.SetNewKey(cases[currentCase].titleKey);

        leftButton.SetActive(currentCase != 0);
        rightButton.SetActive(currentCase != cases.Length - 1 && currentCase < Settings.GetCaseProgress());
    }

    void Update()
    {
        if (!root.activeInHierarchy) return;

        if (AJInput.Instance.GetCancelDown()) Event_Close();
        if (AJInput.Instance.GetConfirmDown()) Event_ChooseCase();
        else if (AJInput.Instance.GetMoveLeftDown() && currentCase > 0)
        {
            IncrementCase(-1);
        }
        else if (AJInput.Instance.GetMoveRightDown() && currentCase < cases.Length - 1 && currentCase < Settings.GetCaseProgress())
        {
            IncrementCase(1);
        }
    }


    [System.Serializable]
    public class MenuCase
    {
        public Image spriteRenderer;
        public string titleKey;
        public string entryPoint;
    }
}
