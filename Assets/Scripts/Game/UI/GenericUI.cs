using System;
using System.Collections.Generic;
using Game.SceneDirection;
using nopact.Commons.Analytics;
using nopact.Commons.SceneDirection;
using nopact.Commons.UI.PanelWorks;
using nopact.Commons.UI.PanelWorks.Localization;
using nopact.Game.PlaySession;
using nopact.Game.SceneDirection;
using nopact.Game.UI.Panels;
using UnityEngine;

namespace nopact.Game.UI
{
public class GenericUI : MonoBehaviour
{

    [SerializeField] protected GameUI uiProvider;
    
    protected IAnalyticsTracker analytics;
    protected DirectorBase director;
    protected ILocalizationProvider localization;
    
    protected  virtual void OnInitializeUI()
    {
        SetMainPanels();
        SetupExtraPanels();
        InitializationComplete();
    }

    protected virtual void SetMainPanels()
    {
        Main = uiProvider.GetPanel<Main>("Main");
        Endgame = uiProvider.GetPanel<Endgame>("Endgame");
        Ingame = uiProvider.GetPanel<Ingame>("Ingame");
        Main.Setup( new UIPanelParameter ( localization) { ActionCallbacks = new Dictionary<string, Action> {{ "Play",  OnPlayCommand}}});
        Endgame.Setup( new UIPanelParameter(localization) { ActionCallbacks =  new Dictionary<string, Action>{{ "Retry", OnRetry }, {"Next", OnNext}}});
        Ingame.Setup( new UIPanelParameter( localization ) { ActionCallbacks = new Dictionary<string, Action> { {"Pause", OnPause},{"Success", ()=>OnIngameUIResult(true)}, { "Fail", ()=> OnIngameUIResult(false)}}});
    }

    protected virtual void InitializationComplete()
    {
        Main.Open();
    }

    protected virtual void SetupExtraPanels()
    {
        
    }

    protected virtual void OnShowEndScreen()
    {
        
    }
    
    protected virtual void OnIngameResult(GameResultParamsBase resultParameterBase)
    {
        Ingame.Close();
        director.UiIngameStateUpdate( resultParameterBase.HasWon);
        var result = new UIResultParamsBase("Level 2-3", resultParameterBase.HasWon ? "Success" : "Failed", "High score: 3500", "2000",
            resultParameterBase.HasWon, false);
        Endgame.SetResult(result);
        Endgame.Open();
    }

    protected virtual void OnGameplaySlice(GameplayOutputSliceBase slice){}

    protected void OnIngameUIResult(bool isSuccessfull){}

    private void OnEnable()
    {
        DirectionEvents.OnInitializeDirection += DirectionEventsOnOnInitializeDirection;
        DirectionEvents.OnGameEnded += DirectionEventsOnOnGameEnded;
        DirectionEvents.OnShowEndScreen += DirectionEventsOnOnShowEndScreen;
        DirectionEvents.OnGameOutputSlice+= DirectionEventsOnOnGameOutputSlice;
    }


    private void OnDisable()
    {
        DirectionEvents.OnInitializeDirection -= DirectionEventsOnOnInitializeDirection;
        DirectionEvents.OnGameEnded -= DirectionEventsOnOnGameEnded;
        DirectionEvents.OnGameOutputSlice-= DirectionEventsOnOnGameOutputSlice;
        DirectionEvents.OnShowEndScreen -= DirectionEventsOnOnShowEndScreen;
    }

    #region UICallbacks

    protected void OnNext()
    {
        Endgame.Close();
        director.UiNextCommand();
        Ingame.Open();
    }

    protected void OnRetry()
    {
        Endgame.Close();
        director.UiRetryCommand();
        Ingame.Open();
    }

    protected void OnPlayCommand()
    {
        Main.Close();
        director.UiPlayCommand();
        Ingame.Open();
    }
    
    protected void OnPause()
    {
        director.UiPauseCommand();
    }

    #endregion
    
    #region EventHandlers
    private void DirectionEventsOnOnInitializeDirection(IAnalyticsTracker analytics, IDirector director)
    {
        this.director = director as DirectorBase;
        if (this.director == null)
        {
            Debug.LogWarning("<color=yellow>[GenericUI] You cannot use this type of IDirector with Generic UI.</color>");
            return;
        }
        this.analytics = analytics;
        OnInitializeUI();
    }
    
    private void DirectionEventsOnOnShowEndScreen()
    {
        OnShowEndScreen();
    }

    private void DirectionEventsOnOnGameOutputSlice(GameplayOutputSliceBase slice)
    {
        OnGameplaySlice(slice );
    }
    
    private void DirectionEventsOnOnGameEnded(GameResultParamsBase gameResult )
    {
        OnIngameResult( gameResult);        
    }
    #endregion
    
    #region UIPanelEncapsulation

    protected virtual Main Main { get; set; }
    protected virtual Endgame Endgame { get; set; }
    protected virtual Ingame Ingame { get; set; }
    
    #endregion
}
}
