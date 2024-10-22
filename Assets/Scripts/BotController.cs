using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotController : MonoBehaviour
{
    private PistolController pistolController;
    private readonly float playerDetectionRange = 8f;
    private readonly float enemyDetectionRange = 14f;
    private readonly float initialTakeActionCooldown = 0.2f;


    private float takeActionCooldown;
    private readonly float idleCooldownMultiplier = 4f;
    private readonly float speedDelta = 2.5f;
    private float enemyTooCloseRange = 6f;
    private float playerTooCloseRange = 4f;
    private LayerMask enemyLayer;
    private LayerMask playerLayer;
    private Vector3[] availableMovementDirections;
    private BotState botState = BotState.Idle;
    private Vector3 optimalMoveTarget;
    // the bot will make the "good" decision botSmartness percent of the time
    private readonly float botSmartness = 2f;


    private enum BotState
    {
        Idle,
        FollowPlayer,
        Aiming,
        EscapeFromEnemy
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyDetectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(optimalMoveTarget, 0.3f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        availableMovementDirections = new Vector3[]
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right,
            (Vector3.left + Vector3.forward).normalized,
            (Vector3.left + Vector3.back).normalized,
            (Vector3.right + Vector3.forward).normalized,
            (Vector3.right + Vector3.back).normalized
        };
        takeActionCooldown = initialTakeActionCooldown;
        enemyLayer = LayerMask.GetMask("Enemy");
        playerLayer = LayerMask.GetMask("Player");
        pistolController = gameObject.FindComponentInChildWithTag<Transform>("Pistol").GetComponent<PistolController>();
        // languagues don't have identifier dynamic renaming without newly initializing :(
        // this is so to convert it to square magnitude for optimization
        playerTooCloseRange *= playerTooCloseRange;
        enemyTooCloseRange *= enemyTooCloseRange;
        StartCoroutine(TakeActionCoroutine());
    }


    void Update()
    {
        if
        (
            botState == BotState.Idle
            || botState == BotState.FollowPlayer
            || botState == BotState.EscapeFromEnemy
            || botState == BotState.Aiming
        )
        {
            MoveToTarget();
        }
    }

    bool DidBotThinkOfThis()
    {
        return Random.Range(0f, 1f) < botSmartness;
    }

    IEnumerator TakeActionCoroutine()
    {
        while (true)
        {
            takeActionCooldown = initialTakeActionCooldown;
            ActivateNextAction();
            yield return new WaitForSeconds(takeActionCooldown);
        }
    }

    void ActivateNextAction()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, enemyDetectionRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Shoot();
            }
        }
        // enemies take priority in state
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, enemyDetectionRange, enemyLayer);
        if (hitEnemies.Length > 0)
        {
            TargetEnemies(hitEnemies);
            return;
        }
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, playerDetectionRange, playerLayer);
        foreach (Collider player in hitPlayers) {
            // follow first player that is too close detected
            if ((player.transform.position - transform.position).sqrMagnitude <= playerTooCloseRange)
            {
                continue;
            }
            FollowPlayer(player);
            return;
        }
        Idle();
    }

    void Idle()
    {
        // pick random from available movement directions to walk to
        optimalMoveTarget = transform.position + availableMovementDirections[Random.Range(0, availableMovementDirections.Length)];
        RotateTo(optimalMoveTarget);
        // reduce cooldown of the TakeActionCoroutine
        takeActionCooldown *= idleCooldownMultiplier;
        botState = BotState.Idle;
    }

    void TargetEnemies(Collider[] hitEnemies)
    {
        if (!DidBotThinkOfThis()) {
            return;
        }
        Collider closestEnemy = hitEnemies
            .OrderBy(e => (e.transform.position - transform.position).sqrMagnitude)
            .FirstOrDefault();
        if ((closestEnemy.transform.position - transform.position).sqrMagnitude > enemyTooCloseRange)
        {
            LockOnEnemy(closestEnemy.transform);
            return;
        }
        // enemy is too close detected
        // escape either away from closest enemy or first enemyless available direction
        Vector3 safestDirection = transform.position - closestEnemy.transform.position;
        // check for enemyless directions on the player side of the plane. The plane is between the player and the enemy
        IEnumerable<Vector3> bestAvailableDirections = availableMovementDirections
            .Where(direction => Vector3.Dot(direction, safestDirection) >= 0);
        foreach (Vector3 availableDirection in availableMovementDirections)
        {
            // check enemyless directions
            if (Physics.Raycast(transform.position, availableDirection, out RaycastHit hit, enemyDetectionRange))
            {
                if (!hit.collider.CompareTag("Enemy"))
                {
                    // no enemies found path
                    safestDirection = availableDirection;
                    break;
                }
            }
        }
        EscapeFromEnemy(transform.position - safestDirection * 3);
    }

    void FollowPlayer(Collider player)
    {
        optimalMoveTarget = FindOptimalMovementTarget(player.transform.position);
        RotateTo(optimalMoveTarget);
        botState = BotState.FollowPlayer;
    }

    private void LockOnEnemy(Transform enemyTransform)
    {
        optimalMoveTarget = FindOptimalMovementTarget(enemyTransform.position);
        RotateTo(optimalMoveTarget);
        botState = BotState.Aiming;
    }

    private void EscapeFromEnemy(Vector3 enemyPosition)
    {
        Vector3 escapePositionFromEnemy = (transform.position - enemyPosition) * 2;
        optimalMoveTarget = FindOptimalMovementTarget(escapePositionFromEnemy);
        RotateTo(optimalMoveTarget);
        botState = BotState.EscapeFromEnemy;
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, optimalMoveTarget, speedDelta * Time.deltaTime);
    }

    private void RotateTo(Vector3 target)
    {
        Vector3 lookAtDirection = target - transform.position;
        lookAtDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookAtDirection, Vector3.up);
    }

    private void Shoot()
    {
        pistolController.Fire();
    }

    private Vector3 FindOptimalMovementTarget(Vector3 target)
    {
        target -= transform.position;
        if (target == Vector3.zero)
        {
            return transform.position;
        }
        IEnumerable<float> similarities = availableMovementDirections.Select(
            availableDirection => Vector3.Dot(target, availableDirection));
        float maxSimilarity = similarities.Max();
        int index = similarities.ToList().IndexOf(maxSimilarity);
        Vector3 movementDirection = availableMovementDirections[index];
        return transform.position + movementDirection;
    }
}