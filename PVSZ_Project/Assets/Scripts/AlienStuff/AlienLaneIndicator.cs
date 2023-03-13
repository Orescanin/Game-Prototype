using TMPro;
using UnityEngine;

public class AlienLaneIndicator : MonoBehaviour
{
    [SerializeField] private GameObject _tank;      // TODO(sftl): handle multiple
    [SerializeField] private GameObject _moowalk;   // TODO(sftl): handle multiple
    
    public TextMeshPro textPro;
    
    public void SetDifficulty(int dif)
    {
        textPro.text = dif.ToString();
    }
    
    public void AddAlien(Alien alien)
    {
        if (alien is AlienTank)         _tank.SetActive(true);
        if (alien is AlienMoonwalker)   _moowalk.SetActive(true);
    }
    
    public void Clear()
    {
        _tank.SetActive(false);
        _moowalk.SetActive(false);
    }
}