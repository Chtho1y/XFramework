// <copyright file="SigninSampleScript.cs" company="Google Inc.">
// Copyright (C) 2017 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;


namespace DarlingEngine.SignIn
{
    public class SignInWithGoogle : MonoBehaviour
    {
        public static SignInWithGoogle Instance;
        
        public UnityEngine.UI.Text statusText;
        public string webClientId = "<Client ID>";
        private const string GoogleUserId = "GoogleUserId";
        private const string GoogleIdToken = "GoogleIdToken";
        
        private GoogleSignInConfiguration _configuration;
        private Action _action;
    
        // Defer the configuration creation until Awake so the web Client ID
        // Can be set via the property inspector in the Editor.
        void Awake()
        {
            Instance = this;
            _configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                UseGameSignIn = false,
                RequestIdToken = true
            };
        }

        public void OnSignIn(Action handler = null)
        {
            _action = null;
            _action += handler;
        
            GoogleSignIn.Configuration = _configuration;
            AddStatusText("Calling SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
                OnAuthenticationFinished);
        }

        public void OnSignOut()
        {
            AddStatusText("Calling SignOut");
            GoogleSignIn.DefaultInstance.SignOut();
        }

        public void OnDisconnect()
        {
            AddStatusText("Calling Disconnect");
            GoogleSignIn.DefaultInstance.Disconnect();
        }

        internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                using (var enumerator = task.Exception?.InnerExceptions.GetEnumerator())
                {
                    if (enumerator != null && enumerator.MoveNext())
                    {
                        var error = (GoogleSignIn.SignInException)enumerator.Current;
                        if (error != null) AddStatusText("Error:" + error.Status + " " + error.Message);
                    }
                    else
                    {
                        AddStatusText("Unexpected Exception!" + task.Exception);
                    }
                }
                _action?.Invoke();
            }
            else if (task.IsCanceled)
            {
                AddStatusText("Canceled");
                _action?.Invoke();
            }
            else
            {
                AddStatusText("Welcome:" + task.Result.DisplayName + "!");
                PlayerPrefs.SetString(GoogleUserId, task.Result.UserId);
                PlayerPrefs.SetString(GoogleIdToken, task.Result.IdToken);
                PlayerPrefs.Save();
                _action?.Invoke();
            }
        }

        public void OnSignInSilently()
        {
            GoogleSignIn.Configuration = _configuration;
            AddStatusText("Calling SignIn Silently");

            GoogleSignIn.DefaultInstance.SignInSilently()
                .ContinueWith(OnAuthenticationFinished);
        }

        private List<string> _messages = new();

        private void AddStatusText(string text)
        {
            statusText.transform.parent.gameObject.SetActive(true);
            Invoke(nameof(HideTip), 5);
            
            if (_messages.Count == 5) _messages.RemoveAt(0);
            _messages.Add(text);
            string txt = "";
            foreach (string s in _messages)
            {
                txt += "\n" + s;
            }
            statusText.text = txt;
        }

        private void HideTip()
        {
            statusText.transform.parent.gameObject.SetActive(false);
        }
    }
}
