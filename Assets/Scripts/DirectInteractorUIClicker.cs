using UnityEngine;
using UnityEngine.UI;

public class SimpleUIClicker : MonoBehaviour
{
    public float rayLength = 10f;
    private LineRenderer line;
    
    void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.cyan;
        line.endColor = Color.cyan;
        line.positionCount = 2;
    }
    
    void Update()
    {
        // Draw ray
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward * rayLength);
        
        // Cast ray for UI
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength))
        {
            line.SetPosition(1, hit.point);
            
            // Check if on UI layer
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                // Get any trigger press (works without Input Actions!)
                if (Input.GetMouseButtonDown(0) || // Mouse for testing
                    Input.GetKeyDown(KeyCode.Space) || // Space for testing
                    Input.GetButtonDown("Fire1")) // Generic button
                {
                    Button btn = hit.collider.GetComponent<Button>();
                    if (btn != null)
                    {
                        Debug.Log($"✅ Clicking: {btn.name}");
                        btn.onClick.Invoke();
                    }
                }
            }
        }
    }
}