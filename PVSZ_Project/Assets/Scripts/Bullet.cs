using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float  _speed;
    [SerializeField] private float  _destroyTime;
    
    [SerializeField] private int    _damage;
    public int                      Damage
    {
        get { return _damage; }
    }
    
    private float _lifetime;
    
    protected void Update()
    {
        _lifetime += Time.deltaTime;
        if (_lifetime > _destroyTime)
        {
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(gameObject);
    }
    
    // NOTE(sftl): passing null defaults to bullet firing to the right
    public void Fire(Vector3? target)
    {
        var dir = (target == null) ? Vector3.right : Vector3.Normalize(target.Value - transform.position);
        
        var angle = Vector2.Angle(transform.right, new Vector2(dir.x, dir.y));
        if (dir.y < 0) angle = -angle;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        GetComponent<Rigidbody2D>().velocity = dir * _speed;
    }
}