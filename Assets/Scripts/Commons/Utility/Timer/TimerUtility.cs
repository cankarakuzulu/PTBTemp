/*
 *
 *  * Copyright (c) 2013 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using nopact.Commons.Domain.Enum;
using nopact.Commons.Utility.Math;
using UnityEngine;

namespace nopact.Commons.Utility.Timer
{
    public class TimerUtility : MonoBehaviour
    {
        public int tickPeriod = 1;

        private float scaledDeltaTime;
        private float realDeltaTime;
        private float realTime;
        private bool levelPaused;
        private bool levelOn;

        private List<CountdownTimer> persisitentTimers;
        private List<CountdownTimer> scaledTimers;
        private List<CountdownTimer> realTimers;
        private List<CountdownTimer> callbacks;

        // Singleton values
        private static volatile TimerUtility instance;
        private static object syncLock = new System.Object();

        private float runDuration;
        private float sessionDuration;
        private float betweenSaveDuration;

        public static TimerUtility Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncLock)
                    {
                        if (instance == null)
                        {
                            instance = UnityEngine.Object.FindObjectOfType(typeof(TimerUtility)) as TimerUtility;
                        }
                    }
                }

                return instance;
            }
        }

        void Awake()
        {
            persisitentTimers = new List<CountdownTimer>();
            scaledTimers = new List<CountdownTimer>();
            realTimers = new List<CountdownTimer>();
            callbacks = new List<CountdownTimer>();

            scaledDeltaTime = 0f;
            realDeltaTime = 0f;
            realTime = Time.realtimeSinceStartup;

            sessionDuration = 0f;
            betweenSaveDuration = 0f;
        }

        void Update()
        {
            // This delta is affected by timescale.
            // If timescale is not equal to 1 then realtimeDelta is not equal to scaledDeltaTime
            scaledDeltaTime = Time.deltaTime;

            realDeltaTime = Time.realtimeSinceStartup - realTime;
            realTime = Time.realtimeSinceStartup;

            if (scaledTimers.Count > 0)
            {
                TicToc(scaledTimers, scaledDeltaTime);
            }

            if (realTimers.Count > 0)
            {
                TicToc(realTimers, realDeltaTime);
            }

            if ( persisitentTimers.Count > 0 )
            {
                TicToc( persisitentTimers, realDeltaTime );
            }

            if (levelOn)
            {
                runDuration += scaledDeltaTime;
            }

            sessionDuration += scaledDeltaTime;
            betweenSaveDuration += scaledDeltaTime;
        }

        private void OnDisable()
        {
            if ( persisitentTimers.Count> 0 )
            {
              
                foreach( var timer in persisitentTimers )
                {

                    PersistentTimer p = timer as PersistentTimer;
                    
                    if ( p!= null )
                    {
                        p.Save();
                    }
                }

            }
        }

        private void TicToc(List<CountdownTimer> timers, float deltaTime)
        {
            // Iterate the list in reverse order so that callback implementations may add new timers.	
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                if (timers[i].Scope != CountdownScope.Level || !levelPaused)
                {
                    timers[i].Remaining -= deltaTime;

                    if (timers[i].PercentCallback != null)
                    {
                        // An arbitrary performance trick
                        if (timers[i].Remaining <= 0 || Time.frameCount % tickPeriod == 0)
                        {
                            timers[i].PercentCallback(timers[i].Id, (timers[i].Duration - MathUtility.Clamp(timers[i].Remaining, 0f, timers[i].Duration)) / timers[i].Duration);
                        }
                    }

                    PersistentTimer p = timers[ i ] as PersistentTimer;
                    if ( p != null && p.SecondCallback != null  )
                    {                  
                        
                        var sec = Mathf.FloorToInt( p.Remaining );
                        sec = sec < 0 ? 0 : sec;                        
                        if ( sec != p.RemainingWholeSecs )
                        {
                            p.SetWholeSecs( sec );
                            p.SecondCallback( p.Id, p.RemainingWholeSecs );
                        }
                    }

                    if (timers[i].Remaining <= 0 && !timers[i].Obsolete)
                    {
                        timers[i].Obsolete = true;

                        callbacks.Add(timers[i]);
                    }
                }
            }

            timers.RemoveAll(item => item.Obsolete == true);

            for (int c = 0; c < callbacks.Count; c++)
            {
                callbacks[c].Callback(callbacks[c].Id);
            }

            callbacks.Clear();
        }
                
      
        public string RegisterTimer(float duration, CountdownScope scope, Action<string> callback, Action<string, float> percentCallback = null)
        {
            CountdownTimer timer = new CountdownTimer(NopactUtility.GenerateId(), duration, scope, false, callback, percentCallback);

            scaledTimers.Add(timer);

            return timer.Id;
        }

        public string RegisterTimer(CountdownTimer timer)
        {
            scaledTimers.Add(timer);

            return timer.Id;
        }

        public string RegisterPersistentTimer( PersistentTimer timer, bool restore = false )
        {
            if ( timer == null )
            {
                return string.Empty;
            }

            timer.Obsolete = false;
            timer.SetConditions( restore );
            persisitentTimers.Add( timer );

            return timer.Id;
        }
        
        // Use this method for timescale-independent timers.
        // e.g. timers that you want to tick even if the game is paused.
        public string RegisterScaleIndependentTimer(float duration, CountdownScope scope, Action<string> callback, Action<string, float> percentCallback = null)
        {
            CountdownTimer timer = new CountdownTimer(NopactUtility.GenerateId(), duration, scope, false, callback, percentCallback);
            realTimers.Add(timer);

            return timer.Id;
        }

        public void CancelTimer(string timer)
        {
            CancelTimer(scaledTimers.FirstOrDefault(t => t.Id == timer));
            CancelTimer(realTimers.FirstOrDefault(t => t.Id == timer));
        }

        private void OnLevelStarted()
        {
            runDuration = 0f;
            levelOn = true;
        }

        private void OnLevelRestarted()
        {
            levelOn = true;
        }

        private void OnLevelFinished(bool win)
        {
            levelOn = false;
            levelPaused = false;
            scaledTimers.RemoveAll(item => item.Scope == CountdownScope.Level);
            realTimers.RemoveAll(item => item.Scope == CountdownScope.Level);
        }

        private void OnLevelPaused(bool paused, bool tutorial)
        {
            levelOn = !paused;
            levelPaused = paused;
        }

        private void CancelTimer(CountdownTimer timer)
        {
            if (timer != null)
            {
                timer.Obsolete = true;
            }
        }

        public float RunDuration
        {
            get
            {
                return runDuration;
            }
        }

        public float SessionDuration
        {
            get
            {
                return this.sessionDuration;
            }
        }

        public float BetweenSaveDuration
        {
            get
            {
                return this.betweenSaveDuration;
            }
            set
            {
                betweenSaveDuration = value;
            }
        }
    }
}
