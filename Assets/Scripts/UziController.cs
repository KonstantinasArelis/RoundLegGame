using System.Collections;
using UnityEngine;

public class UziController : MonoBehaviour
{
    private FireLine fireLine;

    void Start()
    {
        fireLine = GetComponentInChildren<FireLine>();
    }

    public void Fire()
    {
        // just delegate so that you can add extra logic later
        fireLine.Fire();
    }
}
