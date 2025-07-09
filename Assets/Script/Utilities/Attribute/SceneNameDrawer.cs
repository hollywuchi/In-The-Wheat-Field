using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{

    int sceneIndex = -1;

    GUIContent[] sceneNames;

    readonly string[] sceneNameSplite = { "/", ".unity" };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="property"></param>
    /// <param name="label">将所有的场景都保存成这个</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 如果BuildSetting里没有场景，那么就不执行后面的代码
        if (EditorBuildSettings.scenes.Length == 0) return;

        if (sceneIndex == -1)
            GetSceneNameArray(property);

        int oldIndex = sceneIndex;

        sceneIndex = EditorGUI.Popup(position, label, sceneIndex, sceneNames);
        if (oldIndex != sceneIndex)
            property.stringValue = sceneNames[sceneIndex].text;
    }

    private void GetSceneNameArray(SerializedProperty property)
    {
        // 将所有的场景都添加到数组中
        var scenes = EditorBuildSettings.scenes;
        // 初始化目标格式的数组，有多少个场景就多长
        sceneNames = new GUIContent[scenes.Length];

        for (int i = 0; i < sceneNames.Length; i++)
        {
            string path = scenes[i].path;
            // 切分方法，要切分的部分，切分模式
            string[] splitPath = path.Split(sceneNameSplite, System.StringSplitOptions.RemoveEmptyEntries);

            string sceneName = "";

            if (splitPath.Length > 0)
            {
                sceneName = splitPath[splitPath.Length - 1];
            }
            else
            {
                sceneName = "(Delete Scene)";
            }

            sceneNames[i] = new GUIContent(sceneName);
        }

        if (sceneNames.Length == 0)
        {
            sceneNames = new[] { new GUIContent("Chack Your Build Settings") };
        }

        // 如果其中property中没有值或者为空
        if (!string.IsNullOrEmpty(property.stringValue))
        {
            bool nameFound = false;

            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (sceneNames[i].text == property.stringValue)
                {
                    sceneIndex = i;
                    nameFound = true;
                    break;
                }
            }
            if (nameFound == false)
            {
                sceneIndex = 0;
            }
        }
        else
        {
            sceneIndex = 0;
        }

        property.stringValue = sceneNames[sceneIndex].text;
    }
}