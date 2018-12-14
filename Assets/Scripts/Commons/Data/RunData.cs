using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nopact.Commons.Data
{
    [Serializable]
    public abstract class RunData
    {           
        private double elapsedSeconds = 0.0;            
        private bool isRunning;
        private DateTime? startTime;
         
        public virtual void Start()
        {
            startTime = DateTime.Now;
            isRunning = true;
        }
        public virtual void Stop()
        {
            if ( !isRunning )
            {
                return;
            }
            elapsedSeconds += ( DateTime.Now - startTime.Value ).TotalSeconds;
            startTime = null;
            isRunning = false;
        }

        public RunData()
        {

        }

        public double ElapsedSeconds
            {
                get
                {
                    return isRunning ? ( DateTime.Now - startTime.Value ).TotalSeconds + elapsedSeconds : elapsedSeconds;
                }
            }
        }
    }


