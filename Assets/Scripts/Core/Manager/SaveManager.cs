using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles the player's save file
/// </summary>
public class SaveManager
{
    private string savePath = FileManager.savPath + "Saves/";
    private List<GAMEFILE.ITEM> items;
    public GAMEFILE saveFile { get; private set; }


    /// <summary>
    /// Checks if a savefile exists on disk
    /// </summary>
    /// <param name="saveName">The save's name</param>
    /// <returns>True if the savefile exists on disk</returns>
    public bool SaveFileExists(string saveName)
    {
        return System.IO.File.Exists(savePath + saveName + ".txt");
    }

    public SaveManager()
    {
        InitSaveFile();
    }

    /// <summary>
    /// Initializes the save file
    /// </summary>
    private void InitSaveFile()
    {
        saveFile = new GAMEFILE();
        items = new List<GAMEFILE.ITEM>();

        List<string> lines = FileManager.ReadTextAsset(Resources.Load<TextAsset>("General/items"));
        foreach (string line in lines)
        {
            if (!string.IsNullOrEmpty(line) && !line.StartsWith('#'))
            {
                string[] split = line.Split(' ');
                items.Add(new GAMEFILE.ITEM(split[0], split[1]));
            }
        }
    }

    /// <summary>
    /// Resets the items to their default values
    /// </summary>
    public void ResetItems()
    {
        foreach (GAMEFILE.ITEM item in items)
        {
            item.value = item.defaultValue;
        }

        saveFile.items.Clear();
    }

    /// <summary>
    /// Saves the current save file
    /// <param name="saveName">The save's name</param>
    /// </summary>
    public void Save(string saveName = "save")
    {
        FileManager.SaveJSON(savePath + saveName + ".txt", saveFile);
    }

    /// <summary>
    /// Loads the save file from disk
    /// </summary>
    /// <param name="saveName">The save's name</param>
    /// <returns>The new save file</returns>
    public GAMEFILE Load(string saveName)
    {
        if (SaveFileExists(saveName))
        {
            saveFile = FileManager.LoadJSON<GAMEFILE>(savePath + saveName + ".txt");
            foreach (GAMEFILE.ITEM item in saveFile.items)
            {
                items[GetIndexOfItem(item.name)].value = item.value;
            }
        }

        return saveFile;
    }

    /// <summary>
    /// Finds the index of an item
    /// </summary>
    /// <param name="key">The item's key</param>
    /// <returns>The item's index</returns>
    private int GetIndexOfItem(string key)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == key)
            {
                return i;
            }
        }
        return 0;
    }

    /// <summary>
    /// Edits an item's value
    /// </summary>
    /// <param name="key">The item's key</param>
    /// <param name="value">The item's new value</param>
    public void EditItem(string key, string value)
    {
        int index = GetIndexOfItem(key);

        if (value.Equals("++"))
        {
            value = (int.Parse(items[index].value) + 1).ToString();
        }
        else if (value.Equals("--"))
        {
            value = (int.Parse(items[index].value) - 1).ToString();
        }

        items[index].value = value;

        if (items[index].value != items[index].defaultValue && !saveFile.items.Contains(items[index]))
        {
            saveFile.items.Add(items[index]);
        }
        else if (items[index].value == items[index].defaultValue && saveFile.items.Contains(items[index]))
        {
            saveFile.items.Remove(items[index]);
        }
    }

    /// <summary>
    /// Returns the value of an item
    /// </summary>
    /// <param name="key">The item's key</param>
    /// <returns>The item's value</returns>
    public string GetItem(string key)
    {
        return items[GetIndexOfItem(key)].value;
    }

    public SaveInfo[] GetAvailableSaves()
    {
        GAMEFILE temp;
        string[] toTest = { "auto", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        SaveInfo[] list = new SaveInfo[toTest.Length];

        for (int i = 0; i < toTest.Length; i++)
        {
            if (SaveFileExists(toTest[i]))
            {
                temp = FileManager.LoadJSON<GAMEFILE>(savePath + toTest[i] + ".txt");
                list[i] = new SaveInfo
                {
                    saveName = toTest[i],
                    caseDesc = temp.caseDesc,
                    caseName = temp.caseName
                };
            }
        }

        return list;
    }

    public class SaveInfo
    {
        public string saveName;
        public string caseDesc;
        public string caseName;
    }
}
