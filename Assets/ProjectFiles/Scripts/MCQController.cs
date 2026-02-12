using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class MCQController : MonoBehaviour
{
    public class LocalState
    {
        public bool answered = false;
        public int correctOption = -1;
        public List<int> wrongOptions = new List<int>();
    }

    private static Dictionary<int, LocalState> slideMemory
        = new Dictionary<int, LocalState>();

    [Header("Slide Binding")]
    public int slidePageNumber;

    [Header("Panels")]
    public GameObject mcqPanel;
    public GameObject explanationPanel;

    [Header("Question UI")]
    public TMP_Text questionText;
    public Image referenceImage;

    [Header("Options")]
    public Button[] optionButtons;
    public TMP_Text[] optionTexts;

    [Header("Explanation UI")]
    public TMP_Text explanationText;
    public Button explanationActionButton;

    [Header("Explanation Entry Buttons")]
    public Button rightExplanationButton;
    public Button wrongExplanationButton;

    [Header("Sprites")]
    public Sprite defaultButtonSprite;
    public Sprite correctSprite;
    public Sprite wrongSprite;

    [Header("Data")]
    public MCQQuestionData questionData;

    [Header("State Controller")]
    public SlideStateController stateController;

    [Header("Events")]
    public UnityEvent onCorrectOptionSelected;
    public UnityEvent onWrongOptionSelected;

    void OnEnable()
    {
        stateController.OnSlideChanged.AddListener(OnSlideChanged);
        BindButtons();
    }

    void OnDisable()
    {
        stateController.OnSlideChanged.RemoveListener(OnSlideChanged);
    }

    void OnSlideChanged(int index)
    {
        if (index != slidePageNumber) return;

        LoadQuestion();
        RestoreState();
    }

    LocalState GetState()
    {
        if (!slideMemory.ContainsKey(slidePageNumber))
            slideMemory[slidePageNumber] = new LocalState();

        return slideMemory[slidePageNumber];
    }

    void LoadQuestion()
    {
        questionText.text = questionData.questionText;
        explanationText.text = questionData.explanationText;

        if (referenceImage != null)
        {
            if (questionData.referenceImage != null)
            {
                referenceImage.gameObject.SetActive(true);
                referenceImage.sprite = questionData.referenceImage;
                referenceImage.color = Color.white;
            }
            else
            {
                referenceImage.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;

            optionTexts[i].text = questionData.options[i];

            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));

            optionButtons[i].interactable = true;
            optionButtons[i].GetComponent<Image>().sprite = defaultButtonSprite;
        }

        mcqPanel.SetActive(true);
        explanationPanel.SetActive(false);
        HideExplanationButtons();
    }

    void RestoreState()
    {
        var state = GetState();

        foreach (int wrongIndex in state.wrongOptions)
        {
            optionButtons[wrongIndex].GetComponent<Image>().sprite = wrongSprite;
            optionButtons[wrongIndex].interactable = false;
        }

        if (state.correctOption != -1)
        {
            optionButtons[state.correctOption].GetComponent<Image>().sprite = correctSprite;
            DisableAllOptions();
            ShowRightExplanation();
            stateController.CompleteCurrentSlide();
        }
    }

    void OnOptionSelected(int index)
    {
        var state = GetState();

        bool isCorrect = index == questionData.correctOptionIndex;

        if (isCorrect)
        {
            state.correctOption = index;
            state.answered = true;

            optionButtons[index].GetComponent<Image>().sprite = correctSprite;
            DisableAllOptions();
            ShowRightExplanation();

            onCorrectOptionSelected?.Invoke();

            stateController.CompleteCurrentSlide();
        }
        else
        {
            if (!state.wrongOptions.Contains(index))
                state.wrongOptions.Add(index);

            optionButtons[index].GetComponent<Image>().sprite = wrongSprite;
            optionButtons[index].interactable = false;

            ShowWrongExplanation();

            onWrongOptionSelected?.Invoke();
        }
    }

    void BindButtons()
    {
        rightExplanationButton.onClick.RemoveAllListeners();
        wrongExplanationButton.onClick.RemoveAllListeners();
        explanationActionButton.onClick.RemoveAllListeners();

        rightExplanationButton.onClick.AddListener(OpenExplanation);
        wrongExplanationButton.onClick.AddListener(OpenExplanation);
        explanationActionButton.onClick.AddListener(CloseExplanation);
    }

    void OpenExplanation()
    {
        mcqPanel.SetActive(false);
        explanationPanel.SetActive(true);
    }

    void CloseExplanation()
    {
        explanationPanel.SetActive(false);
        mcqPanel.SetActive(true);
    }

    void DisableAllOptions()
    {
        foreach (var btn in optionButtons)
            btn.interactable = false;
    }

    void HideExplanationButtons()
    {
        rightExplanationButton.gameObject.SetActive(false);
        wrongExplanationButton.gameObject.SetActive(false);
    }

    void ShowRightExplanation()
    {
        rightExplanationButton.gameObject.SetActive(true);
        wrongExplanationButton.gameObject.SetActive(false);
    }

    void ShowWrongExplanation()
    {
        wrongExplanationButton.gameObject.SetActive(true);
        rightExplanationButton.gameObject.SetActive(false);
    }
}
