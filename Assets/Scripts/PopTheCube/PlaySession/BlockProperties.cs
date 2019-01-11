using UnityEngine;

namespace nopact.PopTheCube.PlaySession
{
    [CreateAssetMenu(menuName = "Game/BlockProperties", fileName = "Assets/Data/BlockProperties.asset")]
    public class BlockProperties : ScriptableObject
    {
        [SerializeField] protected GameObject[] blockPrefabs;
        [SerializeField] protected GameObject breakingPrefab;
        [SerializeField] protected Material[] blockMaterials;
        public Material GetBlockMaterial(Block.Type type)
        {
            return blockMaterials[(int) type];
        }
        public GameObject GetBlockPrefab(Block.Type type)
        {
            return blockPrefabs[(int) type];
        }

        public GameObject BreakingPrefab => breakingPrefab;
    }
    
}