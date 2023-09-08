using UnityEngine;
using TMPro;

public class Object2Placer : MonoBehaviour
{
    public GameObject objectToPlace;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;

    public Material hologramMaterial;
    public float raycastDistance = 100f;
    public KeyCode placeKey = KeyCode.Mouse0;
    public KeyCode hologramKey = KeyCode.Alpha2;

    public int count;
    public TextMeshProUGUI flagText;

    private bool placingObject = false;
    private GameObject hologramObject;
    private Renderer[] hologramRenderers;
    private Vector3 lastMousePosition;

    void Update()
    {
        flagText.text = count.ToString();

        if (Input.GetKeyDown(hologramKey))
        {
            if (count > 0)
            {
                if (placingObject)
                {
                    // Cancel object placement
                    placingObject = false;
                    Destroy(hologramObject);
                }
                else
                {
                    // Start object placement
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
                    {
                        placingObject = true;
                        hologramObject = Instantiate(objectToPlace, hit.point, Quaternion.identity);
                        hologramRenderers = hologramObject.GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderer in hologramRenderers)
                        {
                            renderer.material = hologramMaterial;
                            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        }
                        lastMousePosition = Input.mousePosition;

                        // Add collider to the hologram object
                        Collider objectCollider = hologramObject.AddComponent<BoxCollider>();
                        objectCollider.isTrigger = true;
                    }
                }
            }
        }

        if (placingObject)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            hologramObject.transform.Rotate(Vector3.up, mouseDelta.x * 0.1f, Space.World);
            hologramObject.transform.Rotate(Vector3.right, -mouseDelta.y * 0.1f, Space.World);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
            {
                // always move hologram object if mouse button is being held down
                if (Input.GetKey(placeKey) || !Physics.CheckSphere(hit.point, 0.1f, obstacleLayer))
                {
                    hologramObject.transform.position = hit.point;
                }
            }

            if (Input.GetKeyUp(placeKey))
            {
                // Use collider bounds to calculate object size
                Vector3 objectSize = new Vector3();
                foreach (Renderer renderer in hologramRenderers)
                {
                    objectSize = Vector3.Max(objectSize, renderer.bounds.size);
                }

                // Check if the final position is valid
                if (!Physics.CheckSphere(hologramObject.transform.position, Mathf.Max(objectSize.x, objectSize.y, objectSize.z) / 5f, obstacleLayer))
                {
                    placingObject = false;
                    count--;
                    Destroy(hologramObject);

                    // Instantiate the new object with a collider
                    GameObject newObject = Instantiate(objectToPlace, hologramObject.transform.position, hologramObject.transform.rotation);

                    // Set child renderer materials to different materials
                    foreach (Transform child in newObject.transform)
                    {
                        Renderer childRenderer = child.GetComponent<Renderer>();
                        if (childRenderer != null)
                        {
                            Material newMaterial = new Material(childRenderer.sharedMaterial); // Create a new material
                            newMaterial.color = Color.blue; // Set the material color to blue (replace with desired color)
                            childRenderer.material = newMaterial; // Set the child renderer material to the new material
                        }
                    }

                    Collider newObjectCollider = newObject.AddComponent<BoxCollider>();
                    newObjectCollider.isTrigger = true;
                }
            }
        }
    }
}
