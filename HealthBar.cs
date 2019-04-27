using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private CharactersController master;
    private ShootingSystem master2;

    public Transform fill;
    public SpriteRenderer fillSR;
    public SpriteRenderer bgSR;

    private Vector3 maxScale;

    private void Start()
    {
        if (GetComponentInParent<CharactersController>() != null)
        {
            master = GetComponentInParent<CharactersController>();
        }
        else if (GetComponentInParent<ShootingSystem>() != null)
        {
            master2 = GetComponentInParent<ShootingSystem>();
        }

        maxScale = fill.localScale;
        SetColor();
    }

    private void Update()
    {
        Recolor();
        UpdateValue();
    }

    private void UpdateValue()
    {
        if (GetComponentInParent<CharactersController>() != null)
        {
            Vector3 tempScale = fill.localScale;
            tempScale.x = maxScale.x * master.curHealth / master.maxHealth;
            fill.localScale = tempScale;
        }
        else if (GetComponentInParent<ShootingSystem>() != null)
        {
            Vector3 tempScale = fill.localScale;
            tempScale.x = maxScale.x * master2.curHealth / master2.maxHealth;
            fill.localScale = tempScale;
        }
    }

    private void SetColor()
    {
        if (GetComponentInParent<CharactersController>() != null)
        {
            CharactersController playerColor = GetComponentInParent<CharactersController>();
            if (playerColor.isWhite)
            {
                bgSR.color = Color.blue;
            }
            else
            {
                bgSR.color = Color.black;
            }
        }
        else if (GetComponentInParent<ShootingSystem>() != null)
        {
            ShootingSystem objColor = GetComponentInParent<ShootingSystem>();
            if (objColor.isWhite)
            {
                bgSR.color = Color.blue;
            }
            else
            {
                bgSR.color = Color.black;
            }
        }
    }

    private void Recolor()
    {
        // recolor the health bar based on the current health of the parent
        if (fill.localScale.x < 0.25f * maxScale.x)
        {
            fillSR.color = Color.red;
        }
        else if (fill.localScale.x < 0.5f * maxScale.x)
        {
            fillSR.color = Color.yellow;
        }
        else
        {
            // Initial colors for parents
            if (GetComponentInParent<CharactersController>() != null)
            {
                CharactersController playerColor = GetComponentInParent<CharactersController>();
                if (playerColor.isWhite)
                {
                    fillSR.color = Color.white;
                }
                else
                {
                    fillSR.color = Color.grey;
                }
            }
            else if (GetComponentInParent<ShootingSystem>() != null)
            {
                ShootingSystem objColor = GetComponentInParent<ShootingSystem>();
                if (objColor.isWhite)
                {
                    fillSR.color = Color.white;
                }
                else
                {
                    fillSR.color = Color.grey;
                }
            }
        }
    }

}
