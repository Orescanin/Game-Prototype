//#define DEBUG_GAMEMANAGER
//#define NO_END_GAME

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

public enum GameState
{
    Playing,
    Paused,
    End,
}

public class GameManager : MonoBehaviour
{
    // NOTE(sftl): singleton
    public static GameManager Instance;
    
    //- scene objects
    public CardManager      cardManager;
    public ResourceManager  resourceManager;
    public GridManager      gridManager;
    public PlayUI           playUI;
    public PointerRaycaster raycaster;
    public WaveGenerator    waveGen;
    
    public EndScreen        endScreen;
    
    //- preview prefabs, // TODO(sftl): it should probably be just one generic prefab with changable image, could be a part of CardInfo
    public Preview guardianPrevPrefab;
    public Preview resourceCollectorPrevPrefab;
    public Preview machineGunPrevPrefab;
    public Preview wallPrevPrefab;
    public Preview towerPrevPrefab;
    
    public Preview  removePrevPrefab;
    
    //- power prefabs
    public ScanUnit    powerScanPrefab;
    
    public Preview      powerScanPrevPrefab;
    public Preview      powerBlockPrevPrefab;
    public Preview      powerSlowPrevPrefab;
    public Preview      powerShieldPrevPrefab; 
    
    //- turns
    public const int    EndTurnNum  = 5;
    public TurnInfo     Turn        = new TurnInfo(1, TurnType.Tech);
    
    public      GameEvent turnIncrementedEvent;
    
    //- game state
    public GameState   GameState = GameState.Playing;
    
    //- utils
    public readonly Vector3 DrawOrderRefPos = Vector3.zero;
    public readonly int     DrawOrderRefNum = 15000;    // SpriteRenderer.sortingOrder max is 32767
    public readonly float   DrawOrderPrecision = 0.2f;  // Real world width for which units are considered to be on the same rendering layer and rendering order is not defined
    
    public Preview          CurrentPreview;
    
    List<Alien>             aliens = new();
    List<List<Alien>>       nextWaveAliens = new();
    
    List<TechUnit>     techs = new();
    List<TechUnit>     temp_techs = new();
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Debug.Log("Tried to create mutliple GameManager instances.");
        }
        
        Instance = this;
    }
    
    void Start()
    {
        GenWaveAndShow();
        gridManager.SetTilesIsBlocked();
    }
    
    void Update()
    {
        if(GameState == GameState.Paused || GameState == GameState.End) return;
        
        if (Input.GetMouseButtonUp(1)) // NOTE(sftl): right click
        {
            if (CurrentPreview != null)
            {
                DestroyPreviewIfNotNull();
                SetPreview(null);
            }
        }
        else if (Input.GetMouseButtonUp(0)) // TODO(sftl): Mouse Down and Mouse Up should both happen on the tile
        {
            var tile = gridManager.SelectedTile;
            if (tile != null) TileClicked(tile);
        }
        
        //-alien loop
        List<Alien> aliensInBase = new();
        
        if (aliens.Count > 0)
        {
            foreach (var alien in aliens)
            {
                // handle aliens in base
                if (gridManager.IsAlienInPlayerBase(alien.transform.position))
                {
                    aliensInBase.Add(alien);
                }
                
                //-handle unit front rendering
                // TODO(sftl): optimise, don't do this every frame, just when unit spawns or changes lane
                // TODO(sftl): when spawning, rendering position may not be accurate the first frame
                // TODO(sftl): handle infinite units
                var diffFromRefPos = DrawOrderRefPos.y - alien.transform.position.y;
                int diffFromRefOrder = (int)(diffFromRefPos / DrawOrderPrecision);
                alien.GetComponent<SpriteRenderer>().sortingOrder = DrawOrderRefNum + diffFromRefOrder;
            }
        }
        
#if NO_END_GAME
        aliensInBase.ForEach(alien => {
                                 OnAlienDeath(alien);
                                 Destroy(alien.gameObject);
                             });
#else
        if (aliensInBase.Count > 0) PlayerLost();
#endif
    }
    
    public void PauseGame()
    {
        Time.timeScale  = 0f;
        GameState       = GameState.Paused;
        
        raycaster.Deactivate();
        
        if (CurrentPreview != null)
        {
#if DEBUG_GAMEMANAGER
            Debug.Log("Preview hidden on pause.");
#endif
            CurrentPreview.gameObject.SetActive(false);
        }
    }
    
    public void ResumeGame()
    {
        Time.timeScale  = 1f;
        GameState       = GameState.Playing;
        
        raycaster.Activate();
        
        if (CurrentPreview != null)
        {
#if DEBUG_GAMEMANAGER
            Debug.Log("Preview un-hidden on pause.");
#endif
            CurrentPreview.gameObject.SetActive(true);
        }
    }
    
    private void TileClicked(Tile tile)
    {
        if (CurrentPreview == null) return;
        
        if (CurrentPreview.Type == PreviewType.Remove)
        {
            var unit = tile.Unit;
            
            if (unit != null)
            {
                RemoveTech(unit);
                Refund(unit); 
            }
            
            return;
        }
        
        var res = ResourceManager.getResources();
        var (_, cost, unitPrefab) = cardManager.CardsInfo[CurrentPreview.CardType];
        
        if (unitPrefab != null && tile.Unit != null) return; // NOTE(sftl): tile is occupied
        
        if (res >= cost)
        {
            resourceManager.payForUnit(cost);
            
            var cardType = CurrentPreview.CardType;
            
            if (unitPrefab != null)
            {
                var pos = tile.transform.position;
                var techUnit = Instantiate(unitPrefab, pos, Quaternion.identity);
                
                techUnit.TurnSpawned = Turn;
                techUnit.Tile = tile;
                tile.Unit = techUnit;
                
                techs.Add(techUnit);
                
                if (techUnit is ScanUnit)   temp_techs.Add(techUnit); 
                if (techUnit is TowerUnit)  gridManager.ChangePower(tile, addPower: true);
            }
            //-powers
            else if (cardType == CardType.Slow)
            {
                aliens.ForEach(action: (Alien a) => { a.SlowForSec(5); });
                
                DestroyPreviewIfNotNull();
                SetPreview(null);
            }
            else if (cardType == CardType.Shield)
            {
                techs.ForEach(action: (TechUnit a) => { a.AddShieldForSec(5); });
                
                DestroyPreviewIfNotNull();
                SetPreview(null);
            }
            else if (cardType == CardType.Block)
            {
                var laneData = gridManager.GetSelectedLaneStartEndPos();
                
                if (laneData != null)
                {
                    var (laneStartPos, laneEndPos) = laneData.Value;
                    var dir = Vector3.Normalize(laneEndPos - laneStartPos);
                    var hits = Physics2D.RaycastAll(laneStartPos, dir, Mathf.Infinity, LayerMask.GetMask("Alien"));
                    
                    foreach (var hit in hits)
                    {
                        var alien = hit.collider.gameObject.GetComponent<Alien>();
                        // if(!alien.IsChangingLanes) 
                        alien.MoveToNeightourLane();
                    }
                    
                    DestroyPreviewIfNotNull();
                    SetPreview(null);
                }
            }
        }
    }
    
    public void CardClicked(Card card)
    {
#if DEBUG_GAMEMANAGER
        Debug.Log("Preview destroyed since new one is initialized.");
#endif
        DestroyPreviewIfNotNull();
        
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        //-units
        if (card.Type == CardType.Guardian)
            SetPreview(Instantiate(guardianPrevPrefab, pos, Quaternion.identity));
        
        if (card.Type == CardType.Collector)
            SetPreview(Instantiate(resourceCollectorPrevPrefab, pos, Quaternion.identity));
        
        if (card.Type == CardType.MachineGun)
            SetPreview(Instantiate(machineGunPrevPrefab, pos, Quaternion.identity));
        
        if (card.Type == CardType.Wall)
            SetPreview(Instantiate(wallPrevPrefab, pos, Quaternion.identity));
        
        if(card.Type == CardType.Scan) 
            SetPreview(Instantiate(powerScanPrevPrefab, pos, Quaternion.identity));
        
        if(card.Type == CardType.Tower) 
            SetPreview(Instantiate(towerPrevPrefab, pos, Quaternion.identity));
        
        //-powers
        else if(card.Type == CardType.Block)
            SetPreview(Instantiate(powerBlockPrevPrefab, pos, Quaternion.identity));
        
        else if(card.Type == CardType.Slow)
            SetPreview(Instantiate(powerSlowPrevPrefab, pos, Quaternion.identity));
        
        else if(card.Type == CardType.Shield)
            SetPreview(Instantiate(powerShieldPrevPrefab, pos, Quaternion.identity));
    }
    
    public void RemoveCardClicked()
    {
#if DEBUG_GAMEMANAGER
        Debug.Log("Preview destroyed since new one is initialized.");
#endif
        DestroyPreviewIfNotNull();
        
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SetPreview(Instantiate(removePrevPrefab, pos, Quaternion.identity));
    }
    
    public void GenWaveAndShow()
    {
        nextWaveAliens = waveGen.GetWave(Turn.Num);
        gridManager.SetAlienLaneIndicators(nextWaveAliens);
    }
    
    public void EndTechTurn()
    {
        Turn.Type = TurnType.Alien;
        playUI.OnAlienTurn();
        
        if (CurrentPreview != null)
        {
            DestroyPreviewIfNotNull();
            SetPreview(null);
        }
        
        SpawnAliens();
    }
    
    public void EndAlienTurn()
    {
        Assert.IsTrue(aliens.Count == 0);
        
        if (CurrentPreview != null)
        {
            DestroyPreviewIfNotNull();
            SetPreview(null);
        }
        
        //-remove temp tech units
        foreach (var unit in temp_techs)
        {
            Destroy(unit.gameObject);
            techs.Remove(unit);
        }
        
        //check if unit is of certain type
        foreach (TechUnit unit in techs)
        {
            if (unit.GetType() == typeof(ResourceUnit))
            {
                ResourceUnit resourceUnit = (ResourceUnit) unit;
                resourceUnit.IncreaseRescources();
            }
        }
        temp_techs.Clear();
        
        //-remove status effects
        foreach (var unit in techs)
        {
            unit.RemoveStatusEffects();
        }
        
        Turn.Type = TurnType.Tech;
        Turn.Num++;
        playUI.OnTechTurn();
        GenWaveAndShow();
        
        Time.timeScale = 1f; // NOTE(sftl): if fast forwarded, reset to normal speed
    }
    
    public void FastForward()
    {
        Time.timeScale = 10f;
    }
    
    void DestroyPreviewIfNotNull()
    {
        if (CurrentPreview == null) return;
        Destroy(CurrentPreview.gameObject);
    }
    
    void SetPreview(Preview newPreview)
    {
        // TODO(sftl): we shouldn't even change preview objects if they are of same type?
        var oldPreview = CurrentPreview;
        
        CurrentPreview = newPreview;
        
        if(oldPreview != null && newPreview == null) gridManager.PreviewCleared(oldPreview);
    }
    
    // NOTE(sftl): also removes all effects that Tech Unit had
    private void RemoveTech(TechUnit unit)
    {
        //-remove effects
        if (unit is TowerUnit) gridManager.ChangePower(unit.Tile, addPower: false);
        
        //-remove unit
        // TODO(sftl): give reseources back for units that are placed this turn or maybe even previous turns
        Destroy(unit.gameObject);
        
        techs.Remove(unit);
        temp_techs.Remove(unit);
        
        unit.Tile.Unit = null;
    }
    
    private void Refund(TechUnit unit)
    {
        int refund;
        int cost = cardManager.GetUnitCost(unit);
        
        if (unit.TurnSpawned == Turn && Turn.Type == TurnType.Tech) refund = cost; // NOTE(sftl): basically undo
        else refund = (int) (cost / 3f);
        
        var res = ResourceManager.getResources() + refund;
        resourceManager.setResources(res);
    }
    
    public void OnTechDeath(TechUnit unit)
    {
        RemoveTech(unit);
    }
    
    void SpawnAliens()
    {
        var availablePos = gridManager.GetAvailableSpawnPos();
        
        // NOTE(sftl): spawning with small random time differences
        var random = new Random();
        for (int i = 0; i < nextWaveAliens.Count; i++)
        {
            foreach (var alienPrefab in nextWaveAliens[i])
            {
                var sec = (float)random.NextDouble(); // NOTE(sftl): range [0, 1)
                var pos = availablePos[i];
                
                StartCoroutine(
                               DoAfterSec(
                                          sec,
                                          () =>
                                          {
                                              var alien = Instantiate(alienPrefab, pos, Quaternion.identity);
                                              aliens.Add(alien);
                                          }
                                          )
                               );
            }
        }
    }
    
    public void OnAlienDeath(Alien alien)
    {
        aliens.Remove(alien);
        
        if (aliens.Count == 0)
        {
            //-handle win
#if !NO_END_GAME
            if (Turn.Num >= EndTurnNum){
                PlayerWon();
                return;
            }
#endif
            
            EndAlienTurn();
            turnIncrementedEvent.Raise();
        }
    }
    
    public void PlayerLost()
    {
        Time.timeScale  = 0f;
        GameState       = GameState.End;
        
        raycaster.Deactivate();
        
        if (CurrentPreview != null)
        {
#if DEBUG_GAMEMANAGER
            Debug.Log("Preview hidden on end game.");
#endif
            CurrentPreview.gameObject.SetActive(false);
        }
        
        endScreen.gameObject.SetActive(true);
        endScreen.SetWinState(won: false);
    }
    
    public void PlayerWon()
    {
        Time.timeScale  = 0f;
        GameState       = GameState.End;
        
        raycaster.Deactivate();
        
        if (CurrentPreview != null)
        {
#if DEBUG_GAMEMANAGER
            Debug.Log("Preview hidden on end game.");
#endif
            CurrentPreview.gameObject.SetActive(false);
        }
        
        endScreen.gameObject.SetActive(true);
        endScreen.SetWinState(won: true);
    }
    
    IEnumerator DoAfterSec(float sec, Action action)
    {
        yield return new WaitForSeconds(sec);
        action();
    }
}
