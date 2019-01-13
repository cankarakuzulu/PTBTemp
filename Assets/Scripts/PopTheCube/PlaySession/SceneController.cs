using System.Collections;
using nopact.Commons.SceneDirection;
using nopact.PopTheCube.PlaySession.Player;
using UnityEngine;

namespace nopact.PopTheCube.PlaySession
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] protected Stack _stack;
        [SerializeField] protected PlayerController _playerController;
        
        private static bool isControlledFromMaster;

        private void Start()
        {
            if (!isControlledFromMaster)
            {
                Initialize();
            }
        }

        private void Update()
        {
            if (!isControlledFromMaster)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ExecuteDashCommand();
                }
            }
        }

        private void ExecuteDashCommand()
        {
            Block b;
            if (_stack.TryDash(_playerController.YPosition, out b))
            {
                _playerController.Dash();
                _playerController.MaxSpeed = Mathf.Min(_playerController.MaxSpeed * 1.2f, 30);
                StartCoroutine(WaitAndBreak(b, _playerController.IsOnLeft));
            }
            else if (b != null)
            {
                _playerController.FailDash();
            }
            else
            {
                _playerController.MaxSpeed = 15;
                _playerController.Dash();
            }
        }

        private void Initialize()
        {
            StartCoroutine(ExecuteStartSequence());
            
        }

        private IEnumerator WaitAndBreak(Block b, bool isOnLeft)
        {
            yield return new WaitForSeconds(0.1f);
            _stack.Break(b, isOnLeft ? Block.DestructionType.Left : Block.DestructionType.Right);
        }

        private IEnumerator ExecuteStartSequence( )
        {
            _stack.Initialize();
            yield return new WaitForSeconds(1.5f);
            _playerController.Initialize();
        }

        public static bool IsControlledFromMaster
        {
            set { isControlledFromMaster = value; }
        }
    }
}