using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace nopact.Game.Camera
{
    public class CameraShaker :MonoBehaviour, ICameraShaker
    {
        [SerializeField] protected Transform uTransform;

        [SerializeField] protected float shakeDuration;
        [SerializeField] protected Vector3 shakeStrength;
        [Range(0, 50)]
        [SerializeField] protected int shakeVibrato;        
        [Range( 0, 100 )]
        [SerializeField] protected float shakeRandomness;
        [SerializeField] protected Ease shakeEase;
        [SerializeField] protected bool willShakeFade;
        [SerializeField] protected bool willSnapToIntegers;

        private Tweener shakeTween;

        public void ShakeIt()
        {
            if( shakeTween != null )
            {
                shakeTween.Complete();
            }
            shakeTween = uTransform.DOShakePosition ( shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, fadeOut: willShakeFade, snapping:willSnapToIntegers );
            shakeTween.SetEase( shakeEase );
            shakeTween.OnComplete( () => OnCompleteTween() );
        }

        private void OnCompleteTween()
        {
            shakeTween = null;
        }
    }

}
