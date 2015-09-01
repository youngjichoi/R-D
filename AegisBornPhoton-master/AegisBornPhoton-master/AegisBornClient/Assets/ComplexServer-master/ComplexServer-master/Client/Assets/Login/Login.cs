using System;
using UnityEngine;
using System.Collections;
using System.Xml.Schema;
using System.Xml.Xsl;


//put gui type stuff here
public class Login : View
{

    public string ServerAddress;
    public string ApplicationName;
    public string UserName;
    public string Password;
    public string LoginUserName;
    public string LoginPassword;
    public string Email;
    public bool logginIn = false;
    public override void Awake()
    {
        
    }

// Use this for initialization
        void Start ()
        {

        Controller = new LoginController(this);
        PhotonEngine.UseExistingOrCreateNewPhotonEngine(ServerAddress, ApplicationName);
        //   string[] arglist = new string[0];
        //   if (Application.srcValue.Split(new[] {"?"}, StringSplitOptions.RemoveEmptyEntries).Length > 1)
        //   {
        //       arglist = Application.srcValue.Split(new [] {"?"}, StringSplitOptions.RemoveEmptyEntries)[1].Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
        //        ;
        //    }
        //    if (arglist.Length == 2)
        //   {
        //   _controller.SendLogin(arglist[0], arglist[1]);
        //
        //}
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        UserName = GUI.TextField(new Rect(5, 5, 300, 30), UserName, 64);
        Password = GUI.TextField(new Rect(5, 40, 300, 30), Password, 64);
        Email = GUI.TextField(new Rect(5, 75, 300, 30), Email, 64);
        if (GUI.Button(new Rect(5, 110, 300, 30), "Register") && UserName != "" && Password != "" && Email != "")
        {
            _controller.SendRegister(UserName, Password, Email);
        }

        GUI.Label(new Rect(5, 145, 300, 30), PhotonEngine.Instance.State.ToString() );

        LoginUserName = GUI.TextField(new Rect(5, 180, 300, 30), LoginUserName, 64);
        LoginPassword = GUI.TextField(new Rect(5, 215, 300, 30), LoginPassword, 64);
        if (GUI.Button(new Rect(5, 250, 300, 30), "Login") && LoginUserName != "" && LoginPassword != "")
        {
            _controller.SendLogin(LoginUserName, LoginPassword);
        }

    }

    private LoginController _controller;
    public override IViewController Controller {get { return (IViewController) _controller; } protected set
    {
        _controller = value as LoginController;
    } }
}
