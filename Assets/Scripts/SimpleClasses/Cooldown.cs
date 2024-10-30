using UnityEngine;

public class Cooldown
{
    private readonly float cooldownTime;
    private float lastTime;

    public Cooldown(float cooldownTime)
    {
        this.cooldownTime = cooldownTime;
        lastTime = 0;
    }

    public bool IsReady()
    {
        if ((lastTime == 0) || (Time.time - lastTime >= cooldownTime))
        {
            lastTime = Time.time;
            return true;
        }
        return false;
    }
}