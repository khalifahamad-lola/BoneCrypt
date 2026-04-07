using UnityEngine;

public class SoulsWallet : MonoBehaviour
{
    private int souls = 0;

    void Start()
    {
        AddSouls(500);
    }
    public void AddSouls(int amount)
    {
        souls += amount;
        Debug.Log($"Souls gained{amount} | Total Souls: {souls}");
    }

    public bool SpendSouls(int amount)
    {
        if (souls < amount)
        {
            Debug.Log("Not Enough Souls.");
            return false;
        }

        souls -= amount;
        Debug.Log("Souls spent: " + amount + " | Remaining: " + souls);
        return true;
    }

    public int GetSouls()
    {
        return souls;
    }
}
