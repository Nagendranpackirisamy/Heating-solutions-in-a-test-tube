using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class SlideCameraController : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public Transform cameraPoint;
        public GameObject slideUI;

        public UnityEvent onNextClicked;
        public UnityEvent onBackClicked;
        public UnityEvent onRepeatedNextClicked;

        // ===== SPECIAL UI =====
        [Header("Special UI Controls")]
        public bool enableNextOnStart = false;

        public bool showNumpad = false;
        public GameObject numpadObject;

        public bool showCalculator = false;
        public GameObject calculatorObject;
    }

    [Header("Camera")]
    public Transform cameraTransform;
    public float moveSpeed = 2f;

    [Header("Steps")]
    public List<Step> steps;

    public Button nextButton;
    public Button previousButton;

    [Header("Slide Counter UI")]
    public TextMeshProUGUI slideCounterText;

    [Header("Slide Counter Values")]
    public int currentSlide = 0;
    public int totalSlides = 0;

    [Header("Debug / Cheat")]
    public bool enableCheatKey = true;

    int currentIndex = 0;
    bool isMoving = false;

    // ===== STATE SYSTEM =====
    bool[] slideCompleted;
    bool[] slideLocked;
    bool[] slideVisited;
    bool[] eventUsed;
    // ========================

    void Start()
    {
        slideCompleted = new bool[steps.Count];
        slideLocked = new bool[steps.Count];
        slideVisited = new bool[steps.Count];
        eventUsed = new bool[steps.Count];

        currentSlide = 0;
        UpdateSlideCounterUI();

        foreach (var s in steps)
            s.slideUI.SetActive(false);

        // ----- FIRST SLIDE -----
        steps[0].slideUI.SetActive(true);

        cameraTransform.position = steps[0].cameraPoint.position;
        cameraTransform.rotation = steps[0].cameraPoint.rotation;

        nextButton.interactable = false;
        UpdateBackButton();

        // ⭐ Apply for first slide
        ApplyStepSettings(0);
        nextButton.interactable = slideCompleted[0];
    }

    void Update()
    {
        if (enableCheatKey && Input.GetKeyDown(KeyCode.N))
        {
            ForceNextSlide_Cheat();
        }

#if ENABLE_INPUT_SYSTEM
        if (enableCheatKey &&
            UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.nKey.wasPressedThisFrame)
        {
            ForceNextSlide_Cheat();
        }
#endif
    }

    void ForceNextSlide_Cheat()
    {
        if (currentIndex >= steps.Count - 1)
            return;

        StopAllCoroutines();
        isMoving = false;

        slideCompleted[currentIndex] = true;
        slideLocked[currentIndex] = true;

        nextButton.interactable = true;

        StartCoroutine(MoveTo(currentIndex + 1));
    }

    // ===== OVERRIDDEN =====
    public void EnableNextButton()
    {
        slideCompleted[currentIndex] = true;
        slideLocked[currentIndex] = true;   // lock permanently
        nextButton.interactable = true;
    }

    public void Next()
    {
        if (isMoving) return;
        if (!slideCompleted[currentIndex]) return;
        if (currentIndex >= steps.Count - 1) return;

        if (!eventUsed[currentIndex])
        {
            steps[currentIndex].onNextClicked?.Invoke();
            eventUsed[currentIndex] = true;
        }

        steps[currentIndex].onRepeatedNextClicked?.Invoke();

        StartCoroutine(MoveTo(currentIndex + 1));
    }

    public void Previous()
    {
        if (currentIndex <= 0) return;

        StopAllCoroutines();
        isMoving = false;

        steps[currentIndex].onBackClicked?.Invoke();

        StartCoroutine(MoveTo(currentIndex - 1));
    }

    IEnumerator MoveTo(int targetIndex)
    {
        isMoving = true;

        foreach (var s in steps)
            s.slideUI.SetActive(false);

        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        Vector3 endPos = steps[targetIndex].cameraPoint.position;
        Quaternion endRot = steps[targetIndex].cameraPoint.rotation;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            cameraTransform.position = Vector3.Lerp(startPos, endPos, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        currentIndex = targetIndex;
        currentSlide = currentIndex;

        UpdateSlideCounterUI();

        steps[currentIndex].slideUI.SetActive(true);

        // ⭐ Apply state logic
        ApplyStepSettings(currentIndex);

        nextButton.interactable = slideCompleted[currentIndex];
        UpdateBackButton();

        isMoving = false;
    }

    // ========== CORE STATE LOGIC ==========
    void ApplyStepSettings(int index)
    {
        Step s = steps[index];

        // ----- Already finished slide -----
        if (slideLocked[index])
        {
            slideCompleted[index] = true;
            nextButton.interactable = true;

            if (s.numpadObject != null)
                s.numpadObject.SetActive(false);

            if (s.calculatorObject != null)
                s.calculatorObject.SetActive(false);

            return;
        }

        // ----- First time visit -----
        slideVisited[index] = true;

        if (s.enableNextOnStart)
        {
            slideCompleted[index] = true;
        }

        if (s.numpadObject != null)
            s.numpadObject.SetActive(s.showNumpad);

        if (s.calculatorObject != null)
            s.calculatorObject.SetActive(s.showCalculator);
    }
    // =====================================

    public void EnableNextSlideNextButton()
    {
        int nextIndex = currentIndex + 1;
        if (nextIndex < steps.Count)
        {
            slideCompleted[nextIndex] = true;
        }
    }

    void UpdateBackButton()
    {
        previousButton.interactable = currentIndex > 0;
    }

    void UpdateSlideCounterUI()
    {
        if (slideCounterText != null)
        {
            slideCounterText.text = (currentSlide + 1) + " / " + totalSlides;
        }
    }
}
