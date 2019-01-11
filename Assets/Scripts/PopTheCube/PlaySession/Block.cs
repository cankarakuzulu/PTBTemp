using System.Collections;
using nopact.PopTheCube.PlaySession.BreakingBlock;
using UnityEngine;

namespace nopact.PopTheCube.PlaySession
{
    public class Block : MonoBehaviour
    {
        private const float G = -23.43f;
        private const float  COLLISION_ELASTICITY =0.2f;
        private const float  AIR_DRAG = 0.0f;
        private GameObject uObject;
        private Transform xform;
        private MeshRenderer renderer;
        private bool isInitialized;
        private BlockProperties properties;
        private BreakingBlock.BreakingBlock breakingBlock;
        #region PublicAPI
        
        public void Initialize( Type blockType, float yPosition, BlockProperties properties )
        {
            this.properties = properties;
            BlockType = blockType;
            YPosition = yPosition;
            SetProperties();
            SetPos();
            isInitialized = true;
        }

        public void Collide(Block  other)
        {
            float resultantTotalVelocityMagnitude = Mathf.Abs (other.Velocity) +Mathf.Abs ( Velocity);
            other.Velocity += -Mathf.Sign(other.Velocity) * resultantTotalVelocityMagnitude *0.5f * ( COLLISION_ELASTICITY + 1);
            Velocity += -Mathf.Sign(Velocity) * resultantTotalVelocityMagnitude*0.5f* ( COLLISION_ELASTICITY + 1);
        }

        public void PhysicsUpdate()
        {
            if (YPosition == Target || YPosition == 0 || !isInitialized)
            {
                return;
            }
            
            SetPos();
            ConstraintPos();
            Drop();
        }

        public void Kill(BreakingBlock.BreakingBlock breaking, DestructionType destructionType)
        {
            renderer.enabled = false;
            isInitialized = false;
            
            StartCoroutine(StartDestroyTimer());
            breakingBlock = breaking;
            if (breakingBlock == null)
            {
                return;
            }
            
            breakingBlock.SetMaterial(properties.GetBlockMaterial(BlockType));
            breakingBlock.Explode( xform.position, destructionType );
        }

        private IEnumerator StartDestroyTimer()
        {
            yield return new WaitForSeconds(6);
            if (breakingBlock != null)
            {
                breakingBlock.Release();    
            }
            breakingBlock = null;
            renderer.enabled = false;
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

        private void ConstraintPos()
        {
            if (YPosition < 0)
            {
                YPosition = 0;
                Velocity = 0;
                IsAtRest = true;
                return;
            }

            if (YPosition < Target)
            {
                Velocity = 0;
                IsAtRest = true;
            }

            if (Velocity != 0)
            {
                IsAtRest = false;
            }
        }
        
        private void SetPos()
        {
            xform.position = new Vector3()
            {
                x=xform.position.x,
                y = YPosition,
                z= xform.position.z
            };
        }
        

        public GameObject UObject => uObject;
        public float Velocity { get; set; }
        public float YPosition { get; set; }
        public float Target { get;  set; }
        public Type BlockType { get; private set; }
        public bool IsAtRest { get; private set; }
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