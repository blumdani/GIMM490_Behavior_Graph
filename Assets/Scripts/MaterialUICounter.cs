using TMPro;
using UnityEngine;

public class MaterialUICounter : MonoBehaviour
{
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI rocksText;
    public TextMeshProUGUI grassText;
    public TextMeshProUGUI medicineText;

    void Update()
    {
        woodText.text = "Wood: " + Inventory.Instance.GetAmount(MaterialType.Tree).ToString();
        rocksText.text = "Rocks: " + Inventory.Instance.GetAmount(MaterialType.Rocks).ToString();
        grassText.text = "Grass: " + Inventory.Instance.GetAmount(MaterialType.Grass).ToString();
        medicineText.text = "Medicine: " + Inventory.Instance.GetAmount(MaterialType.Medicine).ToString();
    }
}