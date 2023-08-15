using Gigya.Socialize.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Yamaha.ESB.Authentication.Models;

namespace Yamaha.ESB.Authentication.Controllers
{
    public class ValidateController : ApiController
    {
        // Post: api/Validate
        [HttpPost]
        public IHttpActionResult Validate([FromBody] ValidateModel request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string apiKey = ConfigurationManager.AppSettings["Gigya.ApiKey"];
            string secretKey = ConfigurationManager.AppSettings["Gigya.SecretKey"];

            string apiMethod = ConfigurationManager.AppSettings["Gigya.ApiMethod.GetUserInfo"];
            GSObject clientParams = new GSObject();
            clientParams.Put("UID", request.Uid);
            GSRequest requestObj = new GSRequest(apiKey, secretKey, apiMethod, clientParams, true, ConfigurationManager.AppSettings["Gigya.UserKey"]);

            requestObj.APIDomain = ConfigurationManager.AppSettings["Gigya.ApiDomain"];

            GSResponse response = requestObj.Send();

            GSObject resObj = response.GetData();
            var parsedResponse = JsonConvert.DeserializeObject<ValidateModel>(resObj.ToJsonString());
            if (parsedResponse.StatusCode == 200)
            {
                if (parsedResponse?.Email == request.Email && parsedResponse?.Uid == request.Uid)
                {
                    return Ok(true);
                }
                else
                {

                    return Ok(new { StatusCode = 401 });
                }
            }
            else
            {
                return Ok(parsedResponse);
            }
        }
    }
}
