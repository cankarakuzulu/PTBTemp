using UnityEngine;
using System.Collections;
using System;

namespace nopact.Commons.Utility.Timer
{
    [Serializable]
    public class SerializableDateTime
    {
        private long binaryValue;

        public SerializableDateTime( DateTime value )
        {
            Value = value;
        }

        public DateTime Value
        {
            get
            {
                return DateTime.FromBinary( binaryValue );
            }
            set
            {
                binaryValue = value.ToBinary();
            }
        }
    }
}
