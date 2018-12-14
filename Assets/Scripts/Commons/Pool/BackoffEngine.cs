using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using nopact.Commons.Pool;

namespace nopact.Commons.Pool
{
    public class BackoffEngine :IEnumerator<int>
    {

        private PoolSettings.BackoffProfiles profile;
        private int currentStep = 0;
        private int total;
        private int remaining;
        

        public void Dispose()
        {
            currentStep = 0;
        }

        public bool MoveNext()
        {
            if ( total == 0 )
            {
                return false;
            }
            currentStep++;
            CalculateNext();

            return true;
        }

        private void CalculateNext()
        {
            switch ( profile )
            {
                case PoolSettings.BackoffProfiles.Immediate:
                    Current = total;                    
                    break;
                case PoolSettings.BackoffProfiles.Quadratic:
                    Current = currentStep * currentStep;                    
                    break;

                case PoolSettings.BackoffProfiles.Fast:
                    Current = Mathf.NextPowerOfTwo( currentStep );
                    if ( total - Current < 0 )
                    {
                        Current = total;
                        total = 0;
                    }
                    else
                    {
                        total -= Current;
                    }
                    break;

                case PoolSettings.BackoffProfiles.Normal:
                    Current = currentStep * 2;
                    break;

                case PoolSettings.BackoffProfiles.Linear:
                    Current = currentStep;
                    break;

                case PoolSettings.BackoffProfiles.Slow:
                    Current = 1;
                    break;

                case PoolSettings.BackoffProfiles.VerySlow:
                    var x = Mathf.NextPowerOfTwo( currentStep );
                    if ( currentStep < x )
                    {
                        Current = 0;
                    }
                    else
                    {
                        Current = 1;
                    }
                    break;
            }
            if ( total - Current <= 0 )
            {
                Current = total;
            }
            total -= Current;            
        }

        public void Reset()
        {
            currentStep = 0;
        }

        public void UpdateRemaining( int total )
        {
            this.total = total;
        }

        public BackoffEngine( PoolSettings.BackoffProfiles profile )
        {
            this.profile = profile;            
        }

        public int Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
