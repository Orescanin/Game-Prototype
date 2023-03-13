using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AlienMoonwalker : Alien
{
    void Start()
    {
        _startVar = transform.position;
        gridManager = GridManager.Instance;
    }
    
    void Update()
    {
        CheckEverything();
        moonWalk();
    }
    
    protected override void SetAttackAnim(bool isAttacking)
    {
        // Debug.Log("attacking animation: " + isAttacking);
        if (isAttacking) 
        {
            animator.SetBool("Move", false);
            animator.SetBool("Attack", true);
        }
        else 
        {
            animator.SetBool("Attack", false);
            animator.SetBool("Move", true);
        }
    }
    
    protected override void PlayDeathAnim()
    {
        // Debug.Log("death animation");
        animator.SetBool("Death", true);
    }
}
