
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using ComplexServerCommon;
    using ComplexServerCommon.MessageObjects;
    using ExitGames.Client.Photon;

public class CharacterCreateController : ViewController
    {
        public CharacterCreateController(View controlledView, byte subOperationCode = 0) : base(controlledView, subOperationCode)
        {
                OperationHandlers.Add((byte)MessageSubCode.CreateCharacter, new CharacterCreateHandler(this));

        }

    public string Error { get; set; }
    public bool SendingCreate { get; set; }
    public bool ShowErrorDialog { get; set; }
    public void SendCreateCharacter(string characterName, string sex, string characterClass)
    {
        Error = "";
        SendingCreate = true;
        ShowErrorDialog = false;
        CharacterCreateDetails details = new CharacterCreateDetails { CharacterName = characterName, Sex = sex, CharacterClass = characterClass};
        XmlSerializer mysSerializer = new XmlSerializer(typeof(CharacterCreateDetails));
        StringWriter outStream = new StringWriter();
        mysSerializer.Serialize(outStream, details);

        Dictionary<byte, object> parameters = new Dictionary<byte, object>()
        {
            {(byte) ClientParameterCode.SubOperationCode, MessageSubCode.CreateCharacter},
            {(byte) ClientParameterCode.CharacterCreateDetails, outStream.ToString()}
        };

        OperationRequest request = new OperationRequest { OperationCode = (byte)ClientOperationCode.Login, Parameters = parameters };
        SendOperation(request, true, 0, false);

    }
}

