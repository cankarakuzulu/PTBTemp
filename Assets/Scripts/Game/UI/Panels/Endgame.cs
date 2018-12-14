using System.Collections;
using System.Collections.Generic;
using nopact.Commons.UI.PanelWorks;
using UnityEngine;
using UnityEngine.UI;

namespace nopact.Game.UI.Panels
{
    public class Endgame : StandardPanel
    {

        [SerializeField] protected Text score, highscore, resultTxt, levelTxt;
        [SerializeField] protected GameObject retryButton, nextButton;
    
        public virtual void SetResult(UIResultParamsBase uiResult)
        {
            nextButton.SetActive( uiResult.IsSuccessful );
            resultTxt.text = uiResult.ResultString;
            levelTxt.text = uiResult.LevelString;
            score.text = uiResult.ScoreString;
            highscore.text = uiResult.HighscoreString;
        }
    
        public void OnClickRetry()
        {
            config.GetAction("Retry").Invoke();
        }

        public void OnClickNext()
        {
            config.GetAction("Next").Invoke();
        }

    }    
}
