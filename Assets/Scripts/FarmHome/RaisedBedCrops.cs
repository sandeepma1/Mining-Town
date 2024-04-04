using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RaisedBedCrops : MonoBehaviour
{
    private List<Transform> cropParents = new List<Transform>();
    private List<SpriteRenderer> cropSprites = new List<SpriteRenderer>();
    private List<Animator> cropAnimators = new List<Animator>();
    private Item currentCropItem;

    private void CreateCropLayout(int layoutId, Transform parent)
    {
        if (cropSprites == null || cropSprites.Count == 0)
        {
            int cropCount = RaisedBedCropLayout.layouts[layoutId].Count;
            for (int i = 0; i < cropCount; i++)
            {
                cropParents[i] = new GameObject().transform;
                cropParents[i].SetParent(parent);
                cropParents[i].localPosition = RaisedBedCropLayout.layout2x2[i];

                cropSprites[i] = Instantiate(Resources.Load<SpriteRenderer>(GameVariables.path_cropPrefabPath) as SpriteRenderer, transform);
                cropSprites[i].transform.SetParent(cropParents[i]);
                cropSprites[i].transform.localPosition = Vector3.zero;
                cropSprites[i].transform.localScale = new Vector3(1, 0, 1);

                cropAnimators[i] = cropSprites[i].GetComponent<Animator>();
                cropAnimators[i].Update(UnityEngine.Random.Range(0.1f, 2.5f));
            }
        }
    }

    //private void CreateCropStages()
    //{
    //    int totalSeconds = currentCropItem.yieldDurationInMins * 60;
    //    int interval = totalSeconds / 3;
    //    for (int i = 0; i < 4; i++)
    //    {
    //        cropStages[i] = interval * i;
    //    }
    //}

    private void AssignSpriteByIndex(int index)
    {
        if (!index.IsWithin(0, 5))
        {
            return;
        }
        Sprite cropSprite = AtlasBank.Instance.GetSpriteByName(currentCropItem.slug + index, AtlasType.UiItems);
        for (int i = 0; i < cropSprites.Count; i++)
        {
            cropSprites[i].sprite = cropSprite;
        }
    }




    public void Shake()
    {
        for (int i = 0; i < cropParents.Count; i++)
        {
            cropParents[i].DOPunchRotation(new Vector3(0, 0, 10), 1, 4, 1);
        }
    }

    #region Animate hide show crops
    private const float showYScale = 1;
    private const float hideYScale = 0;
    private const float animDuration = 0.5f;
    private IEnumerator AnimateShowHideCrops(bool isVisible)
    {
        float pos = isVisible ? showYScale : hideYScale;
        if (cropSprites != null)
        {
            for (int i = 0; i < cropSprites.Count; i++)
            {
                cropSprites[i].transform.DOScaleY(pos, animDuration);
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForEndOfFrame();
    }
    #endregion
}
