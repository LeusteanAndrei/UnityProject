using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class SoundMeterManage : MonoBehaviour
{
    public float currentSoundLevel;
    [SerializeField] private bool enemyToSound;
    public float maxSoundLevel;
    [SerializeField] private float reduceRate;
    [SerializeField] private float cooldownTime;
    private float timeSinceLastIncrease;
    private GameManager gameManager;
    public Slider SoundMeterUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //currentSoundLevel = 0f;
        //maxSoundLevel = 20f;
        gameManager = GetComponent<GameManager>();
        GameObject soundMeterObject = GameObject.Find("SoundMeter");
        if (soundMeterObject != null)
        {
            SoundMeterUI = soundMeterObject.GetComponent<Slider>();
            if (SoundMeterUI != null)
            {
                SoundMeterUI.maxValue = maxSoundLevel;
                SoundMeterUI.value = currentSoundLevel;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        timeSinceLastIncrease += Time.deltaTime;
        if(timeSinceLastIncrease >= cooldownTime && currentSoundLevel > 0f) 
        {
            currentSoundLevel -= reduceRate * Time.deltaTime;
            if (currentSoundLevel < 0f)
            {
                currentSoundLevel = 0f;
            }
        }
        // Update the UI
        if (SoundMeterUI != null)
        {
            SoundMeterUI.value = currentSoundLevel;
        }
    }
    public void IncreaseSoundLevel(float amount)
    {
        currentSoundLevel += amount;
        if (currentSoundLevel > maxSoundLevel)
        {
            currentSoundLevel = maxSoundLevel;
            gameManager.GameOver();
        }
        timeSinceLastIncrease = 0f;
    }
    public void SetCurrentSoundLevel(float level)
    {
        this.currentSoundLevel = level;
    }
    public float GetCurrentSoundLevel()
    {
        return currentSoundLevel;
    }

    public bool GetEnemyToSound() { return enemyToSound; }
}
