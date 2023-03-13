using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Direction
{
    Down = -1,
    Left = 0,
    Up = 1,

}

public class AlienMovement : MonoBehaviour
{
    private bool keepMoving = false;
    private float alienSpeed;
    public Tile tile;
    private Vector3 tilePos;

    private Vector3 previousAlienPos;
    private Direction previousMove = Direction.Left;


    private void Awake()
    {
        tilePos = tile.GetComponent<Transform>().localScale;
    }

    private bool IsTilePassed(Direction direction)
    {
        var currAlienPosX = Mathf.Abs(transform.position.x - previousAlienPos.x);
        var currAlienPosY = Mathf.Abs(transform.position.y - previousAlienPos.y);

        return direction == Direction.Left ? currAlienPosX > tilePos.x : currAlienPosY > tilePos.y;
    }

    private Vector3 GetOneStep(Direction direction)
    {
        if (direction == Direction.Up) return new Vector3(0f, alienSpeed * Time.deltaTime, 0f);
        else if (direction == Direction.Down) return new Vector3(0f, -alienSpeed * Time.deltaTime, 0f);
        return new Vector3(-alienSpeed * Time.deltaTime, 0f, 0f);
    }


    private void AlienMove(Direction direction)
    {
        keepMoving = true;
        if (IsTilePassed(direction))
        {
            previousMove = direction;
            keepMoving = false;
            previousAlienPos = transform.position;
        }
        else
        {
            transform.position += GetOneStep(direction);
        }
    }


}
