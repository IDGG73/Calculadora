using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    [Foldout("Animation")]
    [SerializeField] GameObject interactionMessage;
    [SerializeField] string animatorBoolName = "Show";

    [Foldout("Settings")]
    [SerializeField, Tooltip("Name of the button to interact with.\n\nA list with all available buttons can be found inside 'Settings Manager' Game Object")] string inputAction = "Interact";
    [Space]
    [SerializeField, Tooltip("Disable this Game Object via 'SetActive(false)' right after interaction")] bool disableGameObjectOnInteract;
    [SerializeField, Tooltip("Enable Interaction right after this Game Object is enabled via 'SetActive(true)'")] bool enableInteractionOnEnable = true;

    [Foldout("Events")]
    [SerializeField] UnityEvent onInteract;

    Animator interactionPromptAnimator;

    bool interactable;
    bool playerIsInside;

    private void Awake()
    {
        interactionPromptAnimator = interactionMessage.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (enableInteractionOnEnable)
            Invoke(nameof(EnableInteraction), 0.5f);
    }

    private void Update()
    {
        if (playerIsInside && interactable /*&& !Pause.isPaused && IntroScreen.alreadyBegan*/)
        {
            if (inputAction == string.Empty || !SettingsManager.GetInputAction(inputAction).WasPressedThisFrame())
                return;

            onInteract.Invoke();

            interactable = false;

            if (interactionPromptAnimator)
            {
                if (animatorBoolName != string.Empty)
                    interactionPromptAnimator.SetBool(animatorBoolName, false);
            }
            else
                interactionMessage.SetActive(false);

            if (disableGameObjectOnInteract)
                gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (interactable /*&& collision.gameObject == PlayerController.current.gameObject*/)
        {
            if (interactionPromptAnimator)
            {
                interactionMessage.SetActive(true);

                if (animatorBoolName != string.Empty)
                    interactionPromptAnimator.SetBool(animatorBoolName, true);
            }
            else
                interactionMessage.SetActive(true);
        }

        playerIsInside = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject /*!= PlayerController.current.gameObject*/)
            return;

        if (interactionPromptAnimator)
        {
            if (animatorBoolName != string.Empty)
                interactionPromptAnimator.SetBool(animatorBoolName, false);
        }
        else
            interactionMessage.SetActive(false);

        playerIsInside = false;
    }

    public void EnableInteractionDelayed(float delay) => Invoke(nameof(EnableInteraction), delay);

    public void EnableInteraction()
    {
        interactable = true;

        if (playerIsInside)
        {
            if (interactionPromptAnimator)
            {
                interactionMessage.SetActive(true);

                if (animatorBoolName != string.Empty)
                    interactionPromptAnimator.SetBool(animatorBoolName, true);
            }
            else
                interactionMessage.SetActive(true);
        }
    }
    public void SetInteractable(bool canInteract)
    {
        interactable = canInteract;

        if (playerIsInside && canInteract)
        {
            if (interactionPromptAnimator)
            {
                interactionMessage.SetActive(true);

                if (animatorBoolName != string.Empty)
                    interactionPromptAnimator.SetBool(animatorBoolName, true);
            }
            else
                interactionMessage.SetActive(true);
        }
    }
}
