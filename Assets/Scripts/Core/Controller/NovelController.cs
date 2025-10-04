using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NovelController : MonoBehaviour
{
    public static NovelController instance;

    public bool isReadyForSaving
    {
        get
        {
            return waitingForUserToEndDialog || currentChoice != null || inInteractionMode;
        }
    }

    public bool isInExamination
    {
        get
        {
            return currentExamination != null;
        }
    }

    private List<string> data = new List<string>();
    private string activeChapterFile = "";
    private bool next = false;
    private Coroutine handlingChapterFile = null;
    private int chapterProgress = 0;
    private Choice currentChoice;
    private bool waitingForUserToEndDialog;
    private bool inInteractionMode;

    private string currentCaseName;
    private string currentCaseDesc;

    private bool loadedAutoSave;

    private Examination currentExamination = null;
    private int lastIdxExamination = 0;
    private List<int> examinationPressed;
    private bool nextIsBackward;

    public void Next()
    {
        next = true;
    }

    public void SetNextIsBackward()
    {
        nextIsBackward = true;
    }

    void Start()
    {
        instance = this;

        currentExamination = null;
        examinationPressed = new List<int>();

        print("Is Loading Save : " + GameManager.instance.IsLoadingSave());
        if (GameManager.instance.IsLoadingSave())
        {
            LoadGameFile(GameManager.instance.GetSaveToLoad());
        }
        else
        {
            LoadChapterFile(GameManager.instance.GetNextChapter());
        }
    }



    public void LoadGameFile(string saveName = "save")
    {
        //GameManager.instance.SetSaveToLoad(saveName);
        GAMEFILE activeGameFile = GameManager.GetSaveManager().Load(saveName);

        VNGUI.instance.ForceBgTo(activeGameFile.fadeBg);
        VNGUI.instance.ForceFgTo(activeGameFile.fadeFg);
        VNGUI.instance.SetBgColor(activeGameFile.colorBg);
        VNGUI.instance.SetFgColor(activeGameFile.colorFg);
        InteractionManager.instance.SetActive(false);

        CameraController.instance.SetPosition(activeGameFile.cameraPosition, true);
        CameraController.instance.SetRotation(activeGameFile.cameraRotation, true);

        data = FileManager.ReadTextAsset(Resources.Load<TextAsset>($"Story/{activeGameFile.chapterName}"));
        activeChapterFile = activeGameFile.chapterName;

        CharacterManager.instance.RemoveAllCharacters();
        List<GAMEFILE.CHARACTERDATA> characters = activeGameFile.characterInScene;
        foreach (GAMEFILE.CHARACTERDATA character in characters)
        {
            CharacterManager.instance.AddCharacterFromData(character);
        }

        if (activeGameFile.background0 != null)
        {
            BackgroundManager.instance.ReplaceBackground(0, activeGameFile.background0);
        }
        if (activeGameFile.background1 != null)
        {
            BackgroundManager.instance.ReplaceBackground(1, activeGameFile.background1);
        }

        if (activeGameFile.music != null)
            AudioManager.instance.PlaySong(activeGameFile.music);

        currentChoice = activeGameFile.currentChoice;
        if (currentChoice.answers.Count == 0) currentChoice = null;

        inInteractionMode = activeGameFile.interactionMode;
        InteractionManager.instance.Load(activeGameFile.interactables);

        if (inInteractionMode)
        {
            DialogSystem.instance.Close();
        }
        else
        {
            DialogSystem.instance.Open(activeGameFile.currentTextSystemSpeakerDisplayText,
                activeGameFile.currentTextsIds);
        }

        currentCaseName = activeGameFile.caseName;
        currentCaseDesc = activeGameFile.caseDesc;

        VNGUI.instance.DisplayEvidence(activeGameFile.evidenceDisplaySide, GameManager.GetEvidenceManager().GetEvidence(activeGameFile.evidenceDisplayID));

        loadedAutoSave = saveName.Equals("auto");

        lastIdxExamination = activeGameFile.lastIdxExamination;
        currentExamination = activeGameFile.inExamination ? activeGameFile.currentExamination : null;
        examinationPressed = activeGameFile.examinationPressed;

        VNGUI.instance.SetHealthBarLength(activeGameFile.healthBarLength);
        VNGUI.instance.SetHealthBarGlowingParts(activeGameFile.healthBarGlowAmount);
        VNGUI.instance.SetHealthBarVisible(activeGameFile.healthBarShown);

        VNGUI.instance.SetTestimonyActive(activeGameFile.testimonyActive);

        DialogSystem.instance.JustifyDialog(activeGameFile.dialogAlignement);

        LoadChapterFile(activeGameFile.chapterName, activeGameFile.chapterProgress);
    }

    public void SaveGameFile(string saveName)
    {
        GAMEFILE activeGameFile = GameManager.GetSaveManager().saveFile;

        activeGameFile.chapterName = activeChapterFile;
        activeGameFile.chapterProgress = chapterProgress;
        activeGameFile.currentTextsIds = DialogSystem.instance.currentTextsIds;
        activeGameFile.currentTextSystemSpeakerDisplayText = DialogSystem.instance.speakerNameText.text;

        activeGameFile.characterInScene = CharacterManager.instance.SaveCharacters();

        activeGameFile.background0 = BackgroundManager.instance.GetBackgroundName(0);
        activeGameFile.background1 = BackgroundManager.instance.GetBackgroundName(1);

        activeGameFile.music = AudioManager.activeSong != null ? AudioManager.activeSong.clipName : "";

        activeGameFile.currentChoice = currentChoice;

        activeGameFile.fadeBg = VNGUI.instance.fadeBgAlpha;
        activeGameFile.fadeFg = VNGUI.instance.fadeFgAlpha;

        activeGameFile.colorBg = VNGUI.instance.fadeBgColor;
        activeGameFile.colorFg = VNGUI.instance.fadeFgColor;

        activeGameFile.interactionMode = inInteractionMode;
        activeGameFile.interactables = InteractionManager.instance.GetSaveData();

        activeGameFile.cameraPosition = CameraController.instance.targetPosition;
        activeGameFile.cameraRotation = CameraController.instance.targetRotation;

        activeGameFile.caseName = currentCaseName;
        activeGameFile.caseDesc = currentCaseDesc;
        activeGameFile.date = new GAMEFILE.DateAJ(DateTime.Now);

        activeGameFile.evidenceDisplaySide = VNGUI.instance.GetDisplayedSide();
        activeGameFile.evidenceDisplayID = VNGUI.instance.GetDisplayedEvidenceID();

        activeGameFile.lastIdxExamination = lastIdxExamination;
        activeGameFile.currentExamination = currentExamination;
        activeGameFile.inExamination = currentExamination != null;

        activeGameFile.examinationPressed = examinationPressed;

        activeGameFile.healthBarShown = VNGUI.instance.healthBarShown;
        activeGameFile.healthBarLength = VNGUI.instance.healthBarLength;
        activeGameFile.healthBarGlowAmount = VNGUI.instance.healthBarLengthGlowing;

        activeGameFile.testimonyActive = VNGUI.instance.testimonyActive;

        activeGameFile.dialogAlignement = DialogSystem.instance.dialogAlignement;

        GameManager.GetSaveManager().Save(saveName);
    }

    public void LoadChapterFile(string filename, int chapterProgress = 0)
    {
        if (!GameManager.instance.IsLoadingSave()) inInteractionMode = false;
        activeChapterFile = filename;
        this.chapterProgress = chapterProgress;

        print("Loading chapter : " + $"Story/{filename}");
        data = FileManager.ReadTextAsset(Resources.Load<TextAsset>($"Story/{filename}"));

        if (handlingChapterFile != null)
        {
            StopCoroutine(handlingChapterFile);
            StopAllCoroutines();
        }
        handlingChapterFile = StartCoroutine(HandlingChapterFile());
    }




    IEnumerator HandlingChapterFile()
    {

        VNGUI.instance.SetInExaminationObjectsActive(false);

        if (GameManager.instance.IsLoadingSave())
        {
            GameManager.instance.SetSaveToLoad(null);

            if (currentChoice != null)
            {
                yield return HandleChoice(currentChoice);
            }
            else if (currentExamination != null)
            {
                yield return HandleExamination();
            }
            else if (inInteractionMode)
            {
                yield return HandleInteraction();
            }
            else if (!loadedAutoSave)
            {
                waitingForUserToEndDialog = true;
                next = false;
                while (!next)
                {
                    yield return new WaitForEndOfFrame();
                }
                next = false;
                waitingForUserToEndDialog = false;
            }
            chapterProgress++;
        }

        ChoiceScreen.instance.Hide();

        while (chapterProgress < data.Count)
        {
            string line = data[chapterProgress];

            if (line.Equals("interact"))
            {
                yield return HandleInteraction();
            }
            else if (line.StartsWith("choice"))
            {
                yield return HandlingChoiceLine(line);
            }
            else if (line.StartsWith("examination"))
            {
                yield return HandlingExaminationLine(line);
            }
            else if (line.StartsWith("if"))
            {
                yield return HandlingIf(line);
            }
            else
            {
                yield return HandlingLine(line);
            }
            chapterProgress++;
        }

        handlingChapterFile = null;
    }

    IEnumerator HandleInteraction()
    {
        print("Starting interaction mode");
        inInteractionMode = true;
        InteractionManager.instance.SetActive(true);
        while (InteractionManager.instance.active)
        {
            yield return new WaitForEndOfFrame();
        }
        if (!GameManager.instance.IsLoadingSave()) inInteractionMode = false;
    }

    IEnumerator HandlingExaminationLine(string line)
    {

        // examination END_CHAPTER FAIL_CHAPTER LOOP_CHAPTER [NONE | EVIDENCE_ID PRESENT_ID] 
        //  DEPOSITION_ID1 SPEAKER_NAME SPEAKER_ID DEPOSITION_NEXT1
        //  DEPOSITION_ID2 SPEAKER_NAME SPEAKER_ID DEPOSITION_NEXT2

        currentExamination = new Examination();
        string[] split = line.Split(' ');
        currentExamination.successChaper = split[1];
        currentExamination.failChapter = split[2];
        currentExamination.loopChapter = split[3];
        currentExamination.evidenceToPresent = split[4];
        if (!split[4].Equals("PRESS") && !split[4].Equals("NONE")) currentExamination.evidenceToPresentIdx = int.Parse(split[5]);

        int i = chapterProgress + 1;

        while (i < data.Count && !string.IsNullOrEmpty(data[i]))
        {
            split = data[i].Replace("\t", "").Split(' ');
            currentExamination.parts.Add(new Examination.Part
            {
                textId = split[0],
                speakerId = split[1],
                characterId = split[2],
                nextChapter = split[3]
            });

            i++;
        }

        yield return HandleExamination();
    }

    IEnumerator HandleExamination()
    {
        VNGUI.instance.SetInExaminationObjectsActive(true);

        while (currentExamination != null)
        {
            if (lastIdxExamination >= currentExamination.parts.Count)
            {
                // Exit loop
                string nextChapter = currentExamination.loopChapter;
                currentExamination = null;
                lastIdxExamination = 0;
                LoadChapterFile(nextChapter);
                break;
            }


            if (currentExamination.evidenceToPresent.Equals("PRESS") && currentExamination.parts.Count == examinationPressed.Count)
            {
                // End by pressing all sentences
                // Won't work well if you add new depositions mid examination
                // Does exactly matter since new deposions usualy means you need to present evidence somewhere
                examinationPressed.Clear();
                lastIdxExamination = 0;
                string nextChapter = currentExamination.successChaper;
                currentExamination = null;
                LoadChapterFile(nextChapter);
                break;
            }

            next = false;
            nextIsBackward = false;

            DialogSystem.instance.OpenAllRequirementsForDialogueSystemVisibility(true);
            DialogSystem.instance.Say(currentExamination.parts[lastIdxExamination].textId,
                currentExamination.parts[lastIdxExamination].speakerId,
                currentExamination.parts[lastIdxExamination].characterId.Equals("null") ? null : currentExamination.parts[lastIdxExamination].characterId);

            TextArchitect architect = DialogSystem.instance.textArchitect;


            while (architect.isConstructing)
            {
                if (next)
                {
                    next = false;
                    nextIsBackward = false;
                    architect.skip = true;
                }
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();

            waitingForUserToEndDialog = true;
            while (!next)
            {
                yield return new WaitForEndOfFrame();
            }
            waitingForUserToEndDialog = false;

            if (nextIsBackward) lastIdxExamination--;
            else lastIdxExamination++;

            if (lastIdxExamination < 0) lastIdxExamination = 0;
        }
    }

    public void PressOnDeposition()
    {
        if (currentExamination != null)
        {
            string toLoad = currentExamination.parts[lastIdxExamination].nextChapter;
            if (!examinationPressed.Contains(lastIdxExamination)) examinationPressed.Add(lastIdxExamination);
            currentExamination = null;
            lastIdxExamination++;
            LoadChapterFile(toLoad);
        }
    }

    public void PresentEvidence(string evidenceID)
    {
        if (currentExamination == null) return;

        string toLoad;

        if (!currentExamination.evidenceToPresent.Equals("NONE") && !currentExamination.evidenceToPresent.Equals("PRESS") &&
        evidenceID.Equals(currentExamination.evidenceToPresent) && lastIdxExamination == currentExamination.evidenceToPresentIdx)
        {
            // Presented the correct evidence
            toLoad = currentExamination.successChaper;
            examinationPressed.Clear();
            lastIdxExamination = 0;
        }
        else
        {
            // Failed to present the correct evidence
            toLoad = currentExamination.failChapter;
        }

        currentExamination = null;
        LoadChapterFile(toLoad);
    }


    IEnumerator HandlingIf(string line)
    {
        yield return new WaitForEndOfFrame();

        // if(KEY = VALUE & KEY = VALUE) NEW_CHAPTER
        string[] split = line.Split(new char[] { '(', ')' });

        bool ok = true;

        string[] splitAnds = split[1].Split(" & ");

        foreach (string splitAnd in splitAnds)
        {
            string[] parametersSplit = splitAnd.Split(' ');
            string key = parametersSplit[0];
            string oper = parametersSplit[1];
            string value = parametersSplit[2];

            if (!IsCheckOkay(key, value, oper))
            {
                ok = false;
                break;
            }
        }

        if (ok)
        {
            split[2] = split[2].Replace(" ", "");
            if (split.Length > 3)
            {
                yield return HandlingLine(split[2] + "(" + split[3] + ")");
            }
            else
            {
                LoadChapterFile(split[2]);
            }
        }
    }

    bool IsCheckOkay(string key, string value, string oper)
    {
        string currentValue = GameManager.GetSaveManager().GetItem(key);

        switch (oper)
        {
            case "=":
                return currentValue.Equals(value);
            case ">":
                return int.Parse(currentValue) > int.Parse(value);
            case "<":
                return int.Parse(currentValue) < int.Parse(value);
            case ">=":
                return int.Parse(currentValue) >= int.Parse(value);
            case "<=":
                return int.Parse(currentValue) <= int.Parse(value);
            case "!=":
                return int.Parse(currentValue) != int.Parse(value);
        }

        return false;
    }


    IEnumerator HandlingChoiceLine(string line)
    {
        // Choice
        // ID_REP NEXTCHAPTER
        currentChoice = new Choice();

        int i = chapterProgress + 1;
        while (i < data.Count && !string.IsNullOrEmpty(data[i]))
        {
            string choiceLine = data[i].Replace("\t", "");
            string[] split = choiceLine.Split(' ');
            print(choiceLine + " -> " + split[0] + " " + split[1] + " " + split.Length);
            currentChoice.answers.Add(new Choice.ChoiceAnswer(split[0], split[1]));
            i++;
        }

        if (currentChoice.answers.Count > 0)
        {
            yield return HandleChoice(currentChoice);
        }
    }

    IEnumerator HandleChoice(Choice choice)
    {
        ChoiceScreen.instance.Show(choice);

        yield return new WaitForEndOfFrame();

        while (ChoiceScreen.instance.isWaitingForChoiceToBeMade)
        {
            yield return new WaitForEndOfFrame();
        }

        string action = choice.answers[ChoiceScreen.instance.chosenIndex].action;
        currentChoice = null;

        if (action.StartsWith("Map"))
        {
            string[] parameters = action.Split(new char[] { '(', ')' })[1].Split(';');
            Map.instance.OpenMap(parameters[0], parameters[1]);
        }
        else
        {
            LoadChapterFile(action);
        }
    }

    public IEnumerator HandlingLine(string line, bool isQuickCommand = false)
    {
        if (string.IsNullOrEmpty(line) || line.StartsWith('#')) yield break;

        string[] data = line.Split(new char[] { '(', ')' });
        if (data.Length < 2) yield break;

        string[] parameters = data[1].Split(";");

        print(line);

        switch (data[0])
        {
            case "dialog":
                if (isQuickCommand) break;
                next = false;

                // Speaker - CharacterModel - additive - autoSkip - dialog
                DialogSystem.instance.OpenAllRequirementsForDialogueSystemVisibility(true);
                DialogSystem.instance.Say(parameters[4], parameters[0], parameters[1].Equals("_") ? null : parameters[1], bool.Parse(parameters[2]));

                TextArchitect architect = DialogSystem.instance.textArchitect;


                while (architect.isConstructing)
                {
                    if (next)
                    {
                        next = false;
                        architect.skip = true;
                    }
                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();

                if (!bool.Parse(parameters[3]))
                {
                    waitingForUserToEndDialog = true;
                    while (!next)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }

                waitingForUserToEndDialog = false;

                break;

            case "setBackground":
                BackgroundManager.instance.ReplaceBackground(
                    int.Parse(parameters[0]),
                    parameters[1]);
                break;

            case "playSound":
                AudioClip clip = Resources.Load("Audio/SFX/" + parameters[0]) as AudioClip;
                if (clip != null)
                {
                    AudioManager.instance.PlaySFX(clip);
                }
                break;

            case "hideDialog":
                DialogSystem.instance.Close();
                break;

            case "playMusic":
                AudioManager.instance.PlaySong(parameters[0]);
                break;

            case "removeAllCharacters":
                CharacterManager.instance.RemoveAllCharacters();
                break;

            case "addCharacter":
                CharacterManager.instance.AddCharacter(parameters[0], bool.Parse(parameters[1]));
                break;

            case "removeCharacter":
                CharacterManager.instance.RemoveCharacter(parameters[0]);
                break;

            case "setCharacterPosition":
                CharacterManager.instance.SetCharacterPosition(
                    parameters[0],
                    BackgroundManager.instance.GetCharacterPosition(parameters[1])
                );
                break;

            case "setCharacterRotation":

                CharacterManager.instance.SetCharacterRotation(
                    parameters[0],
                    float.Parse(parameters[1], System.Globalization.CultureInfo.InvariantCulture)
                );
                break;

            case "setCharacterMouth":
                CharacterManager.instance.SetCharacterMouthAnimation(
                    parameters[0],
                    parameters[1]
                );
                break;

            case "setCharacterEye":
                CharacterManager.instance.SetCharacterEyeAnimation(
                    parameters[0],
                    parameters[1]
                );
                break;

            case "setCharacterBody":
                CharacterManager.instance.SetCharacterBodyAnimation(
                    parameters[0],
                    parameters[1],
                    bool.Parse(parameters[2])
                );

                if (bool.Parse(parameters[3]))
                {
                    yield return new WaitForEndOfFrame();
                    while (CharacterManager.instance.IsCharacterInStartupAnim(parameters[0]))
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }

                break;

            case "setInteractionChapter":
                InteractionManager.instance.ChangeObjectChapter(parameters[0], parameters[1]);
                break;

            case "setInteractionHidden":
                InteractionManager.instance.SetObjectHidden(parameters[0], bool.Parse(parameters[1]));
                break;

            case "setCharacterAlpha":
                // Character - Target - Immediate - Wait for end ?

                if (bool.Parse(parameters[2]))
                {
                    CharacterManager.instance.SetCharacterAlpha(parameters[0],
                        float.Parse(parameters[1], System.Globalization.CultureInfo.InvariantCulture));
                    break;
                }

                CharacterManager.instance.TransitionCharacterAlpha(parameters[0],
                    float.Parse(parameters[1], System.Globalization.CultureInfo.InvariantCulture));

                if (bool.Parse(parameters[3]))
                {
                    yield return new WaitForEndOfFrame();
                    while (CharacterManager.instance.IsCharacterFading(parameters[0]))
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }

                break;

            case "flash":

                VNGUI.instance.FlashTo(1, 10);

                yield return new WaitForSeconds(float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture));

                VNGUI.instance.FlashTo(0, 10);
                break;

            case "setCaseName":
                currentCaseName = parameters[0];
                break;

            case "setCaseDesc":
                currentCaseDesc = parameters[0];
                break;

            case "autosave":
                SaveGameFile("auto");
                break;

            case "setBgColor":

                VNGUI.instance.SetBgColor(new Color(
                    float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture),
                    float.Parse(parameters[1], System.Globalization.CultureInfo.InvariantCulture),
                    float.Parse(parameters[2], System.Globalization.CultureInfo.InvariantCulture)
                ));
                break;

            case "setFgColor":

                VNGUI.instance.SetFgColor(new Color(
                    float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture),
                    float.Parse(parameters[1], System.Globalization.CultureInfo.InvariantCulture),
                    float.Parse(parameters[2], System.Globalization.CultureInfo.InvariantCulture)
                ));
                break;

            case "fadeBg":
                float speedBg = parameters.Length > 2 ? float.Parse(parameters[2], System.Globalization.CultureInfo.InvariantCulture) : 2;
                VNGUI.instance.FadeBgTo(float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture), speedBg);
                if (bool.Parse(parameters[1]))
                {
                    yield return new WaitForEndOfFrame();
                    while (VNGUI.instance.fadingBg)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                break;

            case "fadeFg":
                float speedFg = parameters.Length > 2 ? float.Parse(parameters[2], System.Globalization.CultureInfo.InvariantCulture) : 2;
                VNGUI.instance.FadeFgTo(float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture), speedFg);
                if (bool.Parse(parameters[1]))
                {
                    yield return new WaitForEndOfFrame();
                    while (VNGUI.instance.fadingFg)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                break;

            case "shake":
                CameraController.instance.SetShaking(
                    int.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture)
                );

                if (bool.Parse(parameters[1]))
                {
                    yield return new WaitForEndOfFrame();
                    while (CameraController.instance.isShaking)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                break;

            case "cameraPosition":
                // ID ; immediate ; waitForEnd

                CameraController.instance.SetPosition(
                    BackgroundManager.instance.GetCameraPosition(parameters[0]),
                    bool.Parse(parameters[1])
                );

                if (bool.Parse(parameters[2]))
                {
                    yield return new WaitForEndOfFrame();
                    while (!CameraController.instance.atTargetPosition)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                break;

            case "cameraRotation":
                // x ; y ; z ; immediate ; waitForEnd
                // default ; immediate ; waitForEnd

                // !LEGACY!

                int idxR = 1;
                Vector3 rotaiton = new Vector3(0, 0, 0);
                if (!parameters[0].Equals("default"))
                {
                    rotaiton = new Vector3(
                        float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture),
                        float.Parse(parameters[1], System.Globalization.CultureInfo.InvariantCulture),
                        float.Parse(parameters[2], System.Globalization.CultureInfo.InvariantCulture)
                    );
                    idxR = 3;
                }

                CameraController.instance.SetRotation(
                    rotaiton,
                    bool.Parse(parameters[idxR])
                );

                if (bool.Parse(parameters[idxR + 1]))
                {
                    yield return new WaitForEndOfFrame();
                    while (!CameraController.instance.atTargetRotation)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                break;

            case "load":
                if (isQuickCommand) break;
                LoadChapterFile(parameters[0]);
                break;

            case "askEvidence":
                if (isQuickCommand) break;

                VNGUI.instance.OpenCaseFile(false, true);

                yield return new WaitForEndOfFrame();
                while (VNGUI.instance.caseFileMenuOpen)
                {
                    yield return new WaitForEndOfFrame();
                }

                if (VNGUI.instance.caseFileSelectedID.Equals(parameters[0]))
                {
                    LoadChapterFile(parameters[1]);
                }
                else
                {
                    LoadChapterFile(parameters[2]);
                }

                break;

            case "mainMenu":
                if (isQuickCommand) break;
                VNGUI.instance.FadeFgTo(1);
                yield return new WaitForEndOfFrame();
                while (VNGUI.instance.fadingFg)
                {
                    yield return new WaitForEndOfFrame();
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                break;

            case "loadScene":
                if (isQuickCommand) break;
                VNGUI.instance.FadeFgTo(1);
                yield return new WaitForEndOfFrame();
                while (VNGUI.instance.fadingFg)
                {
                    yield return new WaitForEndOfFrame();
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene(parameters[0]);
                break;

            case "voice":
                clip = Resources.Load("Audio/Voice/" + parameters[0]) as AudioClip;
                if (clip != null)
                {
                    AudioManager.instance.PlayVoice(clip);
                }
                break;

            case "map":
                if (isQuickCommand) break;
                Map.instance.OpenMap(parameters[0], parameters[1]);
                break;

            case "variable":
                GameManager.GetSaveManager().EditItem(parameters[0], parameters[1]);
                break;

            case "changeSkybox":
                LightingManager.instance.ChangeData(parameters[0]);
                break;
            case "displayEvidence":
                VNGUI.instance.DisplayEvidence(
                    System.Enum.Parse<EvidenceDisplayManager.EvidenceDisplaySide>(parameters[0]),
                    GameManager.GetEvidenceManager().GetEvidence(parameters[1])
                );
                break;

            case "clearDisplayedEvidence":
                VNGUI.instance.DisplayEvidence(EvidenceDisplayManager.EvidenceDisplaySide.HIDDEN, null);
                break;

            case "wait":
                yield return new WaitForSeconds(float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture));
                break;

            case "changeSpeaker":
                if (!isQuickCommand) break;
                DialogSystem.instance.SetCurrentCharacterID(parameters[0]);
                break;

            case "speechBubble":

                Sprite spriteBubble = Resources.Load<Sprite>("SpeechBubles/" + parameters[0]);
                if (spriteBubble != null)
                {
                    VNGUI.instance.ShowSpeechBubble(spriteBubble);
                }
                else
                {
                    break;
                }

                AudioClip clipBubble = Resources.Load("Audio/SFX/" + parameters[1]) as AudioClip;
                if (clipBubble != null)
                {
                    AudioManager.instance.PlaySFX(clipBubble);
                }


                yield return new WaitForEndOfFrame();
                while (VNGUI.instance.speechBubbleInProgress)
                {
                    yield return new WaitForEndOfFrame();
                }

                break;

            case "setTestimonyActive":
                VNGUI.instance.SetTestimonyActive(bool.Parse(parameters[0]));
                break;

            case "setBG":
                VNGUI.instance.FadeBgTo(float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture),2,true);
                break;

            case "setFG":
                VNGUI.instance.FadeFgTo(float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture),2,true);
                break;

            case "setHealthBarVisible":
                VNGUI.instance.SetHealthBarVisible(bool.Parse(parameters[0]));
                break;

            case "updateHealthBar":
                VNGUI.instance.SetHealthBarLength(int.Parse(GameManager.GetSaveManager().GetItem("health")));
                break;

            case "setHealthBarGlow":
                VNGUI.instance.SetHealthBarGlowingParts(int.Parse(parameters[0]));
                break;

            case "explodeHealthBarGlow":
                VNGUI.instance.HeatlhBarExplodeLastGlowingPart();
                break;

            case "justifyDialog":
                DialogSystem.instance.JustifyDialog(Enum.Parse<HorizontalAlignmentOptions>(parameters[0]));
                break;

            case "startAnimation":
                float length = VNGUI.instance.StartGUIAnimation(parameters[0]);

                yield return new WaitForSeconds(length);

                if (bool.Parse(parameters[1])) VNGUI.instance.RemoveGUIAnimation(parameters[0]);
                break;

            case "removeAnimation":
                VNGUI.instance.RemoveGUIAnimation(parameters[0]);
                break;

            case "removeAllAnimations":
                VNGUI.instance.RemoveAllGUIAnimations();
                break;
        }
    }

}
