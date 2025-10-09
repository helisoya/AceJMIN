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

    [Header("Flow")]
    [SerializeField] private HeathbarManager healthbarManager;
    [SerializeField] private EvidenceDisplayManager evidenceDisplayManager;
    [SerializeField] private GameObject[] inExaminationObjects;
    [SerializeField] private GameObject testimonyObj;
    [SerializeField] private GUIAnimationManager gUIAnimationManager;


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

    public bool testimonyActive { get { return testimonyObj.activeInHierarchy; } }
    public bool healthBarShown { get { return healthbarManager.shown; } }
    public int healthBarLength { get { return healthbarManager.currentHealth; } }
    public int healthBarLengthGlowing { get { return healthbarManager.numberOfLastGlowing; } }

    public bool notInMenu { get { return !pauseMenu.open && !saveMenu.isOpen && !caseFileMenu.open; } }
    public bool caseFileMenuOpen { get { return caseFileMenu.open; } }
    public string caseFileSelectedID { get { return caseFileMenu.SelectedItemID(); } }
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
    /// Starts a new GUI animation
    /// </summary>
    /// <param name="ID">The animation's ID</param>
    /// <returns>The animation's length</returns>
    public float StartGUIAnimation(string ID)
    {
        return gUIAnimationManager.StartNewAnimation(ID);
    }

    /// <summary>
    /// Removes a GUI Animation
    /// </summary>
    /// <param name="ID">The animation's ID</param>
    public void RemoveGUIAnimation(string ID)
    {
        gUIAnimationManager.DeleteAnimation(ID);
    }

    /// <summary>
    /// Removes all existing GUI Animations
    /// </summary>
    public void RemoveAllGUIAnimations()
    {
        gUIAnimationManager.DeleteAllAnimations();
    }

    /// <summary>
    /// Changes if the testimony GFX is active or not
    /// </summary>
    /// <param name="active">Is the GFX active ?</param>
    public void SetTestimonyActive(bool active)
    {
        testimonyObj.SetActive(active);
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
    /// Changes if the examination objects are active or not
    /// </summary>
    /// <param name="active">Are the examination objects active ?</param>
    public void SetInExaminationObjectsActive(bool active)
    {
        foreach (GameObject obj in inExaminationObjects)
        {
            obj.SetActive(active);
        }
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
    /// <param name="immediate">Should the change be immediate ?</param>
    public void FlashTo(float alpha, float speed, bool immediate = false)
    {
        if(immediate) flash.ForceAlphaTo(alpha);
        else flash.FadeTo(alpha, speed);
    }

    /// <summary>
    /// Fades the Background
    /// </summary>
    /// <param name="alpha">Alpha target</param>
    /// <param name="speed">Fading speed</param>
    /// <param name="immediate">Should the change be immediate ?</param>
    public void FadeBgTo(float alpha, float speed = 2, bool immediate = false)
    {
        if(immediate) fadeBg.ForceAlphaTo(alpha);
        else fadeBg.FadeTo(alpha, speed);
    }

    /// <summary>
    /// Fades the Foreground
    /// </summary>
    /// <param name="alpha">Alpha target</param>
    /// <param name="speed">Fading speed</param>
    /// <param name="immediate">Should the change be immediate ?</param>
    public void FadeFgTo(float alpha, float speed = 2, bool immediate = false)
    {
        if(immediate) fadeFg.ForceAlphaTo(alpha);
        else fadeFg.FadeTo(alpha, speed);
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
    /// Display evidence on the GUI
    /// </summary>
    /// <param name="side">The side where to display</param>
    /// <param name="evidence">The evidence to display</param>
    public void DisplayEvidence(EvidenceDisplayManager.EvidenceDisplaySide side, Evidence evidence)
    {
        evidenceDisplayManager.Display(side, evidence);
    }

    /// <summary>
    /// Gets the currently used side for displaying evidence
    /// </summary>
    /// <returns>The currently used side</returns>
    public EvidenceDisplayManager.EvidenceDisplaySide GetDisplayedSide()
    {
        return evidenceDisplayManager.GetCurrentSide();
    }

    /// <summary>
    /// Sets the visibility of the healthbar
    /// </summary>
    /// <param name="visible">Is the healthbar visible ?</param>
    public void SetHealthBarVisible(bool visible)
    {
        if (visible) healthbarManager.Show();
        else healthbarManager.Hide();
    }

    /// <summary>
    /// Changes the healthbar current length
    /// </summary>
    /// <param name="length">The new length</param>
    public void SetHealthBarLength(int length)
    {
        healthbarManager.UpdateHealth(length);
    }

    /// <summary>
    /// Sets the last X parts of the health bar to be glowing
    /// </summary>
    /// <param name="amount">The amount of parts that will be glowing</param>
    public void SetHealthBarGlowingParts(int amount)
    {
        healthbarManager.SetGlowForLast(amount);
    }

    /// <summary>
    /// Explodes the last glowing parts
    /// </summary>
    public void HeatlhBarExplodeLastGlowingPart()
    {
        healthbarManager.ExplodeLastGlowPart();
    }

    /// <summary>
    /// Gets the currently displayed evidence ID
    /// </summary>
    /// <returns>The ID</returns>
    public string GetDisplayedEvidenceID()
    {
        return evidenceDisplayManager.GetEvidenceID();
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
    /// <param name="playSound">Should the linked SFX be played ?</param>
    public void OpenSettings(bool isOnSaveButton = true, bool playSound = false)
    {
        if (!NovelController.instance.isReadyForSaving) return;

        ResetCursor();
        if (!pauseMenu.open) pauseMenu.Show(isOnSaveButton,playSound);
    }

    /// <summary>
    /// Opens the case file menu
    /// </summary>
    /// <param name="canPresent">Can evidence be presented ?</param>
    /// <param name="canExit">Can the menu be exited ?</param>
    public void OpenCaseFile(bool canExit, bool canPresent)
    {
        if (!NovelController.instance.isReadyForSaving && canExit) return;

        ResetCursor();
        if (!caseFileMenu.open) caseFileMenu.Show(canPresent, canExit);
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

    /// <summary>
    /// Click event for opening the case file menu
    /// </summary>
    public void Click_OpenCaseFile()
    {
        OpenCaseFile(true, NovelController.instance.isInExamination);
    }

    /// <summary>
    /// Click event for going in reverse in the examination mode
    /// </summary>
    public void Click_ExaminationGoReverse()
    {
        if (!NovelController.instance.isInExamination) return;
        NovelController.instance.SetNextIsBackward();
        NovelController.instance.Next();
    }

    /// <summary>
    /// Click event for pressing a statement
    /// </summary>
    public void Click_Press()
    {
        if (NovelController.instance.isInExamination) NovelController.instance.PressOnDeposition();
    }

    void Update()
    {
        if (cooldownForAction > 0)
        {
            cooldownForAction -= Time.deltaTime;
        }

        if (notInMenu && AJInput.Instance.GetOptionsDown())
        {
            OpenSettings(true,true);
        }

        if (notInMenu && cooldownForAction <= 0 && (AJInput.Instance.GetConfirmDown() || AJInput.Instance.GetMoveRightDown()))
        {
            SkipDialog();
        }

        if (notInMenu && cooldownForAction <= 0 && NovelController.instance.isInExamination && AJInput.Instance.GetMoveLeftDown())
        {
            Click_ExaminationGoReverse();
        }

        if (notInMenu && cooldownForAction <= 0 && NovelController.instance.isInExamination && AJInput.Instance.GetPressDown())
        {
            Click_Press();
        }

        if (notInMenu && AJInput.Instance.GetCourtRecordDown())
        {
            OpenCaseFile(true, NovelController.instance.isInExamination);
        }
    }
}
