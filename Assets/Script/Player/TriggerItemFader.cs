using UnityEngine;

public class TriggerItemFader : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        ItemFader[] faders = other.GetComponentsInChildren<ItemFader>();
        if(faders.Length > 0)
        {
            foreach(var fader in faders)
            {
                fader.FadeIn();
            }
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        ItemFader[] faders = other.GetComponentsInChildren<ItemFader>();
        if(faders.Length > 0)
        {
            foreach(var fader in faders)
            {
                fader.FadeOut();
            }
        }
    }
}
