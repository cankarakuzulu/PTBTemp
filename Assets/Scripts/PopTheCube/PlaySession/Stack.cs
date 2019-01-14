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
        [SerializeField] protected BlockProperties blockProperties;

        private const float generationHeight = 18f;
        private const float blockHeight = 2.0f;
        
        private List<Block> blockList;
        private BreakingBlock.BreakingBlock[] breakingBlocks;
        private Queue<Block.Type> blockCreationQueue;
        private List<Block> reusableBlocks;

        
        public void Initialize()
        {
            StartGeneration();
            for (int initialBlockIndex = 0; initialBlockIndex < 6; initialBlockIndex++)
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
       /*     if (reusableBlocks != null && reusableBlocks.Count > 0)
            {
                newBlock = reusableBlocks[0];
                reusableBlocks.RemoveAt(0);
                newBlock.Resurrect();
            }
            else
            {*/
                GameObject blockGO = Instantiate(blockProperties.GetBlockPrefab(blockType));
                blockGO.transform.position = Vector3.up * generationHeight;
                newBlock = blockGO.GetComponent<Block>();
          //  }
            
            newBlock.Initialize( blockType, generationHeight, blockProperties );
            blockList.Add(newBlock);
            SetTargets();
        }

        private void RemoveBlock( int blockIndex, Block.DestructionType dType)
        {
            var removalBlock = blockList[blockIndex];
            RemoveBlock(removalBlock, dType);
        }

        private void RemoveBlock(Block b, Block.DestructionType dType)
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
            StartCoroutine(BlockGenerationTimer());
        }
 
        private IEnumerator BlockGenerationTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
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
        }
        
        private void OnDisable()
        {
            Block.OnReadyForUse += BlockOnOnReadyForUse;
        }

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

            for (int blockIndex = 0; blockIndex < blockList.Count; blockIndex++)
            {
                var b = blockList[blockIndex];
                b.PhysicsUpdate();
                //Debug.Log($"Block {blockIndex} Ypos {b.YPosition} Target {b.Target}");
            }
            SetTargets();
            CheckCollisions();
        }

        private void Update()
        {
            if (blockList == null || blockList.Count == 0)
            {
                return;
            }

            CheckLastBlock();
            //CheckMatches();
        }

        private void OnGUI()
        {
            if (blockCreationQueue == null)
            {
                return;
            }
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.black;
            
            GUILayout.BeginArea(new Rect(0,0,Screen.width, 400));
            GUILayout.BeginVertical();
            GUILayout.Label(new GUIContent(
                $"Queue: {blockCreationQueue.Count.ToString()}"
                ), style);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void CheckLastBlock()
        {
            var b = blockList[0];
            if (b.IsAtRest && b.BlockType == Block.Type.D)
            {
                RemoveBlock(b, Block.DestructionType.Match);
            }
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
        
        private void BlockOnOnReadyForUse(Block b)
        {
            if (reusableBlocks == null)
            {
                reusableBlocks = new List<Block>();
            }
            reusableBlocks.Add(b);
        }

        private void StartGeneration()
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
            
            for (int blockIndex = blockList.Count-2; blockIndex > 0; blockIndex--)
            {
                var next = blockList[blockIndex + 1];
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
            }    
        }
    }

    [Serializable]
    public class StackType
    {
                
    }
    
    
}