﻿using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Plugin.CloudFirestore;
using ChatApp.Droid;
using Xamarin.Forms;
using Android.Gms.Extensions;
using System.Collections.Generic;

[assembly: Dependency(typeof(FirebaseAuthService))]
namespace ChatApp.Droid
{
    public class FirebaseAuthService : IFirebaseAuth
    {

        DataClass dataClass = DataClass.GetInstance;

        public FirebaseAuthResponseModel IsLoggedIn()
        {
            try
            {
                if (FirebaseAuth.Instance.CurrentUser.Uid == null)
                {
                    dataClass.isSignedIn = false;
                    dataClass.loggedInUser = new UserModel();
                    return new FirebaseAuthResponseModel() { Status = false, Response = "Currently logged out." };
                }
             
                dataClass.loggedInUser = new UserModel()
                {
                    uid = FirebaseAuth.Instance.CurrentUser.Uid,
                    email = FirebaseAuth.Instance.CurrentUser.Email,
                    username = dataClass.loggedInUser.username,
                    userType = dataClass.loggedInUser.userType,
                    contacts = dataClass.loggedInUser.contacts,
                    createdAt = dataClass.loggedInUser.createdAt
                };
                dataClass.isSignedIn = true;
                return new FirebaseAuthResponseModel() { Status = true, Response = "Currently logged in." };
            }
            catch (Exception ex)
            {
                dataClass.isSignedIn = false;
                dataClass.loggedInUser = new UserModel();
                return new FirebaseAuthResponseModel() { Status = false, Response = ex.Message };
            }
        }

        public async Task<FirebaseAuthResponseModel> LoginWithEmailPassword(string email, string password)
        {
            try
            {
                IAuthResult result = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);

                if (result.User.IsEmailVerified && email == result.User.Email)
                {
                    var document = await CrossCloudFirestore.Current
                                        .Instance
                                        .Collection("users")
                                        .Document(result.User.Uid)
                                        .GetAsync();
                    
                    var user = document.ToObject<UserModel>();

                    dataClass.loggedInUser = new UserModel()
                    {
                        uid = result.User.Uid,
                        email = result.User.Email,
                        username = user.username,
                        userType = user.userType,
                        contacts = user.contacts,
                        createdAt = user.createdAt
                    };
                    dataClass.isSignedIn = true;
                    return new FirebaseAuthResponseModel() { Status = true, Response = "Login successful." };
                }
  
                await FirebaseAuth.Instance.CurrentUser.SendEmailVerification();
                dataClass.loggedInUser = new UserModel();
                dataClass.isSignedIn = false;

                return new FirebaseAuthResponseModel() { Status = false, Response = "Email not verified. Sent another verification email." };
            }
            catch (Exception ex)
            {
                return new FirebaseAuthResponseModel() { Status = false, Response = ex.Message };
            }
        }

        public async Task<FirebaseAuthResponseModel> ResetPassword(string email)
        {
            try
            {
                await FirebaseAuth.Instance.SendPasswordResetEmailAsync(email);
                return new FirebaseAuthResponseModel() { Status = true, Response = "Email has been sent to your email address." }; ;
            }
            catch (Exception ex)
            {
                return new FirebaseAuthResponseModel() { Status = false, Response = ex.Message };
            }
        }

        public FirebaseAuthResponseModel SignOut()
        {
            try
            {
                FirebaseAuth.Instance.SignOut();
                dataClass.isSignedIn = false;
                dataClass.loggedInUser = new UserModel();
                return new FirebaseAuthResponseModel() { Status = true, Response = "Sign out successful." }; 
            }
            catch (Exception ex)
            {
                dataClass.isSignedIn = true;
                return new FirebaseAuthResponseModel() { Status = false, Response = ex.Message };
            }
        }
        
        public async Task<FirebaseAuthResponseModel> SignUpWithEmailPassword(string username, string email, string password)
        {
            try
            {
                await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                await FirebaseAuth.Instance.CurrentUser.SendEmailVerification();

                dataClass.loggedInUser = new UserModel()
                {
                    uid = FirebaseAuth.Instance.CurrentUser.Uid,
                    email = email,
                    username = username,
                    userType = 0,
                    createdAt = DateTime.UtcNow,
                    contacts = new List<string>(),
                };

                return new FirebaseAuthResponseModel() { Status = true, Response = "Sign up successful. Verification email sent." }; ;
            }
            catch (Exception ex)
            {
                return new FirebaseAuthResponseModel() { Status = false, Response = ex.Message };
            }
        }
    }
}
