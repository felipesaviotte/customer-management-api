using CustomerManagementApi.Api.Controllers.V1;
using CustomerManagementApi.Application.Ports.Inbound;
using CustomerManagementApi.Application.RequestModel;
using CustomerManagementApi.Application.ResponseModel;
using CustomerManagementApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CustomerManagementApi.Tests.Api;

public class CustomerControllerTests
{
    private readonly Mock<ICustomerQueryService> _customerQueryServiceMock;
    private readonly Mock<ISaveCustomerUseCase> _saveCustomerUseCaseMock;
    private readonly Mock<IDeleteCustomerUseCase> _deleteCustomerUseCaseMock;
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _customerQueryServiceMock = new Mock<ICustomerQueryService>();
        _saveCustomerUseCaseMock = new Mock<ISaveCustomerUseCase>();
        _deleteCustomerUseCaseMock = new Mock<IDeleteCustomerUseCase>();
        _controller = new CustomerController(_customerQueryServiceMock.Object, _saveCustomerUseCaseMock.Object, _deleteCustomerUseCaseMock.Object);
    }

    #region GetCustomers

    [Fact]
    public async Task GetCustomers_ShouldReturnOk_WithPaginatedResult()
    {
        var expected = new GenericResponseModel<CustomerResponseModel>(1, 10, []);
        _customerQueryServiceMock
            .Setup(s => s.GetCustomers(1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _controller.GetCustomers(1, 10);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetCustomers_WithNameFilter_ShouldPassNameToService()
    {
        var expected = new GenericResponseModel<CustomerResponseModel>(1, 10, []);
        _customerQueryServiceMock
            .Setup(s => s.GetCustomers(1, 10, "João", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _controller.GetCustomers(1, 10, "João");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
        _customerQueryServiceMock.Verify(s => s.GetCustomers(1, 10, "João", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetCustomersById

    [Fact]
    public async Task GetCustomersById_ShouldReturnOk_WhenCustomerExists()
    {
        var expected = CreateCustomerResponse();
        _customerQueryServiceMock
            .Setup(s => s.GetCustomersById("123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _controller.GetCustomersById("123");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetCustomersById_ShouldThrow_WhenCustomerNotFound()
    {
        _customerQueryServiceMock
            .Setup(s => s.GetCustomersById("999", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Cliente com ID '999' não encontrado."));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetCustomersById("999"));
    }

    #endregion

    #region GetCustomersCount

    [Fact]
    public async Task GetCustomersCount_ShouldReturnOk_WithCount()
    {
        _customerQueryServiceMock
            .Setup(s => s.Count(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(42L);

        var result = await _controller.GetCustomersCount();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(42L, okResult.Value);
    }

    #endregion

    #region CreateCustomers

    [Fact]
    public async Task CreateCustomers_ShouldReturnCreated_WhenValid()
    {
        var request = CreateCustomerRequest();
        var expected = CreateCustomerResponse();

        _saveCustomerUseCaseMock
            .Setup(s => s.Executar(string.Empty, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _controller.CreateCustomers(request);

        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(expected, createdResult.Value);
        Assert.Equal($"/{expected.Id}", createdResult.Location);
    }

    #endregion

    #region UpdateCustomer

    [Fact]
    public async Task UpdateCustomer_ShouldReturnOk_WhenValid()
    {
        var request = CreateCustomerRequest();
        var expected = CreateCustomerResponse();

        _saveCustomerUseCaseMock
            .Setup(s => s.Executar("123", request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _controller.UpdateCustomer("123", request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task UpdateCustomer_ShouldThrow_WhenCustomerNotFound()
    {
        var request = CreateCustomerRequest();

        _saveCustomerUseCaseMock
            .Setup(s => s.Executar("999", request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Cliente com ID '999' não encontrado."));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.UpdateCustomer("999", request));
    }

    #endregion

    #region Helpers

    private static CustomerRequestModel CreateCustomerRequest() => new()
    {
        Name = "João Silva",
        DocumentType = DocumentType.CPF,
        DocumentNumber = "12345678901",
        Email = "joao@email.com",
        Phone = "11999999999"
    };

    private static CustomerResponseModel CreateCustomerResponse() => new()
    {
        Id = "abc-123",
        Name = "João Silva",
        DocumentType = DocumentType.CPF,
        DocumentNumber = "12345678901",
        Email = "joao@email.com",
        Phone = "11999999999",
        Status = CustomerStatus.ATIVO
    };

    #endregion
}
