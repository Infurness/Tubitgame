using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/EnergyItem", order = 1)]
public class ScriptableEnergyItem : ScriptableObject
{
    public string IDLable;
    public Sprite ObjectIcon;
    public float energyRecover;
}
