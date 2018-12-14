using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nopact.Commons.UI.PanelWorks.Localization
{
    public interface ILanguageData
    {
        void MigrateLanguage( Dictionary<string, string> localeData );
    }
}
