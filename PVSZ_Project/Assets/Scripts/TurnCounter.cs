using TMPro;
using UnityEngine;

public class TurnCounter :  MonoBehaviour
{
    public TextMeshProUGUI textPro;
    
    
    public void Awake()
    {
        textPro.text = "1";
    }
    
    public void OnTurnIncremented()
    {
        textPro.text = GameManager.Instance.Turn.Num.ToString();
    }
    
}