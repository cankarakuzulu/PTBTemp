using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace nopact.PopTheCube.PlaySession
{
    public class Stack : MonoBehaviour
    {
        public static event Action<Block.Type> OnGenerate;
        public static event Action<int, Block.DestructionType> OnRemoveBlock;
        [SerializeField] protected BlockProperties blockProperties;
        [SerializeField] protected bool isSlave;
        private const float generationHeight = 1f;
        private const float blockHeight = 2.0f;
        
        private List<Block> blockList;
        private BreakingBlock.BreakingBlock[] breakingBlocks;
        private Queue<Block.Type> blockCreationQueue;
        private List<Block> reusableBlocks;
        private bool isLastBlockWasRed;
        
        public void Initialize()
        {
            PrepareForGeneration();
            if (isSlave)
            {
                return;
            }
            OnGenerate?.Invoke(Block.Type.N);
            for (int initialBlockIndex = 0; initialBlockIndex < 99; initialBlockIndex++)
            {
                AddBlockToQueue();
            }
            StartCreationLoop();
        }

        public void Kill()
        {
            
        }

        public void CreateNewBlock( Block.Type blockType )
        {
            Block newBlock;
            if (reusableBlocks != null && reusableBlocks.Count > 0)
            {
                newBlock = reusableBlocks[0];
                reusableBlocks.RemoveAt(0);
                newBlock.Resurrect();
            }
            else
            {
                GameObject blockGO = Instantiate(blockProperties.GetBlockPrefab(blockType));
                blockGO.transform.position = Vector3.up * generationHeight * (1  + blockCreationQueue.Count ) + Vector3.left * transform.position.x;
                newBlock = blockGO.GetComponent<Block>();
           }
            
            newBlock.Initialize( blockType, generationHeight + blockCreationQueue.Count * blockHeight, blockProperties );
            blockList.Add(newBlock);
            SetTargets();
        }
        public bool TryDash(float yPos, out Block block)
        {
            block = null;
            if (blockList == null || blockList.Count == 0)
            {
                return false;
            }

            var bladeRect = new Rect( -2.5f, yPos-0.2f, 5,0.4f);
            DebugRect(bladeRect, Color.yellow, 0.5f);
         
            for (int blockIndex = blockList.Count-1; blockIndex >= 0; blockIndex--)
            {
                var current = blockList[blockIndex];
                
                var hitbox = new Rect( -1.5f, current.YPosition-blockHeight*0.5f, 3, blockHeight );
                
                if (hitbox.Overlaps(bladeRect))
                {
                    DebugRect(hitbox, Color.blue, 0.5f);
                    block = current;
                    if (current.BlockType == Block.Type.N)
                    {
                        return true;
                    }

                    if (hitbox.Contains(new Vector3(0, yPos, 0)))
                    {
                        return false;    
                    }
                }
            }
            return false;
        }

        public void Break(Block block, Block.DestructionType destructionType)
        {
            OnRemoveBlock?.Invoke(blockList.IndexOf(block),destructionType);
        }

        private void AddBlockToQueue()
        {
            if (!isSlave)
            {
                var blockType = Random.value > 0.8f ? Block.Type.D : Block.Type.N;
                OnGenerate?.Invoke(blockType);
            }
        }

        private void AddBlockToQueue(Block.Type blockType)
        {
            if (blockCreationQueue == null)
            {
                blockCreationQueue = new Queue<Block.Type>();
            }
            blockCreationQueue.Enqueue(blockType);
        }
        
        private void RemoveBlock( int blockIndex, Block.DestructionType dType)
        {
            if (blockIndex >= blockList.Count)
            {
                return;
            }
            var removalBlock = blockList[blockIndex];
            RemoveBlock(removalBlock, dType);
        }

        private void RemoveBlock(Block b, Block.DestructionType dType)
        {
            var breakable = GetFreeBreakable;
            b.Kill(breakable, dType );
            blockList.Remove(b);
            SetTargets();
            //AddBlockToQueue();
        }

        private void StartCreationLoop()
        {
            StartCoroutine(BlockGenerationTimer());
        }
 
        private IEnumerator BlockGenerationTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.15f);
                if (blockCreationQueue.Count == 0)
                {
                    continue;
                }

                var blockType = blockCreationQueue.Dequeue();
                CreateNewBlock(blockType);
            }
        }

        #region UnityEvents

        private void OnEnable()
        {
            Block.OnReadyForUse += BlockOnOnReadyForUse;
            OnRemoveBlock += RemoveBlock;
            OnGenerate += AddBlockToQueue;
        }
        
        private void OnDisable()
        {
            Block.OnReadyForUse += BlockOnOnReadyForUse;
            OnRemoveBlock -= RemoveBlock;
            OnGenerate -= AddBlockToQueue;
        }

        private void Start()
        {
            if (isSlave)
            {
                Initialize();
                StartCreationLoop();
            }
        }
        private void FixedUpdate()
        {
            if (blockList == null || blockList.Count == 0)
            {
                return;
            }

            SetTargets();
            CheckCollisions();

            for (int blockIndex = 0; blockIndex < blockList.Count; blockIndex++)
            {
                blockList[blockIndex].PhysicsUpdate();
            }

            for (int blockIndex = 0; blockIndex < blockList.Count; blockIndex++)
            {
                blockList[blockIndex].SetPos();
            }
        }

        private void Update()
        {
            if (blockList == null || blockList.Count == 0)
            {
                return;
            }

            CheckLastBlock();
        }

//        private void OnGUI()
//        {
//            if (blockCreationQueue == null)
//            {
//                return;
//            }
//            GUIStyle style = new GUIStyle();
//            style.fontSize = 24;
//            style.normal.textColor = Color.black;
//            
//            GUILayout.BeginArea(new Rect(0,0,Screen.width, 400));
//            GUILayout.BeginVertical();
//            GUILayout.Label(new GUIContent(
//                $"Queue: {blockCreationQueue.Count.ToString()}"
//                ), style);
//            GUILayout.EndVertical();
//            GUILayout.EndArea();
//        }

        private void CheckLastBlock()
        {
            if (isSlave)
            {
                return;
            }
            
            var b = blockList[0];
            if (b.IsAtRest && b.BlockType == Block.Type.D && !b.IsBeingRemoved)
            {
                b.IsBeingRemoved = true;
                StartCoroutine(LastBlockRemovalRoutine());
            }

            if (b.BlockType == Block.Type.N)
            {
                isLastBlockWasRed = false;
            }
        }

        private IEnumerator LastBlockRemovalRoutine()
        {
            yield return new WaitForSeconds( isLastBlockWasRed? 0.01f : 0.02f);
            OnRemoveBlock?.Invoke(0,Block.DestructionType.Match);
            isLastBlockWasRed = true;
        }

        #endregion
        
        private void BlockOnOnReadyForUse(Block b)
        {
            if (reusableBlocks == null)
            {
                reusableBlocks = new List<Block>();
            }
            reusableBlocks.Add(b);
        }

        private void PrepareForGeneration()
        {
            blockList= new List<Block>();
            CreateBreakables();
        }

        private void DebugRect(Rect rect, Color color, float duration)
        {
            Debug.DrawLine(new Vector3(rect.xMin, rect.yMin,0), new Vector3(rect.xMax, rect.yMin,0),color, duration);
            Debug.DrawLine(new Vector3(rect.xMin, rect.yMax,0), new Vector3(rect.xMax, rect.yMax,0),color, duration);
            Debug.DrawLine(new Vector3(rect.xMin, rect.yMin,0), new Vector3(rect.xMin, rect.yMax,0),color, duration);
            Debug.DrawLine(new Vector3(rect.xMax, rect.yMin,0), new Vector3(rect.xMax, rect.yMax,0),color, duration);
        }
        private void CreateBreakables()
        {
            breakingBlocks = new BreakingBlock.BreakingBlock[20];
            for (int blockIndex = 0; blockIndex < breakingBlocks.Length; blockIndex++)
            {
                var go = Instantiate(blockProperties.BreakingPrefab);
                breakingBlocks[blockIndex] = go.GetComponent<BreakingBlock.BreakingBlock>();
                breakingBlocks[blockIndex].Initialize();
            }
        }

        private BreakingBlock.BreakingBlock GetFreeBreakable
        {
            get
            {
                for (int blockIndex = 0; blockIndex < breakingBlocks.Length; blockIndex++)
                {
                    if (breakingBlocks[blockIndex].IsReleased)
                    {
                        return breakingBlocks[blockIndex];
                    }
                }

                return null;
            }
        }
        
        private void CheckCollisions()
        {
            for (int blockIndex = 0; blockIndex < blockList.Count-1; blockIndex++)
            {
                var next = blockList[blockIndex + 1];
                var current = blockList[blockIndex];
                
                if (current.YPosition >next.YPosition - blockHeight )
                {
                    next.YPosition = current.YPosition + blockHeight;
                    next.Collide(current);
                }
            }
        }

        private void SetTargets()
        {
            if (blockList == null || blockList.Count == 0)
            {
                return;
            }
            
            for (int blockIndex = 0; blockIndex < blockList.Count; blockIndex++)
            {
                var b = blockList[blockIndex];
                b.Target =blockIndex*blockHeight;
            }    
        }

        public int Count => blockList.Count;
    }
    

    [Serializable]
    public class StackType
    {
                
    }
}