using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAuthenticator
{
  public void LoginWithDeviceID();
  public void FacebookLogin();
  public void GoogleLogin();
  public void AppleLogin();
  

}
