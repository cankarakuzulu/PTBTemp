using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nopact.Commons.Persistence;

namespace nopact.Game.Persistence
{
    [Serializable]
    public class GenericPersistentData : PersistentDataBase
    {
        private const string PROGRESSION_KEY = "Progression";
        private const string LEVELSUCCESS_KEY = "LevelSuccess";

        [NonSerialized]
        private GameProgressionData progression;
        [NonSerialized]
        private LevelSuccessData successRecord;

        public override void DataInit()
        {
            data = new Dictionary<string, IDataHolder>();

            var progression = new GameProgressionData();
            progression.Initialize();
            data.Add( PROGRESSION_KEY, progression );

            var successRecord = new LevelSuccessData();
            successRecord.Initialize();
            data.Add( LEVELSUCCESS_KEY, successRecord );
        }

        public override void DataUpdate()
        {            
            if ( data == null )
            {
                data = new Dictionary<string, IDataHolder>();
            }
            progression = GetDataHolder<GameProgressionData>( PROGRESSION_KEY );
            successRecord = GetDataHolder<LevelSuccessData>( LEVELSUCCESS_KEY );
        }

        public override void SetVersion( string version )
        {
            Version = version;
        }

        public LevelSuccessData LevelSuccessRecord { get { return successRecord; } }
  
        public GameProgressionData Progression
        {
            get
            {
                return progression;
            }
        }        
    }
}
