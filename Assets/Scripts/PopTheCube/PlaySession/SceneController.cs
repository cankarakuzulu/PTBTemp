using System;
using System.Collections;
using nopact.Commons.SceneDirection;
using nopact.PopTheCube.PlaySession.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace nopact.PopTheCube.PlaySession
{
    public class SceneController : MonoBehaviour
    {

        public event Action<bool> OnGameCompleted;
        [SerializeField] protected Stack _stack;
        [SerializeField] protected PlayerController _playerController;

        private bool isTouchDown = false;
        private static bool isControlledFromMaster;

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

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

                if (Input.touchCount > 0 && !isTouchDown)
                {
                    ExecuteDashCommand();
                    isTouchDown = true;
                }

                if (Input.touchCount == 0 && isTouchDown)
                {
                    isTouchDown = false;
                }
            }
        }

        private void ExecuteDashCommand()
        {
            Block b;
            if (_stack.TryDash(_playerController.YPosition, out b))
            {
                _playerController.Dash();
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
            _playerController.OnFailed+= PlayerControllerOnOnFailed;
            StartCoroutine(ExecuteStartSequence());
            
        }

        private void PlayerControllerOnOnFailed()
        {
            GameFailed();
        }

        private void GameFailed()
        {
            OnGameCompleted?.Invoke( false );
            _stack.Kill();
            _playerController.Kill();
            SceneManager.LoadScene(0);
             
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