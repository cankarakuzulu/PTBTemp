using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nopact.Commons.SceneDirection
{
    public abstract class GameStatsBase
    {
        
        public GameStatsBase( string levelString )
        {
            this.Score = 0;
            this.LevelString = levelString;
            this.IsSuccessful = null;
        }

        public void IncreaseScore ( uint scoreIncrease )
        {
            Score += scoreIncrease;
        }

        public void SetWin()
        {
            this.IsSuccessful = true;
        }

        public void SetLose()
        {
            this.IsSuccessful = false;
        }

        public uint Score
        {
            get;
            private set;
        }

        public bool? IsSuccessful
        {
            get;
            private set;
        }

        public string LevelString
        {
            get;
            private set;
        }
    }
}
