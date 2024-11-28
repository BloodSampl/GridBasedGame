using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D),typeof(Rigidbody2D))]
public class ExitDoorway : MonoBehaviour
{

  void Reset()
  {
    GetComponent<Rigidbody2D>().isKinematic = true;
    BoxCollider2D collider = GetComponent<BoxCollider2D>();
    collider.size = new Vector2(0.2f, 0.2f);
    collider.isTrigger = true;
  }
  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }
}
