using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // script for singleton instance which shakes the camera

    public static CameraShake instance;

    public CinemachineCamera cinemachineCamera;
    CinemachineBasicMultiChannelPerlin noise;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>(); //  get the perlin noise component ( this does the shaking )
    }

    public void Shake(float amplitude, float frequency, float duration)
    {
        StartCoroutine(DoShake(amplitude, frequency, duration)); // whenever this method is called a coroutine starts
    }

    IEnumerator DoShake(float amplitude, float frequency, float duration)
    {

        float passedTime = 0f; // how much time has passed
        while (passedTime < duration)
        {
            passedTime += Time.deltaTime; // increment it each "shake"
            noise.AmplitudeGain = amplitude; // set the amplitude ( intensity ) of the shake
            noise.FrequencyGain = frequency; // and the frequency ( how fast or slow )

            yield return null;
        }

        // reset to 0 after finish
        noise.AmplitudeGain = 0; 
        noise.FrequencyGain = 0;
    }
}
