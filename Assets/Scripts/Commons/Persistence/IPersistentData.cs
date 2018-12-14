using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nopact.Commons.Persistence
{
    public interface IPersistentData
    {
        string Version { get; }

        void DataInit();
        void DataUpdate();
        void SetVersion( string version );        
    }
}
