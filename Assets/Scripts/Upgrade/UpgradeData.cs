using UnityEngine;

public enum UpgradeType
{
    Algorithm,
    Space,
    Speed,
}

[CreateAssetMenu(menuName = "SSR/OverWeight/Upgrade", fileName = "Upgrade")]
public class UpgradeData : ScriptableObject
{
    [Header("Basic")] 
    public UpgradeType Type;
    
    public string Name;
    [TextArea]
    public string Description;

    public Sprite Icon;

    public int Cost;

    public int Level;

    [Header("Value")] 
    public float AdditionalWeight;
    
    public float AdditionalCapacity;

    public float AdditionalMaxSpeed;

    public float AdditionalSaveCost;
}