// Small, focused interface (Interface Segregation Principle) - anything that
// can be hurt by an explosion implements this. TNT never needs to know about
// Ant, Dessert, Player, etc. individually - just this one contract.
public interface IDamageable
{
    void TakeDamage(float amount);
}
