using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundFxManager : MonoBehaviour
{

    public static SoundFxManager instance;


    [Header("Sound info")]
    [SerializeField] public float effectVolume;
    [SerializeField] public AudioSource soundFXObject;
    [SerializeField] public float meowMultiplier = 0.1f;
    [SerializeField] public float fireMultiplier = 1.0f;


    [Header("Sound clips")]
    [SerializeField] public AudioClip fallSound;
    [SerializeField] public List<AudioClip> meowSounds;
    [SerializeField] public AudioClip teleportSound;
    [SerializeField] public AudioClip fireSound;
    private AudioSource fireLoopSource;


    public AudioSource teleportLoopSource;


    private void Awake()
    {
        if(instance != null && instance !=this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Update()
    {

    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayMeowSound(Transform spawnTransform)
    {


        if (meowSounds == null || meowSounds.Count == 0)
        {
            Debug.LogWarning("No meow sounds assigned!");
            return;
        }

        AudioClip randomMeow = meowSounds[Random.Range(0, meowSounds.Count)];

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = randomMeow;
        audioSource.volume = effectVolume * meowMultiplier;
        Debug.Log(audioSource.volume);

        audioSource.Play();

        Destroy(audioSource.gameObject, randomMeow.length);
    }

    public void PlayFireSoundLoop(Transform spawnTransform)
    {
        if (fireLoopSource != null && fireLoopSource.isPlaying)
            return;


        fireLoopSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        fireLoopSource.clip = fireSound;
        fireLoopSource.loop = true;
        fireLoopSource.playOnAwake = false;
        fireLoopSource.volume = effectVolume * fireMultiplier;

        fireLoopSource.Play();
    }

    public void PlayTeleportSound(Transform spawnTransform, float length)
    {
        StopTeleportSound();

        teleportLoopSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        teleportLoopSource.clip = teleportSound;
        teleportLoopSource.volume = effectVolume;
        teleportLoopSource.loop = false;

        if (teleportSound.length > 0f)
            teleportLoopSource.pitch = teleportSound.length / length;
        else
            teleportLoopSource.pitch = 1f;

        teleportLoopSource.Play();

        Destroy(teleportLoopSource.gameObject, length);
    }

    public void StopTeleportSound()
    {
        if (teleportLoopSource == null) return;

        teleportLoopSource.Stop();
        Destroy(teleportLoopSource.gameObject);
        teleportLoopSource = null;
    }

}
