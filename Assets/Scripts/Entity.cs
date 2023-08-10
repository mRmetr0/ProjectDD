using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Entity : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int speed;
    [SerializeField] protected Skill[] skills;
    [SerializeField] protected HealthBar healthBar;
    protected Animator Animator;

    protected int Health;
    public int Position { get; set; }
    public int Speed => speed;
    public int hp => Health;

    public void Awake()
    {
        Health = maxHealth;
        Animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int pDamage, int pMove = 0)
    {
        Health -= pDamage;
        Move(pMove);
        healthBar.UpdateUI(Health, maxHealth);
        if (Health <= 0)
        {
            Die();
            return;
        }
        Animator.SetTrigger("Damaged");

    }

    private void Die()
    {
        Animator.SetTrigger("Die");
        BattleManager.Instance.RemoveCharacter(this);
    }

    public void TurnStart()
    {
        healthBar.SetBarActive(true);
        ActTurn();
    }

    public virtual void ActTurn() { }

    public IEnumerator TurnEnd()
    {
        yield return new WaitForSeconds(1);
        healthBar.SetBarActive(false);
        HUDManager.Instance.SetText();
        BattleManager.Instance.NextTurn();
    }

    public void Animate(Skill.Type pType)
    {
        switch (pType)
        {
            case (Skill.Type.Melee):
                Animator.SetTrigger("Melee");
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
