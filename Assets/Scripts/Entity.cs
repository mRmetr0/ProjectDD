using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Entity : MonoBehaviour
{
    public enum Modifier
    {
        Bleed,
        Stun,
        Dodge,
        Marks
    }

    [SerializeField] protected int maxHealth;
    [SerializeField] protected int speed;
    [SerializeField] protected Skill[] skills;
    [SerializeField] protected HealthBar healthBar;
    protected Animator Animator;

    protected int Health;
    public int Position { get; set; }
    public int Speed => speed;
    public int hp => Health;

    private int dotDamage = 3;
    private int dotAmount = 0;
    private int dodge = 0;
    private bool stunned = false;
    private int marked = 0;
    
    protected virtual void Awake()
    {
        Health = maxHealth;
        Animator = GetComponentInChildren<Animator>();
        healthBar.UpdateUI(Health, maxHealth, dotAmount>0, dodge>0, stunned, marked > 0);
    }

    public void TakeDamage(int pDamage, int pMove = 0)
    {
        if (dodge > 0)
        {
            dodge--;
        }
        else
        {
            if (marked > 0) pDamage = Mathf.CeilToInt(pDamage * 1.5f);
            Health -= pDamage;
            Move(pMove);
            if (Health <= 0)
                Die();
            else
                Animator.SetTrigger("Damaged");
        }
        healthBar.UpdateUI(Health, maxHealth, dotAmount>0, dodge>0, stunned, marked > 0);
    }

    public void TakeHealing(int pHealing, bool pCleanse)
    {
        if (pCleanse)
        {
            marked = 0;
            dotAmount = 0;
        }
        Health = Mathf.Min(Health + pHealing, maxHealth);
        healthBar.UpdateUI(Health, maxHealth, dotAmount>0, dodge>0, stunned, marked > 0);
        Animator.SetTrigger("Damaged");
    }

    public void GiveMod(Modifier pMod)
    {
        switch (pMod)
        {
            case(Modifier.Bleed):
                dotAmount += 3;
                break;
            case(Modifier.Stun):
                int r = Random.Range(0, 100);
                Debug.Log($"StunChance: {r}");
                if (r > 50)
                    stunned = true;
                break;
            case (Modifier.Dodge):
                dodge++;
                break;
            case (Modifier.Marks):
                marked += 3;
                break;
        }
        healthBar.UpdateUI(Health, maxHealth, dotAmount>0, dodge>0, stunned, marked > 0);
    }

    private void CheckMods()
    {
        marked = Mathf.Max(marked-1, 0);
        if (dotAmount > 0)
        {
            TakeDamage(dotDamage);
            dotAmount--;
        }
        healthBar.UpdateUI(Health, maxHealth, dotAmount>0, dodge>0, stunned, marked > 0);
    }

    private void Die()
    {
        Animator.SetTrigger("Die");
        BattleManager.Instance.RemoveCharacter(this);
    }

    public void TurnStart()
    {
        healthBar.SetBarActive(true);
        CheckMods();
        if (Health <= 0 || stunned)
        {
            stunned = false;
            StartCoroutine(TurnEnd());
            return;
        }
        ActTurn();
    }

    public virtual void ActTurn() { }

    public IEnumerator TurnEnd()
    {
        healthBar.UpdateUI(Health, maxHealth, dotAmount>0, dodge>0, stunned, marked > 0);
        HUDManager.Instance.SetText();
        yield return new WaitForSeconds(1);
        healthBar.SetBarActive(false);
        BattleManager.Instance.NextTurn();
    }

    public void Animate(Skill.Type pType)
    {
        switch (pType)
        {
            case (Skill.Type.Melee1):
                Animator.SetTrigger("Melee1");
                break;
            case (Skill.Type.Melee2):
                Animator.SetTrigger("Melee2");
                break;
            case (Skill.Type.Ranged):
                Animator.SetTrigger("Ranged");
                break;
            case (Skill.Type.Support):
                Animator.SetTrigger("Support");
                break;
        }
    }

    public void Move(int pDirection) //positive is forward, negative is backwards, 0 is no movement;
    {
        if (pDirection == 0) return;
        int newPos = Position - pDirection;
        Entity[] party = (BattleManager.Instance.Heroes.Contains(this) ? BattleManager.Instance.Heroes : BattleManager.Instance.Enemies);
        if (newPos < 0 || newPos > party.Length-1) return;

        int oldPos = Position;
        
        (Position, party[newPos].Position) =
            (party[newPos].Position, oldPos);
        
        StartCoroutine(LerpMove(party[newPos].gameObject.transform, transform));
        
        (party[newPos], party[oldPos]) = (party[oldPos], party[newPos]);
    }

    private IEnumerator LerpMove(Transform Tf1, Transform Tf2)
    {
        Vector3 newPos1 = Tf2.position, newPos2 = Tf1.position;
        float elapsedTime = 0, waitTime = 1.5f;
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            Tf1.position = Vector3.Lerp(Tf1.position, newPos1, (elapsedTime/waitTime));
            Tf2.position = Vector3.Lerp(Tf2.position, newPos2, (elapsedTime/waitTime));
            yield return null;
        }

        Tf1.position = newPos1;
        Tf2.position = newPos2;
        yield return null;
    }
}
