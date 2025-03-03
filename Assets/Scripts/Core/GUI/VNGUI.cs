using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Buttons")]
    [SerializeField] private Button[] buttonsRequiringSaveReady;
    [SerializeField] private Button loadButton;

    [Header("Pause")]
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private SaveMenu saveMenu;


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

    void Awake()
    {
        instance = this;

        fadeFg.ForceAlphaTo(1);
        fadeFg.FadeTo(0);
        fadeBg.ForceAlphaTo(1);
        fadeBg.FadeTo(0);

        flash.ForceAlphaTo(0);
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
    /// Click event for saving
    /// </summary>
    public void Save()
    {
        if (!NovelController.instance.isReadyForSaving) return;

        NovelController.instance.SaveGameFile("save");
    }

    /// <summary>
    /// Click event for loading
    /// </summary>
    public void Load()
    {
        if (GameManager.GetSaveManager().SaveFileExists("save"))
        {
            ResetCursor();
            NovelController.instance.LoadGameFile();
        }
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
    public void OpenSettings()
    {
        ResetCursor();
        if (!pauseMenu.open) pauseMenu.Show();
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
        foreach (Button button in buttonsRequiringSaveReady)
        {
            button.interactable = NovelController.instance.isReadyForSaving;
        }
        loadButton.interactable = loadButton.interactable && GameManager.GetSaveManager().SaveFileExists("save");

        if (NovelController.instance.isReadyForSaving && Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.open) pauseMenu.Close();
            else pauseMenu.Show();
        }

    }
}
