using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    public GameObject loadingScreenPanel;
    public GameObject[] pawPrints;
    public TextMeshProUGUI percentageText;
    public TextMeshProUGUI loadingText;
    private Coroutine messageAnimationCoroutine;
    void Start()
    {
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(true);
        }
        UpdateProgress(0f);

    }

    void Update()
    {

    }
    private IEnumerator AnimateLoadingMessage()
    {
        const string baseText = "Loading";
        string[] dots = { "", ".", "..", "..." };
        int dotIndex = 0;
        while (true)
        {
            if (loadingText != null)
            {
                loadingText.text = baseText + dots[dotIndex];
            }
            dotIndex = (dotIndex + 1) % dots.Length;
            yield return new WaitForSeconds(0.4f);
        }
    }
    public void ShowLoadingScreen()
    {
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(true);
        }
        foreach (GameObject paw in pawPrints)
        {
            if (paw != null)
            {
                paw.SetActive(false);
            }
        }
        UpdateProgress(0f);
        if (loadingText != null)
        {
            if (messageAnimationCoroutine != null)
            {
                StopCoroutine(messageAnimationCoroutine);
            }
            messageAnimationCoroutine = StartCoroutine(AnimateLoadingMessage());
        }
    }
    public void UpdateProgress(float progress)
    {
        if (percentageText != null)
        {
            int percentage = Mathf.RoundToInt(progress * 100f);
            percentageText.text = percentage + "%";
        }
        int totalPaws = pawPrints.Length;
        int pawsToShow = Mathf.RoundToInt(progress * totalPaws);

        for (int i = 0; i < totalPaws; i++)
        {
            if (pawPrints[i] != null)
            {
                pawPrints[i].SetActive(i < pawsToShow);
            }
        }
        if(progress >= 1f)
        {
            if (messageAnimationCoroutine != null)
            {
                StopCoroutine(messageAnimationCoroutine);
            }
            if(loadingText != null)
            {
                loadingText.text = "Loading...";
            }
        }

    }
}
