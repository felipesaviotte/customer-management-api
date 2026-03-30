using CustomerManagementApi.Application.Ports.Inbound;
using CustomerManagementApi.Application.RequestModel;
using CustomerManagementApi.Application.ResponseModel;
using CustomerManagementApi.Domain.Entities;
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

    /// <summary>
    /// Construtor do CustomerController, recebe as dependências via injeção de dependência
    /// </summary>
    /// <param name="customerService">Serviço para consulta de clientes</param>
    /// <param name="saveCustomerUseCase">Caso de uso para criação de clientes</param>
    /// <param name="updateCustomerUseCase">Caso de uso para atualização de clientes</param>
    public CustomerController(ICustomerQueryService customerService, ISaveCustomerUseCase saveCustomerUseCase)
    {
        _customerService = customerService;
        _saveCustomerUseCase = saveCustomerUseCase;
    }

    /// <summary>
    /// Obtém uma lista de clientes paginada
    /// </summary>
    /// <param name="page">Número da página a ser retornada (padrão é 1)</param>
    /// <param name="pageSize">Número de clientes por página (padrão é 100, Max 500)</param>
    /// <param name="name">Nome do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de clientes</returns>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(typeof(GenericResponseModel<CustomerResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomers(int page = PaginationDefaults.DefaultPage, int pageSize = PaginationDefaults.DefaultPageSize, string? name = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _customerService.GetCustomers(page, pageSize, name, cancellationToken));
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
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Número total de clientes</returns>
    [HttpGet]
    [Route("count")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomersCount(CancellationToken cancellationToken = default)
    {
        return Ok(await _customerService.Count(cancellationToken));
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
}
