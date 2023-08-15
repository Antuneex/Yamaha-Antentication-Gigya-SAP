using ApiHelper.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;

namespace IntegrationHubApi.Services
{
    public class BaseMediatR
    {
        public readonly BaseParameter _parameters;

        public BaseMediatR(IHttpContextAccessor httpContextAccessor)
        {
            _parameters = new BaseParameter(
                Convert.ToInt32(httpContextAccessor.HttpContext.Request.Headers["x-authenticated-userid"]),
                httpContextAccessor.HttpContext.Request.Headers["X-Consumer-Username"].ToString()
            );
        }
    }
}