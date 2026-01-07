using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Playables;
public class SnowfallTrigger : MonoBehaviour
{
    [SerializeField] private WetMeterManager wetMeterManager;
    [SerializeField] private bool startSnowfall = false;
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private List<ParticleSystem> snowfallParticleSystems;
    [SerializeField] private List<GoalObject> objectsToDestroy;
    [SerializeField] private PlayableDirector playableDirector;
    private bool timelineActive = false;
    private float previousTimeScale = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wetMeterManager = FindObjectOfType<WetMeterManager>();
        snowfallParticleSystems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
    }

    private void OnEnable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped += OnDirectorStopped;
        }
    }

    private void OnDisable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnDirectorStopped;
        }
    }

    private void OnDirectorStopped(PlayableDirector d)
    {
        if (timelineActive)
        {
            Time.timeScale = previousTimeScale;
            d.timeUpdateMode = DirectorUpdateMode.GameTime;
            timelineActive = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        wetMeterManager.harshWeather = startSnowfall;
        bool allDestroyed = true;
        foreach (GoalObject col in objectsToDestroy)
        {
            if (!col.IsMarked())
            {
                allDestroyed = false;
                break;
            }
        }
        if (allDestroyed && !startSnowfall)
        {
            startSnowfall = true;
            if (playableDirector != null && playableDirector.state != PlayState.Playing && !timelineActive)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                playableDirector.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
                timelineActive = true;
                playableDirector.Play();
            }
            else if (playableDirector == null)
            {
                
            }
        }
        if(startSnowfall)
        {
            foreach (ParticleSystem ps in snowfallParticleSystems)
            {
                if (!ps.isPlaying)
                {
                    ps.Play();
                }
            }
        }
        if (startSnowfall)
        {
            if (destinationPoint != null)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    destinationPoint.position,
                    lerpSpeed * Time.deltaTime
                );
            }
        }
    }
}
