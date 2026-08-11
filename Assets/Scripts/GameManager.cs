using UnityEngine;



// aya: so what shoild this class handle?
// the wave start logic & upgrade (i guess most upgrade logic will be here to update diffculity numbers & other) & scene flow during that
// also the win condition at the very end when all ants die (show the total xp gained & show what killer level did player kill all ants at)

// about the levels thing? this is just an idea and its to make each killer level require a number of xp (e.g. lvl1 takes 8xp & lvl2 needs 20 & so on), i just think its pretty
// cool to add and all of us challenge our selves to kill all ants at the lowest killer level :q

public class GameManager : MonoBehaviour
{
    public GameManager instance { set; get; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
