using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class oturum_ac : MonoBehaviour {

    protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseAuth otherAuth;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();

    private string logText = "";
    protected string email = "";
    protected string password = "";
    protected string displayName = "";
    protected string phoneNumber = "";
    protected string receivedCode = "";
    private bool fetchingToken = false;
    public bool usePasswordInput = false;
    private Vector2 scrollViewVector = Vector2.zero;
    public GameObject giris_ekrani_paneli;
    public GameObject giris_yap_kayit_ol_paneli;
    public GameObject email_onay_kayit_ol_paneli;
    public GameObject email_onay_giris_yap_paneli;
    public GameObject login_panel_mini;
    public GameObject kayit_ol_panel_mini;

    public InputField email_;
    public InputField password_;
    public Toggle beni_hatirla;
    Firebase.Auth.FirebaseUser user;
    private uint phoneAuthTimeoutMs = 60 * 1000;
    // The verification id needed along with the sent code for phone authentication.
    private string phoneAuthVerificationId;
    // Options used to setup secondary authentication object.
    private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions
    {
        ApiKey = "",
        AppId = "",
        ProjectId = ""
    };

    const int kMaxLogSize = 16382;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    public virtual void Start()
    {
        if (PlayerPrefs.GetString("email") != "" && PlayerPrefs.GetString("password") != "")
        {
            email_.text = PlayerPrefs.GetString("email");
            password_.text = PlayerPrefs.GetString("password");
            giris_yap_email();
        }
        else
        {
            giris_yap_kayit_ol_paneli.SetActive(true);
        }
        InitializeFirebase();
    }
    protected void InitializeFirebase()
    {
        DebugLog("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        auth.IdTokenChanged += IdTokenChanged;
        
        // Specify valid options to construct a secondary authentication object.
        if (otherAuthOptions != null &&
            !(String.IsNullOrEmpty(otherAuthOptions.ApiKey) ||
              String.IsNullOrEmpty(otherAuthOptions.AppId) ||
              String.IsNullOrEmpty(otherAuthOptions.ProjectId)))
        {
            try
            {
                otherAuth = Firebase.Auth.FirebaseAuth.GetAuth(Firebase.FirebaseApp.Create(
                  otherAuthOptions, "Secondary"));
                otherAuth.StateChanged += AuthStateChanged;
                otherAuth.IdTokenChanged += IdTokenChanged;
            }
            catch (Exception)
            {
                DebugLog("ERROR: Failed to initialize secondary authentication object.");
            }
        }
        GetUserInfo();
        AuthStateChanged(this, null);
    }
    // Output text to the debug log text field, as well as the console.
    public void DebugLog(string s)
    {
        Debug.Log(s);
        logText += s + "\n";

        while (logText.Length > kMaxLogSize)
        {
            int index = logText.IndexOf("\n");
            logText = logText.Substring(index + 1);
        }
        scrollViewVector.y = int.MaxValue;
    }

    // Display user information.
    void DisplayUserInfo(Firebase.Auth.IUserInfo userInfo, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        var userProperties = new Dictionary<string, string> {
      {"Display Name", userInfo.DisplayName},
      {"Email", userInfo.Email},
      {"Photo URL", userInfo.PhotoUrl != null ? userInfo.PhotoUrl.ToString() : null},
      {"Provider ID", userInfo.ProviderId},
      {"User ID", userInfo.UserId}
    };
        foreach (var property in userProperties)
        {
            if (!String.IsNullOrEmpty(property.Value))
            {
                DebugLog(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
            }
        }
    }

    // Display a more detailed view of a FirebaseUser.
    void DisplayDetailedUserInfo(Firebase.Auth.FirebaseUser user, int indentLevel)
    {
        DisplayUserInfo(user, indentLevel);
        DebugLog("  Anonymous: " + user.IsAnonymous);
        DebugLog("  Email Verified: " + user.IsEmailVerified);
        var providerDataList = new List<Firebase.Auth.IUserInfo>(user.ProviderData);
        if (providerDataList.Count > 0)
        {
            DebugLog("  Provider Data:");
            foreach (var providerData in user.ProviderData)
            {
                DisplayUserInfo(providerData, indentLevel + 1);
            }
        }
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        Firebase.Auth.FirebaseUser user = null;
        if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        if (senderAuth == auth && senderAuth.CurrentUser != user)
        {
            bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                DebugLog("Signed out " + user.UserId);
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                DebugLog("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                DisplayDetailedUserInfo(user, 1);
            }
        }
    }
    // Track ID token changes.
    void IdTokenChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
        {
            senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
              task => DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
        }
    }

    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            DebugLog(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            DebugLog(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                }
                DebugLog(authErrorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            DebugLog(operation + " completed");
            complete = true;
        }
        return complete;
    }
    public void kaydol_email()
    {
        giris_ekrani_paneli.SetActive(true);
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.CreateUserWithEmailAndPasswordAsync(email_.text, password_.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                giris_yap_kayit_ol_paneli.SetActive(true);
                giris_ekrani_paneli.SetActive(false);
                GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Üyelik işlemi yapılamadı");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception);
                giris_yap_kayit_ol_paneli.SetActive(true);
                giris_ekrani_paneli.SetActive(false);
                if (task.Exception.ToString().Contains("The email address is already in use by another account")) 
                {
                    GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Bu e-postaya ait bir hesap zaten mevcut");
                }
                else if (task.Exception.ToString().Contains("The given password is invalid"))
                {
                    GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Şifre minimum 6 karakter olmalı");
                }
                else
                {
                    GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Üyelik işlemi yapılamadı");
                }
                return;
            }
            login_panel_mini.SetActive(true);
            kayit_ol_panel_mini.SetActive(false);
            // Firebase user has been created.
            if (beni_hatirla.isOn == true)
            {
                PlayerPrefs.SetString("email", email_.text);
                PlayerPrefs.SetString("password", password_.text);
            }
            else
            {
                PlayerPrefs.SetString("email", "");
                PlayerPrefs.SetString("password", "");
            }
            Firebase.Auth.FirebaseUser user = auth.CurrentUser;
            if (user != null)
            {
                user.SendEmailVerificationAsync().ContinueWith(taskk => {
                    if (taskk.IsCanceled)
                    {
                        Debug.LogError("SendEmailVerificationAsync was canceled.");
                        return;
                    }
                    if (taskk.IsFaulted)
                    {
                        Debug.LogError("SendEmailVerificationAsync encountered an error: " + taskk.Exception);
                        return;
                    }
                    email_onay_kayit_ol_paneli.SetActive(true);
                    giris_yap_kayit_ol_paneli.SetActive(true);
                    giris_ekrani_paneli.SetActive(false);
                    GameObject.Find("firebase-message").GetComponent<databasee>().yeni_kayit_mi = 1;
                });
            }
        });
    }
    //public void sifre_status()
    //{
    //    if (password_.contentType == InputField.ContentType.Password)
    //    {
    //        password_.contentType = InputField.ContentType.Standard;
    //    }
    //    else
    //    {
    //        password_.contentType = InputField.ContentType.Password;
    //    }
    //}
    public void giris_yap_email()
    {
        if (email_.text == "" && password_.text == "")
        {
            GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Bence boşukları doldurmayı denemelisin :)");
        }
        else if (email_.text != "" && password_.text == "")
        {
            GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Şifreni girmeyi unuttun!");
        }
        else if (email_.text == "" && password_.text != "")
        {
            GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("E-postanı da eklesen olur bu iş :)");
        }
        else if (email_.text != "" && password_.text != "")
        {
            giris_ekrani_paneli.SetActive(true);
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            Firebase.Auth.Credential credential =
         Firebase.Auth.EmailAuthProvider.GetCredential(email_.text, password_.text);
            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    giris_yap_kayit_ol_paneli.SetActive(true);
                    giris_ekrani_paneli.SetActive(false);
                    GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("İşlem başarısız");
                    return;
                }
                if (task.IsFaulted)
                {
                    giris_yap_kayit_ol_paneli.SetActive(true);
                    giris_ekrani_paneli.SetActive(false);
                    Debug.Log("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    if (task.Exception.ToString().Contains("An email address must be provided"))
                    {
                        GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("E-posta kısmı biraz boş :)");
                    }
                    else if (task.Exception.ToString().Contains("A password must be provided"))
                    {
                        GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Şifre girmeyi unuttun :(");
                    }
                    else if (task.Exception.ToString().Contains("The password is invalid or the user does not have a password"))
                    {
                        GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Sanırım şifreni unuttun :/");
                    }
                    else if (task.Exception.ToString().Contains("There is no user record corresponding to this identifier. The user may have been deleted"))
                    {
                        GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Böyle bir kullanıcı yok ki :(");
                    }
                    else
                    {
                        GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Akış yenilenemedi");
                    }
                    return;
                }
                user = task.Result;
                if (user.IsEmailVerified == true)
                {
                    if (beni_hatirla.isOn == true)
                    {
                        PlayerPrefs.SetString("email", email_.text);
                        PlayerPrefs.SetString("password", password_.text);
                    }
                    else
                    {
                        PlayerPrefs.SetString("email", "");
                        PlayerPrefs.SetString("password", "");
                    }
                    GameObject.Find("firebase-message").GetComponent<databasee>().user_uid.text = user.UserId;
                    GameObject.Find("firebase-message").GetComponent<firebasee>().initcagir_fireabase();
                    GameObject.Find("firebase-message").GetComponent<databasee>().intcagir_database();
                }
                else
                {
                    email_onay_giris_yap_paneli.SetActive(true);
                    giris_yap_kayit_ol_paneli.SetActive(true);
                    giris_ekrani_paneli.SetActive(false);
                }
            });
        }
    }
    public void email_onay_sent()
    {
        user.SendEmailVerificationAsync().ContinueWith(taskk => {
            if (taskk.IsCanceled)
            {
                Debug.LogError("SendEmailVerificationAsync was canceled.");
                GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("İşlem başarısız");
                return;
            }
            if (taskk.IsFaulted)
            {
                GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("İşlem başarısız");
                Debug.LogError("SendEmailVerificationAsync encountered an error: " + taskk.Exception);
                return;
            }
            Debug.Log("Email sent successfully.");
            GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Onay e-postası gönderildi");
        });
    }
    public void email_sifre_sifirla()
    {
            auth.SendPasswordResetEmailAsync(email_.text).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    return;
                }
                Debug.Log("Password reset email sent successfully.");
                GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Şifre sıfırlama e-postası gönderildi");
            });
    }
    void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask)
    {
        //EnableUI();
        LogTaskCompletion(authTask, "Sign-in");
    }

    void GetUserInfo()
    {
        if (auth.CurrentUser == null)
        {
            DebugLog("Not signed in, unable to get info.");
        }
        else
        {
            DebugLog("Current user info:");
            
            DisplayDetailedUserInfo(auth.CurrentUser, 1);
        }
    }
    public void SignOut()
    {
        DebugLog("Signing out.");
        auth.SignOut();
    }
    public Task DeleteUserAsync()
    {
        if (auth.CurrentUser != null)
        {
            DebugLog(String.Format("Attempting to delete user {0}...", auth.CurrentUser.UserId));
            return auth.CurrentUser.DeleteAsync().ContinueWith(HandleDeleteResult);
        }
        else
        {
            DebugLog("Sign-in before deleting user.");
            // Return a finished task.
            return Task.FromResult(0);
        }
    }

    void HandleDeleteResult(Task authTask)
    {
        LogTaskCompletion(authTask, "Delete user");
    }
    public void VerifyPhoneNumber()
    {
        var phoneAuthProvider = Firebase.Auth.PhoneAuthProvider.GetInstance(auth);
        phoneAuthProvider.VerifyPhoneNumber(phoneNumber, phoneAuthTimeoutMs, null,
          verificationCompleted: (cred) => {
              DebugLog("Phone Auth, auto-verification completed");
              auth.SignInWithCredentialAsync(cred).ContinueWith(HandleSigninResult);
          },
          verificationFailed: (error) => {
              DebugLog("Phone Auth, verification failed: " + error);
          },
          codeSent: (id, token) => {
              phoneAuthVerificationId = id;
              DebugLog("Phone Auth, code sent");
          },
          codeAutoRetrievalTimeOut: (id) => {
              DebugLog("Phone Auth, auto-verification timed out");
          });
    }
    public void VerifyReceivedPhoneCode()
    {
        var phoneAuthProvider = Firebase.Auth.PhoneAuthProvider.GetInstance(auth);
        // receivedCode should have been input by the user.
        var cred = phoneAuthProvider.GetCredential(phoneAuthVerificationId, receivedCode);
        auth.SignInWithCredentialAsync(cred).ContinueWith(HandleSigninResult);
    }
    public void hesabısil()
    {
        DeleteUserAsync();
    }
}
