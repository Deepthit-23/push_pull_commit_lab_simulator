using UnityEngine;
using System.Collections.Generic;

public class ReactionManager : MonoBehaviour
{
    [Header("References")]
    // Drag ExperimentUIManager GameObject here in Inspector
    public ExperimentUIManager uiManager;

    // -------------------------------------------------------
    // REACTION DATABASE
    // Define which chemical combinations are safe or dangerous
    // -------------------------------------------------------

    public void CheckReaction(List<ChemicalType> chemicals)
    {
        Debug.Log("Checking reaction between: " + 
                  chemicals[0] + " and " + chemicals[1]);

        // Check each known combination
        // HCl + NaOH = Safe (Acid-Base neutralization)
        if (HasBoth(chemicals, ChemicalType.HydrochloricAcid, 
                    ChemicalType.SodiumHydroxide))
        {
            TriggerSafeReaction(
                "Acid-Base Neutralization!",
                "HCl + NaOH → Water + Salt\n" +
                "This is a safe neutralization reaction.\n" +
                "The acid and base cancel each other out!"
            );
        }
        // HCl + H2O2 = Dangerous (produces toxic chlorine gas)
        else if (HasBoth(chemicals, ChemicalType.HydrochloricAcid, 
                         ChemicalType.HydrogenPeroxide))
        {
            TriggerDangerousReaction(
                "DANGER: Toxic Gas Produced!",
                "HCl + H2O2 → Chlorine Gas\n" +
                "This combination produces toxic chlorine gas!\n" +
                "NEVER mix these in a real lab!"
            );
        }
        // NaOH + H2O2 = Dangerous (violent decomposition)
        else if (HasBoth(chemicals, ChemicalType.SodiumHydroxide, 
                         ChemicalType.HydrogenPeroxide))
        {
            TriggerDangerousReaction(
                "DANGER: Violent Reaction!",
                "NaOH + H2O2 → Rapid Decomposition\n" +
                "This causes violent bubbling and heat!\n" +
                "Can cause serious burns - extremely dangerous!"
            );
        }
        // Unknown combination
        else
        {
            TriggerSafeReaction(
                "No Reaction",
                "These chemicals do not react with each other.\n" +
                "No dangerous reaction occurred."
            );
        }
    }

    // -------------------------------------------------------
    // HELPER METHOD
    // Checks if both chemicals are in the list
    // regardless of order they were added
    // -------------------------------------------------------

    private bool HasBoth(List<ChemicalType> chemicals, 
                         ChemicalType a, ChemicalType b)
    {
        return chemicals.Contains(a) && chemicals.Contains(b);
    }

    // -------------------------------------------------------
    // REACTION TRIGGERS
    // -------------------------------------------------------

    private void TriggerSafeReaction(string title, string description)
    {
        Debug.Log("SAFE REACTION: " + title);

        if (uiManager == null)
        {
            Debug.LogError("UIManager not assigned on ReactionManager!");
            return;
        }

        uiManager.ShowSafeReaction(title, description);
    }

    private void TriggerDangerousReaction(string title, string description)
    {
        Debug.Log("DANGEROUS REACTION: " + title);

        if (uiManager == null)
        {
            Debug.LogError("UIManager not assigned on ReactionManager!");
            return;
        }

        uiManager.ShowDangerousReaction(title, description);
    }
}