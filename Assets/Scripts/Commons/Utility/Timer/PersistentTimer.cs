using System;
using System.Collections;
using System.Collections.Generic;
using nopact.Commons.Domain.Enum;
using UnityEngine;

namespace nopact.Commons.Utility.Timer
{
    [Serializable]
    public class PersistentTimer :CountdownTimer
    {
        [NonSerialized] private Action<string, float> secondCallback;
        [NonSerialized] private Action<string> onFaulplayDetected;
        [NonSerialized] private Action<PersistentTimer> onSave;
        private SerializableDateTime start, pause;
        private int remainingWholeSecs;

        public void SetConditions( bool isRestoring = false )
        {
            if( !isRestoring )
            { 
                start =  new SerializableDateTime(DateTime.UtcNow);
                return;
            }
            
            if ( pause.Value.CompareTo( DateTime.UtcNow ) > 1 ) {

                Obsolete = true;
                if ( onFaulplayDetected != null )
                {
                    onFaulplayDetected( Id );
                }
            }

            TimeSpan timespent = DateTime.UtcNow - pause.Value;
            Remaining -= (float)timespent.TotalSeconds;
            remainingWholeSecs = Mathf.FloorToInt( Remaining );

        }

        public void Save()
        {
            pause = new SerializableDateTime(DateTime.UtcNow);
            if( onSave != null )
            {
                onSave( this );
            }
        }

        public void SetWholeSecs( int wholeSecs )
        {
            remainingWholeSecs = wholeSecs;
        }

        public PersistentTimer( string id, float duration, CountdownScope scope, bool obsolete, Action<string> callback, Action<string, float> percentCallback , Action<string, float> secondCallback, Action<string> onFaulplayDetection ) : base( id, duration, scope, obsolete, callback, percentCallback )
        {
            onFaulplayDetected = onFaulplayDetection;
            remainingWholeSecs = Mathf.CeilToInt( duration );
            this.secondCallback = secondCallback;
        }

       public string Reconnect( Action<string> callback, Action<string, float> percentCallback, Action<string, float> secondCallback, Action<string> onFaulplayDetection )
        {
            this.callback = callback;
            this.percentCallback = percentCallback;
            this.secondCallback = secondCallback;
            this.onFaulplayDetected = onFaulplayDetection;

            return id;
        }

        public void SetOnSave ( Action<PersistentTimer> onSave )
        {
            this.onSave = onSave;
        }
        public int RemainingWholeSecs
        {
            get { return remainingWholeSecs; }
        }

        public Action<string, float> SecondCallback
        {
            get
            {
                return secondCallback;
            }
        }

        public SerializableDateTime Start
        {
            get
            {
                return start;
            }
        }
    }

}
