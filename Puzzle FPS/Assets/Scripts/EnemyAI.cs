using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using System.Linq;
using System.Runtime.CompilerServices;

public class EnemyAI : MonoBehaviour, IDamage
{

    private Rigidbody[] _ragdollRigidBodies;
    EnemyAI enemyAI;

    [Header("-----Components-----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    [Header("-----Stats-----")]
    [SerializeField] int hp;
    [SerializeField] int playerFaceSpeed;

    [Header("-----Weapon-----")]
    [SerializeField]
    bool typeSniper;
    [SerializeField]
    bool typeMelee;
    [SerializeField] Transform headPos;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;
    [SerializeField] float bulletSpeed;
    [SerializeField] int shootDist;
    [SerializeField] int viewAngle;


    public float timer;
    float angleToPlayer;
    Vector3 playerDir;
    bool isShooting;
    bool playerInVisionRange;
    bool canDestroyEnemy;

    void Awake()
    {
        _ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }
    //void Start()
    //{
    //}

    // Update is called once per frame
    void Update()
    {
        if (playerInVisionRange && CanSeePlayer() && animator.enabled == true)
        {
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                FacePlayer();
            }
            if (!isShooting)
            {
                StartCoroutine(Shoot());
            }
        }
        CanDestroyEnemy();
    }

    void CanDestroyEnemy()
    {
        if (animator.enabled == false)
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            timer = 10f;
        }
    }

    bool CanSeePlayer()
    {
        playerDir = GameManager.Instance.PlayerController().transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (animator.enabled == false)
            {
                return false;
            }
            else if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.Instance.PlayerController().transform.position);
                return true;
            }
        }
        return false;
    }
    private void LateUpdate()
    {
        UpdateAnimator();
    }
    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        StartCoroutine(FlashDamage());
        if (hp <= 0)
        {
            EnableRagdoll();

            //Destroy(gameObject);
        }
    }

    IEnumerator FlashDamage()
    {
        model.material.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        model.material.color = Color.white;

    }

    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    IEnumerator Shoot()
    {
        animator.SetTrigger("attacking");
        isShooting = true;
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);

        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    private void UpdateAnimator()
    {
        animator.SetBool("playerInVisionRange", playerInVisionRange);
        animator.SetFloat("speed", agent.speed);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInVisionRange = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInVisionRange = false;
        }
    }

    private void DisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidBodies)
        {
            rigidbody.isKinematic = true;
        }
        animator.enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;

    }

    private void EnableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidBodies)
        {
            rigidbody.isKinematic = false;
        }
        animator.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
    }
}
