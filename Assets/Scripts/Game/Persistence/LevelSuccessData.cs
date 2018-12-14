using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nopact.Commons.Persistence;

namespace nopact.Game.Persistence
{
    [Serializable]
    public class LevelSuccessData :IDataHolder
    {

        private Dictionary<ushort, uint> levelFailRecord;
        public void Initialize()
        {
            levelFailRecord = new Dictionary<ushort, uint>();
        }

        public void RecordLevelFail ( ushort level )
        {
            if ( !levelFailRecord.ContainsKey(level) )
            {
                levelFailRecord.Add( level, 0 );
            }

            levelFailRecord[ level ]++;
        }

        public uint GetFails( ushort level )
        {
            if ( !levelFailRecord.ContainsKey( level ) )
            {
                return 0;
            }
            return levelFailRecord[ level ];
        }
    }
}
