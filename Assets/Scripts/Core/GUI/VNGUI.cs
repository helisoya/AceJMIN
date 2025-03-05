using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Reprensents the VN's GUI
/// </summary>
public class VNGUI : MonoBehaviour
{
    public static VNGUI instance;

    [Header("Fades")]
    [SerializeField] private Fade fadeBg;
    [SerializeField] private Fade fadeFg;
    [SerializeField] private Fade flash;

    [Header("Pause")]
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private SaveMenu saveMenu;
    [SerializeField] private CaseFileMenu caseFileMenu;


    [Header("Interaction Mode")]
    [SerializeField] private GameObject interactionModeRoot;

    [Header("Speech Bubble")]
    [SerializeField] private Animator speechBubbleAnimator;
    [SerializeField] private Image speechBubbleImg;
    private float speechBubbleStart;

    private Coroutine routineExit;

    public bool fadingBg { get { return fadeBg.fading; } }
    public bool fadingFg { get { return fadeFg.fading; } }
    public float fadeBgAlpha { get { return fadeBg.currentAlpha; } }
    public float fadeFgAlpha { get { return fadeFg.currentAlpha; } }
    public Color fadeBgColor { get { return fadeBg.currenColor; } }
    public Color fadeFgColor { get { return fadeFg.currenColor; } }
    public bool fadingFlash { get { return flash.fading; } }
    public bool speechBubbleInProgress { get { return Time.time - speechBubbleStart < 1.1f; } }

    public bool notInMenu { get { return !pauseMenu.open && !saveMenu.isOpen && !caseFileMenu.open; } }
    private float cooldownForAction = 0;

    void Awake()
    {
        instance = this;

        fadeFg.ForceAlphaTo(1);
        fadeFg.FadeTo(0);
        fadeBg.ForceAlphaTo(1);
        fadeBg.FadeTo(0);

        flash.ForceAlphaTo(0);

        cooldownForAction = 0;
    }

    /// <summary>
    /// Sets the current cooldown for action (For instance, the action to pass a dialog)
    /// </summary>
    /// <param name="newValue">The new value</param>
    public void SetCoolDownForAction(float newValue)
    {
        cooldownForAction = newValue;
    }

    /// <summary>
    /// Show a speech bubble
    /// </summary>
    /// <param name="bubbleSprite">The speech bubble's sprite</param>
    public void ShowSpeechBubble(Sprite bubbleSprite)
    {
        speechBubbleImg.sprite = bubbleSprite;
        speechBubbleAnimator.SetTrigger("Action");
        speechBubbleStart = Time.time;
    }

    /// <summary>
    /// Flashes the screen to a set alpha
    /// </summary>
    /// <param name="alpha">The target alpha</param>
    /// <param name="speed">The flash's speed</param>
    public void FlashTo(float alpha, float speed)
    {
        flash.FadeTo(alpha, speed);
    }

    /// <summary>
    /// Fades the Background
    /// </summary>
    /// <param name="alpha">Alpha target</param>
    /// <param name="speed">Fading speed</param>
    public void FadeBgTo(float alpha, float speed = 2)
    {
        fadeBg.FadeTo(alpha, speed);
    }

    /// <summary>
    /// Fades the Foreground
    /// </summary>
    /// <param name="alpha">Alpha target</param>
    /// <param name="speed">Fading speed</param>
    public void FadeFgTo(float alpha, float speed = 2)
    {
        fadeFg.FadeTo(alpha, speed);
    }

    /// <summary>
    /// Changes the Background Fading's color
    /// </summary>
    /// <param name="color">The new color</param>
    public void SetBgColor(Color color)
    {
        fadeBg.SetColor(color);
    }

    /// <summary>
    /// Changes the Foreground Fading's color
    /// </summary>
    /// <param name="color">The new color</param>
    public void SetFgColor(Color color)
    {
        fadeFg.SetColor(color);
    }

    /// <summary>
    /// Force the Background to a set alpha
    /// </summary>
    /// <param name="alpha">The new alpha</param>
    public void ForceBgTo(float alpha)
    {
        fadeBg.ForceAlphaTo(alpha);
    }

    /// <summary>
    /// Force the Foreground to a set alpha
    /// </summary>
    /// <param name="alpha">The new alpha</param>
    public void ForceFgTo(float alpha)
    {
        fadeFg.ForceAlphaTo(alpha);
    }

    /// <summary>
    /// Resets the cursor
    /// </summary>
    private void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// Click event for loading to the main menu
    /// </summary>
    public void QuitToMainMenu()
    {
        if (routineExit != null) return;
        ResetCursor();

        routineExit = StartCoroutine(Routine_Exit());
    }

    IEnumerator Routine_Exit()
    {
        InteractionManager.instance.SetActive(false);
        AudioManager.instance.PlaySong(null);
        fadeFg.FadeTo(1);

        yield return new WaitForEndOfFrame();
        while (fadeFg.fading)
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Event for opening the settings
    /// </summary>
    /// <param name="isOnSaveButton">True if the cursor starts on the save button. False if it starts on the load button</param>
    public void OpenSettings(bool isOnSaveButton = true)
    {
        if (!NovelController.instance.isReadyForSaving) return;

        ResetCursor();
        if (!pauseMenu.open) pauseMenu.Show(isOnSaveButton);
    }

    /// <summary>
    /// Opens the case file menu
    /// </summary>
    public void OpenCaseFile()
    {
        if (!NovelController.instance.isReadyForSaving) return;

        ResetCursor();
        if (!caseFileMenu.open) caseFileMenu.Show();
    }

    /// <summary>
    /// Opens the save menu
    /// </summary>
    /// <param name="inSaveMode">True if in save mode. False if in load mode</param>
    public void OpenSaveMenu(bool inSaveMode)
    {
        ResetCursor();
        if (!saveMenu.isOpen) saveMenu.Open(inSaveMode);
    }

    /// <summary>
    /// Event for skipping the dialog
    /// </summary>
    public void SkipDialog()
    {
        NovelController.instance.Next();
    }

    /// <summary>
    /// Changes if the interaction mode is active or not
    /// </summary>
    /// <param name="active">Is the interaction mode active ?</param>
    public void SetInteractionMode(bool active)
    {
        interactionModeRoot.SetActive(active);
        if (active)
        {
            DialogSystem.instance.Close();
        }
    }

    void Update()
    {
        if (cooldownForAction > 0)
        {
            cooldownForAction -= Time.deltaTime;
        }

        if (notInMenu && AJInput.Instance.GetOptionsDown())
        {
            OpenSettings(true);
        }

        if (notInMenu && cooldownForAction <= 0 && AJInput.Instance.GetConfirmDown())
        {
            SkipDialog();
        }

        if (notInMenu && AJInput.Instance.GetCourtRecordDown())
        {
            OpenCaseFile();
        }
    }
}
