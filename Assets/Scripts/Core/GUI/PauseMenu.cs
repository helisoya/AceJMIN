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
    [SerializeField] private Slider sliderBGM;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private Resolution[] resolutions;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    public bool open { get { return root.activeInHierarchy; } }

    [Header("Event System")]
    [SerializeField] private GameObject saveObj;
    [SerializeField] private GameObject loadObj;
    [SerializeField] private LocalizedText descriptionText;
    [SerializeField] private string[] descriptions;
    [SerializeField] private Image[] highlighters;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;
    private int currentIdx;


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

        List<string> list = new List<string>();

        int currentRes = 0;

        Resolution res;
        Resolution reference = Screen.currentResolution;
        for (int i = 0; i < resolutions.Length; i++)
        {
            res = resolutions[i];
            list.Add(res.width + "x" + res.height);
            if (currentRes == 0 && res.width == reference.width && res.height == reference.height && res.refreshRate == reference.refreshRate)
            {
                currentRes = i;
            }
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(list);
        resolutionDropdown.SetValueWithoutNotify(currentRes);
    }

    /// <summary>
    /// Highlight a new row
    /// </summary>
    /// <param name="rowIdx">The row's index</param>
    public void HighlightRow(int rowIdx)
    {
        if (currentIdx != rowIdx)
        {
            if (currentIdx != -1) highlighters[currentIdx].sprite = defaultSprite;
            currentIdx = rowIdx;
            highlighters[currentIdx].sprite = highlightedSprite;
            descriptionText.SetNewKey(descriptions[currentIdx]);
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
    /// <param name="value">The resolution's index</param>
    public void ChangeResolution(int value)
    {
        Resolution newRes = resolutions[value];
        Settings.SetResolution(newRes.width, newRes.height, newRes.refreshRate);
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
    }

}
