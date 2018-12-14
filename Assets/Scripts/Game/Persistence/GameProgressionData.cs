using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nopact.Commons.Persistence;

namespace nopact.Game.Persistence
{
    [Serializable]
    public class GameProgressionData :IDataHolder
    {
        public void Initialize()
        {
            CurrentLevel = 1;
            MaxUnlockedLevel = 1;
        }

        public uint MaxUnlockedLevel { get; set; }
        public uint CurrentLevel { get; set; }
    }
}
