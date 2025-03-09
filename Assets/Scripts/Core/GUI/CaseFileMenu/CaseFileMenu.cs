using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

/// <summary>
/// Represents the case file menu
/// </summary>
public class CaseFileMenu : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject root;
    [SerializeField] private LocalizedText tabNameText;
    [SerializeField] private LocalizedText switchButtonText;
    [SerializeField] private LocalizedText itemNameText;
    [SerializeField] private LocalizedText itemDescText;
    [SerializeField] private GameObject checkButton;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject presentButton;
    [SerializeField] private Image itemSprite;
    [SerializeField] private RectTransform markerRoot;
    [SerializeField] private Transform caseFileButtonsRoot;
    [SerializeField] private PauseMenuGraphicObject[] pageMarkers;
    [SerializeField] private GameObject[] sideButtons;
    private CaseFileButton[] buttons;
    public bool open { get { return root.activeInHierarchy; } }

    [Header("Check Mode")]
    [SerializeField] private GameObject checkModeRoot;
    [SerializeField] private GameObject checkModeMultipleRoot;
    [SerializeField] private Image checkModeImage;
    [SerializeField] private PauseMenuGraphicObject[] checkModeMarkers;


    [Header("Control")]
    private bool isInEvidenceMode;
    private bool isInCheckMode;
    private int currentEvidenceIdx;
    private int currentProfileIdx;
    private int currentMaxPages;
    private int currentTotalItems;
    private Evidence currentItem;
    private int checkModeCurrentIndex;
    private bool canPresent;
    private bool canExit;


    void Awake()
    {
        buttons = new CaseFileButton[caseFileButtonsRoot.childCount];
        for (int i = 0; i < caseFileButtonsRoot.childCount; i++)
        {
            buttons[i] = caseFileButtonsRoot.GetChild(i).GetComponent<CaseFileButton>();
        }
    }

    /// <summary>
    /// Gets the selected item's ID
    /// </summary>
    /// <returns>The selected item's ID</returns>
    public string SelectedItemID()
    {
        if (currentItem == null) return null;
        return currentItem.ID;
    }

    /// <summary>
    /// Shows the menu
    /// </summary>
    /// <param name="canPresent">Can evidence be presented ?</param>
    /// <param name="canExit">Can the menu be exited ?</param>
    public void Show(bool canPresent, bool canExit)
    {
        root.SetActive(true);
        currentEvidenceIdx = 0;
        currentProfileIdx = 0;
        this.canExit = canExit;
        this.canPresent = canPresent;
        exitButton.SetActive(canExit);
        SwitchToEvidence();
    }

    /// <summary>
    /// Hide the menu
    /// </summary>
    public void Hide()
    {
        root.SetActive(false);
    }

    /// <summary>
    /// Switch to the evidence view
    /// </summary>
    public void SwitchToEvidence()
    {
        isInEvidenceMode = true;
        tabNameText.SetNewKey("casefiles_evidence");
        switchButtonText.SetNewKey("casefiles_profiles");
        SetAvaiableItems(GameManager.GetEvidenceManager().GetAvailableEvidence());
        presentButton.SetActive(canPresent);
        buttons[currentEvidenceIdx].OnSelect(null);
    }

    /// <summary>
    /// Switch to the profile view
    /// </summary>
    public void SwitchToProfiles()
    {
        isInEvidenceMode = false;
        tabNameText.SetNewKey("casefiles_profiles");
        switchButtonText.SetNewKey("casefiles_evidence");
        SetAvaiableItems(GameManager.GetEvidenceManager().GetAvailableProfiles());
        presentButton.SetActive(false);
        buttons[currentProfileIdx].OnSelect(null);
    }

    /// <summary>
    /// Changes the currently available items
    /// </summary>
    /// <param name="evidence">The evidences</param>
    private void SetAvaiableItems(List<Evidence> evidence)
    {
        currentTotalItems = evidence.Count;
        currentMaxPages = Mathf.CeilToInt(currentTotalItems / 8.0f);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].Init(i < currentTotalItems ? evidence[i] : null, this, i);
        }

        for (int i = 0; i < pageMarkers.Length; i++)
        {
            if (i >= currentMaxPages)
            {
                pageMarkers[i].SetActive(false);
            }
            else
            {
                pageMarkers[i].SetActive(true);
                pageMarkers[i].Normal();
            }
        }

        foreach (GameObject sideButton in sideButtons)
        {
            sideButton.SetActive(currentMaxPages > 1);
        }
    }

    /// <summary>
    /// Displays an evidence's data
    /// </summary>
    /// <param name="evidence">The evidence</param>
    public void SelectEvidence(Evidence evidence)
    {
        currentItem = evidence;
        int evidenceValue = int.Parse(GameManager.GetSaveManager().GetItem(evidence.ID));
        itemDescText.SetNewKey(evidence.GetDesc(evidenceValue));
        itemNameText.SetNewKey(evidence.Name);
        itemSprite.sprite = evidence.GetSprite();

        checkButton.SetActive(evidence.GetNumberOfChecks() > 0);
    }

    /// <summary>
    /// Highlight a button
    /// </summary>
    /// <param name="idx">The button's index</param>
    public void HighlightButton(int idx)
    {

        if (isInEvidenceMode)
        {
            if (currentEvidenceIdx != idx) buttons[currentEvidenceIdx].Normal();
            currentEvidenceIdx = idx;
        }
        else
        {
            if (currentProfileIdx != idx) buttons[currentProfileIdx].Normal();
            currentProfileIdx = idx;
        }

        buttons[idx].Highlight();
        if (EventSystem.current.currentSelectedGameObject != buttons[idx].gameObject) EventSystem.current.SetSelectedGameObject(buttons[idx].gameObject);

        int idxInRow = idx % 8;
        markerRoot.anchoredPosition = new Vector2(-227 + idxInRow * 65, markerRoot.anchoredPosition.y);
        SetHighlightedMarker(Mathf.FloorToInt(idx / 8.0f));
    }

    /// <summary>
    /// Sets the highlighted page marker
    /// </summary>
    /// <param name="idx">The marker's idx</param>
    private void SetHighlightedMarker(int idx)
    {
        for (int i = 0; i < pageMarkers.Length; i++)
        {
            if (i == idx) pageMarkers[i].Highlight();
            else pageMarkers[i].Normal();
        }
        caseFileButtonsRoot.GetComponent<RectTransform>().anchoredPosition = new Vector3(-520 * idx, 0);
    }

    /// <summary>
    /// Increments the current page
    /// </summary>
    /// <param name="side">The increment's side</param>
    public void IncrementRow(int side)
    {
        int currentButton = isInEvidenceMode ? currentEvidenceIdx : currentProfileIdx;
        int currentPage = Mathf.FloorToInt(currentButton / 8.0f);
        int newPage = (currentPage + side + currentMaxPages) % currentMaxPages;

        HighlightButton(
            side < 0 ? Mathf.Min((newPage * 8) + 7, currentTotalItems - 1) : newPage * 8
        );
    }

    /// <summary>
    /// Switch the tabs
    /// </summary>
    public void SwitchTabs()
    {
        if (isInEvidenceMode) SwitchToProfiles();
        else SwitchToEvidence();
    }

    /// <summary>
    /// Present evidence
    /// </summary>
    public void PresentEvidence()
    {
        Hide();
        if (NovelController.instance.isInExamination) NovelController.instance.PresentEvidence(currentItem.ID);
    }

    /// <summary>
    /// Closes the check mode
    /// </summary>
    public void HideCheckMode()
    {
        isInCheckMode = false;
        checkModeRoot.SetActive(false);
        EventSystem.current.SetSelectedGameObject(buttons[currentEvidenceIdx].gameObject);
    }

    /// <summary>
    /// Opens the check mode
    /// </summary>
    public void OpenCheckMode()
    {
        isInCheckMode = true;
        checkModeRoot.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        if (currentItem.GetNumberOfChecks() > 1)
        {
            checkModeMultipleRoot.SetActive(true);
            for (int i = 0; i < checkModeMarkers.Length; i++)
            {
                if (i >= currentItem.GetNumberOfChecks())
                {
                    checkModeMarkers[i].SetActive(false);
                }
                else
                {
                    checkModeMarkers[i].SetActive(true);
                    checkModeMarkers[i].Normal();
                }
            }
        }
        else
        {
            checkModeMultipleRoot.SetActive(false);
        }

        checkModeCurrentIndex = 0;
        IncrementCheckModeIndex(0);
    }

    /// <summary>
    /// Increments the check mode's index
    /// </summary>
    /// <param name="side">The increment's side</param>
    public void IncrementCheckModeIndex(int side)
    {
        checkModeMarkers[checkModeCurrentIndex].Normal();
        checkModeCurrentIndex = (checkModeCurrentIndex + side + currentItem.GetNumberOfChecks()) % currentItem.GetNumberOfChecks();
        checkModeMarkers[checkModeCurrentIndex].Highlight();
        checkModeImage.sprite = currentItem.GetCheckSprite(checkModeCurrentIndex);
    }

    void Update()
    {
        if (!open) return;

        if (AJInput.Instance.GetCancelDown())
        {
            if (isInCheckMode)
            {
                HideCheckMode();
            }
            else if (canExit)
            {
                Hide();
            }
        }

        if (!isInCheckMode)
        {
            if (AJInput.Instance.GetProfileEvidenceDown())
            {
                SwitchTabs();
            }

            if (AJInput.Instance.GetConfirmDown() && currentItem != null && currentItem.GetNumberOfChecks() > 0)
            {
                OpenCheckMode();
            }

            if (AJInput.Instance.GetPresentDown() && canPresent && currentItem != null)
            {
                PresentEvidence();
            }
        }
        else if (currentItem.GetNumberOfChecks() > 1)
        {
            int side = (AJInput.Instance.GetMoveRightDown() ? 1 : 0) - (AJInput.Instance.GetMoveLeftDown() ? 1 : 0);
            if (side != 0)
            {
                IncrementCheckModeIndex(side);
            }
        }

    }

}
