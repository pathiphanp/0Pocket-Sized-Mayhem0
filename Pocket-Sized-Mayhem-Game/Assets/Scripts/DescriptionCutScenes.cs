using TMPro;
using UnityEngine;

[System.Serializable]
public class DescriptionCutScenes
{
    public string nameDescription;
    [TextArea(5, 10)] public string description;
    public GameObject cutScenesObject;
    public TMP_Text textCutScens;
    public int durationCutScenes;
}
