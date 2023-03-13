#define SHOOTCOMP_DEBUG
using System;
using UnityEngine;

public class ShootComponent : MonoBehaviour
{
    [HideInInspector] public TechUnit Unit;                     // TODO(sftl): make universal
    
    [SerializeField] private Transform  _rayTransform;
    [SerializeField] private Bullet     _bulletPrefab;
    
    [SerializeField] private float      _maxDistance;
    [SerializeField] private float      _minDistance;
    
    [SerializeField] private int        _shootCooldown;         // NOTE(sftl): seconds
    [SerializeField] private bool       _needsPower;
    
    private Action<Bullet, Transform>   _onFire;
    [HideInInspector] public Action<Bullet, Transform> OnFire
    {
        set { _onFire = value; }
    }
    
    [HideInInspector] public bool       NeedsPower => _needsPower;
    
    private float       _nextScan;
    private Vector3?    _target;
    
    void FixedUpdate()
    {
#if SHOOTCOMP_DEBUG
        Debug.DrawRay(_rayTransform.position, _rayTransform.right * _maxDistance, Color.red);
#endif
        
        if (Time.time > _nextScan)
        {
            Ray ray = new Ray(_rayTransform.position, _rayTransform.right);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _maxDistance, LayerMask.GetMask("Alien"));
            
            if (hit && hit.distance >= _minDistance)
            {
                onTarget(hit.point);
            }
        }
    }
    
    void Update()
    {
        if (_target != null)
        {
            if (
                !_needsPower ||
                (_needsPower && Unit.Tile.HasPower)
                ) 
            {
                if (_onFire == null) onFire();
                else _onFire(_bulletPrefab, _rayTransform);
                
                _target     = null;
                _nextScan   = Time.time + _shootCooldown;
            }
        }
    }
    
    private void onTarget(Vector3 target)
    {
        _target = target;
    }
    
    private void onFire()
    {
        var bullet = Instantiate(_bulletPrefab, _rayTransform.transform.position, Quaternion.identity);
        bullet.Fire(_target.Value);
    }
}