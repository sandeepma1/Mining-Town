using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer itemSpriteRenderer;
    private BoxCollider boxCollider;
    private Item item;
    private int count;
    private const float dropHeightY = 5f;
    private const float randomMaxX = 1f;
    private const float randomMinZ = -0.5f;
    private const float randomMaxZ = -1f;
    private Vector3 randomPos;
    private bool isXp;
    private bool isAtFarm;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameVariables.tag_player))
        {
            if (isXp)
            {
                PlayerCurrencyManager.AddXp(count);
                UiResourceSpawnCanvas.OnHarvestResource?.Invoke("xp", transform, count);
                ItemTakenByPlayer();
            }
            else
            {
                if (isAtFarm) // direct add to Barn inventory
                {
                    SaveLoadManager.AddUpdateItemInBarn(item.itemId, count);
                    UiResourceSpawnCanvas.OnHarvestResource?.Invoke(item.slug, transform, 1);
                    ItemTakenByPlayer();
                }
                else // direct add to Backpack
                {
                    if (UiPlayerBackpackCanvas.Instance.IfBackpackHasEmptySpace(new ItemIdWithCount(item.itemId, count)))
                    {
                        SaveLoadManager.AddUpdateItemInBackpack(item.itemId, count);
                        UiResourceSpawnCanvas.OnHarvestResource?.Invoke(item.slug, transform, 1);
                        ItemTakenByPlayer();
                    }
                    else
                    {
                        StartCoroutine(NoSpaceAnimation());
                        UiFeedbackPopupCanvas.OnShowFeedbackPopup?.Invoke(GameVariables.msg_lowOnBackpackSpace);
                    }
                }
            }
        }
    }

    private void ItemTakenByPlayer()
    {
        boxCollider.enabled = false;
        Destroy(gameObject);
    }

    private void GiveItemToPlayer()
    {
        if (isXp)
        {
            PlayerCurrencyManager.AddXp(count);
            UiResourceSpawnCanvas.OnHarvestResource?.Invoke("xp", transform, count);
        }
        if (isAtFarm)
        {
            SaveLoadManager.AddUpdateItemInBarn(item.itemId, count);
            UiResourceSpawnCanvas.OnHarvestResource?.Invoke(item.slug, transform, 1);
        }
        boxCollider.enabled = false;
        transform.DOMove(PlayerMovement.Instance.transform.position, 0.3f).OnComplete(() => Destroy(gameObject));
    }

    private IEnumerator NoSpaceAnimation()
    {
        yield return new WaitForEndOfFrame();
        itemSpriteRenderer.transform.DOLocalMoveY(dropHeightY / 2, 0.1f)
            .OnComplete(() => itemSpriteRenderer.transform.DOLocalMoveY(0.3f, 0.15f).SetEase(Ease.OutBounce));
    }

    public void InitItem(Item item, int count, bool isAtFarm)
    {
        isXp = false;
        this.count = count;
        this.item = item;
        this.isAtFarm = isAtFarm;
        itemSpriteRenderer.sprite = AtlasBank.Instance.GetSpriteByName(item.slug, AtlasType.UiItems);
        StartCoroutine(DropItemAnimation(isAtFarm, false));
    }

    public void InitXp(int count)
    {
        isXp = true;
        this.count = count;
        itemSpriteRenderer.sprite = AtlasBank.Instance.GetSpriteByName("xp", AtlasType.UiItems);
        StartCoroutine(DropItemAnimation(true, true));
    }

    private IEnumerator DropItemAnimation(bool isFarm, bool isXp)
    {
        yield return new WaitForEndOfFrame();
        randomPos = GetRandomPos();
        transform.DOLocalMoveY(dropHeightY, 0.2f)
            .OnComplete(() => transform.DOLocalMove(randomPos, 0.25f).SetEase(Ease.OutBounce)
            .OnComplete(() => boxCollider.enabled = true));
        if (isFarm || isXp)
        {
            transform.DOLocalMoveY(dropHeightY, 0.2f)
           .OnComplete(() => transform.DOLocalMove(randomPos, 0.25f).SetEase(Ease.OutBounce)
           .OnComplete(() => boxCollider.enabled = true)
           .OnComplete(() => GiveItemToPlayer()));
        }
    }

    private Vector3 GetRandomPos()
    {
        return new Vector3(UnityEngine.Random.Range(transform.localPosition.x - randomMaxX, transform.localPosition.x + randomMaxX),
            0, UnityEngine.Random.Range(transform.localPosition.z + randomMinZ, transform.localPosition.z + randomMaxZ));
    }
}