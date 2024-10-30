using UnityEngine;

public class ShotgunController : MonoBehaviour
{

    private FireLine[] fireLines;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireLines = GetComponentsInChildren<FireLine>();
    }

    // Update is called once per frame
    public void Fire()
    {
        // just delegate so that you can add extra logic later
        foreach (FireLine fireLine in fireLines)
        {
            fireLine.Fire();
        }
    }
}
