using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;


#if UNITY_EDITOR
[CustomEditor(typeof(AnimAndAudioControl))]
public class AnimAndAudioControlInspector : Editor
{
    private SerializedObject obj;
    private AnimAndAudioControl testA;
    private SerializedProperty iterator;
    private List<string> propertyNames;
    private PlayType tryGetValue;
    private Dictionary<string, PlayType> specialPropertys
        = new Dictionary<string, PlayType>
        {
            { "_audioClip", PlayType.PT_Audio },//表示字段a只会在枚举值=typeA时显示
            { "_overAction", PlayType.PT_SingleGroupLoop }
        };
    void OnEnable()
    {
        obj = new SerializedObject(target);
        iterator = obj.GetIterator();
        iterator.NextVisible(true);
        propertyNames = new List<string>();
        do
        {
            propertyNames.Add(iterator.name);
        } while (iterator.NextVisible(false));
        testA = (AnimAndAudioControl)target;
    }

    public override void OnInspectorGUI()
    {
        obj.Update();
        GUI.enabled = false;
        foreach (var name in propertyNames)
        {
            if (specialPropertys.TryGetValue(name, out tryGetValue)
                && tryGetValue != testA.playType)
                continue;
            EditorGUILayout.PropertyField(obj.FindProperty(name));
            if (!GUI.enabled)//让第1次遍历到的 Script 属性为只读
                GUI.enabled = true;
        }
        obj.ApplyModifiedProperties();
    }
}
#endif
