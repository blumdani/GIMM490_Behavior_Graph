using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public float playerHealth;
    public float companionHealth;
    public Image playerFill;
    public Image companionFill;
    private float maxHealth = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        //Update Healthbars in UI
        playerFill.fillAmount = (playerHealth / maxHealth);
        companionFill.fillAmount = (companionHealth / maxHealth);
    }
  
    public void PlayerTakeDamage()
    {
        playerHealth -= 10f;
    }

    public void CompanionTakeDamage()
    {
        companionHealth -= 10f;
    }

    public void PlayerHeal()
    {
        playerHealth += 10f;
    }

    public void CompanionHeal()
    {
        companionHealth += 10f;
    }
}
