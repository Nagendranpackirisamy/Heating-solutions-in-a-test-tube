using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlideTimerController : MonoBehaviour
{
    [Header("Slide")]
    public SlideCameraController slideController;
    public int targetPageNumber;

    [Header("Timer Settings")]
    public float duration = 45f;        
    public float speedMultiplier = 1f;

    [Header("UI")]
    public Image progressImage;
    public TMP_Text timerText;

    private float currentTime = 0f;
    private bool isRunning = false;

    private static bool timerCompleted = false;

    void OnEnable()
    {
        if (slideController.CurrentPageNumber != targetPageNumber)
            return;

        if (timerCompleted)
        {
            ShowCompletedState();
        }
        else
        {
            StartTimer();
        }
    }

    private bool hasStarted = false;

    void Update()
    {
        if (timerCompleted) return;

        if (!hasStarted &&
            slideController.CurrentPageNumber == targetPageNumber)
        {
            StartTimer();
            hasStarted = true;
        }

        if (!isRunning) return;

        currentTime += Time.deltaTime * speedMultiplier;

        float normalized = Mathf.Clamp01(currentTime / duration);
        progressImage.fillAmount = normalized;

        UpdateTimerText(currentTime);

        if (currentTime >= duration)
        {
            CompleteTimer();
        }
    }


    void StartTimer()
    {
        currentTime = 0f;
        isRunning = true;

        slideController.nextButton.interactable = false;
        slideController.previousButton.interactable = false;

        progressImage.fillAmount = 0f;
        UpdateTimerText(0f);
    }

    void CompleteTimer()
    {
        isRunning = false;
        timerCompleted = true;

        progressImage.fillAmount = 1f;
        UpdateTimerText(duration);

        slideController.EnableNextButton();
        slideController.previousButton.interactable = true;
    }

    void UpdateTimerText(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int milliseconds = Mathf.FloorToInt((time - seconds) * 100f);

        timerText.text = $"{seconds:00}:{milliseconds:00}";
    }


    void ShowCompletedState()
    {
        progressImage.fillAmount = 1f;
        UpdateTimerText(duration);

        slideController.EnableNextButton();
        slideController.previousButton.interactable = true;
    }
}
