public class Effect
{
    protected CombatEntity target;
    protected int duration;

    public Effect(CombatEntity pTarget, int pDuration)
    {
        target = pTarget;
        duration = pDuration;
    }

    public virtual void CauseEffect()
    {
        duration--;
        if (duration <= 0)
        {
            target.RemoveEffect(this);
        }
    }
}

public class BleedEffect : Effect
{
    private int bleedDamage;
    
    public BleedEffect(CombatEntity pTarget, int pDuration, int pDBleedDamage) : base(pTarget, pDuration)
    {
        bleedDamage = pDBleedDamage;
    }

    public override void CauseEffect()
    {
        target.TakeDamage(bleedDamage);
        base.CauseEffect();
    }
}