using UnityEngine;

namespace nopact.PopTheCube.PlaySession.BreakingBlock
{
	public class BreakingBlock : MonoBehaviour
	{
		[SerializeField] protected Rigidbody[] parts;

		private Vector3[] initialPositions;
		private Quaternion[] initialRotations;
		private Renderer[] renderers;
		private Material currentMaterial;

		public void Explode(Vector3 position, Block.DestructionType destructionType)
		{
			SetDefaults();
			IsReleased = false;
			transform.position = position;
			for (int partIndex = 0; partIndex < parts.Length; partIndex++)
			{
				Vector3 forceTotal = parts[partIndex].transform.up * 12;
				if (destructionType == Block.DestructionType.Left)
				{
					forceTotal += Vector3.right * 7;
				}
				else if (destructionType == Block.DestructionType.Right)
				{
					forceTotal += Vector3.left * 7;
				}
				
				renderers[partIndex].material = currentMaterial;
				renderers[partIndex].enabled = true;
				parts[partIndex].isKinematic = false;
				parts[partIndex].AddForce( forceTotal, ForceMode.Impulse);
				parts[partIndex].AddTorque(transform.forward * 160);
			}
		}

		public void Initialize()
		{
			GetDefaults();
			IsReleased = true;
		}
		public void Release()
		{
			IsReleased = true;
			SetDefaults();
		}
		
		private void GetDefaults()
		{
			initialPositions = new Vector3[parts.Length];
			initialRotations = new Quaternion[initialPositions.Length];
			renderers = new Renderer[initialRotations.Length];

			for (int partIndex = 0; partIndex < parts.Length; partIndex++)
			{
				initialPositions[partIndex] = parts[partIndex].transform.localPosition;
				initialRotations[partIndex] = parts[partIndex].transform.localRotation;
				renderers[partIndex] = parts[partIndex].GetComponent<MeshRenderer>();
				renderers[partIndex].enabled = false;
			}
		}

		private void SetDefaults()
		{
			for (int partIndex = 0; partIndex < parts.Length; partIndex++)
			{
				parts[partIndex].isKinematic = true;
				parts[partIndex].angularVelocity = Vector3.zero;
				parts[partIndex].velocity = Vector3.zero;

				parts[partIndex].transform.localPosition = initialPositions[partIndex];
				parts[partIndex].transform.localRotation = initialRotations[partIndex];
				renderers[partIndex].enabled = false;
			}
		}

		public bool IsReleased { get; private set; }

		public void SetMaterial(Material getBlockMaterial)
		{
			currentMaterial = getBlockMaterial;
		}
	}
}
