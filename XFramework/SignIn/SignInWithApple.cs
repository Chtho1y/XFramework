using System;
using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine;

#region XCODE
/*using AppleAuth.Editor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;*/


/*
public static class SignInWithApplePostprocessor
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
            return;

        var projectPath = PBXProject.GetPBXProjectPath(path);

        // Adds entitlement depending on the Unity version used
#if UNITY_2019_3_OR_NEWER
        var project = new PBXProject();
        project.ReadFromString(System.IO.File.ReadAllText(projectPath));
        var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
        manager.AddSignInWithAppleWithCompatibility(project.GetUnityFrameworkTargetGuid());
        manager.WriteToFile();
#else
            var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", PBXProject.GetUnityTargetName());
            manager.AddSignInWithAppleWithCompatibility();
            manager.WriteToFile();
#endif
    }
}
*/
#endregion

namespace DarlingEngine.SignIn
{
    public class SignInWithApple : MonoBehaviour
    {
        public static SignInWithApple Instance;
        private IAppleAuthManager _appleAuthManager;
    
        private const string AppleUserIdKey = "AppleUserId";
        private const string AppleUserEmail = "AppleUserEmail";
        private const string AppleIdentityToken = "AppleIdentityToken";
        private const string AuthorizationErrorCode = "AuthorizationErrorCode";
    
        private string _curIdentityToken = String.Empty;
        public string CurIdentityToken => _curIdentityToken;
    
        public void SignInFirstTime(Action<bool> action = null)
        {
            AppleAuthLoginArgs loginArgs = new(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
            _appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // Obtained credential, cast it to IAppleIDCredential
                    if (credential is IAppleIDCredential appleIdCredential)
                    {
                        // Apple User ID
                        // You should save the user ID somewhere in the device
                        var userId = appleIdCredential.User;
                        PlayerPrefs.SetString(AppleUserIdKey, userId);

                        // Email (Received ONLY in the first login)
                        var email = appleIdCredential.Email;
                        PlayerPrefs.SetString(AppleUserEmail, email);

                        // Full name (Received ONLY in the first login)
                        var fullName = appleIdCredential.FullName;

                        // Identity token
                        var identityToken = Encoding.UTF8.GetString(
                            appleIdCredential.IdentityToken,
                            0,
                            appleIdCredential.IdentityToken.Length);
                        _curIdentityToken = identityToken;
                        PlayerPrefs.SetString(AppleIdentityToken, identityToken);

                        // Authorization code
                        var authorizationCode = Encoding.UTF8.GetString(
                            appleIdCredential.AuthorizationCode,
                            0,
                            appleIdCredential.AuthorizationCode.Length);

                        // And now you have all the information to create/login a user in your system
                        PlayerPrefs.Save();
                        action?.Invoke(true);
                    }
                },
                error =>
                {
                    // Something went wrong
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    PlayerPrefs.SetString(AuthorizationErrorCode, authorizationErrorCode.ToString());
                    action?.Invoke(false);
                });
        }
    
    
        void Awake()
        {
            Instance = this;
        }
    
    
        void Start()
        {
#if !UNITY_EDITOR
            // If the current platform is supported
            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
                var deserializer = new PayloadDeserializer();
                // Creates an Apple Authentication manager with the deserializer
                _appleAuthManager = new AppleAuthManager(deserializer);    
            }
#endif
        }

        void Update()
        {
            // Updates the AppleAuthManager instance to execute
            // pending callbacks inside Unity's execution loop
            if (_appleAuthManager != null)
            {
                _appleAuthManager.Update();
            }
        }
    }
}
