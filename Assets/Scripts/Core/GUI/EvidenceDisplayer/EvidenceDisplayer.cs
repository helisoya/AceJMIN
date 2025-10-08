using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents an evidence display on the GUI
/// (When the evidence is displayed on screen after an objection for instance)
/// </summary>
public class EvidenceDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image evidenceImage;
    [SerializeField] private LocalizedText evidenceName;
    [SerializeField] private LocalizedText evidenceDesc;

    /// <summary>
    /// Show the displayer
    /// </summary>
    /// <param name="evidence">The evidence to display</param>
    public void Show(Evidence evidence)
    {

        if (evidenceImage != null) evidenceImage.sprite = evidence.sprite;
        if (evidenceName != null) evidenceName.SetNewKey(evidence.Name);
        if (evidenceDesc != null) evidenceDesc.SetNewKey(evidence.GetDesc(int.Parse(GameManager.GetSaveManager().GetItem(evidence.ID))));

        root.SetActive(true);
    }

    /// <summary>
    /// Hide the displayer
    /// </summary>
    public void Hide()
    {
        root.SetActive(false);
    }
}
