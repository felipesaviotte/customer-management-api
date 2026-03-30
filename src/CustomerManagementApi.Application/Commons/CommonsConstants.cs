namespace CustomerManagementApi.Application.Commons
{
    /// <summary>
    /// Classe que contém constantes relacionadas à paginação.
    /// </summary>
    public static class CommonsConstants
    {
        /// <summary>
        /// Indica se a aplicação deve usar um banco de dados em memória, verificando a variável de ambiente "USE_INMEMORY_DB". Se a variável estiver definida como "true" (ignorando maiúsculas/minúsculas), a aplicação usará um banco de dados em memória, caso contrário, usará o MongoDB. Isso é útil para facilitar testes e desenvolvimento local sem a necessidade de configurar um banco de dados real.
        /// </summary>
        public static bool UseInMemoryDb
        {
            get
            {
                var dbMemory = Environment.GetEnvironmentVariable("USE_INMEMORY_DB");
                if (dbMemory == null)
                    return false;

                return bool.TryParse(dbMemory, out var result) && result;
            }
        }

        /// <summary>
        /// Classe que contém os valores padrão para paginação.
        /// </summary>
        public static class PaginationDefaults
        {
            /// <summary>
            /// Página padrão para a paginação.
            /// </summary>
            public const int DefaultPage = 1;

            /// <summary>
            /// Tamanho padrão da página para a paginação.
            /// </summary>
            public const int DefaultPageSize = 100;

            /// <summary>
            /// Tamanho máximo da página para a paginação.
            /// </summary>
            public const int MaxPageSize = 500;
        }

        /// <summary>
        /// Cabeçalho HTTP que identifica a origem da solicitação.
        /// </summary>
        public const string ORIGIN_HEADER = "appOrigin";

        /// <summary>
        /// Ambiente atual da aplicação, recuperado da variável de ambiente "ASPNETCORE_ENVIRONMENT".
        /// </summary>
        public static string Env
        {
            get
            {
                var ambiente = GetNotNullEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToUpper();

                switch (ambiente)
                {
                    case "PROD" or "PRD" or "PRODUÇÃO" or "PRODUCTION":
                        return "prd";
                    case "QA" or "TEST" or "HOMOLOGAÇÃO" or "HOMOLOGATION":
                        return "tst";
                    case "DEV" or "DESENVOLVIMENTO" or "DEVELOPMENT":
                        return "dev";
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Configurações relacionadas ao MongoDB.
        /// </summary>
        public static class Mongo
        {
            /// <summary>
            /// Host do MongoDB.
            /// </summary>
            public static string Host => GetNotNullEnvironmentVariable("MONGODB_HOST");

            /// <summary>
            /// Usuário do MongoDB.
            /// </summary>
            public static string User => GetNotNullEnvironmentVariable("MONGODB_USER");

            /// <summary>
            /// Senha do MongoDB.
            /// </summary>
            public static string Password => GetNotNullEnvironmentVariable("MONGODB_PASSWORD");

            /// <summary>
            /// String de conexão para o MongoDB no ambiente de produção.
            /// </summary>
            public static string ConnectionString
            {
                get
                {
                    var connectionBase = "mongodb";
                    if (!Local) //Ambiente não local geralmente usa MongoDB Atlas, que requer o prefixo "+srv" na string de conexão
                    {
                        connectionBase += "+srv";
                    }
                    connectionBase += $"://{User}:{Password}@{Host}";

                    if (Local)
                        connectionBase += "/?authSource=admin";

                    return connectionBase;
                }
            }

            /// <summary>
            /// Lógica para determinar se o ambiente é local, verificando se o host contém "localhost". Isso é útil para ajustar a string de conexão do MongoDB, já que ambientes locais geralmente não usam MongoDB Atlas e podem ter configurações diferentes.
            /// </summary>
            public static bool Local => Host.IndexOf("localhost") != -1;

            /// <summary>
            /// Nome do banco de dados usado no MongoDB.
            /// </summary>
            public static string Database => $"db-customer-{Env}";

            /// <summary>
            /// Nome da coleção clientes no MongoDB.
            /// </summary>
            public static string CustomerCollection => "customers";
        }

        /// <summary>
        /// Configurações relacionadas ao Kafka.
        /// </summary>
        public static class Kafka
        {
            /// <summary>
            /// Servidor Bootstrap do Kafka.
            /// </summary>
            public static string BootstrapServer => GetNotNullEnvironmentVariable("BOOTSTRAP_SERVERS");

            /// <summary>
            /// Define se o Kafka deve usar SASL SSL para autenticação.
            /// Por padrão é true quando a variável não está definida, vazia ou com espaço em branco.
            /// </summary>
            public static bool UseSaslSsl { get; set; } =
                string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("KAFKA_USE_SASL_SSL")) || (bool.TryParse(Environment.GetEnvironmentVariable("KAFKA_USE_SASL_SSL"), out var useSaslSsl) && useSaslSsl);

            /// <summary>
            /// Nome de usuário para autenticação no Kafka.
            /// </summary>
            public static string SaslUsername => GetNotNullEnvironmentVariable("SASL_USERNAME");

            /// <summary>
            /// Senha para autenticação no Kafka.
            /// </summary>
            public static string SaslPassword => GetNotNullEnvironmentVariable("SASL_PASSWORD");

            /// <summary>
            /// Nome do tópico de links de pagamento no Kafka.
            /// </summary>
            public static string PaymentLinksTopicName => $"{Env}.events.meoo.paymentlinks";
        }

        /// <summary>
        /// Configurações relacionadas ao Schema Registry.
        /// </summary>
        public static class SchemaRegistry
        {
            /// <summary>
            /// URL do Schema Registry.
            /// </summary>
            public static string Url => GetNotNullEnvironmentVariable("KAFKA_CONFLUENT_SCHEMA_URL");

            /// <summary>
            /// Usuário do Schema Registry.
            /// </summary>
            public static string User => GetNotNullEnvironmentVariable("KAFKA_CONFLUENT_SCHEMA_KEY");

            /// <summary>
            /// Senha do Schema Registry.
            /// </summary>
            public static string Password => GetNotNullEnvironmentVariable("KAFKA_CONFLUENT_SCHEMA_SECRET");
        }

        /// <summary>
        /// URLs base usadas na aplicação AdmContratos.
        /// </summary>
        public static class ReceitaFederal
        {
            /// <summary>
            /// URL base para a API da Receita Fderal.
            /// </summary>
            public static string BaseUrl => GetNotNullEnvironmentVariable("BASE_URL_API_RECEITAFEDERAL");

            /// <summary>
            /// Api key para autenticação na API da Receita Federal.
            /// </summary>
            public static string ApiKey => GetNotNullEnvironmentVariable("API_KEY_API_RECEITAFEDERAL");
        }

        /// <summary>
        /// Recupera o valor de uma variável de ambiente, lançando uma exceção se ela não estiver definida.
        /// </summary>
        private static string GetNotNullEnvironmentVariable(string variable)
           => Environment.GetEnvironmentVariable(variable) ?? throw new InvalidOperationException($"Variável de ambiente {variable} não informada.");
    }
}
