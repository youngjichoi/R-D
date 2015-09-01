using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ComplexServerCommon;
using ExitGames.Client.Photon;

public class CharacterCreate : View {

   

    void Awake()
    {
        Controller = new CharacterCreateController(this);

    }

    private CharacterCreateController _controller;

    public override IViewController Controller
    {
        get { return _controller; }
        protected set { _controller = value as CharacterCreateController; }
    }

     string characterName = "";
     string sex = "";
     string characterClass = "";

    void OnGUI()
    {
        //incase char name is in use or they use fowl language
        if (_controller.ShowErrorDialog)
        {
            //TODO - create a dialog to show the message error later
            GUI.Label(new Rect(10,300,505,40), _controller.Error );
        }

        GUI.Label(new Rect(120,116,100,100),"Name:" );
        characterName = GUI.TextField(new Rect(200, 116, 200, 20), characterName, 25);

        GUI.Box(new Rect(10,10,100,300), "Classes");

        if (GUI.Button(new Rect(20, 50, 80, 50), "Fighter" ))
        {
            characterClass = "Fighter";
        }
        if (GUI.Button(new Rect(20, 110, 80, 50), "Mage" ))
        {
            characterClass = "Mage";
        }
        if (GUI.Button(new Rect(20, 170, 80, 50), "Rogue" ))
        {
            characterClass = "Rogue";
        }
        if (GUI.Button(new Rect(20, 230, 80, 50), "Cleric" ))
        {
            characterClass = "Cleric";
        }

        if (GUI.Button(new Rect(150, 170, 80, 50), "Male"))
        {
            sex = "M";
        }
        if (GUI.Button(new Rect(240, 170, 80, 50), "Female"))
        {
            sex = "F";
        }

        if (!_controller.SendingCreate &&
            (GUI.Button(new Rect(200, 265, 100, 25),
                "Create") || (Event.current.type == EventType.KeyDown && Event.current.character == '\n')))
        {
            if(!string.IsNullOrEmpty(characterName) && !string.IsNullOrEmpty(sex) && !string.IsNullOrEmpty(characterClass))
            {
                _controller.SendCreateCharacter(characterName, sex, characterClass);
            }
        }
        if (!_controller.SendingCreate && GUI.Button(new Rect(305, 265, 100, 25), "Cancel"))
        {
            Application.LoadLevel("CharacterSelect");
        }
    }

}
