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
        private const float G = -85f;
        private const float  COLLISION_ELASTICITY =0f;
        private const float  AIR_DRAG = 0.08f;
        private GameObject uObject;
        private Transform xform;
        private new MeshRenderer renderer;
        private bool isInitialized, forcedDrop;
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
            forcedDrop = false;
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
            if (BlockType == Type.N)
            {
                renderer.enabled = false;
            }
            StartCoroutine(WaitForUpdate(breaking, destructionType));
        }

        private IEnumerator WaitForUpdate( BreakingBlock.BreakingBlock breaking, DestructionType destructionType)
        {
            yield return new WaitForEndOfFrame();
            HandleKill( breaking, destructionType );
        }

        private void HandleKill( BreakingBlock.BreakingBlock breaking, DestructionType destructionType)
        {
            if (breaking != null )
            {
                if (BlockType == Type.N)
                {
                    var yRotation = BlockType == Type.D  ? 0 : 45;
                    var scale = BlockType == Type.D ? 0.65f : 1;
                    var totalRot = Quaternion.Euler(-90, yRotation, 0);
                    breaking.transform.rotation = totalRot;
                    breaking.transform.localScale = new Vector3(scale, scale, 1.0f);
                    breaking.SetMaterial(properties.GetBlockMaterial(BlockType));
                    breaking.Explode( xform.position, destructionType );
                    breakingBlock = breaking;            
                    StartCoroutine(StartDestroyTimer());
                    return;
                }
            }
            
            forcedDrop = true;
            StartCoroutine(StartDestroyTimer());
        }

        private IEnumerator StartDestroyTimer()
        {
            yield return new WaitForSeconds(1);
            renderer.enabled = false;
            yield return new WaitForSeconds(3);
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

        private void FixedUpdate()
        {
            if (!forcedDrop)
            {
                return;
            }
            Drop();
            SetPos();
        }

        #endregion
        
        private void SetProperties()
        {
            var yRotation = BlockType == Type.D  ? 0 : 45;
            var totalRot = Quaternion.Euler(-90, yRotation, 0);
            var scale = BlockType == Type.D ? 0.67f : 1;
            xform.rotation = totalRot;
            xform.localScale = new Vector3(scale, scale, 1.0f);
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