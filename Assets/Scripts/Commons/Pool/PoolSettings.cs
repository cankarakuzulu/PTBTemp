using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nopact.Commons.Pool
{
    public class PoolSettings
    {
        public GameObject Prefab { get; set; }
        public ushort Max { get; set; }
        public ushort Min { get; set; }
        public ushort BackoffThreshold { get; set; }
        public BackoffProfiles BackoffProfile { get; set; }
        public float BackoffInitiationDelay { get; set; }
        public float BackoffRefreshRate { get; set; }

        public enum BackoffProfiles
        {
            None,
            VerySlow,
            Slow,
            Linear,
            Normal,
            Fast,
            Quadratic,
            Immediate
        }
    }
}
