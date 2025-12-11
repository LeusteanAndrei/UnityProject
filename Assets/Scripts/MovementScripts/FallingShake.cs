using System.Runtime.CompilerServices;
using UnityEngine;

public class FallingShake : MonoBehaviour
{

    [Header("Shake settings")]
    [SerializeField] private float shakeDuration = 0.1f; // how long to shake
    [SerializeField] private float shakeFrequency = 0.3f; // how fast and often
    [SerializeField] private float shakeMultiplier = 0.2f; // a dampening factor to not let the shake grow a lot
    [SerializeField] private float minimumImpactValue = 0.5f; // the value of the minimum impact for the shake to take effect


    private bool falling = false; // checks wether the object is falling
    Rigidbody rb;


    private float previousVelocity;
    private float currentVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        currentVelocity = rb.linearVelocity.y; // set the new velocity

        falling = rb.linearVelocity.y < 0; // set the falling variable

        previousVelocity = rb.linearVelocity.y; // set the previous velocity
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if we collisioned with something, check wether we should shake

        if (!falling) return;  // if we didn't fall no shake

        float impact = collision.impulse.magnitude; // get the magnitude of the impulse

        if (impact < minimumImpactValue) return;
        float shakeStrength = impact * shakeMultiplier; // and shake scaling off of the impact ( bigger mass/impact => more shake )
        // multiply by the dampening factor "shakeMultiplier" so it doesn't grow out of control

        CameraShake.instance.Shake(shakeStrength, shakeFrequency, shakeDuration);
    }





}
