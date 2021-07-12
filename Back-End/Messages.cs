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


        public string ReturnJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }


    public class LoginMessage:Message
    {
        public LoginMessage()
        {
            errorCode = 300;
            data.Add("loginState", false);
            data.Add("userName", null);
            data.Add("userAvatar", null);
        }
    }

    public class CheckPhoneMessage:Message
    {
        public CheckPhoneMessage()
        {
            errorCode = 300;
            data.Add("phoneunique", false);
        }
    }

    public class RegisterMessage:Message
    {
        public RegisterMessage()
        {
            errorCode = 300;
            data.Add("registerState", false);
        }
    }

    public class CustomerDetailMessage:Message
    {
        public CustomerDetailMessage()
        {
            errorCode = 400;
            data.Add("userNickName", null);
            data.Add("userAvatar", null);
            data.Add("evalNum", null);
            data.Add("userGroupLevel", null);
            data.Add("emailTag", false);
            data.Add("userScore", null);
            data.Add("registerDate", null);
            data.Add("hostCommentList", null);
            data.Add("mood", null);
            data.Add("userBirthDate", null);
            data.Add("userSex", null);
        }
    }

    public class VCcodeMessage:Message
    {
        public VCcodeMessage()
        {
            errorCode = 300;
            data.Add("sendstate", false);
        }
    }

    public class AdminMessage:Message
    {
        AdminMessage()
        {
            errorCode = 300;
            data.Add("avatar", null);
            data.Add("ID", null);
            data.Add("name",null);
        }
    }

    public class VerifyControllerMessage:Message
    {
        public VerifyControllerMessage()
        {
            errorCode = 300;
            data.Add("verifycode", null);
            data.Add("codeimg", null);
        }
    }

    public class ChangePasswordMessage:Message
    {
        public ChangePasswordMessage()
        {
            errorCode = 400;
            data.Add("changestate", false);
        }
            
    }

    public class GetStaysByPosMessage:Message
    {
        public GetStaysByPosMessage()
        {
            errorCode = 300;
            data.Add("staynum", 0);
            data.Add("stayinfo", null);
        }
    }

    public class GetStayByPageMessage:Message
    {
        public GetStayByPageMessage()
        {
            errorCode = 400;
            data.Add("examineStayList", null);
        }
    }

    public class GetStayMessage : Message {
        public GetStayMessage() {
            errorCode = 300;
            //this.data.Add("stayList", null);
        }
    }

    public class GetStayByIdMessage:Message
    {
        public GetStayByIdMessage()
        {
            errorCode = 400;
            data.Add("detailedAddress", null);
            data.Add("stayType", null);
            data.Add("stayCapability", null);
            data.Add("roomList", null);
            data.Add("publicToliet", null);
            data.Add("publicBath", null);
            data.Add("hasBarrierFree", null);
            data.Add("stayPicList", null);
        }
    }

    public class GetReportByPageMessage:Message
    {
        public GetReportByPageMessage()
        {
            errorCode = 400;
            data.Add("reportList", null);
        }
    }

    public class GetReportByIdMessage:Message
    {
        public GetReportByIdMessage()
        {
            errorCode = 400;
            data.Add("orderId", null);
            data.Add("reportTime", null);
            data.Add("reportReason", null);
            data.Add("hostId", null);
            data.Add("stayId", null);
            data.Add("hostCredit", null);
        }
    }

    public class IDVerifyMessage : Message {
        public IDVerifyMessage() {
            errorCode = 400;
            data.Add("verifyResult", 0);
            data.Add("trueName", null);
            data.Add("trueID", null);
        }
    }

    public class GetStayByLngAndLatMessage:Message
    {
        public GetStayByLngAndLatMessage()
        {
            errorCode = 300;
            data.Add("stayPositionNum", 0);
            data.Add("stayPositionInfo", null);
        }
    }

    public class GetNearByPageMessage:Message
    {
        public GetNearByPageMessage()
        {
            errorCode = 400;
            data.Add("nearbyList", null);
        }
    }

    public class UploadStayExamineMessage:Message
    {
        public UploadStayExamineMessage()
        {
            errorCode = 400;
            data.Add("isSuccess", false);
        }
    }

    public class GetStayTypeMessage : Message {
        public GetStayTypeMessage() {
            errorCode = 300;
            data.Add("typeList", null);
        }
    }


    public class GetHostInfoMessage:Message {
        public GetHostInfoMessage() {
            errorCode = 400;
            data.Add("hostAvatar", null);
            data.Add("hostNickName", null);
            data.Add("hostRealName", null);
            data.Add("hostSex", null);
            data.Add("hostLevel", null);
            data.Add("hostLevelName", null);
            data.Add("hostScore", null);
            data.Add("publishedNum", null);
            data.Add("unpublishedNum", null);
            data.Add("pendingReviewNum", null);
            data.Add("reviewNum", null);
            data.Add("emailTag", null);
            data.Add("phoneTag", null);
            data.Add("authenticationTag", null);
            data.Add("hostCreateTime", null);
            data.Add("averageRate", null);
            data.Add("unpublishedStayInfo", null);
            data.Add("pendingStayInfo", null);
            data.Add("publishedHouseInfo", null);
        }
    }
}
