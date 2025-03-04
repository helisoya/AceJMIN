using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Represents the pause menu
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private bool isInGame = true;
    [SerializeField] private GameObject root;

    [Header("Data")]
    [SerializeField] private Slider sliderBGM;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private TextMeshProUGUI resolutionText;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private LocalizedText descriptionText;
    public bool open { get { return root.activeInHierarchy; } }

    [Header("Control")]
    [SerializeField] private GameObject saveObj;
    [SerializeField] private GameObject loadObj;
    [SerializeField] private PauseMenuRow[] rows;
    private Resolution[] resolutions;
    private int currentIdx;
    private int currentRes;


    /// <summary>
    /// Resets the options values
    /// </summary>
    void ResetValues()
    {
        SETTINGSAVE save = Settings.GetSave();
        sliderBGM.SetValueWithoutNotify(save.valBGM);
        sliderSFX.SetValueWithoutNotify(save.valSFX);
        fullscreenToggle.SetIsOnWithoutNotify(save.fullscreen);


        resolutions = Screen.resolutions;
        currentRes = 0;

        Resolution res;
        Resolution reference = Screen.currentResolution;
        for (int i = 0; i < resolutions.Length; i++)
        {
            res = resolutions[i];
            if (currentRes == 0 && res.width == reference.width && res.height == reference.height && res.refreshRate == reference.refreshRate)
            {
                currentRes = i;
                UpdateResolutionText(res);
            }
        }
    }

    /// <summary>
    /// Update the resolution text
    /// </summary>
    /// <param name="resolution">The new resolution to display</param>
    private void UpdateResolutionText(Resolution resolution)
    {
        resolutionText.text = resolution.width + "x" + resolution.height + "(" + resolution.refreshRate + ")";
    }

    /// <summary>
    /// Highlight a new row
    /// </summary>
    /// <param name="rowIdx">The row's index</param>
    public void HighlightRow(int rowIdx)
    {
        if (currentIdx != rowIdx)
        {
            if (currentIdx != -1) rows[currentIdx].Normal();
            currentIdx = rowIdx;
            rows[currentIdx].Highlight();
            descriptionText.SetNewKey(rows[currentIdx].GetDescriptionID());
        }
    }

    /// <summary>
    /// Shows the pause menu
    /// </summary>
    /// <param name="isOnSaveButton">True if the cursor starts on the save button. False if it starts on the load button</param>
    public void Show(bool isOnSaveButton = true)
    {
        ResetValues();
        root.SetActive(true);
        HighlightRow(isInGame ? 0 : 1);
        EventSystem.current.SetSelectedGameObject(isOnSaveButton ? saveObj : loadObj);
    }

    /// <summary>
    /// Changes the BGM volume
    /// </summary>
    public void SetBGM()
    {
        Settings.SetVolumeBGM(sliderBGM.value);
    }

    /// <summary>
    /// Changes the SFX volume
    /// </summary>
    public void SetSFX()
    {
        Settings.SetVolumeSFX(sliderSFX.value);
    }

    /// <summary>
    /// Changes the game's resolution
    /// </summary>
    /// <param name="idx">The resolution's index</param>
    private void ChangeResolution(int idx)
    {
        Resolution newRes = resolutions[idx];
        Settings.SetResolution(newRes.width, newRes.height, newRes.refreshRate);
        UpdateResolutionText(newRes);
    }

    /// <summary>
    /// Increment the resolution
    /// </summary>
    /// <param name="side">The side of the incrementation</param>
    public void IncrementResolution(int side)
    {
        currentRes = (currentRes + side + resolutions.Length) % resolutions.Length;
        ChangeResolution(currentRes);
    }


    /// <summary>
    /// Changes if the game is in fullscreen or not
    /// </summary>
    /// <param name="value">Is the game in fullscreen ?</param>
    public void ChangeFullScreen(bool value)
    {
        Settings.SetFullscreen(value);
    }

    /// <summary>
    /// Closes the pause menu
    /// </summary>
    public void Close()
    {
        root.SetActive(false);
    }

    /// <summary>
    /// Changes the game's language
    /// </summary>
    /// <param name="newVal">The new language's code</param>
    public void ChangeLanguage(string newVal)
    {
        Settings.ChangeLanguage(newVal);
    }

    /// <summary>
    /// Opens the save menu
    /// </summary>
    public void OpenSaveMenu()
    {
        Close();
        VNGUI.instance.OpenSaveMenu(true);
    }

    /// <summary>
    /// Opens the load menu
    /// </summary>
    public void OpenLoadMenu()
    {
        Close();
        VNGUI.instance.OpenSaveMenu(false);
    }

    /// <summary>
    /// Returns to the main menu
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (isInGame) SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        if (!open) return;

        if (AJInput.Instance.GetCancelDown())
        {
            Close();
        }
        if (AJInput.Instance.GetReturnToTitleDown())
        {
            ReturnToMainMenu();
        }

        if (currentIdx == 3)
        {
            // Change Resolution
            int side = (AJInput.Instance.GetMoveRightDown() ? 1 : 0) - (AJInput.Instance.GetMoveLeftDown() ? 1 : 0);
            if (side != 0)
            {
                IncrementResolution(side);
            }
        }
    }

}
