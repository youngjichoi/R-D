using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using ComplexServerCommon;
using ComplexServerCommon.MessageObjects;
using ExitGames.Client.Photon;

public class CharacterListHandler : PhotonOperationHandler
{
    public CharacterListHandler(ViewController controller) : base(controller)
    {
    }

    public override byte Code
    {
        get { return (byte) MessageSubCode.ListCharacters; }
    }

    public override void OnHandleResponse(OperationResponse response)
    {
        _controller.DebugReturn(DebugLevel.WARNING, "Onhandle caught");

        var controller = _controller as CharacterSelectController;
        if (controller != null)
        {
            var values = response.Parameters[(byte) ClientParameterCode.CharacterList] as Hashtable;
            if (values != null)
            {
                _controller.DebugReturn(DebugLevel.WARNING, "Loading values");

                var mySerializer = new XmlSerializer(typeof (CharacterListItem));
                StringReader inStream;
                 string vc = values.Count.ToString();
                _controller.DebugReturn(DebugLevel.WARNING, vc);

                foreach (DictionaryEntry dictionaryEntry in values)
                {
                    inStream = new StringReader(Convert.ToString(dictionaryEntry.Value));
                    controller.CharacterList.Add((CharacterListItem) mySerializer.Deserialize(inStream));
                }
            }
            else
            {
                _controller.DebugReturn(DebugLevel.WARNING, "Hashtable was not returned");
            }
        }
    }
}

