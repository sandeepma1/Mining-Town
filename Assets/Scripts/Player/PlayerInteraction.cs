using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public static Action<IInteractable, List<IInteractable>> OnNearestIInteractableBuilding;
    public static Action<bool> OnToggleColliderTrigger;
    public static Action<List<int>> OnNearbyScannedBedIndexes;
    [Header("Debug")]
    [SerializeField] private bool autoPickaxe;
    [SerializeField] private bool autoSword;

    [Space(20)]
    [SerializeField] private LayerMask interactableSearchLayer;
    [SerializeField] private LayerMask breakableSearchLayer;
    [SerializeField] private LayerMask chopableSearchLayer;
    [SerializeField] private LayerMask monsterSearchLayer;
    [SerializeField] private Transform playerMeshRotation;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform weaponHolder; //do not delete
    [SerializeField] private BasicSword basicSword;
    [SerializeField] private BasicPickaxe basicPickaxe;
    [SerializeField] private BasicAxe basicAxe;
    [SerializeField] private float maxMonsterAttackRange = 2f;//if this distance, then hit monster only not breakables

    private PlayerMovement playerMovement;
    private CapsuleCollider capsuleCollider;
    private const float rotationSpeed = 10;

    private float maxInteractionRange = 2;
    private IInteractable nearestIneractable;
    private IBreakable nearestBreakable;
    private IChopable nearestChopable;
    private IMonster nearestMonster;
    private Transform closestInteractableTransform;
    private Transform nearestMonsterTransform;
    private Transform closestBreakableTransform;
    private Transform closestChopableTransform;
    private float searchRadius = 5;
    private float pickaxeTimer = 0;
    private float axeTimer = 0;
    private float swordTimer = 0;
    private PlayerMode playerMode;
    private bool isMonsterNear;
    private float scanSize;
    private float halfScanSize;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        SceneLoader.OnSceneChanged += OnSceneChanged;
    }

    private IEnumerator Start()
    {
        GameEvents.OnPauseGame += OnPauseGame;
        GameEvents.OnResumeGame += OnResumeGame;
        PlayerCurrencyManager.OnPlayerDied += PlayDeadAnimation;
        PlayerCurrencyManager.OnPlayerHitted += PlayHittedAnimation;
        UiCropsCanvas.OnScanNearbyBeds += OnScanNearbyBeds;
        Joystick.OnKnobButtonClicked += OnKnobButtonClicked;
        OnToggleColliderTrigger += ToggleColliderTrigger;
        capsuleCollider = GetComponent<CapsuleCollider>();
        yield return new WaitForEndOfFrame();
        FindClosestInteractable();
    }

    private void OnDestroy()
    {
        PlayerCurrencyManager.OnPlayerDied -= PlayDeadAnimation;
        PlayerCurrencyManager.OnPlayerHitted -= PlayHittedAnimation;
        UiCropsCanvas.OnScanNearbyBeds -= OnScanNearbyBeds;
        Joystick.OnKnobButtonClicked -= OnKnobButtonClicked;
        OnToggleColliderTrigger -= ToggleColliderTrigger;
        GameEvents.OnPauseGame -= OnPauseGame;
        GameEvents.OnResumeGame -= OnResumeGame;
    }

    private void Update()
    {
        if (GameEvents.IsGamePaused())
        {
            return;
        }
        switch (playerMode)
        {
            case PlayerMode.FarmMode:
                UpdateFarmMode();
                break;
            case PlayerMode.MinesMode:
                UpdateMinesMode();
                break;
            case PlayerMode.ForestMode:
                UpdateMinesMode();
                break;
            default:
                break;
        }
    }


    #region Farm Mode
    private void OnKnobButtonClicked()
    {
        if (playerMode == PlayerMode.FarmMode && !playerMovement.isPlayerMoving && nearestIneractable != null)
        {
            nearestIneractable.InteractOnClick();
        }
    }

    private void UpdateFarmMode()
    {
        if (playerMovement.isPlayerMoving || closestInteractableTransform == null)
        {
            FindClosestInteractable();
            if (closestInteractableTransform != null)
            {
                OnNearestIInteractableBuilding?.Invoke(nearestIneractable, new List<IInteractable>());
            }
            else
            {
                OnNearestIInteractableBuilding?.Invoke(null, new List<IInteractable>());
            }
        }
    }


    #region While planting multiple crops at once
    private void OnScanNearbyBeds(float size)
    {
        scanSize = size;
        halfScanSize = scanSize / 2;
        Collider[] hitColliders = Physics.OverlapBox(transform.position,
            new Vector3(halfScanSize, halfScanSize, halfScanSize), Quaternion.identity, interactableSearchLayer);
        List<int> raisedBedIndexes = new List<int>();

        if (hitColliders.Length > 0)
        {
            List<IInteractable> raisedBedIInteractables = new List<IInteractable>();
            for (int i = 0; i < hitColliders.Length; i++)
            {
                RaisedBed raisedBed = hitColliders[i].GetComponent<RaisedBed>();
                if (raisedBed != null && raisedBed.RaisedBedState == RaisedBedState.Idle && raisedBedIndexes.Count <= 9)
                {
                    raisedBedIndexes.Add(raisedBed.GetBedIndex());
                    raisedBedIInteractables.Add(raisedBed.GetComponent<IInteractable>());
                }
            }
            OnNearestIInteractableBuilding?.Invoke(nearestIneractable, raisedBedIInteractables);
        }
        OnNearbyScannedBedIndexes?.Invoke(raisedBedIndexes);
    }

    // Draw a yellow sphere at the transform's position
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawCube(transform.position, new Vector3(scanSize, scanSize, scanSize));
    //}

    #endregion
    #endregion


    #region Mines Mode
    private void UpdateMinesMode()
    {
        if (playerMovement.isPlayerMoving)
        {
            nearestMonster = null;
            nearestMonsterTransform = null;
        }
        if (playerMovement.isPlayerMoving || closestInteractableTransform == null || nearestMonsterTransform == null)
        {
            FindClosestMonster();
            FindClosestBreakable();
            FindClosestChopable();

            //TODO: Refactor this iffs
            //If monster and breakable near then focus on monster
            if (nearestMonsterTransform != null && closestBreakableTransform != null && closestChopableTransform != null)//Sword mode
            {
                basicPickaxe.ToggleWeaponVisiblity(false);
                basicAxe.ToggleWeaponVisiblity(false);
                basicSword.ToggleWeaponVisiblity(true);
                SelectionCircle.OnSetToThisParent?.Invoke(nearestMonsterTransform);
                isMonsterNear = true;
            }
            else if (nearestMonsterTransform != null && (closestBreakableTransform == null || closestChopableTransform == null))//Sword mode
            {
                basicPickaxe.ToggleWeaponVisiblity(false);
                basicAxe.ToggleWeaponVisiblity(false);
                basicSword.ToggleWeaponVisiblity(true);
                SelectionCircle.OnSetToThisParent?.Invoke(nearestMonsterTransform);
                isMonsterNear = true;
            }
            else if (nearestMonsterTransform == null && closestBreakableTransform != null)//Pickaxe mode
            {
                basicPickaxe.ToggleWeaponVisiblity(true);
                basicAxe.ToggleWeaponVisiblity(false);
                basicSword.ToggleWeaponVisiblity(false);
                SelectionCircle.OnSetToThisParent?.Invoke(closestBreakableTransform);
                isMonsterNear = false;
            }
            else if (nearestMonsterTransform == null && closestChopableTransform != null)//Axe mode
            {
                basicPickaxe.ToggleWeaponVisiblity(false);
                basicAxe.ToggleWeaponVisiblity(true);
                basicSword.ToggleWeaponVisiblity(false);
                SelectionCircle.OnSetToThisParent?.Invoke(closestChopableTransform);
                isMonsterNear = false;
            }
            else
            {
                SelectionCircle.OnSetToThisParent?.Invoke(null);//NO mode
                basicPickaxe.ToggleWeaponVisiblity(false);
                basicAxe.ToggleWeaponVisiblity(false);
                basicSword.ToggleWeaponVisiblity(false);
                isMonsterNear = false;
            }

            if (isMonsterNear)
            {
                //pickaxeTimer = 0;
                FocusMonster();
            }
            else
            {
                FocusBreakables();
                FocusChopables();
            }
        }
    }


    #region Monster Interaction
    private void FocusMonster()
    {
        if (nearestMonsterTransform != null && IsClosedToObject(nearestMonsterTransform, maxMonsterAttackRange))
        {
            AttackMonster();
        }
    }

    private void AttackMonster()
    {
        if (!playerMovement.isPlayerMoving && autoSword)
        {
            swordTimer += Time.deltaTime;
            if (swordTimer >= GameVariables.swordHitRate)
            {
                LookTowardsObject(nearestMonster.GetTransform());
                basicSword.Attack(); //This is physics based hit system, unlike pickaxe
                PlaySwordAnimation();
            }
        }
    }
    #endregion


    #region Pickaxe Interaction
    private void FocusBreakables()
    {
        if (closestBreakableTransform != null && IsClosedToObject(closestBreakableTransform, maxInteractionRange))
        {
            if (autoPickaxe)
            {
                InteractBreakableWithPickaxe();
            }
        }
    }

    private void InteractBreakableWithPickaxe()
    {
        if (!playerMovement.isPlayerMoving && PlayerCurrencyManager.Energy >= GameVariables.energy_pickaxe)
        {
            pickaxeTimer += Time.deltaTime;
            if (pickaxeTimer >= GameVariables.pickaxeHitRate)
            {
                PlayerCurrencyManager.ReduceEnergy(GameVariables.energy_pickaxe);
                pickaxeTimer = 0;
                LookTowardsObject(nearestBreakable.GetTransform());
                Invoke("HitBreakable", pickaxeImpactDelay);
                PlayPickaxeAnimation();
            }
        }
    }

    private const float pickaxeImpactDelay = 0.2f;
    private void HitBreakable()
    {
        nearestBreakable.Hit(1);
    }
    #endregion


    #region Axe Interaction
    private void FocusChopables()
    {
        if (closestChopableTransform != null && IsClosedToObject(closestChopableTransform, maxInteractionRange))
        {
            if (autoPickaxe)
            {
                InteractChopableWithAxe();
            }
        }
    }

    private void InteractChopableWithAxe()
    {
        if (!playerMovement.isPlayerMoving && PlayerCurrencyManager.Energy >= GameVariables.energy_pickaxe)
        {
            axeTimer += Time.deltaTime;
            if (axeTimer >= GameVariables.axeHitRate)
            {
                PlayerCurrencyManager.ReduceEnergy(GameVariables.energy_axe);
                axeTimer = 0;
                LookTowardsObject(nearestChopable.GetTransform());
                Invoke("HitChopable", axeImpactDelay);
                PlayPickaxeAnimation();
            }
        }
    }

    private const float axeImpactDelay = 0.2f;
    private void HitChopable()
    {
        nearestChopable.Hit(1);
    }
    #endregion
    #endregion


    private void FindClosestInteractable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius, interactableSearchLayer);
        if (hitColliders.Length > 0)
        {
            IInteractable currentFarmIneractable = GetClosestItem(hitColliders);
            if (nearestIneractable != currentFarmIneractable)
            {
                nearestIneractable = currentFarmIneractable;
                closestInteractableTransform = nearestIneractable.GetTransform();
            }
        }
        else
        {
            nearestIneractable = null;
            closestInteractableTransform = null;
        }
        UiPlayerHudCanvas.OnShowPlayerBubble?.Invoke(nearestIneractable);
    }

    private IInteractable GetClosestItem(Collider[] groundOverlap)
    {
        IInteractable bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < groundOverlap.Length; i++)
        {
            if (groundOverlap[i] == null)
            {
                continue;
            }
            Vector3 directionToTarget = groundOverlap[i].transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = groundOverlap[i].transform.GetComponent<IInteractable>();
            }
        }
        return bestTarget;
    }

    private void FindClosestBreakable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius, breakableSearchLayer);
        if (hitColliders.Length > 0)
        {
            IBreakable currentBreakable = GetClosestBreakable(hitColliders);
            if (nearestBreakable != currentBreakable)
            {
                nearestBreakable = currentBreakable;
                closestBreakableTransform = nearestBreakable.GetTransform();
            }
        }
        else
        {
            nearestBreakable = null;
            closestBreakableTransform = null;
        }
    }

    private IBreakable GetClosestBreakable(Collider[] groundOverlap)
    {
        IBreakable bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < groundOverlap.Length; i++)
        {
            if (groundOverlap[i] == null)
            {
                continue;
            }
            Vector3 directionToTarget = groundOverlap[i].transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = groundOverlap[i].transform.GetComponent<IBreakable>();
            }
        }
        return bestTarget;
    }

    private void FindClosestChopable()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius, chopableSearchLayer);
        if (hitColliders.Length > 0)
        {
            IChopable currentChopable = GetClosestChopable(hitColliders);
            if (nearestChopable != currentChopable)
            {
                nearestChopable = currentChopable;
                closestChopableTransform = nearestChopable.GetTransform();
            }
        }
        else
        {
            nearestChopable = null;
            closestChopableTransform = null;
        }
    }

    private IChopable GetClosestChopable(Collider[] groundOverlap)
    {
        IChopable bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < groundOverlap.Length; i++)
        {
            if (groundOverlap[i] == null)
            {
                continue;
            }
            Vector3 directionToTarget = groundOverlap[i].transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = groundOverlap[i].transform.GetComponent<IChopable>();
            }
        }
        return bestTarget;
    }

    private void FindClosestMonster()
    {
        Collider[] nearestMonsterColliders = Physics.OverlapSphere(transform.position, searchRadius, monsterSearchLayer);
        if (nearestMonsterColliders.Length > 0)
        {
            IMonster monster = GetClosestMonster(nearestMonsterColliders);
            if (nearestMonster != monster)
            {
                nearestMonster = monster;
                nearestMonsterTransform = nearestMonster.GetTransform();
            }
        }
        else
        {
            nearestMonster = null;
            nearestMonsterTransform = null;
        }
    }

    private IMonster GetClosestMonster(Collider[] nearestMonsterColliders)
    {
        IMonster bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < nearestMonsterColliders.Length; i++)
        {
            // If nearest is in the neartest monster's colliders then focus on the same monster
            if (nearestMonsterTransform == nearestMonsterColliders[i].transform)
            {
                return nearestMonster;
            }
            if (nearestMonsterColliders[i] == null)
            {
                continue;
            }
            Vector3 directionToTarget = nearestMonsterColliders[i].transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = nearestMonsterColliders[i].transform.GetComponent<IMonster>();
            }
        }
        return bestTarget;
    }

    private void OnSceneChanged(Scenes scenes)
    {
        GameEvents.ResumeGame();
        switch (scenes)
        {
            case Scenes.Loading:
                break;
            case Scenes.FarmHome:
                playerMode = PlayerMode.FarmMode;
                break;
            case Scenes.Mines:
                playerMode = PlayerMode.MinesMode;
                break;
            case Scenes.Forest:
                playerMode = PlayerMode.ForestMode;
                break;
            case Scenes.Town:
                break;
            default:
                break;
        }

        switch (playerMode)
        {
            case PlayerMode.FarmMode:
                if (SaveLoadManager.saveData != null)
                { transform.position = SaveLoadManager.saveData.playerStats.playerPosition; }
                searchRadius = 1;
                break;
            case PlayerMode.MinesMode:
                searchRadius = 5;
                break;
            case PlayerMode.ForestMode:
                searchRadius = 5;
                break;
            default:
                break;
        }
        basicPickaxe.ToggleWeaponVisiblity(false);
        basicAxe.ToggleWeaponVisiblity(false);
        basicSword.ToggleWeaponVisiblity(false);
    }

    private void ToggleColliderTrigger(bool isTrigger)
    {
        capsuleCollider.isTrigger = isTrigger;
    }

    private void LookTowardsObject(Transform target)
    {
        playerMeshRotation.DOLookAt(target.position, 0.20f);
    }

    private bool IsClosedToObject(Transform target, float range)
    {
        return Vector3.Distance(transform.position, target.position) <= range;
    }

    private void LookTowardsObjectSlerp(Vector3 position)
    {
        playerMeshRotation.rotation = Quaternion.Slerp(playerMeshRotation.rotation,
           Quaternion.LookRotation((position - transform.position).normalized),
           Time.deltaTime * rotationSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(GameVariables.tag_ladder))
        {
            Ladder ladder = collision.collider.gameObject.GetComponent<Ladder>();
            if (ladder != null)
            {
                ladder.OnLadderInteract();
            }
        }
        if (collision.gameObject.CompareTag(GameVariables.tag_forestExit))
        {
            ForestExit forestExit = collision.collider.gameObject.GetComponent<ForestExit>();
            if (forestExit != null)
            {
                forestExit.OnForestExitInteract();
            }
        }
        if (collision.gameObject.CompareTag(GameVariables.tag_water))
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shakeable"))
        {
            IShakeable shakeable = other.GetComponent<IShakeable>();
            if (shakeable != null)
            {
                shakeable.Shake();
            }
        }
    }


    #region All Animation Functions
    private void OnResumeGame()
    {
        animator.speed = 1;
    }

    private void OnPauseGame()
    {
        animator.speed = 0;
    }

    private void PlayPickaxeAnimation()
    {
        animator.SetTrigger("PickaxeHit");
    }

    private void PlaySwordAnimation()
    {
        animator.SetTrigger("SwordAttack");
    }

    private void PlayHittedAnimation()
    {
        animator.SetTrigger("Hitted");
    }

    private void PlayDeadAnimation()
    {
        animator.SetTrigger("Dead");
    }
    #endregion
}


public enum PlayerMode
{
    FarmMode,
    MinesMode,
    ForestMode
}