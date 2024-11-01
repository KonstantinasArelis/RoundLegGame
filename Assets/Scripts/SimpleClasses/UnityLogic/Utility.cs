using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
}


