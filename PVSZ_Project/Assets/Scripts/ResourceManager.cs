
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI resourcesUI;
    private static int totalResources;
    [SerializeField] public int startingResources = 300;
    [SerializeField] int gainPerTurn = 100;
    [SerializeField] int gainPerTurnMultiplier = 10;
    
    public void Awake()
    {
        totalResources = startingResources;
        updateUI(startingResources);
    }
    
    public void Start()
    {
        CardManager.Instance.UpdateCardsForResources(totalResources);
    }
    
    public void OnTurnIncremented()
    {
        totalResources += gainPerTurn + GameManager.Instance.Turn.Num * gainPerTurnMultiplier;
        //Debug.Log("total " + totalResources);
        updateUI(totalResources);
        
        CardManager.Instance.UpdateCardsForResources(totalResources);
    }
    
    private void updateUI(int r)
    {
        resourcesUI.text = r.ToString();
    }
    
    public void payForUnit(int unitCost)
    {
        totalResources -= unitCost;
        updateUI(totalResources);
        
        CardManager.Instance.UpdateCardsForResources(totalResources);
    }
    
    public static int getResources()
    {
        return totalResources;
    }
    
    public void setResources(int newResources)
    {
        totalResources = newResources;
        updateUI(totalResources);
        
        CardManager.Instance.UpdateCardsForResources(totalResources);
    }
    
    
}
