using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nopact.Commons.Pool
{
    public interface IPoolable
    {
        GameObject UGameObject { get; }
        Transform UTransform { get; }
        bool IsActive { get; }        
        void OnPoolEnable();
        void OnPoolDisable();
        void OnPoolReset();
        void OnWillBeKilled();
    }
}
