using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private Dictionary<MaterialType, int> Materials =
        new Dictionary<MaterialType, int>();

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Initialize Materials
        foreach (MaterialType type in System.Enum.GetValues(typeof(MaterialType)))
        {
            Materials[type] = 0;
        }
        Materials[MaterialType.Medicine] = 7;
    }

    public void AddMaterial(MaterialType type)
    {
        Materials[type]++;
        Debug.Log("Added: " + Materials[type]);
    }

    public void RemoveMaterial(MaterialType type)
    {
        Materials[type]--;
        Debug.Log("Removed: " + Materials[type]);
    }

    public int GetAmount(MaterialType type)
    {
        return Materials[type];
    }
}

