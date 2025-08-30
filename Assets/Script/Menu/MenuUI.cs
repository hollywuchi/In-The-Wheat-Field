using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject[] panle;

    public void SwitchPanle(int index)
    {
        for (int i = 0; i < panle.Length; i++)
        {
            if (i == index)
            {
                // 直接让这个元素变为最后一个
                panle[i].transform.SetAsLastSibling();
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("EXIT GAME");
    }
}
