/*
 *
 *  * Copyright (c) 2013 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using System;
using nopact.Commons.Domain.Enum;

namespace nopact.Commons.Utility.Timer
{
    [Serializable]
    public class CountdownTimer
    {
        protected string id;
        [NonSerialized] protected Action<string> callback;
        [NonSerialized] protected Action<string, float> percentCallback;

        private float duration;
        private float remaining;
        private CountdownScope scope;
        private bool obsolete;
        
        public CountdownTimer(string id, float duration, CountdownScope scope, bool obsolete, Action<string> callback, Action<string, float> percentCallback)
        {

            this.id = id;
            this.duration = duration;
            this.remaining = duration;
            this.scope = scope;
            this.obsolete = obsolete;
            this.callback = callback;
            this.percentCallback = percentCallback;

        }

        public void Reset()
        {
            obsolete = false;
            remaining = duration;
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public float Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                duration = value;
            }
        }

        public float Remaining
        {
            get
            {
                return this.remaining;
            }
            set
            {
                remaining = value;
            }
        }

        public CountdownScope Scope
        {
            get
            {
                return this.scope;
            }
        }

        public bool Obsolete
        {
            get
            {
                return this.obsolete;
            }
            set
            {
                obsolete = value;
            }
        }

        public Action<string> Callback
        {
            get
            {
                return this.callback;
            }
        }

        public Action<string, float> PercentCallback
        {
            get
            {
                return this.percentCallback;
            }
        }


    }
}
