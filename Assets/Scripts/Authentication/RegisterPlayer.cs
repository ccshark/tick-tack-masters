using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RegisterPlayer : MonoBehaviour
{
	public Text displayNameInput, userNameInput, passwordInput;


	public void register() {
		Debug.Log("Register player");
		new GameSparks.Api.Requests.RegistrationRequest()
			.SetDisplayName(displayNameInput.text)
			.SetPassword(passwordInput.text)
			.SetUserName(userNameInput.text)
			.Send((response) => {
				if (!response.HasErrors) {
					Debug.Log("Player Registered");
                    authorizePlayer();
				}
				else
				{
					Debug.Log("Error Registering Player");
				}
			}
			);
	}

    public void back() {
        Application.LoadLevel(0);
    }


    private void authorizePlayer()
    {
        Debug.Log("Authorizing Player...");
        new GameSparks.Api.Requests.AuthenticationRequest()
            .SetUserName(userNameInput.text)
            .SetPassword(passwordInput.text)
            .Send((response) => {

                if (!response.HasErrors)
                {
                    Debug.Log("Player Authenticated... \n User Name: " + response.DisplayName);
                    Application.LoadLevel(1);
                }
                else
                {
                    Debug.Log("Error Authenticating Player... \n " + response.Errors.JSON.ToString());
                }

            });
    }
}
	

