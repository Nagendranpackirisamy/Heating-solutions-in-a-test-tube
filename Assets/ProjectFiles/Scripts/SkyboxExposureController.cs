using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkyboxExposureController : MonoBehaviour
{
    [System.Serializable]
    public class PageExposure
    {
        public int pageNumber;
        public float exposure = 0.5f;
    }

    [Header("Slide Controller Reference")]
    public SlideCameraController slideController;

    [Header("Exposure Settings")]
    public float changeDuration = 2f;
    public float defaultExposure = 1f;

    [Header("Page Specific Exposures")]
    public List<PageExposure> pageExposures = new List<PageExposure>();

    private Material skyboxMat;
    private Coroutine exposureRoutine;
    private int lastPage = -1;

    // Stores pages that were triggered
    private HashSet<int> activatedPages = new HashSet<int>();

    void Start()
    {
        skyboxMat = RenderSettings.skybox;

        if (skyboxMat == null || !skyboxMat.HasProperty("_Exposure"))
        {
            Debug.LogError("Skybox does not support exposure.");
            enabled = false;
            return;
        }

        defaultExposure = skyboxMat.GetFloat("_Exposure");

        if (slideController != null)
            lastPage = slideController.CurrentPageNumber;
    }

    void Update()
    {
        if (slideController == null) return;

        int currentPage = slideController.CurrentPageNumber;

        if (currentPage != lastPage)
        {
            lastPage = currentPage;
            ApplyExposureForPage(currentPage);
        }
    }

    float GetPageExposure(int page)
    {
        foreach (var p in pageExposures)
        {
            if (p.pageNumber == page)
                return p.exposure;
        }

        return defaultExposure;
    }

    void ApplyExposureForPage(int page)
    {
        // If page was activated, use its exposure
        if (activatedPages.Contains(page))
        {
            float target = GetPageExposure(page);
            SetExposure(target);
        }
        else
        {
            SetExposure(defaultExposure);
        }
    }

    // -------------------------------------------------------
    // FUNCTION YOU CALL FROM EVENT
    // -------------------------------------------------------
    public void TriggerExposureForCurrentPage()
    {
        if (slideController == null) return;

        int page = slideController.CurrentPageNumber;

        activatedPages.Add(page);

        float target = GetPageExposure(page);
        SetExposure(target);
    }

    // -------------------------------------------------------
    // SMOOTH EXPOSURE CHANGE
    // -------------------------------------------------------
    public void SetExposure(float targetExposure)
    {
        if (exposureRoutine != null)
            StopCoroutine(exposureRoutine);

        exposureRoutine = StartCoroutine(ChangeExposure(targetExposure));
    }

    IEnumerator ChangeExposure(float target)
    {
        float start = skyboxMat.GetFloat("_Exposure");
        float time = 0f;

        while (time < changeDuration)
        {
            time += Time.deltaTime;
            float t = time / changeDuration;

            float value = Mathf.Lerp(start, target, t);
            skyboxMat.SetFloat("_Exposure", value);

            yield return null;
        }

        skyboxMat.SetFloat("_Exposure", target);
    }
}
