using UnityEngine;
using System.Collections.Generic;

public class MixingBeaker : MonoBehaviour
{
    [Header("References")]
    public ReactionManager reactionManager;

    [Header("Beaker State")]
    private List<ChemicalType> chemicalsInBeaker = new List<ChemicalType>();
    private int chemicalsNeeded = 2;
    private bool isActive = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ChemicalBottle>(out ChemicalBottle bottle))
        {
            Debug.Log("Bottle entered beaker zone: " + bottle.chemicalType);
            bottle.SetNearbyBeaker(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ChemicalBottle>(out ChemicalBottle bottle))
        {
            Debug.Log("Bottle left beaker zone: " + bottle.chemicalType);
            bottle.ClearNearbyBeaker();
        }
    }

    public void ReceiveChemical(ChemicalType chemical)
    {
        if (!isActive) return;

        if (chemicalsInBeaker.Contains(chemical))
        {
            Debug.Log("Chemical already in beaker!");
            return;
        }

        chemicalsInBeaker.Add(chemical);
        Debug.Log("Added " + chemical + " to beaker. Total: " 
                  + chemicalsInBeaker.Count);

        if (chemicalsInBeaker.Count >= chemicalsNeeded)
        {
            Debug.Log("Two chemicals mixed! Checking reaction...");
            CheckReaction();
        }
    }

    private void CheckReaction()
    {
        isActive = false;

        if (reactionManager == null)
        {
            Debug.LogError("ReactionManager not assigned on MixingBeaker!");
            return;
        }

        reactionManager.CheckReaction(chemicalsInBeaker);
    }

    public void ResetBeaker()
    {
        chemicalsInBeaker.Clear();
        isActive = true;
        Debug.Log("Beaker reset!");
    }
}