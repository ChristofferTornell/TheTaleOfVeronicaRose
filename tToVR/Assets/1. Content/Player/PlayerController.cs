using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Stats")]
    [Tooltip("Pre-calculation movement speed value. (Both grounded and airborne.)")]
    [SerializeField] private float moveSpeedBase = 8;
    [HideInInspector] public float moveSpeedResult = 0;
    private Vector3 moveDirection;
    [SerializeField] private float rotateSpeedX = 0f;

    [Space]

    [Tooltip("Pre-calculation jump height value.")]
    [SerializeField] private float jumpHeightBase = 20;
    [HideInInspector] public float jumpHeightResult = 0;
    [Tooltip("Increasing this makes the game object drop faster from being airborne.")]
    [SerializeField] private float gravityScale = 0.015f;

    [Header("Input")]
    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private string horizontalAxisInputName = "Horizontal";
    [SerializeField] private string verticalAxisInputName = "Vertical";
    [SerializeField] private string jumpInputName = "Jump";

    [Header("Insertables")]
    [SerializeField] private Animator animator = null;
    [SerializeField] private GameObject playerModel = null;
    private Transform pivot;



    private void Start()
    {
        pivot = CameraController.instance.pivot;
        moveSpeedResult = moveSpeedBase;
        jumpHeightResult = jumpHeightBase;
    }

    private void Update()
    {
        //Resetting moveDirection when grounded        
        if (characterController.isGrounded) { moveDirection.y = 0f; }
        animator.SetBool("grounded", characterController.isGrounded);
        animator.SetFloat("moveSpeed", (Mathf.Abs(Input.GetAxisRaw(verticalAxisInputName)) + (Mathf.Abs(Input.GetAxisRaw("Horizontal")))));

        CheckInputs();
        UpdateMovement();
        LookAtInputDirection();
    }
    private void Jump(float _jumpHeight)
    {
        if (characterController.isGrounded) { moveDirection.y = _jumpHeight; }
    }

    private void UpdateMovement()
    {
        //Apply gravity
        moveDirection.y += Physics.gravity.y * gravityScale;

        //Apply the result to character controller
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void CheckInputs()
    {
        //Saving yDirection
        float yDirection = moveDirection.y;
        moveDirection = (transform.forward * Input.GetAxisRaw(verticalAxisInputName)) + (transform.right * Input.GetAxisRaw(horizontalAxisInputName)); //NOTE: Not doing input raw causes some issues where character doesn't stop right away after releasing key.
        moveDirection = moveDirection.normalized * moveSpeedResult;

        //Applying yDirection after calculating in movespeed to not have movespeed apply to vertical movement.
        moveDirection.y += yDirection;
        if (Input.GetButtonDown(jumpInputName))
        {
            Jump(jumpHeightResult);
        }
    }

    private void LookAtInputDirection()
    {
        if (Input.GetAxisRaw(horizontalAxisInputName) != 0 || Input.GetAxisRaw(verticalAxisInputName) != 0) { transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f); }
        if (moveDirection.x != 0 || moveDirection.z != 0) 
        { 
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeedX * Time.deltaTime);
        }
    }
}