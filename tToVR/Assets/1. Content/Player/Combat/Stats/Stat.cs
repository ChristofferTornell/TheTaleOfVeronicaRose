using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

[Serializable]
public class Stat
{
    public float baseValue = 0f;
    protected float lastBaseValue = float.MinValue;
    protected bool isDirty = true;
    protected float Value;
    public virtual float value
    {
        get
        {
            if (isDirty || baseValue != lastBaseValue)
            {
                lastBaseValue = baseValue;
                Value = CalculateFinalValue();
                isDirty = false;
            }
            return Value;
        }
    }
    protected readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> statModifiersPublic;

    public Stat()
    {
        statModifiers = new List<StatModifier>();
        statModifiersPublic = statModifiers.AsReadOnly(); //Other classes are unable to edit the list, while we still are able to, as the reference of the private is read only, not the list istelf.
    }
    public Stat(float _baseValue) : this()
    {
        baseValue = _baseValue;
    }

    public virtual void AddModifier(StatModifier modifier)
    {
        isDirty = true;
        statModifiers.Add(modifier);
        statModifiers.Sort(CompareModifierOrder); //OPTIMIZE: Can be optimized via 'heap'.
    }

    public virtual bool RemoveModifier(StatModifier modifier)
    {
        if (statModifiers.Remove(modifier)) //If something got removed from the list, set isDirty.
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public virtual bool RemoveAllModifiersFromSource(object source)
    {

        bool didRemove = false;
        for (int i = statModifiers.Count -1; i >= 0; i--)
        {
            if (statModifiers[i].source == source)
            {
                isDirty = true;
                didRemove = true;

                statModifiers.RemoveAt(i); 
            }
        }
        return didRemove;
    }

    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.order < b.order)
            return -1;
        else if (a.order > b.order)
            return 1;
        return 0;
    }

    protected virtual float CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdditive = 0f;
        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier currentMod = statModifiers[i];

            switch (currentMod.type)
            {
                case StatModifierType.flat:
                    finalValue += statModifiers[i].value;
                    break;
                case StatModifierType.percentAdditive:
                    sumPercentAdditive += currentMod.value;

                    if (i+1 >= statModifiers.Count || statModifiers[i+1].type != StatModifierType.percentAdditive) //If the next statModifier in the array is not a percentAdditive, it's safe to calculate it all together. (the modifiers are sorted after their type, remember?)
                    {
                        finalValue *= 1 + sumPercentAdditive;
                    }
                    break;
                case StatModifierType.percentMultiplicative:
                    finalValue *= 1 + statModifiers[i].value;
                    break;
                default:
                    break;
            }
        }
        return (float)Math.Round(finalValue, 4);
    }
}
