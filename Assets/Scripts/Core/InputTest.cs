// InputTest.cs
/*using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    private PlayerInput playerInput;
    
    private void Awake()
    {
        playerInput = new PlayerInput();
    }
    
    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.PhysicalAttack.performed += OnPhysicalAttack;
        playerInput.Player.MagicalAttack.performed += OnMagicalAttack;
    }
    
    private void OnDisable()
    {
        playerInput.Player.PhysicalAttack.performed -= OnPhysicalAttack;
        playerInput.Player.MagicalAttack.performed -= OnMagicalAttack;
        playerInput.Disable();
    }
    
    private void OnPhysicalAttack(InputAction.CallbackContext context)
    {
        Debug.Log("ЛЕВАЯ КНОПКА МЫШИ НАЖАТА! Физическая атака.");
    }
    
    private void OnMagicalAttack(InputAction.CallbackContext context)
    {
        Debug.Log("ПРАВАЯ КНОПКА МЫШИ НАЖАТА! Магическая атака.");
    }
}*/