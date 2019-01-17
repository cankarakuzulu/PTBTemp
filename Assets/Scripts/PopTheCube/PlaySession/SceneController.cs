using System;
using System.Collections;
using nopact.Commons.Domain.Enum;
using nopact.Commons.SceneDirection;
using nopact.Game.Camera;
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
        [SerializeField] protected CameraShaker _shaker;
        private SceneState _sceneState;
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
            if (_sceneState != SceneState.PlayerReady   || _playerController.Motion == PlayerController.MotionTypes.Dash
                )
            {
                return;
            }
            
            Block b;
            if (_stack.TryDash(_playerController.YPosition, out b))
            {
                _playerController.Dash();
                StartCoroutine(WaitAndBreak(b, _playerController.IsOnLeft));
            }
            else if (b != null)
            {
                _playerController.FailDash();
                StartCoroutine(FailTouchCoroutine());
            }
            else
            {
                _playerController.MarkEmptyPass();
                _playerController.Dash();
            }
        }

        private IEnumerator FailTouchCoroutine()
        {
            _sceneState = SceneState.GameOver;
            yield return new WaitForSeconds(0.06f);
            _shaker.ShakeIt();
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
       
        
        private void GameWon()
        {
            OnGameCompleted?.Invoke( true );
            _stack.Kill();
            _playerController.Kill();
            SceneManager.LoadScene(0);
        }


        private IEnumerator WaitAndBreak(Block b, bool isOnLeft)
        {
            yield return new WaitForSeconds(0.035f);
            _stack.Break(b, isOnLeft ? Block.DestructionType.Left : Block.DestructionType.Right);
            if (_playerController.ChainDashCount > 0)
            {
                RegisterChainDash(_playerController.ChainDashCount);
            }

            if (_stack.Count < 1)
            {
                GameWon();
            }
        }

        private void RegisterChainDash(uint playerControllerChainDashCount)
        {
            Debug.Log(playerControllerChainDashCount.ToString());
        }

        private IEnumerator ExecuteStartSequence( )
        {
            _stack.Initialize();
            _sceneState++;
            yield return new WaitForSeconds(2.0f);
            _playerController.Initialize();
            _sceneState++;
        }

        public static bool IsControlledFromMaster
        {
            set { isControlledFromMaster = value; }
        }

        public enum SceneState
        {
            NotReady =0,
            StackReady,
            PlayerReady,
            GameOver
        }
    }
}