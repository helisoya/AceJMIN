using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the game's evidence
/// </summary>
public class EvidenceManager
{
    private Dictionary<string, Evidence> evidences;

    public EvidenceManager()
    {
        evidences = new Dictionary<string, Evidence>();
        LoadEvidences();
    }

    /// <summary>
    /// Loads the evidence
    /// </summary>
    private void LoadEvidences()
    {
        evidences.Clear();

        bool isProfile = false;

        Evidence evidence;
        List<string> lines = FileManager.ReadTextAsset(Resources.Load<TextAsset>("General/evidence"));
        string[] split;
        string[] checkSprites;
        foreach (string line in lines)
        {
            if (!string.IsNullOrEmpty(line) && !line.StartsWith('#'))
            {
                if (line.Equals("[Evidence]")) isProfile = false;
                else if (line.Equals("[Profile]")) isProfile = true;
                else
                {
                    split = line.Split(' ');
                    if (split.Length > 1)
                    {
                        checkSprites = new string[split.Length - 1];
                        for (int i = 1; i < split.Length; i++)
                        {
                            checkSprites[i - 1] = split[i];
                        }
                    }
                    else
                    {
                        checkSprites =  new string[0];
                    }
                    evidence = new Evidence(split[0], isProfile, checkSprites);
                    evidences.Add(split[0], evidence);
                }
            }
        }
    }

    /// <summary>
    /// Gets an evidence's data
    /// </summary>
    /// <param name="ID">The evidence's ID</param>
    /// <returns>The evidence's data</returns>
    public Evidence GetEvidence(string ID)
    {
        if (ID == null) return null;

        if (evidences.TryGetValue(ID, out Evidence evidence))
        {
            return evidence;
        }
        return null;
    }

    /// <summary>
    /// Gets the list of available profiles
    /// </summary>
    /// <returns>The available profiles</returns>
    public List<Evidence> GetAvailableProfiles()
    {
        List<Evidence> list = new List<Evidence>();
        foreach (Evidence evidence in evidences.Values)
        {
            if (evidence.isProfile && !GameManager.GetSaveManager().GetItem(evidence.ID).Equals("-1"))
            {
                list.Add(evidence);
            }
        }
        return list;
    }

    /// <summary>
    /// Gets the list of available evidence
    /// </summary>
    /// <returns>The available evidence</returns>
    public List<Evidence> GetAvailableEvidence()
    {
        List<Evidence> list = new List<Evidence>();
        foreach (Evidence evidence in evidences.Values)
        {
            if (!evidence.isProfile && !GameManager.GetSaveManager().GetItem(evidence.ID).Equals("-1"))
            {
                list.Add(evidence);
            }
        }
        return list;
    }
}


/// <summary>
/// Represents an evidence
/// </summary>
public class Evidence
{
    public string ID;
    public bool isProfile;
    private string[] checkSprites;
    public string Name { get { return this.ID + "_name"; } }
    public Sprite sprite { get; private set; }
    public Sprite[] spritesCheck { get; private set; }

    public Evidence(string ID, bool isProfile, string[] checkSprites)
    {
        this.ID = ID;
        this.isProfile = isProfile;
        this.checkSprites = checkSprites;
        this.sprite = Resources.Load<Sprite>("Evidence/" + ID);
        spritesCheck = new Sprite[checkSprites.Length];
        for (int i = 0; i < spritesCheck.Length; i++)
        {
            spritesCheck[i] = Resources.Load<Sprite>("Evidence/Check/" + checkSprites[i]);
        }
    }

    /// <summary>
    /// Gets the evidence's description
    /// </summary>
    /// <param name="idx">The description index</param>
    /// <returns>The description</returns>
    public string GetDesc(int idx)
    {
        return ID + "_desc" + idx.ToString();
    }

    /// <summary>
    /// Gets the number of check sprites
    /// </summary>
    /// <returns>The number of check sprites</returns>
    public int GetNumberOfChecks()
    {
        if (checkSprites == null) return 0;
        return checkSprites.Length;
    }
}