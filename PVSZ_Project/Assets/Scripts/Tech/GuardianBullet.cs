using UnityEngine;

public class GuardianBullet : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        // TODO(sftl): effect
        base.OnTriggerEnter2D(col);
    }
}