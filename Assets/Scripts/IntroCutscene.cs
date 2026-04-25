using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private GameObject introCamera;
    [SerializeField] private GameObject playerCamera;

    [Header("Player")]
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerLook playerLook;

    [Header("Optional Cinemachine")]
    [SerializeField] private GameObject gameplayCinemachine;

    [Header("UI")]
    [SerializeField] private QuestFeedbackUI questFeedbackUI;

    [Header("Timing")]
    [SerializeField] private float introDuration = 11f;

    void Start()
    {
        if (playerMove != null)
            playerMove.canMove = false;

        if (playerLook != null)
            playerLook.enabled = false;

        if (gameplayCinemachine != null)
            gameplayCinemachine.SetActive(false);

        if (introCamera != null)
            introCamera.SetActive(true);

        if (playerCamera != null)
            playerCamera.SetActive(false);

        Invoke(nameof(StartGame), introDuration);
    }

    void StartGame()
    {
        if (introCamera != null)
            introCamera.SetActive(false);

        if (playerCamera != null)
            playerCamera.SetActive(true);

        if (gameplayCinemachine != null)
            gameplayCinemachine.SetActive(true);

        if (playerMove != null)
            playerMove.canMove = true;

        if (playerLook != null)
        {
            playerLook.ResetLookInput();
            playerLook.enabled = true;
            playerLook.SuppressLookInputTemporarily(0.2f);
        }

        if (questFeedbackUI != null)
        {
            questFeedbackUI.ShowMessage(
                "Objective",
                "Talk to Brother Cael."
            );
        }
    }
}