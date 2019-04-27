using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactersController : MonoBehaviour
{
    // 1. Basic parameters
    public float maxHealth = 100f;
    public float curHealth;
    public float attackDamage = 10f;
    public float armor = 10f;

    public float attackSpeed = 2f;
        // Parameters used to make the player attack one single target at a fixed rate
        protected float attackTimer = 0f;

    public float attackScope = 3f;
    public float cooldownReduction = 0f;
    public float movementSpeed = 0.3f;

    public int killBounty = 10;
    public int killExp = 10;

    public bool isWhite = true;
    public bool isMonster = false;

    public Transform spawningPoint;

    public string[] Buff = null;
    //--------------------------------

    // 2. Interaction fields
    public GameObject healthBar;

    // Two lists storing object targets (turrets and nexuses) and enemy targets (minions and champions)
    protected List<ShootingSystem> inactiveTargets = new List<ShootingSystem>();
    protected List<CharactersController> activeTargets = new List<CharactersController>();

    public GameObject attacker;
    public GameObject killerObj;

    protected Rigidbody2D rgdbody;
    protected Animator anim;
    //--------------------------------

    // 3. Status Indicators
    protected bool onGround = false;
    protected bool isAttacking = false;
    protected bool isStunned = false;
    protected bool isIgnited = false;
    //--------------------------------
    
    protected virtual void Start()
    {
        // 1
        curHealth = maxHealth;

        // Add enemy objects into the list
        ShootingSystem[] objects = FindObjectsOfType<ShootingSystem>();
        foreach (ShootingSystem obj in objects)
        {
            if (obj.isWhite != this.isWhite) // only enemies
            {
                inactiveTargets.Add(obj);
            }
        }

        rgdbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        activeTargets = new List<CharactersController>(); // empty the list
        // Update the list of enemies since minions are getting spawned continuously 
        CharactersController[] enemies = FindObjectsOfType<CharactersController>();
        foreach (CharactersController enemy in enemies)
        {
            if (enemy.isMonster)
            {
                activeTargets.Add(enemy); // add monsters to the list first
            }
            else if (enemy.isWhite != this.isWhite)
            {
                activeTargets.Add(enemy);
            }
        }

        // 3
        ChangeBounty();
        UpdateStatus();
    }

    protected virtual void Die()
    {
        // Find the killer
        killerObj = attacker;
        ChampionController killer = null;
        if (killerObj != null)
        {
            killer = killerObj.GetComponent<ChampionController>();
        }

        if (killer != null)
        {
            killer.experience += this.killExp;
            killer.money += this.killBounty;
        }
    }

    protected virtual void Run() {}

    protected virtual void BasicDamage() {}

    protected virtual void SpecialDamage() {}

    public virtual void TakeDamage(GameObject attacker, float damage)
    {
        this.attacker = attacker;
        curHealth -= damage;
    }

    protected virtual void ChangeBounty() {
        killBounty += 100 * (int)(Time.time / 300f);
    }

    protected virtual void UpdateStatus() {
        if (isIgnited)
        {
            curHealth -= 0.05f * maxHealth;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("ground"))
        {
            onGround = true;
            //Debug.Log("on ground");
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("ground"))
        {
            onGround = false;
            //Debug.Log("not on ground");
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
      
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
       
    }
}
