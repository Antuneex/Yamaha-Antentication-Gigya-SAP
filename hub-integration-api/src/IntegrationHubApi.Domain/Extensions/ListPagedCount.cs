using System.Collections.Generic;

namespace IntegrationHubApi.Domain.Extensions
{
    public class ListPagedCount<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int Count { get; set; }

        public int CountTotal { get; set; }
    }
}
