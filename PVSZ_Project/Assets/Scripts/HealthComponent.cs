using System;
using UnityEngine;

// NOTE(sftl): Could become visual health bar
public class HealthComponent : MonoBehaviour, Damageable
{
    [SerializeField] private int    _health;
    public int                       Health
    {
        get => (_health > 0) ? _health : 0;
    }
    
    private Action _onDeath;
    public Action   OnDeath
    {
        set { _onDeath = value; }
    }
    
    private bool _isAlive = true;
    
    public void TakeDamage(int amount)
    {
        if (!_isAlive) return;
        
        _health -= amount;
        
        if (_health <= 0)
        {
            _isAlive = false;
            _onDeath();
        }
    }
}