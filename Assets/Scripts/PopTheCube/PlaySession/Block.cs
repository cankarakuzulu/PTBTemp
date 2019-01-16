using System;
using System.Collections;
using System.Diagnostics.Contracts;
using nopact.PopTheCube.PlaySession.BreakingBlock;
using UnityEngine;

namespace nopact.PopTheCube.PlaySession
{
    public class Block : MonoBehaviour
    {
        public static event Action<Block> OnReadyForUse;
        private const float G = -24f;
        private const float  COLLISION_ELASTICITY =0.05f;
        private const float  AIR_DRAG = 0.05f;
        private GameObject uObject;
        private Transform xform;
        private MeshRenderer renderer;
        private bool isInitialized;
        private BlockProperties properties;
        private BreakingBlock.BreakingBlock breakingBlock;
        private float target;
        #region PublicAPI
        
        public void Initialize( Type blockType, float yPosition, BlockProperties properties )
        {
            renderer.enabled = true;
            this.properties = properties;
            BlockType = blockType;
            YPosition = yPosition;
            Velocity = 0;
            IsAtRest = false;
            SetProperties();
            SetPos();
            isInitialized = true;
            IsBeingRemoved = false;
            
        }
        
        public void Resurrect()
        {
            gameObject.SetActive(true);
        }

        public void Collide(Block  other)
        {
            IsAtRest = false;
            float resultantTotalVelocityMagnitude =other.Velocity - Velocity;
            other.Velocity += -Mathf.Sign(other.Velocity) * resultantTotalVelocityMagnitude *0.5f * ( COLLISION_ELASTICITY + 1);
            Velocity += -Mathf.Sign(Velocity) * resultantTotalVelocityMagnitude*0.5f* ( COLLISION_ELASTICITY + 1);
        }

        public void PhysicsUpdate()
        {
            if ( !isInitialized)
            {
                return;
            }
           
            IsAtRest = ConstraintPos();
            
            if (!IsAtRest)
            {
                Drop();    
            }
//            DebugLevel( YPosition, Color.green);
//            DebugLevel( YPosition +1f, Color.red);
//            DebugLevel( YPosition - 1f, Color.cyan);
                
        }
        
        public void SetPos()
        {
            xform.position = new Vector3()
            {
                x=xform.position.x,
                y = YPosition,
                z= xform.position.z
            };
        }
           
        public void Kill(BreakingBlock.BreakingBlock breaking, DestructionType destructionType)
        {
            isInitialized = false;
            renderer.enabled = false;
            StartCoroutine(WaitForUpdate(breaking, destructionType));
        }

        private IEnumerator WaitForUpdate( BreakingBlock.BreakingBlock breaking, DestructionType destructionType)
        {
            yield return new WaitForEndOfFrame();
            if (breaking != null)
            {
                breaking.SetMaterial(properties.GetBlockMaterial(BlockType));
                breaking.Explode( xform.position, destructionType );
                breakingBlock = breaking;            
                StartCoroutine(StartDestroyTimer());
            }
        }

        private IEnumerator StartDestroyTimer()
        {
            yield return new WaitForSeconds(6);
            if (breakingBlock != null)
            {
                breakingBlock.Release();    
            }
            breakingBlock = null;
            gameObject.SetActive(false);
            OnReadyForUse(this);
        }

        #endregion

        #region UnityEvents

        private void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
            xform = GetComponent<Transform>();
            uObject = gameObject;
        }

        
        #endregion
        
        private void SetProperties()
        {
            renderer.material = properties.GetBlockMaterial(BlockType);
        }
        
        private void Drop()
        {
            Velocity += G * Time.deltaTime;
            YPosition += Velocity * Time.deltaTime - Velocity * AIR_DRAG * Time.deltaTime;
        }

        private bool ConstraintPos()
        {
            if (YPosition < 0)
            {
                YPosition = 0;
                Velocity = 0;
                return true;
            }

            if (YPosition <= Target)
            {
                Velocity = 0;
                YPosition = Target;
                return true;
            }

            return false;
        }
        
        
        private void DebugLevel(float pos, Color color)
        {
            Vector3 p1 = new Vector3{ x= -2, y = pos, z = 0 };
            Vector3 p2 = new Vector3{ x= 2, y = pos, z = 0 };
            
            Debug.DrawLine( p1, p2, color );
        }

        public GameObject UObject => uObject;
        public float Velocity { get; set; }
        public float YPosition { get; set; }
        public float Target {
            get { return target; }
            set
            {
                if (value != target)
                {
                    IsAtRest = false;
                }

                target = value;
            } }
        public Type BlockType { get; private set; }
        public bool IsAtRest { get; private set; }
        public bool IsBeingRemoved { get; set; }
        public enum Type
        {
            D,
            N
        }

        public enum DestructionType
        {
            Match,
            Left,
            Right
        }

    }
    
    
}