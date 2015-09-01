
    using System.Collections.Generic;
    using ComplexServerCommon;
    using ExitGames.Client.Photon;


//send shit to server from here
public class LoginController : ViewController
    {
        public LoginController(View controlledView, byte subOperationCode = 0) : base(controlledView, subOperationCode)
        {
        OperationHandlers.Add((byte)MessageSubCode.Login,new LoginHandler(this));
        }

        public void SendLogin(string username, string password)
        {
        var Param = new Dictionary<byte, object>()
        {
            {(byte) ClientParameterCode.UserName, username},
            {(byte) ClientParameterCode.Password, password},
            {(byte) ClientParameterCode.SubOperationCode, (int) MessageSubCode.Login}
        };
        SendOperation(new OperationRequest() { OperationCode = (byte)ClientOperationCode.Login, Parameters = Param }, true, 0, false);
    }

    public void SendRegister(string username, string password, string email)
    {
        var Param = new Dictionary<byte, object>()
        {
            {(byte) ClientParameterCode.UserName, username},
            {(byte) ClientParameterCode.Password, password},
            {(byte) ClientParameterCode.Email, email},
            {(byte) ClientParameterCode.SubOperationCode, (int) MessageSubCode.Register}
        };

        SendOperation(new OperationRequest() {OperationCode = (byte)ClientOperationCode.Login, Parameters = Param}, true, 0, false);
    }

    
}

