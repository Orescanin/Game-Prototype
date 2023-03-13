using System;
using UnityEngine;

public class StatusComponent : MonoBehaviour
{
    [SerializeField] private GameObject _shield;
    [SerializeField] private GameObject _noPower;
    
    [SerializeField] private bool _hasShield;
    public bool HasShield
    {
        get { return _hasShield; }
        set { 
            _hasShield = value;
            _shield.SetActive(value);
        }
    }
    
    // NOTE(sftl): clears temporary effects
    public void Clear()
    {
        HasShield = false;
    }
    
    public void OnPowerChaged(bool hasPower)
    {
        _noPower.SetActive(!hasPower);
    }
}