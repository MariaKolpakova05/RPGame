using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float mouseSensitivity = 2f;
    
    private CharacterController controller;
    private Camera playerCamera;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isGrounded;
    private Animator animator;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerCamera = GetComponentInChildren<Camera>();
        
        //блокировка курсора в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);
        
        //Анимации
        if (animator != null)
        {
            bool isMoving = moveX != 0 || moveZ != 0;
            bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);
            
            animator.SetBool("IsWalking", isMoving && !isRunning);
            animator.SetBool("IsRunning", isRunning);
        }
    }

}