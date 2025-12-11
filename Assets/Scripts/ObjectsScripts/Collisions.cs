using UnityEngine;
using System.Collections.Generic;
public class Collisions : MonoBehaviour
{
    [Header("Sound settings")]
    [SerializeField] private float damageThreshold;
    [SerializeField] private float currentDamage;
    [SerializeField] private float soundMultiplier = 1f;
    [SerializeField] private float stunMin = 0.5f;
    [SerializeField] private float stunMax = 3f;
    [SerializeField] private float stunScale = 20f; // speed*multiplier value that yields max stun
    [SerializeField] private bool destroyed = false;
    
    [HideInInspector] public SoundMeterManage soundMeter;
    [HideInInspector] public float currentSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundMeter = GameObject.Find("GameManager").GetComponent<SoundMeterManage>();
    }
    // Update is called once per frame
    void Update()
    {
        currentSpeed = GetComponent<Rigidbody>().linearVelocity.magnitude;
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Surface>() !=null)
        {
            Surface surface = other.gameObject.GetComponent<Surface>();
            float loudness = currentSpeed * surface.GetHardness() * soundMultiplier;
            TakeDamage(loudness);
            int audioIncrease = Mathf.CeilToInt(loudness / 10f);
            soundMeter.IncreaseSoundLevel(audioIncrease);
            Vector3 origin = other.contactCount > 0 ? other.GetContact(0).point : transform.position;
            float radius = Mathf.Clamp(loudness, 1f, 30f) / 2f;
            var hits = Physics.OverlapSphere(origin, radius, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits.Length; i++)
            {
                var h = hits[i];
                if (h == null) continue;
                if (h.CompareTag("Enemy"))
                {
                    Debug.Log($"Enemy at {h.transform.position} distracted by sound");
                    h.GetComponent<EnemyMovement>().GetDistracted(gameObject);
                }
            }
            if(other.gameObject.GetComponent<Collisions>() != null)
            {
                Collisions otherCollision = other.gameObject.GetComponent<Collisions>();
                float combinedSpeed = currentSpeed + otherCollision.currentSpeed;
                float combinedLoudness = combinedSpeed * surface.GetHardness() * otherCollision.soundMultiplier;
                otherCollision.TakeDamage(combinedLoudness);
            }
            
        }
        if(other.gameObject.GetComponent<EnemyMovement>() != null)
        {
            if(gameObject.transform.position.y>other.gameObject.transform.position.y + 1f && !gameObject.CompareTag("Player"))
            {
            var enemy = other.gameObject.GetComponent<EnemyMovement>();
            float intensity = currentSpeed * soundMultiplier;
            float t = Mathf.Clamp01(intensity / Mathf.Max(0.0001f, stunScale));
            float stunDuration = Mathf.Lerp(stunMin, stunMax, t);
            enemy.GetStunned(stunDuration);
            }
        }
    }
    public void TakeDamage(float damage)
    {
        currentDamage += damage;
        if(currentDamage >= damageThreshold)
        {
            destroyed = true;
            Debug.Log("Object destroyed due to damage");
        }
    }
    public bool IsDestroyed()
    {
        return destroyed;
    }
}
