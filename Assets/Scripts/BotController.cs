using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotController : MonoBehaviour
{
    private GunController gunController;
    private readonly float detectionRange = 10f;
    private readonly float TakeActionCooldown = 0.2f;
    private readonly float speedDelta = 2f;
    private float enemyTooCloseRange = 6f;
    private float playerTooCloseRange = 5f;
    private LayerMask enemyLayer;
    private LayerMask playerLayer;
    private Vector3[] availableMovementDirections;
    private bool isLockedIn = false;
    private bool shouldMove = false;
    private Vector3 optimalMoveTarget;

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
        enemyLayer = LayerMask.GetMask("Enemy");
        playerLayer = LayerMask.GetMask("Player");
        gunController = gameObject.FindComponentInChildWithTag<Transform>("Gun").GetComponent<GunController>();
        playerTooCloseRange *= playerTooCloseRange;
        enemyTooCloseRange *= enemyTooCloseRange;
        StartCoroutine(TakeActionCoroutine());
    }

    void Update()
    {
        if (shouldMove)
        {
            MoveToTarget();
        }
    }

    IEnumerator TakeActionCoroutine()
    {
        while (true) {
            TargetEnemies();
            if (!isLockedIn)
            {
                FollowPlayers();
            }
            yield return new WaitForSeconds(TakeActionCooldown);
        }
    }

    void TargetEnemies()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        Vector3 enemyPositionAverage = Vector3.zero;
        int tooCloseEnemiesCount = 0;
        foreach (Collider enemy in hitEnemies) {
            // lock on the first enemy detected
            if ((enemy.transform.position - transform.position).sqrMagnitude <= enemyTooCloseRange)
            {
                ++tooCloseEnemiesCount;
                enemyPositionAverage += enemy.transform.position;
            }
            if (tooCloseEnemiesCount > 0)
            {
                continue;
            }
            isLockedIn = true;
            LockOnEnemy(enemy.transform);
            return;
        }
        if (tooCloseEnemiesCount > 0)
        {
            enemyPositionAverage /= tooCloseEnemiesCount;
            // escape away the center of all enemies that are too close
            EscapeFromEnemy(enemyPositionAverage);
            return;
        }
        isLockedIn = false;
    }

    void FollowPlayers()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        foreach (Collider player in hitPlayers) {
            // follow first player detected
            if ((player.transform.position - transform.position).sqrMagnitude <= playerTooCloseRange)
            {
                continue;
            }
            shouldMove = true;
            optimalMoveTarget = FindOptimalMovementTarget(player.transform.position);
            RotateTo(optimalMoveTarget);
            return;
        }
        shouldMove = false;
    }

    private void LockOnEnemy(Transform enemyTransform)
    {
        optimalMoveTarget = FindOptimalMovementTarget(enemyTransform.position);
        RotateTo(optimalMoveTarget);
        Shoot();
    }

    private void EscapeFromEnemy(Vector3 enemyPosition)
    {
        Vector3 escapePositionFromEnemy = (transform.position - enemyPosition) * 2;
        optimalMoveTarget = FindOptimalMovementTarget(escapePositionFromEnemy);
        RotateTo(optimalMoveTarget);
        shouldMove = true;
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
        gunController.Fire();
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