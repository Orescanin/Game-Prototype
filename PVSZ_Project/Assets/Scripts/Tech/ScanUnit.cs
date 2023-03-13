using UnityEngine;

public class ScanUnit : ShootTechUnit
{
    [SerializeField] private float          _rotationSpeed;
    [SerializeField] private float          _maxSideAngle;
    
    private bool    _isRotatingRight;
    private float   _angle;
    
    override protected void Update()
    {
        base.Update();
        
        //-calc rotation
        if (_isRotatingRight)
        {
            _angle += _rotationSpeed * Time.deltaTime;
            if (_angle > _maxSideAngle)
            {
                _isRotatingRight = false;
            }
        }
        else 
        {
            // NOTE(sftl): how do negative angles work? is this correct?
            _angle -= _rotationSpeed * Time.deltaTime;
            if (_angle < -_maxSideAngle)
            {
                _isRotatingRight = true;
            }
        }
        
        //-rotate
        transform.rotation = Quaternion.Euler(0f, 0f, _angle);
    }
    
    protected override void OnDeath()
    {
        // TODO(sftl): effect
        base.OnDeath();
    }
}