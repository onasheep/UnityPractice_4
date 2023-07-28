using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : LivingEntity, IDamageable
{
    public LayerMask whatIsTarget;

    private LivingEntity targetEntity;
    private NavMeshAgent navMeshAgent;

    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;

    private Animator zombieAnimator;
    private AudioSource zombieAudioPlayer;
    private Renderer zombieRenderer;

    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttackTime;

    private Vector3 tempHitPoint;

    private bool hasTarget
    {
        get
        {
            if (targetEntity && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioPlayer = GetComponent<AudioSource>();

        zombieRenderer = GetComponentInChildren<Renderer>();
    }

    public void SetUp(ZombieData zombieData)
    {
        startingHealth = zombieData.health;
        health = zombieData.health;

        damage = zombieData.damage;

        navMeshAgent.speed = zombieData.speed;
        zombieRenderer.material.color = zombieData.skinColor;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
    {
        zombieAnimator.SetBool("HasTarget", hasTarget);
    }

    private IEnumerator UpdatePath()
    {
        while(dead == false)
        {
            if(hasTarget)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                navMeshAgent.isStopped = true;

                Collider[] colliders = 
                    Physics.OverlapSphere(transform.position, 20f, whatIsTarget);

                for(int i =0; i <colliders.Length; i++)
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();

                    if(livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity;

                        break;
                    }       // loop : whatIsTarget 레이어를 지닌 주변 콜라이더를 모두 순회하는 루프
                }

            }
            yield return new WaitForSeconds(0.25f);
        }       // loop : 좀비가 살아 있는 동안 반복하는 루프

    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(!dead)
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            zombieAudioPlayer.PlayOneShot(hitSound);
        }

        base.OnDamage(damage, hitPoint, hitNormal);
    }

    public override void Die()
    {
        base.Die();

        Collider[] zombieColliders = GetComponents<Collider>();
        for (int i = 0; i < zombieColliders.Length; i++)
        {
            zombieColliders[i].enabled = false;
        }
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;

        zombieAnimator.SetTrigger("Die");
        zombieAudioPlayer.PlayOneShot(deathSound);
    }

    private void OnTriggerStay(Collider other)
    {
        if(!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            LivingEntity attackTarget =
                other.GetComponent<LivingEntity>();

            if(attackTarget != null & attackTarget == targetEntity)
            {
                lastAttackTime = Time.time;


                // 프레임 당 계속 돌아가는 Stay, Update, Coroutine 등에서 임시 변수를 계속 생성하는 경우
                // 상위에서 temp로 사용하는 변수를 만들고 값만 바꿔 주는게 더 메모리 관리가 좋다.
                // 임시변수가 스코프로 들어 올때마다 생성되고 사라지고가 반복되기 때문

                //Vector3 hitPoint =
                //    other.ClosestPoint(transform.position);
                tempHitPoint = other.ClosestPoint(transform.position);

                Vector3 hitNormal =
                    transform.position - other.transform.position;

                attackTarget.OnDamage(damage, tempHitPoint, hitNormal);
            }
        }
    }
}
