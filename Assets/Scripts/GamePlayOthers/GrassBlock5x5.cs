using UnityEngine;

public class GrassBlock5x5 : MonoBehaviour
{
    //https://lindenreid.wordpress.com/2018/01/07/waving-grass-shader-in-unity/
    [SerializeField] private GameObject[] grasses;

    private void Start()
    {
        for (int i = 0; i < grasses.Length; i++)
        {
            GrassManager.OnAddGrass?.Invoke(grasses[i].transform);
            for (int k = 0; k < 3; k++)
            {
                int grassRanId = Random.Range(0, 15);
                int grassId;
                if (grassRanId == 0)
                {
                    grassId = 9;
                }
                else
                {
                    grassId = 6;
                }
                Transform spriteTransform = grasses[i].transform.GetChild(k).transform;
                SpriteRenderer spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = AtlasBank.Instance.GetSpriteByName("Grass" + grassId, AtlasType.UiItems);
                if (grassRanId > 8)
                {
                    spriteRenderer.flipX = true;
                }
                spriteTransform.localPosition = new Vector3(Random.Range(-0.45f, 0.45f), 0, Random.Range(-0.45f, 0.45f));
                spriteTransform.localEulerAngles = new Vector3(55, Random.Range(-15, 15), 0);
                spriteRenderer.sortingOrder = (int)(spriteTransform.position.z * -100);

            }
        }
    }
}