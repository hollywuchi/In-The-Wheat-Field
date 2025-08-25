using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "SceneSoundList_SO",menuName = "Audio/SceneSoundList_SO")]
public class SceneSoundList_SO : ScriptableObject
{
    public List<SceneSoundItem> sceneSoundItems;

    public SceneSoundItem GetSceneSound(string name)
    {
        return sceneSoundItems.Find(s => s.sceneName == name);
    }
}

[System.Serializable]
public class SceneSoundItem
{
    [SceneName] public string sceneName;
    public SoundName ambient;
    public SoundName music;
}
