using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 dir;
    public float speed = 5;
    private Transform player;
    private bool isMoving;

    private void Start()
    {
        player = GetComponentInChildren<SpriteRenderer>().transform;
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (!isMoving)
        {
            dir.x = Input.GetAxisRaw("Horizontal");
            dir.y = Input.GetAxisRaw("Vertical");
            
            if (dir != Vector2.zero)
            {
                var targetPos = transform.position;
                targetPos.x += dir.x;
                targetPos.y += dir.y;
                StartCoroutine(Move(targetPos));
            }
        }
       
        if (dir.x != 0)
        {
            dir = new Vector2(dir.x, 0);
        }
        else
        {
            dir = Vector2.zero;
        }

       
        FlipPlayer();
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }

    void FlipPlayer()
    {
        if (dir.x != 0)
        {
            player.transform.localScale = new Vector3( Mathf.Sign(dir.x),
                player.transform.localScale.y,
                player.transform.localScale.z);
        }
    }
}
