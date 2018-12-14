using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace nopact.Commons.UI.PanelWorks
{
    [Serializable]
    public abstract class TweenWrapperBase<T> where T : class
    {
        public string name;
        public Type type;        
        public float duration;
        public bool isOnlySettingFromState;
        public bool isUsingFrom;
        public float floatFrom, floatTarget;       
        public RectTransform rectTransform;
        public Transform transform;
        public Vector3 vectorFrom, vectorTarget;
        public Image image;
        public Color colorFrom, colorTarget;
        public GameObject setActiveGameObject;
        public bool setActiveState;
        public string id;
        public bool isUsingEaseCurve;
        public AnimationCurve easeCurve;
        
        protected string timerID;
        public abstract T Render( Action onDone );
        public abstract void Play();
        public abstract void Pause();
        public abstract void ForceComplete();
        public abstract void Stop();

        public enum Type
        {
            Wait,
            AnchorPos,
            AnchorPosX,
            AnchorPosY,            
            AnchorMin,
            AnchorMax,
            SizeDelta,
            Rotation,
            LocalRotation,
            Fade,
            Color3,
            Color4,
            SetActive,            
            AnchorMinX,
            AnchorMinY,
            AnchorMaxX,
            AnchorMaxY,
            PlaySound,
            Color4f,
            Color3f,
            UniformScale,
            NonUniformScale,
            Position,
            LocalPosition
        }
    }
}
