using UnityEngine;
using UnityEngine.InputSystem;

// Central hub for reading input actions and exposing their values to the rest of the game.
[RequireComponent(typeof(PlayerInput))]
public class InputHub : MonoBehaviour
{
    // Keep a single persistent instance so input data is available across scenes.
    public static InputHub Instance { get; private set; }

    // Current movement input vector from the "Move" action.
    public Vector2 Move { get; private set; }

    // Reference to the attached PlayerInput component.
    private PlayerInput input;

    private void Awake()
    {
        // Prevent duplicate input hubs from being created when a scene reloads.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Cache the input component so we can subscribe to its action events.
        input = GetComponent<PlayerInput>();
    }

    // Called whenever an input action is triggered by the Input System.
    public void OnAction(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
                // Store the latest movement vector for other systems to read.
                Move = context.ReadValue<Vector2>();
                break;
            // Add more cases for other actions as needed.
        }
    }

    private void OnEnable()
    {
        // Listen for all triggered actions while this behaviour is active.
        input.onActionTriggered += OnAction;
    }

    private void OnDisable()
    {
        // Remove the callback to avoid duplicate subscriptions.
        input.onActionTriggered -= OnAction;
    }
}
