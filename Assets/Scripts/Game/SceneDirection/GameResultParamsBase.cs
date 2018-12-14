namespace nopact.Game.PlaySession
{
    public class GameResultParamsBase
    {
        public GameResultParamsBase(bool hasWon)
        {
            HasWon = hasWon;
        }
        public bool HasWon { get; private set; }
    }
}