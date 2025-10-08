using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogSystem : MonoBehaviour
{
    public static DialogSystem instance;

    [SerializeField] private ELEMENTS elements;
    [SerializeField] private AudioSource sfxSource;

    private string currentSpeakerID;

    public bool isSpeaking { get { return speaking != null; } }
    [HideInInspector] public bool isWaitingForUserInput = false;

    [HideInInspector] public string targetSpeech = "";
    private Coroutine speaking = null;
    public TextArchitect textArchitect { get; private set; }
    private string currentCharacterID = null;
    public List<string> currentTextsIds { get; private set; }
    public HorizontalAlignmentOptions dialogAlignement
    {
        get
        {
            return elements.speechText.horizontalAlignment;
        }
    }


    void Awake()
    {
        instance = this;

        currentTextsIds = new List<string>();
    }

    /// <summary>
    /// Changes the currently speaking character
    /// </summary>
    /// <param name="newID">The new character's ID</param>
    public void SetCurrentCharacterID(string newID)
    {
        if (currentCharacterID != null)
        {
            CharacterManager.instance.SetCharacterTalking(currentCharacterID, false);
        }

        currentSpeakerID = newID;

        if (currentCharacterID != null)
        {
            CharacterManager.instance.SetCharacterTalking(currentCharacterID, true);
        }
    }

    /// <summary>
    /// Justify the dialog box
    /// </summary>
    /// <param name="alignement">The new alignement</param>
    public void JustifyDialog(HorizontalAlignmentOptions alignement)
    {
        elements.speechText.horizontalAlignment = alignement;
    }

    /// <summary>
    /// Say something and show it on the speech box.
    /// </summary>
    public void Say(string speech, string speaker = "", string characterID = null, bool additive = false, string sfxName = "BlipMale")
    {
        StopSpeaking();

        if (additive)
        {
            speechText.text = targetSpeech;

        }

        speaking = StartCoroutine(Speaking(speech, additive, characterID, speaker,sfxName));
    }


    public void StopSpeaking()
    {
        if (isSpeaking)
        {
            StopCoroutine(speaking);
        }
        if (textArchitect != null && textArchitect.isConstructing)
        {
            textArchitect.Stop();
        }
        speaking = null;
    }



    public void UpdateTextLanguage()
    {
        if (speechText == null) return;
        string speaker = new string(currentSpeakerID);
        TagManager.Inject(ref speaker);
        string speech = DetermineGlobalSpeechText();
        TagManager.Inject(ref speech, false);

        speechText.text = speech;
        speakerNameText.text = speaker;
        speechText.maxVisibleCharacters = speech?.Length ?? 0;
    }

    IEnumerator Speaking(string speech, bool additive, string characterID, string speaker = "",string sfxName = "BlipMale")
    {
        speechPanel.SetActive(true);

        currentSpeakerID = new string(speaker);
        TagManager.Inject(ref speaker);

        if (!additive)
        {
            currentTextsIds.Clear();
        }
        currentTextsIds.Add(speech);

        speech = Locals.GetLocal(speech);
        TagManager.Inject(ref speech, false);

        string additiveSpeech = additive ? speechText.text : "";
        targetSpeech = additiveSpeech + speech;

        sfxSource.Stop();
        sfxSource.clip = Resources.Load("Audio/SFX/" + sfxName) as AudioClip;

        textArchitect = new TextArchitect(speechText.GetComponent<TextMeshProUGUI>(), speech, sfxSource, additiveSpeech);

        speakerNameText.text = DetermineSpeaker(speaker);
        speakerNameText.transform.parent.gameObject.SetActive(speakerNameText.text != "" && speakerNameText.text != "narrator");

        SetCurrentCharacterID(characterID);

        isWaitingForUserInput = false;

        while (textArchitect.isConstructing)
        {
            yield return new WaitForEndOfFrame();
        }
        //if skipping stopped the display text from updating correctly, force it to update at the end.

        SetCurrentCharacterID(null);

        //text finished
        isWaitingForUserInput = true;
        while (isWaitingForUserInput)
        {
            yield return new WaitForEndOfFrame();
        }

        StopSpeaking();
    }

    string DetermineGlobalSpeechText()
    {
        string globalText = "";
        string[] split;

        foreach (string id in currentTextsIds)
        {
            split = Locals.GetLocal(id).Split(new char[] { '[', ']' });
            for (int i = 0; i < split.Length; i += 2)
            {
                globalText += split[i];
            }
        }

        return globalText;

    }

    string DetermineSpeaker(string s)
    {
        string retVal = speakerNameText.text;
        if (s != speakerNameText.text && s != "")
            retVal = s.ToLower().Contains("narrator") ? "" : s;

        return retVal;
    }

    public void Close()
    {
        StopSpeaking();
        foreach (GameObject ob in speechPanelRequirements)
        {
            ob.SetActive(false);
        }
    }

    public void OpenAllRequirementsForDialogueSystemVisibility(bool v)
    {
        for (int i = 0; i < speechPanelRequirements.Length; i++)
        {
            speechPanelRequirements[i].SetActive(v);
        }
    }

    public void Open(string speakerName = "", List<string> speech = null)
    {
        if (speakerName == "" && (speech == null || speech.Count == 0))
        {
            OpenAllRequirementsForDialogueSystemVisibility(false);
            return;
        }
        OpenAllRequirementsForDialogueSystemVisibility(true);

        speakerNameText.text = speakerName;

        speakerNamePane.SetActive(speakerName != "" && speakerName != "narrator");

        currentTextsIds = speech;
        string textToDisplay = DetermineGlobalSpeechText();
        TagManager.Inject(ref textToDisplay, false);

        speechText.text = textToDisplay;
        speechText.maxVisibleCharacters = textToDisplay.Length;
    }

    public bool isClosed
    {
        get { return !speechBox.activeInHierarchy; }
    }

    [System.Serializable]
    public class ELEMENTS
    {
        public GameObject speechPanel;

        public GameObject speakerNamePane;
        public TMP_Text speakerNameText;

        public TMP_Text speechText;
    }
    public GameObject speechPanel { get { return elements.speechPanel; } }
    public TMP_Text speakerNameText { get { return elements.speakerNameText; } }
    public TMP_Text speechText { get { return elements.speechText; } }

    public GameObject speakerNamePane { get { return elements.speakerNamePane; } }

    public GameObject[] speechPanelRequirements;
    public GameObject speechBox;
}
