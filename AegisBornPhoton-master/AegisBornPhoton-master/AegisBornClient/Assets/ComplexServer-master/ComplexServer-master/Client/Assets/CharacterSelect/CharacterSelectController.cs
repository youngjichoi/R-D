using System.Collections.Generic;
using System.Diagnostics;
using ComplexServerCommon;
using ComplexServerCommon.MessageObjects;
using ExitGames.Client.Photon;

public class CharacterSelectController : ViewController {

    public CharacterSelectController(View controlledView) : base(controlledView)
    {
        CharacterList = new List<CharacterListItem>();
        OperationHandlers.Add((byte)MessageSubCode.ListCharacters, new CharacterListHandler(this));
        OperationHandlers.Add((byte) MessageSubCode.SelectCharacter, new SelectCharacterHandler(this));
    }

    public List<CharacterListItem> CharacterList { get; protected set; }
     
    public void SendGetList()
    {
        
        Dictionary<byte, object> parametes = new Dictionary<byte, object>();

        parametes.Add((byte)ClientParameterCode.SubOperationCode, MessageSubCode.ListCharacters);
        
        OperationRequest request = new OperationRequest {OperationCode = (byte)ClientOperationCode.Login, Parameters = parametes};

        SendOperation(request, true, 0, true); 
    }

    public void SendCharacterSelect(int characterId)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>
        {
            {(byte) ClientParameterCode.SubOperationCode, MessageSubCode.SelectCharacter},
            {(byte) ClientParameterCode.CharacterId, characterId}
        };

        OperationRequest request = new OperationRequest
        {
            OperationCode = (byte) ClientOperationCode.Login,
            Parameters = parameters
        };

        SendOperation(request, true, 0, true);

    }
}
