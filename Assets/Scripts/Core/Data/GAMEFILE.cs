using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[System.Serializable]
public class GAMEFILE
{
    public string chapterName;
    public int chapterProgress = 0;

    public List<string> currentTextsIds;
    public string currentTextSystemSpeakerDisplayText = "";

    public List<CHARACTERDATA> characterInScene = null;

    public string background0;
    public string background1;

    public string music;

    public Choice currentChoice;

    public List<ITEM> items;

    public float fadeBg;
    public float fadeFg;
    public Color colorBg;
    public Color colorFg;
    public bool interactionMode;
    public List<INTERACTABLEDATA> interactables;

    public Vector3 cameraPosition;
    public Vector3 cameraRotation;
    public string caseName;
    public string caseDesc;
    public DateAJ date;

    public EvidenceDisplayManager.EvidenceDisplaySide evidenceDisplaySide;
    public string evidenceDisplayID;

    public Examination currentExamination;
    public int lastIdxExamination;
    public bool inExamination;
    public List<int> examinationPressed;

    public bool healthBarShown;
    public int healthBarLength;
    public int healthBarGlowAmount;

    public bool testimonyActive;
    public HorizontalAlignmentOptions dialogAlignement;

    public GAMEFILE()
    {
        this.chapterName = "test2";
        this.chapterProgress = 0;
        this.characterInScene = new List<CHARACTERDATA>();
        this.background0 = null;
        this.background1 = null;
        this.music = null;
        this.currentChoice = null;
        this.items = new List<ITEM>();
        this.fadeBg = 0;
        this.fadeFg = 0;
        this.interactionMode = false;
        this.interactables = new List<INTERACTABLEDATA>();
        this.cameraPosition = new Vector3(0, 0, -10);
        this.cameraRotation = Vector3.zero;
        this.caseDesc = "";
        this.caseName = "";

        this.evidenceDisplaySide = EvidenceDisplayManager.EvidenceDisplaySide.HIDDEN;
        this.evidenceDisplayID = null;
        this.currentExamination = null;
        this.inExamination = false;
        this.lastIdxExamination = 0;
        this.examinationPressed = new List<int>();
        this.healthBarShown = false;
        this.healthBarLength = 0;
        this.healthBarGlowAmount = 0;
        this.testimonyActive = false;
        this.dialogAlignement = HorizontalAlignmentOptions.Left;
        this.date = new DateAJ();
    }

    [System.Serializable]
    public class DateAJ
    {
        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;

        public DateAJ(DateTime date)
        {
            year = date.Year;
            month = date.Month;
            day = date.Day;
            hour = date.Hour;
            minute = date.Minute;
        }

        public DateAJ() : this(DateTime.Now) { }
    }

    [System.Serializable]
    public class CHARACTERDATA
    {
        public string characterName;
        public string bodyAnimationSet = "";
        public string eyeAnimationSet = "";
        public string mouthAnimationSet = "";
        public Vector3 position;
        public float rotation;
        public float alpha;

        public CHARACTERDATA(Character character)
        {
            characterName = character.characterName;
            position = character.characterPosition;
            rotation = character.characterRotation;
            alpha = character.characterAlpha;
            bodyAnimationSet = character.bodyAnimationSet;
            eyeAnimationSet = character.eyeAnimationSet;
            mouthAnimationSet = character.mouthAnimationSet;
        }
    }

    [System.Serializable]
    public class INTERACTABLEDATA
    {
        public string ID;
        public string chapter;
        public bool hidden;

        public INTERACTABLEDATA(string ID, string chapter, bool hidden)
        {
            this.ID = ID;
            this.chapter = chapter;
            this.hidden = hidden;
        }
    }

    [System.Serializable]
    public class ITEM
    {
        public string name;
        public string value;
        public string defaultValue;

        public ITEM(string key, string defaultValue)
        {
            this.name = key;
            this.value = defaultValue;
            this.defaultValue = defaultValue;
        }
    }
}
