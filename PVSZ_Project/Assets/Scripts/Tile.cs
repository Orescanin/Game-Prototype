using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    
    [SerializeField] private SpriteRenderer _renderer;
    
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _blockedIndicator;
    [SerializeField] private GameObject _powerIndicator;
    
    public int Column;
    public int Row;
    
    private bool   _isBlocked;
    public bool     IsBlocked 
    {
        get { return _isBlocked; }
        set { 
            _isBlocked = value;
            _blockedIndicator.SetActive(value);
        }
    }
    
    private int _power;
    public bool HasPower { get => _power > 0; }
    public event Action<bool> PowerChanged;
    
    public TechUnit Unit;
    
    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }
    
    public void OnPointerRayEnter()
    {
        GameManager.Instance.gridManager.TileHover(this);
    }
    
    public void OnPointerRayExit()
    {
        GameManager.Instance.gridManager.TileHoverExit(this);
    }
    
    public void SetHighlight(bool isHighlighted)
    {
        _highlight.SetActive(isHighlighted);
    }
    
    public void ChangePower(bool addPower)
    {
        var hadPower = HasPower;
        
        if (addPower) _power++;
        else _power--;
        
        var hasPower = HasPower;
        
        if (hasPower != hadPower)
        {
            PowerChanged?.Invoke(HasPower);
            _powerIndicator.SetActive(HasPower);
        }
    }
}
