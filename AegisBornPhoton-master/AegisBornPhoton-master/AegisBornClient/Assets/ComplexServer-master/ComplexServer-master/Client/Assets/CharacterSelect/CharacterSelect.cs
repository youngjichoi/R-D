using UnityEngine;
using ComplexServerCommon.MessageObjects;

public class CharacterSelect : View {

	// Use this for initialization
	void Awake ()
	{
	    Controller = new CharacterSelectController(this);
	    _controller.SendGetList();


	}

    private CharacterSelectController _controller;

    public override IViewController Controller
    {
        get { return _controller; }
        protected set { _controller = value as CharacterSelectController; }
    }
	
	// Update is called once per frame
    public void OnGUI()
    {
        GUI.Box(new Rect(300,10,100,300), "Characters" );
        int yPos = 50;
        foreach (CharacterListItem characterListItem in _controller.CharacterList)
        {
            if (GUI.Button(new Rect(310, yPos, 80, 50), characterListItem.Name))
            {
                _controller.SendCharacterSelect(characterListItem.Id);
            }
            yPos += 60;
        }
        if (GUI.Button(new Rect(100, 165, 100, 125), "New Character"))
        {
            Application.LoadLevel("CharacterCreate");
        }
        if (GUI.Button(new Rect(100, 95, 100, 25), "Back"))
        {
            Application.LoadLevel("Login");
        }
        {
            
        }
    }
	
	}

