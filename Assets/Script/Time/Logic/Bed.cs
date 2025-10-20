using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    private bool wantSleep;
    private GameObject sign => transform.GetChild(0).gameObject;
    void Update()
    {
        if(Input.GetMouseButtonDown(1) && wantSleep)
        {
            UIManager.Instance.OpenSleepUI();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        wantSleep = true;
        sign.SetActive(wantSleep);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        wantSleep = false;
        sign.SetActive(wantSleep);
    }
}
