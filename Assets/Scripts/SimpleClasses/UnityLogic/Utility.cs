using UnityEngine;

public static class Utility
{
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
    {
        foreach(Transform tr in parent.transform)
        {
            if  (tr.CompareTag(tag))
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }

    public static void DestroyChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static float Max(this Vector3 vector)
        => Mathf.Max(vector.x, vector.y, vector.z);

    public static Color WithTweakedAlpha(this Color color, float alpha)
        => new (color.r, color.g, color.b, alpha);

    public static T[] LoadResourcesToArray<T>(string filepath) where T : ScriptableObject
    {
        UnityEngine.Object[] objects = Resources.LoadAll(filepath);
        T[] resources = new T[objects.Length];
        objects.CopyTo(resources, 0);
        return resources;
    }

    // SPECIFIC to URP
    public static void SetMaterialToTransparent(Material material)
    {
        // Set Surface Type to Transparent
        material.SetFloat("_Surface", 1); // 1 is Transparent, 0 is Opaque for URP

        // Set Blending Mode to Alpha blending
        material.SetInt("_BlendSrc", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_BlendDst", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        // Disable Depth Write for transparency sorting
        material.SetInt("_ZWrite", 0);

        // Enable transparency-related keywords
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");

        // Set the render queue to Transparent
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    public static Directions GetCollidableObjectBoundaries(GameObject gameObject)
    {
        return new Directions{
            front = gameObject.transform.position.z + gameObject.GetComponent<Collider>().bounds.extents.z,
            right = gameObject.transform.position.x + gameObject.GetComponent<Collider>().bounds.extents.x,
            back = gameObject.transform.position.z - gameObject.GetComponent<Collider>().bounds.extents.z,
            left = gameObject.transform.position.x - gameObject.GetComponent<Collider>().bounds.extents.x
        };
    }

}


