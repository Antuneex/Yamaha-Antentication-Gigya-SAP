using Dapper;
using System.Data;
using System.Threading.Tasks;
using ApiHelper.Database.DapperHelper;
using IntegrationHubApi.Domain.Configs;
using IntegrationHubApi.Domain.Interfaces.Repositories;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams;

namespace IntegrationHubApi.Infra.Repositories
{
    /// <summary>
    /// Repositório do Integração de Apis
    /// </summary>
    public class TeamsMeetingRepository : BaseSQLConnection, ITeamsMeetingRepository
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="customConfig"></param>
        public TeamsMeetingRepository(CustomConfig customConfig) : base(customConfig.ConnectionStringintegrationHubWrite, customConfig.ENVIROMENT_ID)
        { }


        /// <summary>
        /// Recupera configuração do teams pelo consumidor
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public async Task<TeamsConfiguration> GetConfigurationByConsumer(string consumer)
        {
            string sql = @"SELECT	TOP 1 
		                            t.clientId, 
                                    t.secret, 
                                    t.tenantId, 
                                    t.IdConsumer, 
                                    t.redirecturi, 
                                    t.contauol 'Default', 
                                    login 'OrganizerLogin' , 
                                    password 'OrganizerPassword'
                            FROM	Integration.Teams t
                            JOIN	Integration.Consumer c
                              ON	c.idConsumer = t.IdConsumer
                           WHERE	c.Chave = @consumer";

            using var connection = GetSqlConnection;

            object param = new { consumer };

            var config = await connection.QueryFirstOrDefaultAsync<TeamsConfiguration>(sql, param: param, commandType: CommandType.Text);

            return config;
        }


        /// <summary>
        /// Registra atualização no banco de dados
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task Update(int tokenId, int userId)
        {
            return Task.FromResult("");
        }
    }
}
