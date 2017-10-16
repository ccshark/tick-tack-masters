using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AuthenticatePlayer : MonoBehaviour 
{
	public Text userNameInput, passwordInput; // these are set through the editor

	public void AuthorizePlayerBttn()
	{
		Debug.Log (userNameInput.text);
		Debug.Log (passwordInput.text);
		Debug.Log ("Authorizing Player...");
        new GameSparks.Api.Requests.AuthenticationRequest ()
			.SetUserName (userNameInput.text)
			.SetPassword (passwordInput.text)
			.Send ((response) => {

				if(!response.HasErrors)
				{
					Debug.Log("Player Authenticated... \n User Name: "+response.DisplayName);
					Application.LoadLevel(1);
				}
				else
				{
					Debug.Log("Error Authenticating Player... \n "+response.Errors.JSON.ToString());
				}

			});
	}

	public void AuthenticateDeviceBttn()
	{
		Debug.Log ("Authenticating Device...");
		new GameSparks.Api.Requests.DeviceAuthenticationRequest ()
			.SetDisplayName ("Randy")
			.Send ((response) => {

				if(!response.HasErrors)
				{
					Debug.Log("Device Authenticated...");
				}
				else 
				{
					Debug.Log("Error Authenticating Device...");
				}
			});
	}

    public void register() {
        Application.LoadLevel(2);
    }
}

