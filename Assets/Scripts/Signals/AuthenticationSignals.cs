
 using Facebook.Unity;
 using PlayFab;

 public class OnPlayFabLoginSuccessesSignal
 {
     public string PlayerID;
     public PlayFabAuthenticationContext AuthenticationContext;
 }
public class OnLoginFailedSignal
{
 public string Reason;
}

 public class OnGoogleSignInSuccessSignal
 {
     public string AuthCode;
    
     
 }

 public class OnGoogleSignInFailed
 {
     public string Reason;
 }

 public class OnFacebookLoginSuccessSignal
 {
     public AccessToken AccessToken;
 }

 public class OnFacebookLoginFailedSignal
 {
     public bool Canceled;
 }

 public class OnAppleLoginSuccessSignal
 {
     public string IdToken;
     public string Email;
     public string Name;
 }
 public class OnAppleLoginFailedSignal
 {
     public string Reason;
 }