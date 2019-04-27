using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingSystem : MonoBehaviour
{
    public GameObject projectile;
    public Transform shootingPoint;

    // Basic parameters for a shooting system
    public float fireRate;
    public float fireScope;
    public float maxHealth;
    public float curHealth;
    public bool isWhite;

    public int destroyBounty = 300;
    public int destroyExp = 100;

    public GameObject healthBar;

    public GameObject attacker;
    public GameObject destroyerObj;

    protected float fireTimer = 0f;
    protected CharactersController[] targets;

    protected virtual void Start()
    {
        curHealth = maxHealth;
        Instantiate(healthBar, transform);
    }

    protected virtual void Update()
    {
        // Detect the current health
        if (curHealth <= 0f)
        {
            Die();
        }

        // Update the targets
        targets = FindObjectsOfType<CharactersController>();

        fireTimer += Time.deltaTime;
        if (fireTimer > fireRate)
        {
            CauseDamage();
        }
    }

    protected virtual void Die()
    {
        destroyerObj = attacker;
        ChampionController destroyer = null;
        if (destroyerObj != null)
        {
            destroyer = destroyerObj.GetComponent<ChampionController>();
        }
       
        if (destroyer != null)
        {
            destroyer.experience += this.destroyExp;
            destroyer.money += this.destroyBounty;
        }

        Destroy(this.gameObject);
    }

    protected virtual void CauseDamage()
    {
        for (int curTarget = 0; curTarget < targets.Length; curTarget++)
        {
            if (targets[curTarget] != null)
            {
                float distance = Vector3.Distance(transform.position,
                    targets[curTarget].transform.position);

                // in the scope * on the same level * the target is alive
                if ((distance < fireScope) && (System.Math.Abs(targets[curTarget].transform.position.y
                    - transform.position.y) < 1f) && (targets[curTarget].curHealth > 0)
                    && (targets[curTarget].isWhite != isWhite))
                {
                    GameObject proj = Instantiate(projectile, shootingPoint.position,
                        Quaternion.Euler(shootingPoint.forward)) as GameObject;
                    proj.transform.SetParent(shootingPoint.parent);
                    // call the fire method of projectile
                    proj.GetComponent<Projectile>().Fire(projectile, targets[curTarget]);

                    fireTimer = 0f;
                    break;
                }
            }
        }
    }

    public virtual void TakeDamage(GameObject attacker, float damage)
    {
        this.attacker = attacker;
        curHealth -= damage;
    }
}
