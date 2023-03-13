using System;
using UnityEngine;

public abstract class TechUnit : MonoBehaviour, Damageable
{
    [SerializeField] protected HealthComponent  _healthComponent;
    [SerializeField] protected StatusComponent  _statusComponent;
    
    [SerializeField] protected SpriteRenderer   _spriteRenderer;
    [SerializeField] protected Animator         _animator;
    
    public Tile     Tile;
    public TurnInfo TurnSpawned;
    
    private float _removeShieldTime;
    
    protected virtual void Awake()
    {
        _healthComponent.OnDeath = OnDeath;
    }
    
    protected virtual void Start()
    {
        //-rendering order
        var diffFromRefPos = GameManager.Instance.DrawOrderRefPos.y - transform.position.y;
        int diffFromRefOrder = (int)(diffFromRefPos / GameManager.Instance.DrawOrderPrecision);
        _spriteRenderer.sortingOrder = GameManager.Instance.DrawOrderRefNum + diffFromRefOrder;
    }
    
    protected virtual void Update()
    {
        CheckShield();
    }
    
    public void TakeDamage(int amount)
    {
        if (!_statusComponent.HasShield) _healthComponent.TakeDamage(amount);
    }
    
    protected virtual void OnDeath()
    {
        GameManager.Instance.OnTechDeath(this);
    }
    
    public void AddShieldForSec(float sec)
    {
        _removeShieldTime = Time.time + sec;
        _statusComponent.HasShield = true;
    }
    
    public void RemoveStatusEffects()
    {
        _statusComponent.Clear();
    }
    
    private void CheckShield()
    {
        if (Time.time > _removeShieldTime && _statusComponent.HasShield) 
        {
            _statusComponent.HasShield = false;
        }
    }
}