using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the Main Menu
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private string menuMusic;
    [SerializeField] private CaseSelector caseSelector;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private SaveMenu saveMenu;
    [SerializeField] private Fade fade;

    [Header("Main Screen")]
    [SerializeField] private GameObject mainScreenRoot;
    [SerializeField] private GameObject[] mainScreenButtons;


    [Header("Name Input")]
    [SerializeField] private GameObject nameInputRoot;
    [SerializeField] private TMP_InputField nameInput;


    [Header("Audio")]
    [SerializeField] private AudioClip moveSFX;
    [SerializeField] private AudioClip selectSFX;


    private Coroutine fading;

    public static MainMenuManager instance;


    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        AudioManager.instance.PlaySong(menuMusic);
        fade.ForceAlphaTo(1);
        fade.FadeTo(0);
    }

    public void PlaySelectSFX()
    {
        AudioManager.instance.PlaySFX(moveSFX);
    }

    /// <summary>
    /// Sets the currently active button on the main screen
    /// </summary>
    /// <param name="idx">The button's index</param>
    public void SetActiveMainScreenButton(int idx)
    {
        EventSystem.current.SetSelectedGameObject(mainScreenButtons[idx]);
    }

    /// <summary>
    /// Click event for starting a new game
    /// </summary>
    public void Event_NewGame()
    {
        if (fading != null) return;
        AudioManager.instance.PlaySFX(selectSFX);
        caseSelector.Event_Open();
    }

    /// <summary>
    /// Click event for confirming the name input and starting the game 
    /// </summary>
    public void Event_ConfirmName()
    {
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            GameManager.instance.SetSaveToLoad(null);
            GameManager.instance.SetNextChapter("Intro/Intro");
            GameManager.GetSaveManager().ResetItems();
            GameManager.GetSaveManager().EditItem("playerName", nameInput.text);
            AudioManager.instance.PlaySong(null);

            StartTransitionToVN();
        }
    }

    /// <summary>
    /// Click event for loading the saved game
    /// </summary>
    public void Event_Continue()
    {
        if (fading != null) return;
        saveMenu.Open(false, true);
    }

    /// <summary>
    /// Click event for opening the settings
    /// </summary>
    public void Event_OpenSettings()
    {
        if (fading != null) return;
        pauseMenu.Show(true);
    }

    /// <summary>
    /// Click event for quitting the game
    /// </summary>
    public void Event_Quit()
    {
        if (fading != null) return;
        Application.Quit();
    }

    /// <summary>
    /// Starts the transition to the VN Scene
    /// </summary>
    public void StartTransitionToVN()
    {
        if (fading != null) return;

        fading = StartCoroutine(Transition());
    }

    /// <summary>
    /// Routine for the transition to the VN Scene
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator Transition()
    {
        fade.FadeTo(1);

        yield return new WaitForEndOfFrame();
        while (fade.fading)
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene("VN");
    }
}
