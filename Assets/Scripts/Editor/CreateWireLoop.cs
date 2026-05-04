#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CreateWireLoop : MonoBehaviour
{
    [MenuItem("Tools/Create Wire Loop")]
    static void CreateWireLoopTool()
    {
        // === ROOT OBJECT ===
        GameObject wireLoop = new GameObject("WireLoop");
        wireLoop.transform.position = Vector3.zero;

        // === HANDLE (thin cylinder) ===
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        handle.name = "Handle";
        handle.transform.SetParent(wireLoop.transform);
        handle.transform.localPosition = new Vector3(0f, 0f, 0f);
        handle.transform.localScale = new Vector3(0.015f, 0.12f, 0.015f); // Very thin, ~24cm tall

        // Set handle color to dark grey (metal)
        Renderer handleRenderer = handle.GetComponent<Renderer>();
        Material handleMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        handleMat.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Dark grey metal
        handleMat.name = "WireLoop_Handle_Mat";
        handleRenderer.material = handleMat;

        // === WIRE STEM (thin cylinder going up from handle) ===
        GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stem.name = "WireStem";
        stem.transform.SetParent(wireLoop.transform);
        stem.transform.localPosition = new Vector3(0f, 0.2f, 0f);
        stem.transform.localScale = new Vector3(0.004f, 0.08f, 0.004f); // Very thin wire

        // Set wire color to silver/platinum (nichrome)
        Renderer stemRenderer = stem.GetComponent<Renderer>();
        Material wireMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        wireMat.color = new Color(0.75f, 0.75f, 0.78f, 1f); // Silver/platinum
        wireMat.SetFloat("_Metallic", 0.9f);
        wireMat.SetFloat("_Smoothness", 0.8f);
        wireMat.name = "WireLoop_Wire_Mat";
        stemRenderer.material = wireMat;

        // === LOOP (ring made from a flattened torus-like arrangement of small spheres) ===
        GameObject loopParent = new GameObject("Loop");
        loopParent.transform.SetParent(wireLoop.transform);
        loopParent.transform.localPosition = new Vector3(0f, 0.29f, 0f);

        // Create ring from small capsules arranged in a circle
        float ringRadius = 0.015f;
        int segments = 16;
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float nextAngle = (float)(i + 1) / segments * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * ringRadius;
            float z = Mathf.Sin(angle) * ringRadius;
            float nextX = Mathf.Cos(nextAngle) * ringRadius;
            float nextZ = Mathf.Sin(nextAngle) * ringRadius;

            // Small cylinder segment for the ring
            GameObject ringSegment = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ringSegment.name = "RingSegment_" + i;
            ringSegment.transform.SetParent(loopParent.transform);

            // Position at midpoint between current and next point
            ringSegment.transform.localPosition = new Vector3(
                (x + nextX) / 2f,
                0f,
                (z + nextZ) / 2f
            );

            // Scale to be a tiny segment
            ringSegment.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);

            // Rotate to connect the dots
            Vector3 direction = new Vector3(nextX - x, 0, nextZ - z);
            if (direction != Vector3.zero)
            {
                ringSegment.transform.localRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
            }

            // Apply wire material
            ringSegment.GetComponent<Renderer>().material = wireMat;
            
            // Remove individual colliders (we'll add one to the parent)
            DestroyImmediate(ringSegment.GetComponent<Collider>());
        }

        // === ADD COLLIDERS & COMPONENTS TO ROOT ===
        // Add a single capsule collider to the whole wire loop
        CapsuleCollider col = wireLoop.AddComponent<CapsuleCollider>();
        col.center = new Vector3(0f, 0.1f, 0f);
        col.radius = 0.03f;
        col.height = 0.35f;
        col.direction = 1; // Y-axis

        // Add Rigidbody
        Rigidbody rb = wireLoop.AddComponent<Rigidbody>();
        rb.mass = 0.1f;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // === SELECT THE NEW OBJECT ===
        Selection.activeGameObject = wireLoop;
        
        // === SAVE AS PREFAB ===
        string prefabPath = "Assets/Prefabs/Props/WireLoop.prefab";
        
        // Ensure directory exists
        if (!System.IO.Directory.Exists("Assets/Prefabs/Props"))
        {
            System.IO.Directory.CreateDirectory("Assets/Prefabs/Props");
        }
        
        // Save prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(
            wireLoop, prefabPath, InteractionMode.UserAction);

        Debug.Log("✅ Wire Loop created and saved as prefab at: " + prefabPath);
        Debug.Log("📍 Move it into FlameTestArea and position it near the samples on the table.");
        
        EditorUtility.DisplayDialog(
            "Wire Loop Created!", 
            "Wire Loop has been created in the scene and saved as a prefab.\n\n" +
            "Next steps:\n" +
            "1. Move it into FlameTestArea in the Hierarchy\n" +
            "2. Position it on the table near the samples\n" +
            "3. The game scripts will add XR Grab Interactable at runtime",
            "OK"
        );
    }
}
#endif
