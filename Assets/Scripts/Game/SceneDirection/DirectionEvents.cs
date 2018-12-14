using System;
using Game.SceneDirection;
using nopact.Commons.Analytics;
using nopact.Commons.SceneDirection;
using nopact.Game.PlaySession;

namespace nopact.Game.SceneDirection
{
	public static class DirectionEvents
	{

		public static event Action<IAnalyticsTracker, IDirector> OnInitializeDirection;
		public static event Action<GameResultParamsBase> OnGameEnded;
		public static event Action<GameplayOutputSliceBase> OnGameOutputSlice;
		public static event Action OnShowEndScreen; 

		public static void InitializeDirection(IAnalyticsTracker tracker, IDirector director)
		{
			OnInitializeDirection?.Invoke(tracker, director);
		}

		public static void GameEnded(GameResultParamsBase result)
		{
			OnGameEnded?.Invoke(result);
		}

		public static void ShowEndScreen()
		{
			OnShowEndScreen?.Invoke();
		}

		public static void SliceGameplayOutput(GameplayOutputSliceBase slice)
		{
			OnGameOutputSlice?.Invoke( slice );
		}
	}
}
