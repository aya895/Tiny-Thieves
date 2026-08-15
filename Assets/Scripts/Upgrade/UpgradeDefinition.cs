using UnityEngine;

public abstract class UpgradeDefinition : ScriptableObject
{
    [Header("Display")]
    [SerializeField] private string title;

    [SerializeField, TextArea]
    private string description;

    [SerializeField] private Sprite icon;

    public string Title => title;
    public string Description => description;
    public Sprite Icon => icon;

    public abstract void Apply(UpgradeContext context);
}