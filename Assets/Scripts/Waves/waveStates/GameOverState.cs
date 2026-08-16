public class GameOverState : IWaveState
{
    private readonly WaveManager context;


    public GameOverState(WaveManager context)
    {
        this.context = context;
    }


    public void Enter()
    {
        context.BeginGameOver();
    }


    public void Update()
    {
    }


    public void Exit()
    {
    }
}