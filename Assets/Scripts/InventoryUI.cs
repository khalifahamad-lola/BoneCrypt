using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject itemEntryPrefab;

    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private SoulsWallet wallet;

    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Optional Camera")]
    [SerializeField] private GameObject gameplayCinemachine;
    [SerializeField] private GameObject playerCamera;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI statsText;

    private bool isOpen = false;

    public bool IsOpen => isOpen;

    private void Start()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<PlayerInventory>();

        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (wallet == null)
            wallet = FindFirstObjectByType<SoulsWallet>();

        if (playerLook == null)
            playerLook = FindFirstObjectByType<PlayerLook>();

        if (playerMove == null)
            playerMove = FindFirstObjectByType<PlayerMove>();

        if (playerAttack == null)
            playerAttack = FindFirstObjectByType<PlayerAttack>();

        if (playerInteract == null)
            playerInteract = FindFirstObjectByType<PlayerInteract>();

        if (playerBlock == null)
            playerBlock = FindFirstObjectByType<PlayerBlock>();

        if (dialogueManager == null)
            dialogueManager = FindFirstObjectByType<DialogueManager>();

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Do not allow opening inventory while dialogue is active
            if (dialogueManager != null && dialogueManager.IsDialogueOpen)
                return;

            ToggleInventory();
        }

        // Safety: if dialogue starts while inventory is open, force-close inventory
        if (isOpen && dialogueManager != null && dialogueManager.IsDialogueOpen)
        {
            ForceCloseInventoryForDialogue();
        }
    }

    public void ToggleInventory()
    {
        if (isOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        // Hard block inventory while dialogue is open
        if (dialogueManager != null && dialogueManager.IsDialogueOpen)
            return;

        isOpen = true;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        RefreshInventoryUI();
        UpdateStatsText();

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

        if (gameplayCinemachine != null)
            gameplayCinemachine.SetActive(false);

        if (playerCamera != null)
            playerCamera.SetActive(true);
    }

    public void CloseInventory()
    {
        isOpen = false;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (gameplayCinemachine != null)
            gameplayCinemachine.SetActive(true);

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
            playerLook.SuppressLookInputTemporarily(0.2f);
        }
    }

    private void ForceCloseInventoryForDialogue()
    {
        isOpen = false;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        Time.timeScale = 1f;

        // Dialogue wants the cursor unlocked and visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Dialogue manager already handles player state, so do NOT re-enable controls here
    }

    public void RefreshInventoryUI()
    {
        if (contentParent == null || itemEntryPrefab == null || inventory == null)
            return;

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (InventorySlot slot in inventory.slots)
        {
            if (slot == null || slot.item == null)
                continue;

            GameObject obj = Instantiate(itemEntryPrefab, contentParent);

            InventorySlotUI slotUI = obj.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.Setup(slot.item, slot.quantity, this);
            }
        }
    }

    private void UpdateStatsText()
    {
        if (statsText == null || playerStats == null)
            return;

        statsText.text =
            "Health: " + playerStats.currentHealth + "/" + playerStats.maxHealth + "\n" +
            "Damage: " + playerStats.damage + "\n" +
            "Stamina: " + playerStats.currentStamina + "/" + playerStats.maxStamina;

        if (wallet != null)
        {
            statsText.text += "\nSouls: " + wallet.GetSouls();
        }
    }

    public void SelectItem(ItemData item)
    {
        Debug.Log("Selected item: " + item.itemName);
    }

    public void SelectEquippedWeapon()
    {
        Debug.Log("Selected equipped weapon.");
    }

    public void SelectEquippedShield()
    {
        Debug.Log("Selected equipped shield.");
    }
}