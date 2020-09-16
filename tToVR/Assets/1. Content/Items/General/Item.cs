using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string title;
    [TextArea] public string description;
    public Sprite icon;
}
