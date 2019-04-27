using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionController : CharactersController
{
    // 1. Basic parameters
    public int level = 1;
    public int experience = 0;
    private readonly int[] upgradeThreshold = 
        {50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 550, 600};

    public int money = 0;

    public float jumpForce = 0.1f;
        private int jumpCount = 0;
    public float criticalRate = 0.25f;

    public int killCount = 0;
    public int deathCount = 0;
    public int assistCount = 0;

    public float respawnRate = 10f;
        private float respawnTimer;
    private bool isDead = false;

    public static readonly string[][] CHAMPIONS 
        = new string[][] { new string[] {"Suoli"}, new string[] {"Langzi"} };
    public int champGenre = 0;
    public string champName = null;
    //--------------------------------

    // 2. Interaction Fields
    private GameObject levelNumber;
    public Sprite[] numbers;
    public NexusController home;

    private GameObject KDAPanel;
    private GameObject respawnTimerObj;

    protected override void Start()
    {
        base.Start();
        levelNumber = healthBar.transform.GetChild(2).gameObject;
        KDAPanel = GameObject.Find("KDA");
        respawnTimerObj = GameObject.Find("RespawnTimer");
        respawnTimerObj.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        // Set the timer
        attackTimer += Time.deltaTime;
        // To be implemented with buttons
        if (Input.GetKeyDown(KeyCode.J) && attackTimer > attackSpeed)
        {
            BasicDamage();
        }

        if (Input.GetKeyUp(KeyCode.J))
        {
            anim.SetBool("isAttacking", false);
        }

        // Update the level
        if (level < 13 && experience > upgradeThreshold[level - 1])
        {
            experience -= upgradeThreshold[level - 1];
            level++;
            levelNumber.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = 
                numbers[level - 1];
        }

        if (curHealth <= 0f && !isDead)
        {
            Die();
        }
        else if (isDead)
        {
            Respawn();
        }

        // Update the KDA panel
        KDAPanel.transform.GetChild(0).gameObject.GetComponent<Text>().text = "K: " + killCount;
        KDAPanel.transform.GetChild(1).gameObject.GetComponent<Text>().text = "D: " + deathCount;
        KDAPanel.transform.GetChild(2).gameObject.GetComponent<Text>().text = "A: " + assistCount;
    }

    private void FixedUpdate()
    {
        if (onGround)
        {
            Run();
            Jump();
        }
        else
        {
            SecondJump();
        }
    }

    protected override void Die()
    {
        base.Die();
        isDead = true;
        deathCount++;
        GetComponent<SpriteRenderer>().enabled = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Respawn()
    {
        respawnTimer += Time.deltaTime;
        respawnTimerObj.SetActive(true);
        Text respawnText = respawnTimerObj.transform.GetChild(0).gameObject.GetComponent<Text>();
        respawnText.text = "Respawn in " + (int)(respawnRate - respawnTimer);

        if (respawnTimer >= respawnRate)
        {
            isDead = false;
            curHealth = maxHealth;
            transform.position = home.spawnPoint.position;
            healthBar.transform.GetChild(0).localScale =
                healthBar.transform.GetChild(1).localScale;
            respawnTimerObj.SetActive(false);
            GetComponent<SpriteRenderer>().enabled = true;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            respawnTimer = 0f;
        }
    }

    // To be implemented with the joystick and buttons
    protected override void Run()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        if (moveHorizontal > 0)
        {
            if (transform.localScale.x < 0)
            {
                // change the facing direction of the champion
                Vector3 tempScale = transform.localScale;
                tempScale.x = -transform.localScale.x;
                transform.localScale = tempScale;
                // keep the direction and position of the health bar
                Vector3 tempBarScale = healthBar.transform.localScale;
                tempBarScale.x = -tempBarScale.x;
                healthBar.transform.localScale = tempBarScale;
                Vector3 tempBarPos = healthBar.transform.localPosition;
                tempBarPos.x = -tempBarPos.x;
                healthBar.transform.localPosition = tempBarPos;
            }  
        }
        else if (moveHorizontal < 0)
        {
            if (transform.localScale.x > 0)
            {
                Vector3 tempScale = transform.localScale;
                tempScale.x = -transform.localScale.x;
                transform.localScale = tempScale;

                Vector3 tempBarScale = healthBar.transform.localScale;
                tempBarScale.x = -tempBarScale.x;
                healthBar.transform.localScale = tempBarScale;
                Vector3 tempBarPos = healthBar.transform.localPosition;
                tempBarPos.x = -tempBarPos.x;
                healthBar.transform.localPosition = tempBarPos;
            }
        }

        rgdbody.AddForce(new Vector2(moveHorizontal * movementSpeed, 0f), ForceMode2D.Force);  
    }

    private void Jump() {
        // Detect the jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpCount == 0)
            {
                rgdbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                jumpCount++;
            }
        }
    }

    private void SecondJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpCount == 1)
            {
                rgdbody.AddForce(new Vector2(0, 0.8F * jumpForce), ForceMode2D.Impulse);
                jumpCount++;
            }
        }
    }

    protected override void BasicDamage()
    {
        float originalAttackDamage = attackDamage;
        if (Random.value < criticalRate) // 0.0 <= value <= 1.0
        {
            attackDamage *= 1.5f; // a critical strike
            Debug.Log("Critical!");
        }

        if (anim != null)
        {
            anim.SetBool("isAttacking", true);
        }

        bool attacked = false;

        if (!attacked)
        {
            for (int curATarget = 0; curATarget < activeTargets.Count; curATarget++)
            {
                if (activeTargets[curATarget] != null)
                {
                    float dis = Vector3.Distance(transform.position,
                            activeTargets[curATarget].transform.position);
                    bool faced = (transform.localScale.x *
                        (activeTargets[curATarget].transform.position.x - transform.position.x)) > 0;

                    if ((dis < attackScope) && (activeTargets[curATarget].curHealth > 0f) && faced)
                    {
                        //Debug.Log("distance: " + dis);
                        //Debug.Log("curTarget: " + curATarget);
                        activeTargets[curATarget].TakeDamage(this.gameObject, attackDamage);
                        isAttacking = true;
                        attacked = true;
                        //Debug.Log("attacked");
                    }
                }
            }
        }

        if (!attacked)
        {
            for (int curITarget = 0; curITarget < inactiveTargets.Count; curITarget++)
            {
                if (inactiveTargets[curITarget] != null)
                {
                    float dis = Vector3.Distance(transform.position,
                            inactiveTargets[curITarget].transform.position);
                    float dx = inactiveTargets[curITarget].transform.position.x - transform.position.x;
                    // hard code segment
                    bool faced = false;
                    if (Mathf.Abs(dx) < 0.5f)
                    {
                        faced = true;
                    }
                    else
                    {
                        faced = (transform.localScale.x * dx) > 0;
                    }
                    //-----------------------------------------------------------

                    if ((dis < attackScope) && (inactiveTargets[curITarget].curHealth > 0f) && faced)
                    {
                        inactiveTargets[curITarget].TakeDamage(this.gameObject, attackDamage);
                        isAttacking = true;
                        attacked = true;
                    }
                }
            }
        }

        if (!attacked)
        {
            //Debug.Log("End of attacking");
            isAttacking = false;
        }

        // reset the timer
        attackTimer = 0f;
        attackDamage = originalAttackDamage; // reset the attack damage
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("ground"))
        {
            onGround = true;
            jumpCount = 0;
            //Debug.Log("on ground");
        }
    }
}
