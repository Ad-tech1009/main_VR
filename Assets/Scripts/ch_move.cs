using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRCharacterMovement : MonoBehaviour
{
    [Header("Locomotion Settings")]
    public float moveSpeed = 3.0f; // Speed of movement
    public float rotationSpeed = 60.0f; // Speed of rotation

    [Header("References")]
    public CharacterController characterController; // Reference to CharacterController
    public Transform vrRig; // The VR rig (usually the XR Origin or Camera Offset)
    public Transform head; // The player's head (usually the Main Camera)

    [Header("Input Actions")]
    public InputActionProperty moveInput; // Input for movement
    public InputActionProperty turnInput; // Input for rotation

    private void Start()
    {
        // Ensure the required components and references are assigned
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                Debug.LogError("CharacterController is missing! Please attach one to this GameObject.");
                enabled = false;
                return;
            }
        }

        if (vrRig == null || head == null)
        {
            Debug.LogError("Please assign the VR Rig and Head Transform!");
            enabled = false;
            return;
        }

        UpdateCharacterHeight(); // Initial adjustment of CharacterController height
    }

    private void Update()
    {
        UpdateCharacterHeight(); // Adjust CharacterController height based on head position
        HandleMovement(); // Process movement input
        HandleRotation(); // Process rotation input
    }

    private void UpdateCharacterHeight()
    {
        // Adjust the CharacterController to match the player's height
        if (head.localPosition.y <= 0) return; // Skip if the head is at an invalid position

        Vector3 center = vrRig.InverseTransformPoint(head.position);
        characterController.center = new Vector3(center.x, characterController.height / 2, center.z);
        characterController.height = Mathf.Clamp(head.localPosition.y, 1.0f, 2.5f); // Limit the height to reasonable values
    }

    private void HandleMovement()
    {
        // Get input from the controller or keyboard
        Vector2 input = moveInput.action.ReadValue<Vector2>();
        Vector3 direction = vrRig.TransformDirection(new Vector3(input.x, 0, input.y));

        // Apply gravity to ensure the player remains grounded
        Vector3 gravity = Physics.gravity * Time.deltaTime;
        direction += gravity;                             

        // Apply movement
        characterController.Move(direction * moveSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        // Get input for turning
        float turn = turnInput.action.ReadValue<float>();

        // Rotate the VR Rig
        vrRig.Rotate(Vector3.up, turn * rotationSpeed * Time.deltaTime);
    }
}
