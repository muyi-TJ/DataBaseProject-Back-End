using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Back_End
{
    public class Message
    {
        public readonly string[] msgType = new string[2] { "invalid", "success" };

        public int errorCode { get; set; }

        public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();

        public string msg { get; set; }

        public string ReturnJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }


    public class LoginMessage:Message
    {
        public LoginMessage()
        {
            errorCode = 400;
            data.Add("loginState", false);
            data.Add("userName", null);
            data.Add("userAvatar", null);
            msg = msgType[0];
        }
    }

    public class CheckPhoneMessage:Message
    {
        public CheckPhoneMessage()
        {
            errorCode = 400;
            data.Add("phoneunique", false);
            msg = msgType[0];
        }
    }

    public class RegisterMessage:Message
    {
        public RegisterMessage()
        {
            errorCode = 400;
            data.Add("registerSate", false);
            msg = msgType[0];
        }
    }

    public class CustomerDetailMessage:Message
    {
        public CustomerDetailMessage()
        {
            errorCode = 404;
            data.Add("userNickName", null);
            data.Add("userAvatar", null);
            data.Add("evalNum", null);
            data.Add("userGroupLevel", null);
            data.Add("emailTag", false);
            data.Add("userScore", null);
            data.Add("registerDate", null);
            data.Add("hostCommentList", null);
            msg = msgType[0];
        }
    }

    public class VCcodeMessage:Message
    {
        public VCcodeMessage()
        {
            errorCode = 400;
            data.Add("sendstate", false);
            msg = msgType[0];
        }
    }
}
