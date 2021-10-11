
 using PlayFab;

 public class OnPlayFabLoginSuccessesSignal
 {
     public string playerID;
     public PlayFabAuthenticationContext authenticationContext;
 }
public class OnLoginFailedSignal
{
 public string reason;
}

 public class OnGoogleSignInSuccessSignal
 {
     public string authCode;
    
     
 }

 public class OnGoogleSignInFailed
 {
     public string reason;
 }