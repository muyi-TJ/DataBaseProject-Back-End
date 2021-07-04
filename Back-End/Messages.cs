using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Back_End
{
    public class Message
    {
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
        }
    }

    public class CheckPhoneMessage:Message
    {
        public CheckPhoneMessage()
        {
            errorCode = 400;
            data.Add("phoneunique", false);
        }
    }

    public class RegisterMessage:Message
    {
        public RegisterMessage()
        {
            errorCode = 400;
            data.Add("registerSate", false);
        }
    }
}
