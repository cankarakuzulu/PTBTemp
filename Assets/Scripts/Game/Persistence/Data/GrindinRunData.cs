using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nopact.Commons.Data;

namespace nopact.Game.Persistence.Data
{
    [Serializable]
    public class GrindinRunData : RunData
    {
        public ushort Level { get; set; }
        public uint RemainingLives { get; set; }        
    }
}
