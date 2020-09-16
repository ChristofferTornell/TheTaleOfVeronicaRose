using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatModifierType
{
    flat = 100,
    percentAdditive = 200,
    percentMultiplicative = 300
}
public class StatModifier : MonoBehaviour
{
    public readonly float value;
    public readonly StatModifierType type;
    public readonly int order;
    public readonly object source;

    public StatModifier(float _value, StatModifierType _type, int _order, object _source)
    {
        value = _value;
        type = _type;
        order = _order;
    }

    //For all constructors: The input type will dictate what order it will have. (hence the (int)_type).

    //You can create a StatModifier without assigning order nor a source, it is optional. 
    public StatModifier(float _value, StatModifierType _type) : this(_value, _type, (int)_type, null) { }

    //You can create a StatModifier and assigning a set order. 
    public StatModifier(float _value, StatModifierType _type, int _order) : this(_value, _type, _order, null) { }

    //You can create a StatModifier and assigning a set source. 
    public StatModifier(float _value, StatModifierType _type, object source) : this(_value, _type, (int)_type, null) { } 

}
