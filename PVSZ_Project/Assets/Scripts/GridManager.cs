using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private GameObject _parent;
    
    public AlienLaneIndicator indicatorPrefab;
    List<AlienLaneIndicator> indicators = new();
    public GameObject indicatorParent;
    
    public int NumOfLanes { get { return _height; } }
    
    public Tile SelectedTile = null;
    List<List<Tile>> tiles = new();
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        
        GenerateGrid();
    }
    
    void GenerateGrid()
    {
        int width = (int)_tilePrefab.GetComponent<Transform>().localScale.x;
        int height = (int)_tilePrefab.GetComponent<Transform>().localScale.y;
        
        for (int i = 0; i < _width; )
        {
            tiles.Add(new List<Tile>());
            
            for (int j = 0; j < _height; )
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(i, j), Quaternion.identity);
                spawnedTile.Row = j;
                spawnedTile.Column = i;
                spawnedTile.name = $"Tile {i} {j}";
                spawnedTile.transform.parent = _parent.transform;
                var isOffset = (i + j)/(width) % 2 == 1;
                spawnedTile.Init(isOffset);
                tiles[i].Add(spawnedTile);
                j += height;
            }
            i += width;
        }
        
        _parent.transform.position = (new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10))*-1;
        
        SpawnAlienLaneIndicators();
    }
    
    public void TileHover(Tile tile) {
        var preview     = GameManager.Instance.CurrentPreview;
        var cardType    = preview?.CardType;
        
        //-select
        if (
            !tile.IsBlocked ||
            (
             // NOTE(sftl): enable selecting blocked tiles when user is casting Power, not unit
             tile.IsBlocked && preview != null &&
             (
              cardType == CardType.Block ||
              cardType == CardType.Slow  ||
              cardType == CardType.Shield
              )
             )
            )
        {
            SelectedTile = tile;
        }
        
        // TODO(sftl): make highlighting more generic
        
        //-highlight
        if (cardType == CardType.Block) // NOTE(sftl): highlight lane 
        {
            foreach (var column in tiles)
            {
                column[tile.Row].SetHighlight(true);
            }
        }
        else if (cardType == CardType.Slow) // NOTE(sftl): highligh whole grid
        {
            foreach (var column in tiles)
            {
                column.ForEach(action: (Tile t) => { t.SetHighlight(true); });
            }
        }
        else if (cardType == CardType.Shield) // NOTE(sftl): highligh whole grid
        {
            foreach (var column in tiles)
            {
                column.ForEach(action: (Tile t) => { t.SetHighlight(true); });
            }
        }
        else
        {
            if (!tile.IsBlocked) tile.SetHighlight(true);
        }
    }
    
    public void TileHoverExit(Tile tile) {
        var preview     = GameManager.Instance.CurrentPreview;
        var cardType    = preview?.CardType;
        
        //-remove highlight
        if (cardType == CardType.Block) 
        {
            foreach (var column in tiles)
            {
                column[tile.Row].SetHighlight(false);
            }
        }
        else if (cardType == CardType.Slow) 
        {
            foreach (var column in tiles)
            {
                column.ForEach(action: (Tile t) => { t.SetHighlight(false); });
            }
        }
        else if (cardType == CardType.Shield)
        {
            foreach (var column in tiles)
            {
                column.ForEach(action: (Tile t) => { t.SetHighlight(false); });
            }
        }
        else
        {
            if (!tile.IsBlocked) tile.SetHighlight(false);
        }
        
        //-deselect
        SelectedTile = null;
    }
    
    // NOTE(sftl): player can clear preview while inside grid
    public void PreviewCleared(Preview oldPreview)
    {
        if (SelectedTile == null) return; // NOTE(sftl): change doesn't concern grid
        
        var tile = SelectedTile;
        var cardType = oldPreview?.CardType;
        
        if (tile == null) return;
        
        //-remove highlight
        if (cardType == CardType.Block) 
        {
            foreach (var column in tiles)
            {
                var currTile = column[tile.Row];
                if(currTile != tile) currTile.SetHighlight(false);
            }
        }
        else if (cardType == CardType.Slow || cardType == CardType.Shield) 
        {
            foreach (var column in tiles)
            {
                column.ForEach(action: (Tile currTile) => { if(currTile != tile) currTile.SetHighlight(false); });
            }
        }
    }
    
    public bool canAlienMove(Vector3 startP,bool isUp)
    {
        float up = startP.y + _tilePrefab.transform.localScale.y;
        float down = startP.y - _tilePrefab.transform.localScale.y;
        var firstTile = tiles[0][0];
        var lastTile = tiles[0].Last();
        var bukizila = _tilePrefab.transform.localScale.y / 2;
        
        if (down < firstTile.transform.position.y-bukizila && !(isUp))
            return false;
        else if (up > lastTile.transform.position.y + bukizila && isUp)
            return false;
        
        return true;
    }
    
    public bool IsAlienInPlayerBase(Vector3 pos)
    {
        var tileW = _tilePrefab.transform.localScale.y;
        var firstTile = tiles[0][0];
        
        if (pos.x < firstTile.transform.position.x - tileW) return true;
        return false;
    }
    
    // NOTE(sftl): returns null if none is selected
    public Tile GetSelectedTileIfAvailable()
    {
        return SelectedTile;
    }
    
    public List<Vector3> GetAvailableSpawnPos()
    {
        var r = new List<Vector3>();
        var offset = new Vector3(_tilePrefab.transform.localScale.x * 2f, 0f, 0f);
        
        // NOTE(sftl): get first row y positions
        var lastColumn = tiles.Last();
        foreach (var tile in lastColumn)
        {
            r.Add(tile.transform.position + offset);
        }
        
        return r;
    }
    
    // NOTE(sftl): return null if no lane is selected
    public (Vector3, Vector3)? GetSelectedLaneStartEndPos()
    {
        if (SelectedTile == null) return null;
        return (tiles.First()[SelectedTile.Row].transform.position, tiles.Last()[SelectedTile.Row].transform.position);
    }
    
    public float GetNeighbourLaneY(Alien alien)
    {
        var tileSize = _tilePrefab.transform.localScale.y;
        var alienPos = alien.transform.position;
        
        var possibilities = new List<float>();
        if (canAlienMove(alienPos, isUp: true)) possibilities.Add(alienPos.y + tileSize);   // TODO(sftl): local scale is used frequently, should we have a field?
        if (canAlienMove(alienPos, isUp: false)) possibilities.Add(alienPos.y - tileSize);  // NOTE(sftl): if can't move up, Alien must be able to move down
        
        return possibilities[UnityEngine.Random.Range(0, possibilities.Count)];
    }
    
    public void SpawnAlienLaneIndicators()
    {
        var lastRow = tiles.Last();
        var offset = new Vector3(_tilePrefab.transform.localScale.x, 0f, 0f);
        
        foreach (var tile in lastRow)
        {
            var curr = Instantiate(indicatorPrefab, tile.transform.position + offset, Quaternion.identity);
            curr.transform.SetParent(indicatorParent.transform);
            indicators.Add(curr);
        }
    }
    
    public void SetAlienLaneIndicators(List<List<Alien>> nextWave)
    {
        indicators.ForEach (it => it.Clear());  // NOTE(sftl): clearing from last turn
        
        for (int i = 0; i < indicators.Count; i++)
        {
            var laneDifficuly = 0;
            
            nextWave[i].ForEach(
                                alien => 
                                {
                                    laneDifficuly ++;      // = alien.Difficulty;
                                    indicators[i].AddAlien(alien);
                                }
                                );
            
            indicators[i].SetDifficulty(laneDifficuly);
        }
    }
    
    public void SetTilesIsBlocked()
    {
        var percentage      = 10;
        
        var maxNumOfBlocked = (_width * _height) / 100f * percentage;
        var numOfBlocked    = 0;
        
        foreach (var column in tiles)
        {
            foreach (var tile in column)
            {
                if (numOfBlocked < maxNumOfBlocked && UnityEngine.Random.Range(1, 101) < percentage)
                {
                    tile.IsBlocked = true;
                    numOfBlocked++;
                }
                else 
                {
                    tile.IsBlocked = false;
                }
            }
        }
        
        return;
    }
    
    public void ChangePower(Tile tile, bool addPower)
    {
        bool isInGridW(int x) => x < _width && x >= 0;
        bool isInGridH(int x) => x < _height && x >= 0;
        
        for (int i = tile.Column - 1; i < tile.Column + 2; i++)
        {
            for (int j = tile.Row - 1; j < tile.Row + 2; j++)
            {
                if (
                    isInGridH(j) && isInGridW(i) &&
                    (i != tile.Column || j != tile.Row) // isn't that exact tile
                    )
                {
                    var currTile = tiles[i][j];
                    if (!currTile.IsBlocked) currTile.ChangePower(addPower);
                }
            }
        }
    }
}