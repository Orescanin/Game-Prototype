using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public enum CardType
{
    //units
    Guardian,
    Collector,
    MachineGun,
    Wall,
    Scan,
    Tower,
    
    //powers
    Block = 1000,
    Slow,
    Shield
}

public class Card : MonoBehaviour, ICard, IPointerDownHandler
{
    [SerializeField] private Image _cardBody;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _resourceNum;
    
    [SerializeField] private ParticleSystem _clickParticles;
    
    public CardType Type;
    
    private bool _enabled = true;
    public bool Enabled
    {
        get { return _enabled; }
        set {
            _enabled = value;
            
            // NOTE(sftl): null is default material for UI components
            var material = (value) ? null : CardManager.Instance.DisabledMaterial;
            _cardBody.material  = material;
            _image.material     = material;
        }
    }
    
    private int _cost;
    public int Cost
    {
        get { return _cost; }
        set { 
            _cost = value;
            _resourceNum.text = $"{value}";
        }
    }
    
    public void OnPointerDown(PointerEventData pointerData)
    {
        if ((pointerData.button == PointerEventData.InputButton.Left) && _enabled)
        {
            GameManager.Instance.CardClicked(this);
            _clickParticles.Play();
        }
    }
}