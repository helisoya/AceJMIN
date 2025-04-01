using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// GameManager for the game
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private SaveManager saveManager;
    private EvidenceManager evidenceManager;

    private string chapterToLoad;
    private string loadSaveName = null;
    [Header("General Informations")]
    [SerializeField] private AudioMixer mixer;

    [Header("Debug")]
    [SerializeField] private bool debug;
    [SerializeField] private string debug_nextChapter;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);

            instance = this;
            Locals.Init();
            saveManager = new SaveManager();
            evidenceManager = new EvidenceManager();

            if (debug)
            {
                chapterToLoad = debug_nextChapter;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (instance == this)
        {
            Settings.Init();
        }
    }

    /// <summary>
    /// Returns the save manager
    /// </summary>
    /// <returns>The save manager</returns>
    public static SaveManager GetSaveManager()
    {
        if (instance != null) return instance.saveManager;
        return null;
    }

    /// <summary>
    /// Returns the evidence manager
    /// </summary>
    /// <returns>The evidence manager</returns>
    public static EvidenceManager GetEvidenceManager()
    {
        if (instance != null) return instance.evidenceManager;
        return null;
    }

    /// <summary>
    /// Sets the savefile to load
    /// </summary>
    /// <param name="value">The savefile's name</param>
    public void SetSaveToLoad(string value)
    {
        loadSaveName = value;
    }

    /// <summary>
    /// Gets the savefile to load
    /// </summary>
    /// <returns>The savefile to load</returns>
    public string GetSaveToLoad()
    {
        return loadSaveName;
    }

    /// <summary>
    /// Returns if the player is loading a save
    /// </summary>
    /// <returns>Is the player loading a save ?</returns>
    public bool IsLoadingSave()
    {
        return loadSaveName != null;
    }

    /// <summary>
    /// Sets the next chapter to be loaded when loading the VN mode
    /// </summary>
    /// <param name="value">The next chapter</param>
    public void SetNextChapter(string value)
    {
        chapterToLoad = value;
    }

    /// <summary>
    /// Returns the next chapter to be loaded
    /// </summary>
    /// <returns>The next chapter</returns>
    public string GetNextChapter()
    {
        return chapterToLoad;
    }

    /// <summary>
    /// Returns the game's audio mixer
    /// </summary>
    /// <returns>The audio mixer</returns>
    public AudioMixer GetAudioMixer()
    {
        return mixer;
    }
}
