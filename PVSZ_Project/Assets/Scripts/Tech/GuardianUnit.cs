using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianUnit : ShootTechUnit
{
    [SerializeField] private ParticleSystem shootParticles;
    
    protected override void Awake()
    {
        base.Awake();
        _shootComponent.OnFire = OnFire;
    }
    
    public void OnFire(Bullet bulletPrefab, Transform rayTransform)
    {
        Instantiate(bulletPrefab, rayTransform.position, Quaternion.identity).Fire(target: null);
        
        shootParticles.Play();
        _animator.SetTrigger("Shoot");
        AudioManager.Instance.Play_GuardianShoot();
    }
}