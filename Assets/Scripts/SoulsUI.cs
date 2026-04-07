using UnityEngine;
using TMPro;

public class SoulsUI : MonoBehaviour
{
    [SerializeField] private SoulsWallet wallet;
    [SerializeField] private TextMeshProUGUI soulsText;

    void Update()
    {
        soulsText.text = "Souls: " + wallet.GetSouls();
    }
}