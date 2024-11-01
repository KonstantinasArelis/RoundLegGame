using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] private GameObject buildingPrefab;
    private GameObject lastHighlightedBuilding;
    private Color defaultColor = Color.gray;
    private Color highlightedColor = Color.grey;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HighlightSelectedBuilding();
        Build();
        Destroy();
    }

    private void Build()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("Enemy"))
                {
                    // don't allow to build on player or enemies
                    return;
                }
                Vector3Int buildingPosition = new (
                    Mathf.RoundToInt(hit.point.x + hit.normal.x / 2),
                    Mathf.RoundToInt(hit.point.y + hit.normal.y / 2),
                    Mathf.RoundToInt(hit.point.z + hit.normal.z / 2));
                // parent under this script so it's organised
                Instantiate(buildingPrefab, buildingPosition, Quaternion.identity, transform);
            }
        } 
    }

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
