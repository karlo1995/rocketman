using UnityEngine;
using System;
using System.Collections;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class SingletonPrefabAttribute : Attribute
{
    private string _name;
    public string Name { get { return this._name; } }

    public SingletonPrefabAttribute() { this._name = string.Empty; }
    public SingletonPrefabAttribute(string name) { this._name = name; }
}