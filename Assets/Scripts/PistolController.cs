using System.Collections;
using UnityEngine;

public class PistolController : MonoBehaviour
{
    private LineFire lineFire;

    void Start()
    {
        lineFire = GetComponentInChildren<LineFire>();
    }

    public void Fire()
    {
        // just delegate so that you can add extra logic later
        lineFire.Fire();
    }
}
