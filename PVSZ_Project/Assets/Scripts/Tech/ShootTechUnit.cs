using UnityEngine;

public class ShootTechUnit : TechUnit
{
    [SerializeField] protected ShootComponent _shootComponent;
    
    protected override void Start()
    {
        base.Start();
        
        if (_shootComponent.NeedsPower) 
        {
            _statusComponent.OnPowerChaged(Tile.HasPower);      // NOTE(sftl): init
            Tile.PowerChanged += _statusComponent.OnPowerChaged;
        }
    }
    
    protected override void Awake()
    {
        base.Awake();
        _shootComponent.Unit = this;
    }
    
    void OnDestroy()
    {
        Tile.PowerChanged -= _statusComponent.OnPowerChaged;
    }
}