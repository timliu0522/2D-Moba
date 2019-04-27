using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionController : CharactersController
{
    // 1. Basic parameters
    protected float aggro = 0f;
    //--------------------------------

    // 2. Interaction Fields
    public Transform[] iconMap; // These targets indicate the path for minions
    protected List<MinionController> allyMinions = new List<MinionController>();
    //--------------------------------

    // 4. Helper Fields
    private Vector3 dirNormalized; // Indicate the direction for the minion on each level
    private int curTarget = 0;
    private bool lastTarget = false; // Mark if the current target is the last target
    
    protected override void Start()
    {
        base.Start();

        // Set the direction
        dirNormalized = new Vector3(iconMap[curTarget].transform.position.x - 
            transform.position.x, 0f, 0f).normalized;
    }

    protected override void Update()
    {
        base.Update();

        allyMinions = new List<MinionController>(); // empty the list
        // Update the list of ally minions since minions are getting spawned continuously 
        MinionController[] allies = FindObjectsOfType<MinionController>();
        foreach (MinionController ally in allies)
        {
            if (!this.Equals(ally) && ally.isWhite == this.isWhite)
            {
                allyMinions.Add(ally);
            }
        }

        // Set the timer
        attackTimer += Time.deltaTime;
        if (attackTimer > attackSpeed)
        {
            BasicDamage();
        }

        // Detect if the current target is the last one
        if (Vector3.Distance(transform.position, iconMap[iconMap.Length - 1].transform.position) < 0.1f)
        {
            lastTarget = true;
        }

        if (!lastTarget)
        {
            if (onGround && !isAttacking)
            {
                Run();
            }
        }

        if (curHealth <= 0f)
        {
            Die();
        }
    }

    protected override void Die()
    {
        base.Die();

        Destroy(this.healthBar.gameObject);
        Destroy(this.gameObject);
    }

    protected override void Run()
    {
        for (int curAlly = 0; curAlly < allyMinions.Count; curAlly++)
        {
            float dis = Vector3.Distance(transform.position, allyMinions[curAlly].transform.position);
            bool isInTheWay = (dirNormalized.x *
                (allyMinions[curAlly].transform.position.x - transform.position.x)) > 0;

            if (dis < 0.5f && isInTheWay)
            {
                return;
            }
        }

        // Face the direction it is going
        if (dirNormalized.x * transform.localScale.x < 0f)
        {
            Vector3 tempScale = new Vector3(-transform.localScale.x,
                                transform.localScale.y, transform.localScale.z);
            transform.localScale = tempScale;
        }

        transform.position += dirNormalized * movementSpeed * Time.deltaTime;
    }

    protected override void BasicDamage()
    {
        //Debug.Log(attackTimer);

        bool attacked = false;
        if (!attacked)
        {
            for (int curITarget = 0; curITarget < inactiveTargets.Count; curITarget++)
            {
                if (inactiveTargets[curITarget] != null)
                {
                    float dis = Mathf.Abs(inactiveTargets[curITarget].transform.position.x -
                        transform.position.x);
                    bool faced = (transform.localScale.x *
                        (inactiveTargets[curITarget].transform.position.x - transform.position.x)) > 0;

                    if ((dis < attackScope) && (inactiveTargets[curITarget].curHealth > 0f) && faced)
                    {
                        // Deal with object targets firstly
                        inactiveTargets[curITarget].TakeDamage(this.gameObject, attackDamage);
                        isAttacking = true;
                        attacked = true;
                        break;
                    }
                }
            }
        }

        if (!attacked)
        {
            float minDisMinion = float.PositiveInfinity;
            float minDisChamp = float.PositiveInfinity;
            MinionController targetMinion = null;
            ChampionController targetChamp = null;
            for (int curATarget = 0; curATarget < activeTargets.Count; curATarget++)
            {
                if (activeTargets[curATarget] != null)
                {
                    float dis = Vector3.Distance(transform.position,
                            activeTargets[curATarget].transform.position);
                    bool faced = (transform.localScale.x *
                        (activeTargets[curATarget].transform.position.x - transform.position.x)) > 0;

                    if ((dis < attackScope) && (activeTargets[curATarget].curHealth > 0f))
                    {
                        if (!faced)
                        {
                            Vector3 tempScale = new Vector3(-transform.localScale.x,
                                transform.localScale.y, transform.localScale.z);
                            transform.localScale = tempScale;
                        }

                        if (activeTargets[curATarget] is MinionController)
                        {
                            if (dis < minDisMinion)
                            {
                                minDisMinion = dis;
                                targetMinion = (MinionController) activeTargets[curATarget];
                            }
                        }
                        else if (activeTargets[curATarget] is ChampionController)
                        {
                            if (dis < minDisChamp)
                            {
                                minDisChamp = dis;
                                targetChamp = (ChampionController) activeTargets[curATarget];
                            }
                        }
                    }
                    
                }
            }

            // Deal with minion targets secondly
            if (targetMinion != null)
            {
                targetMinion.TakeDamage(this.gameObject, attackDamage);
                isAttacking = true;
                attacked = true;
            } 
            // Finally deal with champion targets
            else if (targetChamp != null)
            {
                targetChamp.TakeDamage(this.gameObject, attackDamage);
                isAttacking = true;
                attacked = true;
            }
        }

        if (!attacked)
        {
            //Debug.Log("End of attacking");
            isAttacking = false;
        }

        // reset the timer
        attackTimer = 0f;
    }

}
