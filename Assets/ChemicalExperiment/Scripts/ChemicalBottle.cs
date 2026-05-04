using UnityEngine;

public enum ChemicalType
{
    None,
    HydrochloricAcid,
    SodiumHydroxide,
    HydrogenPeroxide
}

public class ChemicalBottle : MonoBehaviour
{
    [Header("Chemical Settings")]
    public ChemicalType chemicalType = ChemicalType.None;

    [Header("Pouring Settings")]
    public float pourAngleThreshold = 100f;
    public float pourDistance = 0.3f;

    private bool isPouring = false;
    private MixingBeaker nearbyBeaker = null;
    private bool hasBeenPoured = false;

    void Update()
    {
        if (hasBeenPoured || nearbyBeaker == null) return;

        float tiltAmount = Vector3.Dot(transform.up, Vector3.down);

        if (tiltAmount > 0.5f)
        {
            if (!isPouring)
            {
                isPouring = true;
                Pour();
            }
        }
        else
        {
            isPouring = false;
        }
    }

    public void SetNearbyBeaker(MixingBeaker beaker)
    {
        nearbyBeaker = beaker;
        Debug.Log(chemicalType + " bottle is near the beaker!");
    }

    public void ClearNearbyBeaker()
    {
        nearbyBeaker = null;
        isPouring = false;
        Debug.Log(chemicalType + " bottle left the beaker area.");
    }

    private void Pour()
    {
        if (hasBeenPoured) return;
        hasBeenPoured = true;
        Debug.Log("Pouring " + chemicalType + " into beaker!");
        nearbyBeaker.ReceiveChemical(chemicalType);
    }

    public void ResetBottle()
    {
        hasBeenPoured = false;
        isPouring = false;
        Debug.Log(chemicalType + " bottle has been reset.");
    }
}