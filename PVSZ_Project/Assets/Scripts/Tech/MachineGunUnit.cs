using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunUnit : ShootTechUnit
{
    override protected void Awake()
    {
        base.Awake();
        _shootComponent.OnFire = OnFire;
    }
    
    public void OnFire(Bullet bulletPrefab, Transform rayTransform)
    {
        IEnumerator ShootAfterSec(float sec)
        {
            yield return new WaitForSeconds(sec);
            Instantiate(bulletPrefab, rayTransform.position, Quaternion.identity).Fire(target: null);
        }
        
        // NOTE(sftl): shooting
        StartCoroutine(ShootAfterSec(0.00f));
        StartCoroutine(ShootAfterSec(0.05f));
        StartCoroutine(ShootAfterSec(0.10f));
        
        // Drugi nacin bez korutine ali ne radi za prvi shot
        // if(Time.time > secondAttack + miniDelay){
        //     secondAttack=nextAttack;
        //     Instantiate(shotPrefab, reyPos.position, Quaternion.identity);
        // }
        
        //animator.SetFloat("Shoot", nextAttack);
        AudioManager.Instance.Play_GuardianShoot();
    }
}