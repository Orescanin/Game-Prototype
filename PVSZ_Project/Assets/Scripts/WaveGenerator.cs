using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WaveGenerator : MonoBehaviour
{
    // NOTE(sftl): prefabs
    public Alien standard;
    public Alien moon_walker;
    public Alien tank;
    
    List<Alien> prefabs;                                    // NOTE(sftl): utility list of prefabs for easier picking, sorted by alien difficulty from gratest to lowest
    
    List<int> difficulties;                                 // NOTE(sftl): per turn, sum of individual alien difficulties
    List<List<List<Alien>>> turn_combs;                     // NOTE(sftl): list of combinations per turn, of which one must be generated
    
    bool is_predefined = false;
    List<List<Alien>> predefined;                           // NOTE(sftl): aliens per lane
    
    void Awake()
    {
        prefabs = new()
        {
            standard,
            moon_walker,
            tank,
            // NOTE(sftl): add new prefabs here
            
        };
        prefabs.Sort(
                     (Alien x, Alien y) => { return y.Difficulty.CompareTo(x.Difficulty); } // NOTE(sftl): from gratest to lowest
                     );
        
        difficulties = new()
        {
            // turn 0
            1,
            // turn 1
            5,
            // turn 2
            10,
            // turn 3
            15,
            // turn 4
            20,
            
        };
        
        turn_combs = new()
        {
            // turn 0
            new()
            {
                // combination 0
                new()
                {
                },
            },
            
            // turn 1
            new()
            {
                // combination 0
                new()
                {
                    standard, standard
                },
            },
            
            // turn 2
            new()
            {
                // combination 0
                new()
                {
                    tank
                },
                // combination 1
                new()
                {
                    moon_walker, moon_walker
                },
            },
            
            // turn 3
            new()
            {
                // combination 0
                new()
                {
                },
            },
            
            // turn 4
            new()
            {
                // combination 0
                new()
                {
                    tank, tank, tank, tank
                },
            },
            
        };
        
        predefined = new()
        {
            // lane 0
            new()
            {
                standard, standard
            },
            
            // lane 1
            new()
            {
                moon_walker
            },
            
            // lane 2
            new()
            {
            },
            
            // lane 3
            new()
            {
                tank
            },
            
            // lane 4
            new()
            {
            },
        };
    }
    
    // NOTE(sftl): returns alien prefabs by lane
    public List<List<Alien>> GetWave(int turn_num)
    {
        if (is_predefined) return predefined;
        
        turn_num--; // NOTE(sftl): we want zero as a first turn to easier reference utilities
        var random = new System.Random();
        
        // NOTE(sftl): add combination
        List<List<Alien>> combs;
        if      (turn_num < turn_combs.Count) combs = turn_combs[turn_num];
        else    combs = new() { new() }; // NOTE(sftl): empty combination
        
        var comb = combs[random.Next(0, combs.Count)]; // NOTE(sftl): random combination
        
        int df;
        if      (turn_num < difficulties.Count) df = difficulties[turn_num];
        else    df = difficulties.Last(); // NOTE(sftl): continue with the latest difficulty
        
        var comb_df = comb.Sum((Alien x) => x.Difficulty);
        df -= comb_df;
        
        List<Alien> wave = new(comb);
        
        // NOTE(sftl): fill wave until difficulty is satisfied
        int low_bound = 0;
        while(df > 0)
        {
            for (; low_bound < prefabs.Count; low_bound++)
            {
                if (prefabs[low_bound].Difficulty <= df) break;
            }
            if (low_bound == prefabs.Count) break; // NOTE(sftl): no more valid aliens
            
            var alien = prefabs[random.Next(low_bound, prefabs.Count)];
            wave.Add(alien);
            df -= alien.Difficulty;
        } 
        
        // NOTE(sftl): group by lane
        var wave_lanes = new List<List<Alien>>();
        var num_of_lanes = GameManager.Instance.gridManager.NumOfLanes;
        
        for (int i = 0; i < num_of_lanes; i++) wave_lanes.Add(new());
        
        foreach (var alien in wave)
        {
            wave_lanes[random.Next(num_of_lanes)].Add(alien);
        }
        
        return wave_lanes;
    }
}

