using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private float smoothSpeed = 5f;
    
    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 300f;
    [SerializeField] private bool invertY = false;
    
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool canRotate = true;
    
    private void Start()
    {
        //блок и скрытие курсора
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform; //поиск по тегу за кем следить
        }
        
        //подписываемся на события паузы
        GameEvents.PlayerDied += OnGamePaused;
    }
    //чтобы камера двигалась в след кадре после игрока
    private void LateUpdate()
    {
        if (target == null) return;
        
        //проверяем, можно ли вращать камеру
        if (Cursor.lockState == CursorLockMode.Locked && canRotate)
        {
            HandleMouseRotation(); 
        }
        
        UpdateCameraPosition(); 
    }
    
    private void HandleMouseRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        if (invertY)
            mouseY = -mouseY;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 60f);
        yRotation += mouseX;
    }
    
    private void UpdateCameraPosition()
    {
        //вычисление позиции камеры вокруг игрока
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Vector3 desiredPosition = target.position + rotation * offset;
        //плавное перемещение
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f); //камера смотрит на игрока
    }
    
    private void OnGamePaused()
    {
        canRotate = false; //нельзя вращать камерой, если игра на паузе
    }
    //отписка от события
    private void OnDestroy()
    {
        GameEvents.PlayerDied -= OnGamePaused;
    }
}