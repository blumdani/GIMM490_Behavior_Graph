using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private HealthController healthController;

    public float attackDistance = 2f;
    public float attackCooldown = 3f;

    private float attackTimer = 0f;

    void Start()
    {
        healthController = FindFirstObjectByType<HealthController>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.stoppingDistance = attackDistance;
    }

    void Update()
    {
        if (player == null) return;

        // Always move toward player
        agent.SetDestination(player.position);

        // Countdown cooldown timer
        attackTimer -= Time.deltaTime;

        // Check if agent reached stopping distance
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;

            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }

    void Attack()
    {
        Debug.Log("Enemy attacks!");
        healthController.PlayerTakeDamage();
    }
}
