using UnityEngine;

public class ArrowPulse : MonoBehaviour
{


    public Transform arrowScaler;

    public float minScale;
    public float maxScale;

    public float maxPulseSpeed = 1.5f;
    public float minPulseSpeed = 0.3f;
    public float maxDistance = 30f;

    private ArrowScript arrowScript;

    private Vector3 minScaleVector;
    private Vector3 maxScaleVector;


    float pulseTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arrowScript = GetComponent<ArrowScript>();
        minScaleVector = arrowScaler.localScale * minScale;
        maxScaleVector = arrowScaler.localScale * maxScale;

    }



    // Update is called once per frame
    void LateUpdate()
    {
        if (!arrowScript || !arrowScript) return;

        float distance = Vector3.Distance(transform.position, arrowScript.objectToLookTowards.transform.position);

        float pulseSpeed = Mathf.Lerp( maxPulseSpeed, minPulseSpeed, distance/maxDistance);

        pulseSpeed = Mathf.Clamp( pulseSpeed, minPulseSpeed, maxPulseSpeed);

        pulseTimer += Time.deltaTime * pulseSpeed;
        float t = (Mathf.Sin(pulseTimer * Mathf.PI * 2) + 1) / 2;

        transform.localScale = Vector3.Lerp(minScaleVector, maxScaleVector, t);
    }
}
