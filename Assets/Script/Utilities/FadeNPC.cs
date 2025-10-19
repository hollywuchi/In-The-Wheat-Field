using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FadeNPC : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<SpriteRenderer>().enabled = false;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        collision.GetComponent<SpriteRenderer>().enabled = true;
    }

}
