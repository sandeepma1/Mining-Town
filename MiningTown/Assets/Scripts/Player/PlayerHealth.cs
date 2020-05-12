using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance = null;
    [SerializeField] private bool isGodMode;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private float maxPlayerHealth = 50;
    [SerializeField] private SpriteRenderer outlineSprite;
    [SerializeField] private SpriteRenderer filledSprite;
    [SerializeField] private TextMeshPro healthText;
    private float playerHealth;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerHealth = maxPlayerHealth;
        SetHealthText(playerHealth);
        float rotX = PrefabBank.Instance.GetMainCamera().transform.eulerAngles.x;
        outlineSprite.transform.eulerAngles = new Vector3(rotX, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (interactionLayer == (interactionLayer | (1 << other.gameObject.layer)))
        {
            //Damage taken by player
            float damage = other.GetComponent<IInteractable>().GetDamageValue();
            TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            if (isGodMode)
            {
                return;
            }
            //Player died
            UiPlayerDiedCanvas.ShowPlayerDiedCanvas?.Invoke();
        }
        else
        {
            filledSprite.transform.localScale = new Vector3(playerHealth / maxPlayerHealth, 1);
            SetHealthText(playerHealth);
        }
    }

    private void SetHealthText(float health)
    {
        healthText.text = (health.ToString("F0"));
    }
}