using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 10f;

    private CharactersController m_target;
    private Vector3 m_direction;
    private int damageCount = 0;

    private void Update()
    {
        // move toward the targets
        transform.position = Vector3.MoveTowards(transform.position,
                    m_target.transform.position, speed * Time.deltaTime);

        CauseDamage();
    }

    public void  Fire(GameObject launcher, CharactersController target)
    {
        if (target) m_target = target;   
    }

    private void CauseDamage()
    {
        if (m_target.GetComponent<CharactersController>().curHealth > 0)
        {
            if (Vector3.Distance(transform.position, m_target.transform.position) < 0.1f)
            {
                if (damageCount == 0)
                {
                    // cause damage to the target
                    m_target.GetComponent<CharactersController>().TakeDamage(this.gameObject, damage);
                    Destroy(this.gameObject);
                    damageCount++;
                }
                else
                {
                    damageCount = 0;
                }
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
       
    }
}
