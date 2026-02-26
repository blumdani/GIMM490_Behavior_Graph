using UnityEngine;
using Unity.Behavior;

public class BlackboardVariableSync : MonoBehaviour
{
    public BehaviorGraphAgent behaviorAgent;
    public HealthController healthController;
    public BlackboardVariable<MaterialType> LowestMat;

    /* // Cached previous values (so we only update when changed)
     private float lastPlayerHealth;
     private float lastCompanionHealth;

     void Start()
     {
         // Force initial sync
         lastPlayerHealth = 70f;
         lastCompanionHealth = 70f;
     }*/

    void Update()
    {
        Debug.Log("Blackboard Sync Running");
        SyncHealth();
        SyncInventory();
    }

    // ---------------- HEALTH ----------------

    void SyncHealth()
    {
        behaviorAgent.SetVariableValue("PlayerHealth", healthController.playerHealth);
        behaviorAgent.SetVariableValue("CompanionHealth", healthController.companionHealth);
    }

    // ---------------- INVENTORY ----------------

    void SyncInventory()
    {
        int wood = Inventory.Instance.GetAmount(MaterialType.Tree);
        int rock = Inventory.Instance.GetAmount(MaterialType.Rocks);
        int grass = Inventory.Instance.GetAmount(MaterialType.Grass);
        int medicine = Inventory.Instance.GetAmount(MaterialType.Medicine);

        Debug.Log($"W:{wood} R:{rock} G:{grass} M:{medicine}");

        MaterialType lowest = GetLowestMaterial(wood, rock, grass, medicine);
        LowestMat.Value = lowest;
        behaviorAgent.SetVariableValue("MedicineCount", medicine);
    }

    MaterialType GetLowestMaterial(int wood, int rock, int grass, int medicine)
    {
        int min = medicine;
        MaterialType lowest = MaterialType.Medicine;

        if (rock < min)
        {
            min = rock;
            lowest = MaterialType.Rocks;
        }

        if (grass < min)
        {
            min = grass;
            lowest = MaterialType.Grass;
        }

        if (wood < min)
        {
            lowest = MaterialType.Tree;
        }

        return lowest;
    }

}
