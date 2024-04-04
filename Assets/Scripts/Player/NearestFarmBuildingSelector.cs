using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class NearestFarmBuildingSelector : MonoBehaviour
{
    [SerializeField] private SpriteRenderer singleSelector;
    [SerializeField] private SpriteRenderer[] multipleSelectors;

    private Vector3 hidePosition = new Vector3(5000, 5000, 5000);
    private Vector3 scalBig = new Vector3(1.05f, 1.05f, 1.05f);

    private Transform lastParentTransform;
    private Transform nearestTransform;

    private bool isSingleSelectorHidden;
    private bool isMultipleSelectorsHidden;

    private const float yPos = 0.1f;
    private const float animDuraition = 0.15f;

    private void Start()
    {
        UiCropsCanvas.OnCropMenuClosed += HideMultipleSelectors;
        UiBuildingEditModeCanvas.OnToggleEditMode += ToggleHideSelector;
        UiBuildCanvas.OnIsInBuildMode += ToggleHideSelector;
        PlayerInteraction.OnNearestIInteractableBuilding += SelectNearestBuilding;
        StartCoroutine(LoopAnimation());
        HideMultipleSelectors();
    }

    private void OnDestroy()
    {
        UiCropsCanvas.OnCropMenuClosed -= HideMultipleSelectors;
        PlayerInteraction.OnNearestIInteractableBuilding -= SelectNearestBuilding;
        UiBuildCanvas.OnIsInBuildMode -= ToggleHideSelector;
        UiBuildingEditModeCanvas.OnToggleEditMode -= ToggleHideSelector;
    }

    private void SelectNearestBuilding(IInteractable interactable, List<IInteractable> interactables)
    {
        HideMultipleSelectors();
        //Single selector boxes
        if (interactable != null)
        {
            nearestTransform = interactable.GetTransform();
        }
        else
        {
            HideSingleSelector();
            return;
        }
        if (nearestTransform == null)
        {
            HideSingleSelector();
        }
        else if (lastParentTransform != nearestTransform)
        {
            ShowSingleSelector(interactable.GetColliderSize().x, interactable.GetColliderSize().z);
        }

        //Multi selector boxes
        if (interactables.Count <= 0)
        {
            return;
        }
        ShowMultipleSelectors(interactables);
    }

    private void ShowMultipleSelectors(List<IInteractable> interactables)
    {
        isMultipleSelectorsHidden = false;
        for (int i = 0; i < interactables.Count; i++)
        {
            multipleSelectors[i].enabled = true;
            multipleSelectors[i].transform.localPosition = new Vector3(interactables[i].GetTransform().position.x, yPos, interactables[i].GetTransform().position.z);
            multipleSelectors[i].size = new Vector2(interactables[i].GetColliderSize().x, interactables[i].GetColliderSize().z);
        }
    }

    private void HideMultipleSelectors()
    {
        if (!isMultipleSelectorsHidden)
        {
            for (int i = 0; i < multipleSelectors.Length; i++)
            {
                multipleSelectors[i].enabled = false;
            }
            isMultipleSelectorsHidden = true;
        }
    }

    private void ShowSingleSelector(float xSize, float ySize)
    {
        isSingleSelectorHidden = false;
        singleSelector.transform.position = new Vector3(nearestTransform.position.x, yPos, nearestTransform.position.z);
        singleSelector.size = new Vector2(xSize, ySize);
        lastParentTransform = nearestTransform;
    }

    private void HideSingleSelector()
    {
        if (!isSingleSelectorHidden)
        {
            lastParentTransform = null;
            nearestTransform = null;
            singleSelector.transform.position = hidePosition;
            isSingleSelectorHidden = true;
        }
    }

    private IEnumerator LoopAnimation()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(animDuraition * 2);
            singleSelector.transform.DOScale(scalBig, animDuraition);
            for (int i = 0; i < multipleSelectors.Length; i++)
            {
                multipleSelectors[i].transform.DOScale(scalBig, animDuraition);
            }
            yield return new WaitForSeconds(animDuraition);
            singleSelector.transform.DOScale(Vector3.one, animDuraition);
            for (int i = 0; i < multipleSelectors.Length; i++)
            {
                multipleSelectors[i].transform.DOScale(Vector3.one, animDuraition);
            }
        }
    }

    private void ToggleHideSelector(bool isVisible)
    {
        singleSelector.color = isVisible ? Color.clear : Color.white;
        for (int i = 0; i < multipleSelectors.Length; i++)
        {
            multipleSelectors[i].color = isVisible ? Color.clear : Color.white;
        }
    }
}