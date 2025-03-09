using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a cross-examination
/// </summary>
[System.Serializable]
public class Examination
{
    public string failChapter;
    public string loopChapter;
    public string successChaper;
    public string evidenceToPresent;
    public int evidenceToPresentIdx;
    public List<Part> parts;

    public Examination()
    {
        parts = new List<Part>();
    }

    /// <summary>
    /// Represents a part in the cross examination
    /// </summary>
    [System.Serializable]
    public class Part
    {
        public string textId;
        public string speakerId;
        public string characterId;
        public string nextChapter;
    }
}
