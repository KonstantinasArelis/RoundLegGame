using System.Collections;
using UnityEngine;

public class PistolController : MonoBehaviour
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
