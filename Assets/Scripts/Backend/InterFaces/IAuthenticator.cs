using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAuthenticator
{
  public void LoginWithDeviceID();
  public void LinkToFaceBook();
  public void LinkToGoogleAccount();
  public void LinkToAppleID();
  public void LoginWithGoogle();

  public void LoginWithFaceBook();
  

  public void LoginWithAppleID();

}
