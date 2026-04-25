using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class VendorUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject vendorPanel;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject vendorItemEntryPrefab;
    [SerializeField] private TextMeshProUGUI soulsText;

    [Header("References")]
    [SerializeField] private Vendor currentVendor;
    [SerializeField] private SoulsWallet wallet;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject gameplayCinemachine;
    [SerializeField] private GameObject playerCamera;

    [Header("Look Restore")]
    [SerializeField] private float lookSuppressDuration = 0.2f;

    public bool IsOpen => vendorPanel != null && vendorPanel.activeSelf;

    private void Start()
    {
        if (vendorPanel != null)
            vendorPanel.SetActive(false);

        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();
    }

    public void OpenVendor(Vendor vendor)
    {
        currentVendor = vendor;

        if (vendorPanel != null)
            vendorPanel.SetActive(true);

        RefreshUI();

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerLook != null)
        {
            playerLook.ResetLookInput();
            playerLook.enabled = false;
        }

        if (playerMove != null)
            playerMove.canMove = false;

        if (playerAttack != null)
            playerAttack.enabled = false;

        if (playerInteract != null)
            playerInteract.enabled = false;

        if (playerBlock != null)
            playerBlock.enabled = false;

        if (playerInput != null)
            playerInput.enabled = false;

        if (gameplayCinemachine != null)
            gameplayCinemachine.SetActive(false);

        if (playerCamera != null)
            playerCamera.SetActive(true);
    }

    public void CloseVendor()
    {
        if (vendorPanel != null)
            vendorPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (gameplayCinemachine != null)
            gameplayCinemachine.SetActive(true);

        if (playerInput != null)
            playerInput.enabled = true;

        if (playerMove != null)
            playerMove.canMove = true;

        if (playerAttack != null)
            playerAttack.enabled = true;

        if (playerInteract != null)
            playerInteract.enabled = true;

        if (playerBlock != null)
            playerBlock.enabled = true;

        if (playerLook != null)
        {
            playerLook.enabled = true;
            playerLook.ResetLookInput();
            playerLook.SuppressLookInputTemporarily(lookSuppressDuration);
        }
    }

    public void TryBuy(int index)
    {
        if (currentVendor == null) return;

        currentVendor.Buy(index);
        RefreshUI();
    }

    public void RefreshUI()
    {
        RefreshSouls();
        RefreshVendorItems();
    }

    private void RefreshSouls()
    {
        if (soulsText != null && wallet != null)
        {
            soulsText.text = "Souls: " + wallet.GetSouls();
        }
    }

    private void RefreshVendorItems()
    {
        if (contentParent == null || vendorItemEntryPrefab == null || currentVendor == null)
            return;

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentVendor.stock.Length; i++)
        {
            VendorEntry entry = currentVendor.stock[i];
            GameObject obj = Instantiate(vendorItemEntryPrefab, contentParent);

            VendorItemEntryUI entryUI = obj.GetComponent<VendorItemEntryUI>();
            if (entryUI != null)
            {
                entryUI.Setup(entry, i, this);
            }
        }
    }
}