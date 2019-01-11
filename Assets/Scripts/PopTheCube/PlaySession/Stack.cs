using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using nopact.Commons.Utility.Timer;
using nopact.PopTheCube.PlaySession.BreakingBlock;
using nopact.PopTheCube.PlaySession.Player;
using Random = UnityEngine.Random;

namespace nopact.PopTheCube.PlaySession
{
    public class Stack : MonoBehaviour
    {
        [SerializeField] protected BlockProperties blockProperties;

        private const float generationHeight = 18f;
        private const float blockHeight = 2.0f;
        
        private Boo.Lang.List<Block> blockList;
        private BreakingBlock.BreakingBlock[] breakingBlocks;
        private Queue<Block.Type> blockCreationQueue;

        public void Initialize()
        {
            StartGeneration();
            for (int initialBlockIndex = 0; initialBlockIndex < 6; initialBlockIndex++)
            {
                AddBlockToQueue();
            }
            StartCreationLoop();
        }


        public void CreateNewBlock( Block.Type blockType )
        {
            GameObject blockGO = Instantiate(blockProperties.GetBlockPrefab(blockType));
            blockGO.transform.position = Vector3.up * generationHeight;
            Block newBlock = blockGO.GetComponent<Block>();
            newBlock.Initialize( blockType, generationHeight, blockProperties );
            blockList.Add(newBlock);
            SetTargets();
        }

        public void RemoveBlock( int blockIndex, Block.DestructionType dType)
        {
            var removalBlock = blockList[blockIndex];
            RemoveBlock(removalBlock, dType);
        }

        public void RemoveBlock(Block b, Block.DestructionType dType)
        {
            var breakable = GetFreeBreakable;
            b.Kill(breakable, dType );
            blockList.Remove(b);
            SetTargets();
            AddBlockToQueue();            
        }

        public bool TryDash(float yPos, out Block block)
        {
            block = null;
            if (blockList == null || blockList.Count == 0)
            {
                return false;
            }
            
            for (int blockIndex = blockList.Count-1; blockIndex >= 0; blockIndex--)
            {
                var current = blockList[blockIndex];
                
                if (Mathf.Abs(current.YPosition - yPos )< blockHeight *0.5f)
                {
                    block = current;
                    if (current.BlockType == Block.Type.N)
                    {
                        return true;
                    }
                     return false;
                }
            }
            return false;
        }

        public void Break(Block block, Block.DestructionType destructionType)
        {
            RemoveBlock(block, destructionType);
        }

        private void AddBlockToQueue()
        {
            if (blockCreationQueue == null)
            {
                blockCreationQueue = new Queue<Block.Type>();
            }
            blockCreationQueue.Enqueue(Random.value > 0.8f ? Block.Type.D : Block.Type.N);
        }
        
        private void StartCreationLoop()
        {
            StartCoroutine(OnBlockAdditionTimer());
        }
 
        private IEnumerator OnBlockAdditionTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (blockCreationQueue.Count == 0)
                {
                    continue;
                }

                var blockType = blockCreationQueue.Dequeue();
                CreateNewBlock(blockType);
            }
        }

        #region UnityEvents
        private void Start()
        {
            StartGeneration();
        }
        private void FixedUpdate()
        {
            if (blockList == null || blockList.Count == 0)
            {
                return;
            }
            CheckCollisions();
            for (int blockIndex = 0; blockIndex < blockList.Count; blockIndex++)
            {
                var b = blockList[blockIndex];
                b.PhysicsUpdate();
            }    
        }

        private void Update()
        {
            if (blockList == null || blockList.Count == 0)
            {
                return;
            }
            CheckMatches();
        }

        private void CheckMatches()
        {
            bool isMatching = false;
            for (int blockIndex = blockList.Count-1; blockIndex > 1; blockIndex--)
            {
                int depth = 0;
                CalculateMatchDepth(blockIndex, ref depth);

                if (depth >= 3)
                {
                    int target = blockIndex - depth + 1;
                    for (; blockIndex >= target; blockIndex--)
                    {
                        RemoveBlock( blockIndex, Block.DestructionType.Match);
                    }    
                }
            }    
        }

        private void CalculateMatchDepth(int index, ref int depth)
        {
            while (true)
            {
                Block b;
                if (index < 0 || !(b = blockList[index]).IsAtRest || b.BlockType != Block.Type.D)
                {
                    return;
                }
                depth++;
                index --;
            }
        }

        #endregion
        
        private void StartGeneration()
        {
            blockList= new Boo.Lang.List<Block>();
            CreateBreakables();
        }

        private void CreateBreakables()
        {
            breakingBlocks = new BreakingBlock.BreakingBlock[12];
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
            
            for (int blockIndex = blockList.Count-1; blockIndex > 0; blockIndex--)
            {
                var current = blockList[blockIndex];
                var previous = blockList[blockIndex-1];

                if (current.YPosition < previous.YPosition + blockHeight )
                {
                    current.YPosition = previous.YPosition + blockHeight;
                    current.Collide( previous );
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
                //Debug.Log($"Block Index {blockIndex} Pos: {b.YPosition} Target: {b.Target}");
            }    
        }
    }

    [Serializable]
    public class StackType
    {
                
    }
    
    
}