using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public Vector2 dmgRange;
    public float chaseSpeed;
    public float alertRange;
    private PlayerController player;
    private Vector2 targetPos;
    private List<Vector2> availableMovList = new List<Vector2>();
    private LayerMask obsMask;
    
    bool isMoving;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        obsMask = LayerMask.GetMask("Wall","Enemy","Player");
        targetPos = transform.position;
        StartCoroutine(Movement());
    }

    void Patrol()
    {
        availableMovList.Clear();
        Vector2 size = Vector2.one * .8f;
        Collider2D hitUp = Physics2D.OverlapBox(targetPos + Vector2.up, size, 0,obsMask);
        if (!hitUp)
        {
            availableMovList.Add(Vector2.up);
        }
        Collider2D hitRight = Physics2D.OverlapBox(targetPos + Vector2.right, size, 0,obsMask);
        if (!hitRight)
        {
            availableMovList.Add(Vector2.right);
        }
        Collider2D hitDown = Physics2D.OverlapBox(targetPos + Vector2.down, size, 0,obsMask);
        if (!hitDown)
        {
            availableMovList.Add(Vector2.down);
        }
        Collider2D hitLeft = Physics2D.OverlapBox(targetPos + Vector2.left, size, 0,obsMask);
        if (!hitLeft)
        {
            availableMovList.Add(Vector2.left);
        }

        if (availableMovList.Count > 0)
        {
            int randomIndex = Random.Range(0, availableMovList.Count);
            targetPos += availableMovList[randomIndex];
        }
        StartCoroutine(SmoothMove(2f));
    }

    IEnumerator SmoothMove(float speed)
    {
        isMoving = true;

        while (Vector2.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 5 * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        yield return new WaitForSeconds(speed);
        isMoving = false;
    }

    void Attack()
    {
        int roll = Random.Range(0, 100);
        if (roll > 50)
        {
            float dmgAmount = Mathf.Ceil(Random.Range(dmgRange.x, dmgRange.y));
            Debug.Log(name + "Attacked for + " + dmgAmount + " points of damage");
        }
        else
        {
            Debug.Log(name + "Missed");
        }
    }

    Vector2 FindNextStep(Vector2 startPos, Vector2 targetPos)
    {
        return startPos;
    }
    IEnumerator Movement()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f);
            if (!isMoving)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (dist <= alertRange)
                {
                    if (dist <= 1.1f)
                    {
                        Attack();
                        yield return new WaitForSeconds(Random.Range(.5f, 1.15f));
                    }
                    else
                    {
                        Vector2 newPos = FindNextStep(transform.position, player.transform.position);
                        if (newPos != targetPos)
                        {
                            // chase
                            targetPos = newPos;
                            StartCoroutine(SmoothMove(1));
                        }
                        else
                        {
                            Patrol();
                        }
                    }
                }
                else
                {
                    Patrol();
                }
            } 
        }
    }
}
