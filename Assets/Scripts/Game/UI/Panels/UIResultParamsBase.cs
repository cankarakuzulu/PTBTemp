namespace nopact.Game.UI
{
    public class UIResultParamsBase
    {
        public UIResultParamsBase(string levelString, string resultString, string highscoreString, string scoreString, bool isSuccessful, bool hasHighscorebreached)
        {
            LevelString = levelString;
            ResultString = resultString;
            HighscoreString = highscoreString;
            ScoreString = scoreString;
            IsSuccessful = isSuccessful;
            HasHighscorebreached = hasHighscorebreached;
        }

        public string LevelString { get; private set; }
        public string ResultString { get; private set; }
        public string HighscoreString { get; private set; }
        public string ScoreString { get; private set; }
        public bool IsSuccessful { get; private set; }
        public bool HasHighscorebreached { get; private set; }
    }    
}
