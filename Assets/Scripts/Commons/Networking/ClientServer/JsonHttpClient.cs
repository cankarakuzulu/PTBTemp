using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace nopact.Commons.Networking.ClientServer
{
    public sealed class JsonHttpClient
    {
        private static volatile JsonHttpClient instance;
        private static object syncLock = new System.Object();

        private JsonHttpClient()
        {
        }

        public static JsonHttpClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncLock)
                    {
                        if (instance == null)
                        {
                            instance = new JsonHttpClient();
                        }
                    }
                }

                return instance;
            }
        }

        public IEnumerator Get(string endpoint, string language, Action<bool, System.Object> onResponseReceived, Type responseType)
        {
            using (var webRequest = UnityWebRequest.Get(endpoint))
            {
                SetGetDefaultHeaders(webRequest, language);
            
                yield return webRequest.SendWebRequest();

                ProcessResponse(webRequest, onResponseReceived, responseType);
            }
        }
    
        public IEnumerator Post(string endpoint, BaseRequest request, string language, Action<bool, System.Object> onResponseReceived, Type responseType)
        {
            using (var webRequest = new UnityWebRequest(endpoint, "POST"))
            {
                SetPostDefaultHeaders(webRequest, language);
            
                webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(request)));
                Debug.Log("json:" + JsonUtility.ToJson(request));
                webRequest.downloadHandler = new DownloadHandlerBuffer();
            
                yield return webRequest.SendWebRequest();

                ProcessResponse(webRequest, onResponseReceived, responseType);
            }
        }

        private void ProcessResponse(UnityWebRequest webRequest, Action<bool, System.Object> onResponseReceived, Type responseType)
        {
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                if (webRequest.isNetworkError)
                {
                    Debug.LogError("Network Error: " + webRequest.error);     
                }
                else if (webRequest.isHttpError)
                {
                    Debug.LogError("Http Error: " + webRequest.responseCode);
                }

                if (onResponseReceived != null)
                {
                    onResponseReceived(false, null);
                }
            }
            else
            {
                var payload = JsonUtility.FromJson(webRequest.downloadHandler.text, responseType);
                
                if (onResponseReceived != null)
                {
                    onResponseReceived(true, payload);
                }
            }
        }

        private void SetGetDefaultHeaders(UnityWebRequest webRequest, string lang)
        {
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Accept-Language", lang);
        }
    
        private void SetPostDefaultHeaders(UnityWebRequest webRequest, string lang)
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
        
            SetGetDefaultHeaders(webRequest, lang);
        }
    }
}