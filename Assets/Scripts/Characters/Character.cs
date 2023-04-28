using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character")]
public class Character : ScriptableObject
{
    [Header("Information")]
    public Sprite icon;
    public string label;
    public int price;

    [Space]
    [Header("Skin")]
    public Color color = Color.white;
    public Sprite eyesBorder, eyesWhite, eyesClosed;
    public Vector2 size = new Vector2(0.22f, 0.22f);
}