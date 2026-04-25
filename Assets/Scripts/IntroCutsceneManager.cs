using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private QuestFeedbackUI questFeedbackUI;

    [SerializeField] private GameObject cutsceneCamera;

    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform playerBody;
    [SerializeField] private CharacterController playerController;

    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private PlayerInput playerInput;

    [Header("Positioning After Cutscene")]
    [SerializeField] private Transform playerWakePosition;
    [SerializeField] private Transform playerLookTarget;

    [Header("Timing")]
    [SerializeField] private float cutsceneDuration = 3f;
    [SerializeField] private float transitionDelay = 0.15f;

    [Header("Goblin Dialogue")]
    [SerializeField] private string goblinName = "Skrik";

    [TextArea(2, 5)]
    [SerializeField] private string[] goblinLines;

    private void Start()
    {
        if (dialogueManager == null)
            dialogueManager = FindFirstObjectByType<DialogueManager>();

        if (questFeedbackUI == null)
            questFeedbackUI = FindFirstObjectByType<QuestFeedbackUI>();

        LockPlayerControls();
        StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        yield return new WaitForSeconds(cutsceneDuration);

        MovePlayerToWakePosition();
        FacePlayerTowardGoblin();

        if (cutsceneCamera != null)
            cutsceneCamera.SetActive(false);

        yield return new WaitForSeconds(transitionDelay);

        if (dialogueManager != null)
        {
            dialogueManager.StartSimpleDialogue(
                goblinName,
                goblinLines,
                OnGoblinDialogueFinished
            );
        }
    }

    private void MovePlayerToWakePosition()
    {
        if (playerRoot == null || playerWakePosition == null)
            return;

        if (playerController != null)
            playerController.enabled = false;

        playerRoot.position = playerWakePosition.position;
        playerRoot.rotation = playerWakePosition.rotation;

        if (playerController != null)
            playerController.enabled = true;
    }

    private void FacePlayerTowardGoblin()
    {
        if (playerBody == null || playerLookTarget == null)
            return;

        Vector3 dir = playerLookTarget.position - playerBody.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
            playerBody.forward = dir.normalized;
    }

    private void OnGoblinDialogueFinished()
    {
        UnlockPlayerControls();

        if (questFeedbackUI != null)
        {
            questFeedbackUI.ShowMessage(
                "Objective",
                "Talk to the crazy old man."
            );
        }
    }

    private void LockPlayerControls()
    {
        if (playerLook != null) playerLook.enabled = false;
        if (playerMove != null) playerMove.enabled = false;
        if (playerAttack != null) playerAttack.enabled = false;
        if (playerInteract != null) playerInteract.enabled = false;
        if (playerBlock != null) playerBlock.enabled = false;
        if (playerInput != null) playerInput.enabled = false;
    }

    private void UnlockPlayerControls()
    {
        if (playerLook != null) playerLook.enabled = true;
        if (playerMove != null) playerMove.enabled = true;
        if (playerAttack != null) playerAttack.enabled = true;
        if (playerInteract != null) playerInteract.enabled = true;
        if (playerBlock != null) playerBlock.enabled = true;
        if (playerInput != null) playerInput.enabled = true;
    }
}