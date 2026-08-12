using UnityEngine;

// The Strategy contract. UpgradeSelectionUI only ever talks to this base
// type - it doesn't know or care whether a given upgrade is a simple stat
// bump or something with unique custom logic. That's what makes new
// upgrades pure content (new asset) rather than code changes (OCP).
public abstract class UpgradeDefinition : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField] [TextArea] private string description;
    [SerializeField] private Sprite icon;

    public string Title => title;
    public string Description => description;
    public Sprite Icon => icon;

    // Every upgrade applies itself - the caller never needs a switch
    // statement or type-check to know what to do with one.
    public abstract void Apply();
}
