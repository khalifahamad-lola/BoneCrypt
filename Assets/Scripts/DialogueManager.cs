using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private PlayerInput playerInput;

    [Header("Gameplay Camera")]
    [SerializeField] private GameObject gameplayCinemachine;
    [SerializeField] private GameObject playerCamera;

    [Header("Restore Timing")]
    [SerializeField] private float restoreDelay = 0.1f;
    [SerializeField] private float lookSuppressDuration = 0.2f;

    private NPCDialogue currentNPC;
    private string currentSpeakerName;
    private string[] currentLines;
    private int currentLineIndex;
    private bool isDialogueOpen = false;
    private Action onSimpleDialogueFinished;

    public bool IsDialogueOpen => isDialogueOpen;

    private void Awake()
    {
        if (dialogueUI == null)
            dialogueUI = FindFirstObjectByType<DialogueUI>();

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

        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();
    }

    private void Start()
    {
        if (dialogueUI != null)
            dialogueUI.SetDialogueManager(this);
    }

    public void StartDialogue(NPCDialogue npc)
    {
        if (npc == null || npc.DialogueLines == null || npc.DialogueLines.Length == 0)
            return;

        currentNPC = npc;
        currentSpeakerName = npc.NPCName;
        currentLines = npc.DialogueLines;
        currentLineIndex = 0;
        onSimpleDialogueFinished = null;

        OpenDialogue();
    }

    public void StartSimpleDialogue(string speakerName, string[] lines, Action onFinished = null)
    {
        if (lines == null || lines.Length == 0)
            return;

        currentNPC = null;
        currentSpeakerName = speakerName;
        currentLines = lines;
        currentLineIndex = 0;
        onSimpleDialogueFinished = onFinished;

        OpenDialogue();
    }

    private void OpenDialogue()
    {
        isDialogueOpen = true;

        DisablePlayerControlsForDialogue();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (dialogueUI != null)
            dialogueUI.ShowDialogue(currentSpeakerName, currentLines[currentLineIndex]);
    }

    public void ShowNextLine()
    {
        if (!isDialogueOpen || currentLines == null)
            return;

        currentLineIndex++;

        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        if (dialogueUI != null)
            dialogueUI.ShowDialogue(currentSpeakerName, currentLines[currentLineIndex]);
    }

    public void EndDialogue()
    {
        bool openingVendorAfterDialogue =
            currentNPC != null && currentNPC.Role == NPCDialogue.DialogueRole.Vendor;

        if (dialogueUI != null)
            dialogueUI.HideDialogue();

        isDialogueOpen = false;

        if (currentNPC != null)
            currentNPC.OnDialogueFinished();

        onSimpleDialogueFinished?.Invoke();

        if (!openingVendorAfterDialogue)
        {
            StartCoroutine(RestoreControlSmoothly());
        }

        currentNPC = null;
        currentLines = null;
        currentSpeakerName = "";
        onSimpleDialogueFinished = null;
    }

    private void DisablePlayerControlsForDialogue()
    {
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

    private IEnumerator RestoreControlSmoothly()
    {
        if (playerLook != null)
            playerLook.ResetLookInput();

        yield return new WaitForSecondsRealtime(restoreDelay);

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

        yield return null;

        if (playerLook != null)
        {
            playerLook.enabled = true;
            playerLook.ResetLookInput();
            playerLook.SuppressLookInputTemporarily(lookSuppressDuration);
        }
    }
}