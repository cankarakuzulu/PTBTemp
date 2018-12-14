/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using nopact.Commons.Utility.Debug;
using nopact.Commons.Utility.Singleton;

namespace nopact.Commons.App
{
    public class App : GenericSingletonMonoBehaviour<App>
    {
        public AppInfo appInfo;

        protected override void InitializeOnAwake()
        {
            if (appInfo == null)
            {
                DebugWrapper.LogError("npE: AppInfo not provided! Drag'n Drop App/AppInfo.asset in the inspector or create a new info asset.");
                DebugWrapper.Break();
            }
        }

        public string AppId
        {
            get
            {
                return appInfo.appId;
            }
        }

        public string Version
        {
            get
            {
                return appInfo.version;
            }
        }

        public string ServerUrl
        {
            get
            {
                return appInfo.serverUrl;
            }
        }

        public string Secret
        {
            get
            {
                return appInfo.secret;
            }
        }

        public string CSK
        {
            get
            {
                return appInfo.csk;
            }
        }
    }
}
