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
            Vector3Int buildingPosition = new (
                Mathf.RoundToInt(hit.point.x + hit.normal.x / 2),
                Mathf.RoundToInt(groundY + hit.normal.y / 2),
                Mathf.RoundToInt(hit.point.z + hit.normal.z / 2));
            // parent under this script so it's organised
            GameObject building = Instantiate(currentBuilding.prefab, buildingPosition, currentBuilding.prefab.transform.rotation, transform);
            building.tag = "Building";
            var outline = building.AddComponent<Outline>();
            outline.OutlineColor = highlightedColor;
            outline.enabled = false;
            BuildingPlacedEvent?.Invoke(currentBuilding);
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
                    Destroy(hit.collider.gameObject);
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
}
