using Microsoft.Extensions.Configuration;
using System;

namespace IntegrationHubApi.Domain.Configs
{
    public class CustomConfig
    {
        private bool ConfigFromEnviroment { get; set; }

        private IConfiguration Configuration { get; }

        public CustomConfig(IConfiguration configuration)
        {
            ConfigFromEnviroment = Convert.ToBoolean(configuration["ConfigFromEnviroment"]);

            Configuration = configuration;
        }

        public string ConnectionStringintegrationHubRead
        {
            get
            {
                return GetString("IntegrationApiRead_SQL");
            }
        }

        public string ConnectionStringintegrationHubWrite
        {
            get
            {
                return GetString("IntegrationApiWrite_SQL");
            }
        }

        public int ENVIROMENT_ID
        {
            get
            {
                return GetInt("ENVIROMENT_ID");
            }
        }

        public int DEBUG_MODE
        {
            get
            {
                return GetInt("DEBUG_MODE");
            }
        }

        public string FIREBASE_TOKEN
        {
            get
            {
                return GetString("FIREBASE_TOKEN");
            }

        }

        public string FIREBASE_URL
        {
            get
            {
                return GetString("FIREBASE_URL");
            }

        }

        #region New Relic 

        public string NewRelicUrl
        {
            get
            {
                if (ConfigFromEnviroment)
                    return Environment.GetEnvironmentVariable("NewRelic_Url");
                else
                    return Configuration["NewRelic_Url"];
            }
        }

        public string NewRelicService
        {
            get
            {
                if (ConfigFromEnviroment)
                    return Environment.GetEnvironmentVariable("NewRelic_Service");
                else
                    return Configuration["NewRelic_Service"];
            }
        }

        public string NewRelicGuids
        {
            get
            {
                if (ConfigFromEnviroment)
                    return Environment.GetEnvironmentVariable("NewRelic_Guids");
                else
                    return Configuration["NewRelic_Guids"];
            }
        }

        public string NewRelicKey
        {
            get
            {
                if (ConfigFromEnviroment)
                    return Environment.GetEnvironmentVariable("NewRelic_Key");
                else
                    return Configuration["NewRelic_Key"];
            }
        }


        #endregion

        public string MicrosoftTeamsTokenUrl
        {
            get
            {
                return GetString("Teams_TokenUrl");
            }
        }
        public string MicrosoftTeamsScope
        {
            get
            {
                return GetString("Teams_Scope");
            }
        }
        public string MicrosoftTeamsCreateUrl
        {
            get
            {
                return GetString("Teams_CreateUrl");
            }
        }

        public string MicrosoftTeamsAuthorizeUrl
        {
            get
            {
                return GetString("Teams_AuthorizeUrl");
            }
        }
        public string MicrosoftTeamsDefaultConsumer
        {
            get
            {
                return GetString("Teams_DefaultConsumer");
            }
        }

        public string MicrosoftTeamsKey
        {
            get
            {
                return GetString("Teams_Key");
            }
        }

        private int GetInt(string param)
        {
            if (ConfigFromEnviroment)
                return Convert.ToInt32(Environment.GetEnvironmentVariable(param));
            else
                return Convert.ToInt32(Configuration[param]);
        }

        private string GetString(string param)
        {
            if (ConfigFromEnviroment)
                return Environment.GetEnvironmentVariable(param);
            else
                return Configuration[param];
        }


    }
}