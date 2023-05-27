﻿namespace Web.ApiGateway.Models.LogModels
{
    public class ElasticSearchOptions
    {
        public string ConnectionString { get; set; }
        public string AuthUserName { get; set; }
        public string AuthPassword { get; set; }
        public string DefaultIndex { get; set; }
        public string LogIndex { get; set; }
    }
}
