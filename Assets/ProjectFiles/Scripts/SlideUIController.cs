using UnityEngine;
using System.Collections.Generic;

public class SlideUIController : MonoBehaviour
{
    public SlideStateController stateController;
    public List<GameObject> slideUIs;

    void OnEnable()
    {
        stateController.OnSlideChanged.AddListener(UpdateUI);
    }

    void OnDisable()
    {
        stateController.OnSlideChanged.RemoveListener(UpdateUI);
    }

    void UpdateUI(int index)
    {
        for (int i = 0; i < slideUIs.Count; i++)
        {
            slideUIs[i].SetActive(i == index);
        }
    }
}