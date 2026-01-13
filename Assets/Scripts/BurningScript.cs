using UnityEngine;

public class BurningScript : MonoBehaviour
{

    public bool shouldBurn = false;
    public float burningIntensity = 0;
    public ParticleSystem fireParticlesPrefab;
    public float burnIncreaseRate = 1f;
    public float maxIntensity = 10f;
    public float spreadThreshold = 5f;

    public bool isBurning;
    ParticleSystem fireInstance;
    float timer = 0;
    

    Vector3 initialScale = new Vector3(1f, 1f, 1f);
    Vector3 targetScale = new Vector3(1.5f, 1.5f, 3f);

    void Start()
    {
    }

    void Update()
    {
        if (shouldBurn)
        {
            if (timer >= 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                Burn();
            }
        }
    }

    void Ignite()
    {
        if (fireInstance != null) return;

        fireInstance = Instantiate(
            fireParticlesPrefab,
            transform
        );

        fireInstance.transform.localPosition = Vector3.zero;
        fireInstance.transform.localRotation = Quaternion.identity;
        fireInstance.transform.localScale = initialScale;

        fireInstance.Play();
        isBurning = true;
    }

    private void Burn()
    {
        if (!isBurning) Ignite();

        float t = burningIntensity / spreadThreshold; 
        fireInstance.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

        burningIntensity += burnIncreaseRate * Time.deltaTime;

        burningIntensity = Mathf.Min(burningIntensity, maxIntensity);

        if (fireInstance != null)
        {
            var emission = fireInstance.emission;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(burningIntensity * 10f);
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.GetComponent<Chandelier>() != null)
        {
            shouldBurn = true;
            timer = .4f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject go = other.gameObject;
        if (go.GetComponent<BurningScript>() != null)
        {
            BurningScript fire = go.GetComponent<BurningScript>();

            if (fire.burningIntensity >= spreadThreshold)
                shouldBurn = true;
        }
    }
}
