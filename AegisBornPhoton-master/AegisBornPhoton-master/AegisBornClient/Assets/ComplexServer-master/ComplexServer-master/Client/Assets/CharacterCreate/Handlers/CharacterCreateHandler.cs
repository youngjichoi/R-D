
    using ComplexServerCommon;
    using ExitGames.Client.Photon;
using UnityEngine;

public class CharacterCreateHandler : PhotonOperationHandler
    {
        public CharacterCreateHandler(ViewController controller) : base(controller)
        {
        }

    public override byte Code
    {
        get { return (byte) MessageSubCode.CreateCharacter; }
    }

    public override void OnHandleResponse(OperationResponse response)
    {
        var controller = _controller as CharacterCreateController;
        if (controller != null)
        {
            controller.SendingCreate = false;
            if (response.ReturnCode != 0)
            {
                controller.ShowErrorDialog = true;
                controller.Error = response.DebugMessage;
                return;
            }
            Application.LoadLevel("CharacterSelect");
        }
    }
    }

