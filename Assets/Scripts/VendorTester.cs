using UnityEngine;

public class VendorTester : MonoBehaviour
{
    [SerializeField] private Vendor vendor;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            vendor.Buy(0); // buy first item
        }

        
    }
}