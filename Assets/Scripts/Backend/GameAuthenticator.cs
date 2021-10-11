using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameAuthenticator : MonoBehaviour
{

    [Inject] private IAuthenticator authenticator;
    void Start()
    { 
        authenticator.LoginWithGoogle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
