using System.Collections.Generic;
using UnityEngine;

// This script is used to control the player's game object in the third-person perspective.
public class ThirdPController : MonoBehaviour
{
    public CharacterController controller;
    // the Main Camera in the scene
    public Transform camera;
    public float speed = 6.0f;
    public float runSpeed = 12.0f;

    private float currentSpeed = 0.0f;

    // parameters for smooth turning
    public float turnSmoothingTime = 0.1f;
    float turnSmoothingVelocity;

    // parameters for jumping
    bool isGrounded;
    public float jumpHeight = 1.0f;
    public float jumpAdjustment = -2.0f;
    public float gravity = -9.81f;

    public Vector3 playerVelocity;

    public bool bhopEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen.
    }

    // Update is called once per frame
    void Update() {
        // Debug.Log(isGrounded);
        // Debug.Log(controller.isGrounded);
        // We are using GetAxisRaw in case the player is using a controller.
        // Not tested yet.
        if (controller.isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        if (direction.magnitude >= 0.1f) // if no input, stop applying movement
        {
            // Movement and also handles jumping while moving
            // This if section might need to be moved to a new function and changed if bhop or other movement mechanics are added.


            // Calculate the angle between the player's input direction and the positive x-axis.
            // This angle is then used to rotate the player's game object so that it faces the direction of movement.
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            // Smoothly rotate the player's game object to face the direction of movement.
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothingVelocity, turnSmoothingTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // if Jumping while moving
            if (controller.isGrounded) {
                if (bhopEnabled) {
                    if (Input.GetButton("Jump")) {
                        Jump();
                    }
                } else {
                    if (Input.GetButtonDown("Jump")) {
                        Jump();
                    }
                }
                // player cannot "run" while in the air
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                    currentSpeed = runSpeed;
                } else {
                    currentSpeed = speed;
                }
            }
            // current speed is preserved while in the air
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        } else if (controller.isGrounded) {
            // jumping in place
                if (bhopEnabled) {
                    if (Input.GetButton("Jump")) {
                        Jump();
                    }
                } else {
                    if (Input.GetButtonDown("Jump")) {
                        Jump();
                    }
                }
            }
        playerVelocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // Character Controller handles collision detection different. Make sure to read docs.
    // I don't believe it can detect triggers, so an alternative method would be creating an empty GameObject child
    // and then giving that child a collider (make sure it doesn't collide with Player) to detect triggers.

    // private void OnCollisionStay(Collision collision) {
    //     if (collision.gameObject.tag == "Ground") {
    //         isGrounded = true;
    //     }
    // }
    // private void OnCollisionExit(Collision collision) {
    //     if (collision.gameObject.tag == "Ground") {
    //         isGrounded = false;
    //     }
    // }
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        // Debug.Log("Controller collision detected");
    }
    void Jump() {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * jumpAdjustment * gravity);
    }
}