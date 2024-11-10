using QuickOutline;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BuildSystem : MonoBehaviour
{
    public BuildingData currentBuilding = null;
    private GameObject lastHighlightedBuilding;
    private Color highlightedColor = Color.white;

    private float groundY;

    public UnityEvent<BuildingData> BuildingPlacedEvent;
    public UnityEvent<int> BuildingDestroyedEvent;

    void Awake()
    {
        var ground = GameObject.Find("Plane").transform;
        groundY = ground.GetComponent<Collider>().bounds.max.y;
    }

    // Update is called once per frame
    void Update()
    {
        Build();
        HighlightSelectedBuilding();
        Destroy();
    }

    private void Build()
    {
        // ignore clicks when
        if (
            currentBuilding == null 
            || !Input.GetMouseButtonDown(0)
            // UI elements
            || EventSystem.current.IsPointerOverGameObject()
        )
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (
            hit.collider.gameObject.CompareTag("Player")
            || hit.collider.gameObject.CompareTag("Enemy")
            || hit.point.y > groundY
            )
            {
                // don't allow to build
                return;
            }
            // some buildings could have "Placement" empty obejct to show what is their bottom Y
            Transform placement = currentBuilding.prefab.transform.Find("Placement");
            float placeOffsetY = placement == null
                ? hit.normal.y/2 : (currentBuilding.prefab.transform.position.y - placement.position.y);
            Vector3 buildingPosition = new (
                Mathf.RoundToInt(hit.point.x + hit.normal.x / 2),
                groundY + placeOffsetY,
                Mathf.RoundToInt(hit.point.z + hit.normal.z / 2));
            PlaceBuilding(buildingPosition);
        }
    }

    // what to do with score when you destroy it?
    private void Destroy()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Building"))
                {
                    var cost = hit.collider.gameObject.GetComponent<IntDataCarrier>().value;
                    Destroy(hit.collider.gameObject);
                    BuildingDestroyedEvent?.Invoke(cost);
                }
            }
        }
    }

    private void HighlightSelectedBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == lastHighlightedBuilding)
            {
                return;
            }
            if (lastHighlightedBuilding != null)
            {
                lastHighlightedBuilding.GetComponent<Outline>().enabled = false;
            }
            if (hit.collider.CompareTag("Building"))
            {
                hit.collider.gameObject.GetComponent<Outline>().enabled = true;
                lastHighlightedBuilding = hit.collider.gameObject;
            }
            else
            {
                lastHighlightedBuilding = null;
            }
        }
    }

    private void PlaceBuilding(Vector3 buildingPosition)
    {
        // parent under this script so it's organised
        GameObject building = Instantiate(currentBuilding.prefab, buildingPosition, currentBuilding.prefab.transform.rotation, transform);
        building.tag = "Building";
        var outline = building.AddComponent<Outline>();
        outline.OutlineColor = highlightedColor;
        outline.enabled = false;
        building.AddComponent<IntDataCarrier>().value = currentBuilding.cost;
        BuildingPlacedEvent?.Invoke(currentBuilding);
    }
}
