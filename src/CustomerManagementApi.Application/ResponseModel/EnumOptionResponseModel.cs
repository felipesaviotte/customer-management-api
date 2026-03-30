namespace CustomerManagementApi.Application.ResponseModel
{
    /// <summary>
    /// Modelo de resposta para opções de enum.
    /// </summary>
    public class EnumOptionResponseModel
    {
        /// <summary>
        /// Valor inteiro do enum.
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Descrição do enum.
        /// </summary>
        public required string Description { get; set; }
    }
}
