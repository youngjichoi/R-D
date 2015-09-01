using UnityEngine;
using ComplexServerCommon;
using ExitGames.Client.Photon;

public class LoginHandler : PhotonOperationHandler {

    public LoginHandler(LoginController controller) : base(controller)
    {
        
    }

    public override byte Code
    {
        get { return (byte) MessageSubCode.Login; }
    }

    public override void OnHandleResponse(OperationResponse response)
    {
        if (response.ReturnCode != 0)
        {
            return;
        }
        Debug.Log("asdf");
        Application.LoadLevel("CharacterSelect");
    }
}
