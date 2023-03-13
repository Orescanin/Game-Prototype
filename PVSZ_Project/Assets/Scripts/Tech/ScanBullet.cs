using UnityEngine;

public class ScanBullet : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        // TODO(sftl): effect
        base.OnTriggerEnter2D(col);
    }
}