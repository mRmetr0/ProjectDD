using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CombatEntity : MonoBehaviour
{
    [SerializeField] 
    private int maxHealth;
    private int health;

    private List<Skill> skills;
    private List<Effect> effects;
    

    private int position;

    public List<Skill> Skills => skills;
    
    
    //Publics:
    public int Position
    {
        get { return position; }
        set { position = value; }
    }

    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public virtual void StartTurn()
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            effects[i].CauseEffect();
        }
    }

    public void AddEffect(Effect addedEffect)
    {
        effects.Add(addedEffect);
    }

    public void RemoveEffect(Effect removedEffect)
    {
        if (effects.Contains(removedEffect))
            effects.Remove(removedEffect);
    }

    public void HealDamage(int healValue)
    {
        health = Mathf.Clamp(health + healValue, 0, maxHealth);
    }

    public void TakeDamage(int damageValue)
    {
        health -= damageValue;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{name} died");
    }
}
