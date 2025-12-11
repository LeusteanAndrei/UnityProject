using UnityEngine;
using UnityEngine.UI;

public class StaminaBarScript : MonoBehaviour
{

    [Header("Stamina bar settings")]
    [SerializeField] public Image staminaBar; // the bar level component
    [SerializeField] private float currentStamina = 100, maxStamina = 100;
    [SerializeField] private float appearTime = 5; // how many seconds it stays visible without changing
    [SerializeField] private float fadeSpeed = 1f; // how fast it changes visibility


    [Header("Recharge")]
    [SerializeField] private float rechargeDelay; // the delay until starting to recharge
    [SerializeField] private float rechargeAmount; // how fast it recharges


    private float lastDrainedTime; // last time the bar was drained
    private float lastChangeTime = 0; // the last time it changed ( going up or down )


    void Start()
    {
        lastChangeTime = 0;
        ChangeBarOpacity(1f); 
    }

    void Update()
    {
        TryRecharge(); // funciton for trying to recharge
        TryAppear(); // function checking if it should be visible or invisible
    }


    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(
            transform.position - Camera.main.transform.position
        );
    }

    private void TryRecharge()
    {
        lastDrainedTime  += Time.deltaTime; // increas drain time
        if (lastDrainedTime > rechargeDelay)
            // if bigger than delay, recharge
            RechargeStamina(rechargeAmount * Time.fixedDeltaTime);
            
    }

    void TryAppear()
    {
        lastChangeTime += Time.deltaTime; // increase change time
        if (lastChangeTime  > appearTime)
        {
            // if bigger than appear time, make it invisible
            ChangeBarOpacity(0f);
        }
        else
        {
            // else make it visible
            ChangeBarOpacity(1f);
        }

    }

    void ChangeBarOpacity(float opacity)
    {
        // makes bar invisible
        //for each child ( background and bar level component change it's opacity to the desired target

        foreach (Transform child in transform)
        {
            Image childImage = child.GetComponent<Image>();
            if (childImage != null)
            {
                Color color = childImage.color;
                color.a = Mathf.MoveTowards(color.a, opacity, fadeSpeed * Time.deltaTime); // change smoothly depending on the fade speed
                childImage.color = color;
            }
        }
    }


    public void ConsumeStamina(float amount)
    {
        // consumes stamina
        currentStamina -= amount ;
        if (currentStamina < 0)
        {

            // if it's zero, stop consuming
            currentStamina  = 0;
        }
        staminaBar.fillAmount = currentStamina / maxStamina; // fill amount = percentage to be filled


        // reset the  variables
        lastDrainedTime = 0;
        lastChangeTime = 0;
    }

    public void RechargeStamina(float amount)
    {
        // recharges
        currentStamina += amount ;
        if (currentStamina > maxStamina)
        {
            // if bigger than maxStamina then it's full, and we don't reset change time anymore
            currentStamina = maxStamina;
            staminaBar.fillAmount = 1;
            return;
        }
        staminaBar.fillAmount = currentStamina / maxStamina;

        // reset change time
        lastChangeTime = 0;
    }


    public float getStamina()
        { return currentStamina; }
}
