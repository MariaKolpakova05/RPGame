using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private bool lookAtCamera = true;
    
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
    }
    
    private void LateUpdate()
    {
        if (lookAtCamera && mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                           mainCamera.transform.rotation * Vector3.up);
        }
    }
    
    public void UpdateHealth(float normalizedHealth)
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = normalizedHealth;
            healthFill.color = Color.Lerp(Color.red, Color.green, normalizedHealth);
        }
    }
}