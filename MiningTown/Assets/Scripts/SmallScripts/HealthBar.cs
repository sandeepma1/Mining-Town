using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outlineSprite;
    [SerializeField] private SpriteRenderer filledSprite;

    private void Start()
    {
        float rotX = PrefabBank.Instance.GetMainCamera().transform.eulerAngles.x;
        transform.eulerAngles = new Vector3(rotX, 0, 0);
        outlineSprite.gameObject.SetActive(false);
    }

    public void OnHealthChanged(float healthFill)
    {
        if (!outlineSprite.gameObject.activeSelf)
        {
            outlineSprite.gameObject.SetActive(true);
        }
        filledSprite.transform.localScale = new Vector3(healthFill, 1);
    }
}