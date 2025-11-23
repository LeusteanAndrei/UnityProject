using UnityEngine;
using UnityEngine.UI;

public class StaminaBarScript : MonoBehaviour
{

    [Header("Stamina bar settings")]
    [SerializeField] public Image staminaBar;
    [SerializeField] private float currentStamina = 100, maxStamina = 100;
    [SerializeField] private float appearTime = 5;
    [SerializeField] private float fadeSpeed = 1f;


    [Header("Recharge")]
    [SerializeField] private float rechargeDelay;
    [SerializeField] private float rechargeAmount;

    [Header("Position")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private float lastDrainedTime;
    private float lastChangeTime = 0;


    void Start()
    {
        lastChangeTime = 0;
        ChangeBarOpacity(1f); 
    }

    void Update()
    {
        TryRecharge();
        TryAppear();
    }


    private void FixedUpdate()
    {
        MovetToTarget();
    }

    void MovetToTarget()
    {
        if (target != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position);
            transform.position = screenPosition + offset;
        }
    }

    private void TryRecharge()
    {
        lastDrainedTime  += Time.deltaTime;
        if (lastDrainedTime > rechargeDelay)
            RechargeStamina(rechargeAmount * Time.fixedDeltaTime);
            
    }

    void TryAppear()
    {
        lastChangeTime += Time.deltaTime;
        if (lastChangeTime  > appearTime)
        {
            ChangeBarOpacity(0f);
        }
        else
        {
            ChangeBarOpacity(1f);
        }

    }

    void ChangeBarOpacity(float opacity)
    {
        foreach (Transform child in transform)
        {
            Image childImage = child.GetComponent<Image>();
            if (childImage != null)
            {
                Color color = childImage.color;
                color.a = Mathf.MoveTowards(color.a, opacity, fadeSpeed * Time.deltaTime);
                childImage.color = color;
            }
        }
    }


    public void ConsumeStamina(float amount)
    {
        currentStamina -= amount ;
        if (currentStamina < 0)
        {
            currentStamina  = 0;
        }
        staminaBar.fillAmount = currentStamina / maxStamina;

        lastDrainedTime = 0;
        lastChangeTime = 0;
    }

    public void RechargeStamina(float amount)
    {
        currentStamina += amount ;
        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
            staminaBar.fillAmount = 1;
            return;
        }
        staminaBar.fillAmount = currentStamina / maxStamina;
        lastChangeTime = 0;
    }


    public float getStamina()
        { return currentStamina; }
}
