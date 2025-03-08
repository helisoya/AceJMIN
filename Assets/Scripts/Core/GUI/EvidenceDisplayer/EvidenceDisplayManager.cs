using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the evidence displayers
/// </summary>
public class EvidenceDisplayManager : MonoBehaviour
{
    public enum EvidenceDisplaySide
    {
        LEFT,
        RIGHT,
        FULL,
        HIDDEN
    };

    [SerializeField] private EvidenceDisplayer[] displayers;
    private Evidence currentEvidence;
    private EvidenceDisplaySide currentSide = EvidenceDisplaySide.HIDDEN;

    /// <summary>
    /// Display a new evidence
    /// </summary>
    /// <param name="side">The side where to display the display</param>
    /// <param name="evidence">The evidence to display</param>
    public void Display(EvidenceDisplaySide side, Evidence evidence)
    {
        if (currentSide != EvidenceDisplaySide.HIDDEN) displayers[(int)currentSide].Hide();
        currentEvidence = evidence;
        currentSide = side;
        if (currentSide != EvidenceDisplaySide.HIDDEN)
        {
            displayers[(int)side].Show(evidence);
        }
    }

    /// <summary>
    /// Gets the current side used
    /// </summary>
    /// <returns>The current side used</returns>
    public EvidenceDisplaySide GetCurrentSide()
    {
        return currentSide;
    }

    /// <summary>
    /// Gets the currently displayed evidence
    /// </summary>
    /// <returns>The currently displayed evidence</returns>
    public string GetEvidenceID()
    {
        if (currentEvidence != null) return currentEvidence.ID;
        return null;
    }
}
