using UnityEngine;
using ComplexServerCommon;
    using ExitGames.Client.Photon;

public class SelectCharacterHandler : PhotonOperationHandler
    {
    public SelectCharacterHandler(ViewController controller) : base(controller)
    {

    }

    public override byte Code
    {
        get { return (byte) MessageSubCode.SelectCharacter; }
    }

    public override void OnHandleResponse(OperationResponse response)
    {
        if (response.ReturnCode == 0)
        {
            Application.LoadLevel("CharacterLoading");
        }
    }
    }

