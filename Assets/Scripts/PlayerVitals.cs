using UnityEngine;
using UnityEngine.UI;

public class PlayerVitals : MonoBehaviour
{
    public Slider healthSlider;
    public int maxHealth;
    public int healthFallRate;


    public Slider thirstSlider;
    public int maxThirst;
    public int thirstFallRate;

    public Slider staminaSlider;
    public int maxStamina;
    private int staminaFallRate;
    public int staminaFallMultiplier;
    private int staminaRegainRate;
    public int staminaRegainMultiplier;

    private CharacterController characterController;
    private AdventurerMoveController playerController;

    void Start()
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        thirstSlider.maxValue = maxThirst;
        thirstSlider.value = maxThirst;

        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;

        staminaFallRate = 1;
        staminaRegainRate = 1;

        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<AdventurerMoveController>();
    }

    void Update()
    {
        #region Health
        
        if (thirstSlider.value <= 0)
        {
            healthSlider.value -= Time.deltaTime / healthFallRate * 2;
        }        

        if (healthSlider.value <= 0)
        {
            CharacterDeath();
        }

        #endregion

        #region Health

        if (thirstSlider.value >= 0)
        {
            thirstSlider.value -= Time.deltaTime / thirstFallRate;
        }
        else if (thirstSlider.value <= 0)
        {
            thirstSlider.value = 0;
        }
        else if (thirstSlider.value >= maxThirst)
        {
            thirstSlider.value = maxThirst;
        }

        #endregion

        #region Stamina

        if (Input.GetKey(KeyCode.LeftShift)) // run
        {
            staminaSlider.value -= Time.deltaTime / staminaFallRate * staminaFallMultiplier;
        }
        else if (playerController.Speed == 1f) // walk
        {
            staminaSlider.value -= Time.deltaTime / staminaFallRate * (staminaFallMultiplier / 2);
        }
        else
        {
            staminaSlider.value += Time.deltaTime / staminaRegainRate * staminaRegainMultiplier;
        }

        if (staminaSlider.value >= maxStamina)
        {
            staminaSlider.value = maxStamina;
        }
        else if (staminaSlider.value <= 0)
        {
            staminaSlider.value = 0;
            playerController.SetSlow(true);
        }
        else if (staminaSlider.value >= 0)
        {
            playerController.SetSlow(false);
        }

        #endregion
    }

    void CharacterDeath()
    {
        // TODO
    }
}
