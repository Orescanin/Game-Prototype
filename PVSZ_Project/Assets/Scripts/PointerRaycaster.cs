//#define RAY_DEBUG

using UnityEngine;

// NOTE(sftl): works on tiles when game is in play mode
public class PointerRaycaster : MonoBehaviour
{
    int     dist    = 50;                       // NOTE(sftl): must be greater that tile from camera distance
    Vector2 dir     = Vector2.zero;
    int     mask;
    
    Tile currTile;
    
    public void Start()
    {
        mask = LayerMask.GetMask("Tile");
    }
    
    public void Update()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
#if RAY_DEBUG
        Debug.DrawRay(pos, dir * dist, Color.blue);
#endif
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, dist, mask);
        
        if (hit.collider != null) // NOTE(sftl): first tile hit
        {
            var newTile = hit.transform.gameObject.GetComponent<Tile>();
            if (newTile != currTile) // NOTE(sftl): hit next tile
            {
                currTile?.OnPointerRayExit();
                newTile.OnPointerRayEnter();
            }
            currTile = newTile;
        }
        else
        {
            currTile?.OnPointerRayExit();
            currTile = null;
        }
    }
    
    public void Activate()
    {
        gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        currTile?.OnPointerRayExit();
        currTile = null;
        gameObject.SetActive(false);
    }
}