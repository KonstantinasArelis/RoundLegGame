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
}


