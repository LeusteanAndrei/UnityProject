using UnityEngine;

public class Collisions : MonoBehaviour
{
    public float currentSpeed;
    [SerializeField] private float damageThreshold;
    [SerializeField] private float soundMultiplier = 1f;
    public SoundMeterManage soundMeter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundMeter = GameObject.Find("GameController").GetComponent<SoundMeterManage>();
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
            //Debug.Log(loudness);
            if (loudness > damageThreshold)
            {
                //break object
                Debug.Log("Object broken");
            }
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
                    Debug.Log("Enemy alerted by sound!");
                }
            }
            
        }
    }
}
