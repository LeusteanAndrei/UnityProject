using UnityEngine;

public class SoundFxManager : MonoBehaviour
{

    public static SoundFxManager instance;


    [Header("Sound info")]
    [SerializeField] public float effectVolume;
    [SerializeField] public AudioSource soundFXObject;

    [Header("Sound clips")]
    [SerializeField]public AudioClip fallSound;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }


    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        // spawn game object
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        // assign clip
        audioSource.clip = audioClip;
        // assign volume
        audioSource.volume = volume;
        // play sound
        audioSource.Play();

        // get length
        float clipLength = audioSource.clip.length;

        // destroy game object after a amount of time
        Destroy(audioSource.gameObject, clipLength);
    }

}
