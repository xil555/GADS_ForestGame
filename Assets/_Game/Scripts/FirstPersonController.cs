using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float crouchSpeed = 2.5f; // Slower speed for crouch-walking
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Crouch Settings")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchTransitionSpeed = 10f; // How fast the camera lerps down

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 15f;

    // Private state variables
    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isCrouching = false;

    // Input Actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // --- INPUT SYSTEM SETUP ---
        var inputMap = new InputActionMap("Player");

        moveAction = inputMap.AddAction("Move");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        lookAction = inputMap.AddAction("Look", binding: "<Mouse>/delta");

        // Add Jump and Crouch bindings
        jumpAction = inputMap.AddAction("Jump", binding: "<Keyboard>/space");
        crouchAction = inputMap.AddAction("Crouch", binding: "<Keyboard>/ctrl"); // Left Control to crouch

        // Enable all actions
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        crouchAction.Enable();
    }

    void Update()
    {
        HandleLook();
        HandleCrouch();
        HandleMovement();
        HandleJump();
    }

    void HandleLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        // Read the button hold state. If Control is held down, isCrouching is true.
        isCrouching = crouchAction.IsPressed();

        // Determine the target height based on the crouch state
        float targetHeight = isCrouching ? crouchHeight : standingHeight;

        // Smoothly lerp the controller's height
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        // Crucial fix: Adjust the controller's center so the player's "feet" stay on the floor as they shrink
        controller.center = new Vector3(0, controller.height / 2f, 0);
    }

    void HandleMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Dynamically choose speed based on crouch state
        float currentSpeed = isCrouching ? crouchSpeed : walkSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleJump()
    {
        // Jump only if grounded, pressing jump, and NOT crouching (prevents weird physics glitches)
        if (jumpAction.WasPressedThisFrame() && controller.isGrounded && !isCrouching)
        {
            // Physics equation for jump velocity: v = sqrt(height * -2 * gravity)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void OnDestroy()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        crouchAction.Disable();
    }
}