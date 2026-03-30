using CustomerManagementApi.Application.Ports.Inbound;
using CustomerManagementApi.Application.RequestModel;
using CustomerManagementApi.Application.ResponseModel;
using CustomerManagementApi.Domain.Enums;
using System.ComponentModel;
using static CustomerManagementApi.Application.Commons.CommonsConstants;

namespace CustomerManagementApi.Api.Controllers.V1;

/// <summary>
/// Controller para gerenciar operações relacionadas a Clientes
/// </summary>
[ExcludeFromCodeCoverage]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customer")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerQueryService _customerService;
    private readonly ISaveCustomerUseCase _saveCustomerUseCase;
    private readonly IDeleteCustomerUseCase _deleteCustomerUseCase;

    /// <summary>
    /// Construtor do CustomerController, recebe as dependências via injeção de dependência
    /// </summary>
    /// <param name="customerService">Serviço para consulta de clientes</param>
    /// <param name="saveCustomerUseCase">Caso de uso para criação de clientes</param>
    /// <param name="deleteCustomerUseCase">Caso de uso para exclusão de clientes</param>
    public CustomerController(ICustomerQueryService customerService, ISaveCustomerUseCase saveCustomerUseCase, IDeleteCustomerUseCase deleteCustomerUseCase)
    {
        _customerService = customerService;
        _saveCustomerUseCase = saveCustomerUseCase;
        _deleteCustomerUseCase = deleteCustomerUseCase;
    }

    /// <summary>
    /// Obtém uma lista de clientes paginada
    /// </summary>
    /// <param name="page">Número da página a ser retornada (padrão é 1)</param>
    /// <param name="pageSize">Número de clientes por página (padrão é 100, Max 500)</param>
    /// <param name="name">Nome do cliente</param>
    /// <param name="status">Status do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de clientes</returns>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(typeof(GenericResponseModel<CustomerResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomers(int page = PaginationDefaults.DefaultPage, int pageSize = PaginationDefaults.DefaultPageSize, string? name = null, CustomerStatus? status = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _customerService.GetCustomers(page, pageSize, name, status, cancellationToken));
    }

    /// <summary>
    /// Obtém os detalhes de um cliente específico pelo ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(GenericResponseModel<CustomerResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomersById(string id, CancellationToken cancellationToken = default)
    {
        return Ok(await _customerService.GetCustomersById(id, cancellationToken));
    }

    /// <summary>
    /// Obtém a contagem total de clientes
    /// </summary>
    /// <param name="status">Status do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Número total de clientes</returns>
    [HttpGet]
    [Route("count")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomersCount(CustomerStatus? status = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _customerService.Count(status, cancellationToken));
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    /// <param name="createCustomerRequestModel"></param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista o cliente criado com Identificador</returns>
    [HttpPost]
    [Route("")]
    [ProducesResponseType(typeof(CustomerResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCustomers(CustomerRequestModel createCustomerRequestModel, CancellationToken cancellationToken = default)
    {
        var result = await _saveCustomerUseCase.Executar(string.Empty, createCustomerRequestModel, cancellationToken);
        return Created($"/{result.Id}", result);
    }

    /// <summary>
    /// Atualiza os dados de um cliente
    /// </summary>
    /// <param name="id">Identificador do cliente</param>
    /// <param name="updateCustomerRequestModel">Dados a serem atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados atualizados do cliente</returns>
    [HttpPut]
    [Route("{id}")]
    [ProducesResponseType(typeof(CustomerResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCustomer(string id, CustomerRequestModel updateCustomerRequestModel, CancellationToken cancellationToken = default)
    {
        var result = await _saveCustomerUseCase.Executar(id, updateCustomerRequestModel, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retorna as opções do enum CustomerStatus
    /// </summary>
    /// <returns>Lista de id e descrição dos status possíveis</returns>
    [HttpGet]
    [Route("status")]
    [ProducesResponseType(typeof(IEnumerable<EnumOptionResponseModel>), StatusCodes.Status200OK)]
    public IActionResult GetStatusOptions()
    {
        var options = Enum.GetValues<CustomerStatus>()
            .Select(e => new EnumOptionResponseModel
            {
                Id = (int)e,
                Description = e.GetType()
                    .GetField(e.ToString())!
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .Cast<DescriptionAttribute>()
                    .FirstOrDefault()?.Description ?? e.ToString()
            });
        return Ok(options);
    }

    /// <summary>
    /// Retorna as opções do enum DocumentType
    /// </summary>
    /// <returns>Lista de id e descrição dos tipos de documento possíveis</returns>
    [HttpGet]
    [Route("document_types")]
    [ProducesResponseType(typeof(IEnumerable<EnumOptionResponseModel>), StatusCodes.Status200OK)]
    public IActionResult GetDocumentTypesOptions()
    {
        var options = Enum.GetValues<DocumentType>()
            .Select(e => new EnumOptionResponseModel
            {
                Id = (int)e,
                Description = e.GetType()
                    .GetField(e.ToString())!
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .Cast<DescriptionAttribute>()
                    .FirstOrDefault()?.Description ?? e.ToString()
            });
        return Ok(options);
    }

    /// <summary>
    /// Remove um cliente pelo ID
    /// </summary>
    /// <param name="id">Identificador do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>204 No Content</returns>
    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCustomer(string id, CancellationToken cancellationToken = default)
    {
        await _deleteCustomerUseCase.Executar(id, cancellationToken);
        return NoContent();
    }
}
