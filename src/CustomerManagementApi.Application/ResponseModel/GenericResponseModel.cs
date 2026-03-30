namespace CustomerManagementApi.Application.ResponseModel
{
    /// <summary>
    /// Modelo de resposta genérico para paginar dados.
    /// </summary>
    /// <typeparam name="T">Tipo dos dados a serem retornados.</typeparam>
    public class GenericResponseModel<T>
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GenericResponseModel{T}"/>.
        /// </summary>
        /// <param name="page">Número da página atual.</param>
        /// <param name="pageSize">Número de itens por página.</param>
        /// <param name="data">Coleção de dados a serem retornados.</param>
        public GenericResponseModel(int page, int pageSize, IEnumerable<T> data)
        {
            Page = page;
            PageSize = pageSize;
            Data = data;
        }

        /// <summary>
        /// Obtém ou define o número da página atual.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Obtém ou define o número de itens por página.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Obtém ou define a coleção de dados a serem retornados.
        /// </summary>
        public IEnumerable<T> Data { get; set; }
    }
}
