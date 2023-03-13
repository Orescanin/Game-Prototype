using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienTank : Alien
{
    // Start is called before the first frame update
    void Start()
    {
        _startVar = transform.position;
        gridManager = GridManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        CheckEverything();
        MoveIt(1);
    }
}
