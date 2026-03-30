using CustomerManagementApi.Application.Extensions;
using CustomerManagementApi.Domain.Exceptions;
using CustomerManagementApi.Domain.ValueObjects;
using System;
using Xunit;

namespace CustomerManagementApi.Tests.Domain
{
    public class StringExtensionTests
    {
        [Theory]
        [InlineData("usuario@dominio.com", true)]
        [InlineData("usuariousuario@dominiodominio.com", true)]
        [InlineData("usuario.nome@dominio.com.br", true)]
        [InlineData("usuario-nome@sub.dominio.org", true)]
        [InlineData("usuario@dominio.travel", true)]
        [InlineData("usuario@[192.168.0.1]", true)]
        [InlineData("usuario@dominio", false)]
        [InlineData("@dominio.com", false)]
        [InlineData("usuario@.com", false)]
        [InlineData("usuario@dominio..com", false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData(null, false)]
        public void IsValidEmail_ShouldValidateCorrectly(string email, bool expected)
        {
            try
            {
                Email.Create(email);

                Assert.True(expected);
            }
            catch (Exception ex)
            {
                Assert.True(ex is DomainException);
                Assert.False(expected);
            }
        }

        [Theory]
        [InlineData("11985107295", true)]
        [InlineData("11 985107295", true)]
        [InlineData("11 98510-7295", true)]
        [InlineData("(11)98510-7295", true)]
        [InlineData("119851072", false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData(null, false)]
        public void IsValidPhone_ShouldReturnTrueForValidPhone(string phone, bool expected)
        {
            try
            {
                Phone.Create(phone);

                Assert.True(expected);
            }
            catch (Exception ex)
            {
                Assert.True(ex is DomainException);
                Assert.False(expected);
            }
        }

        [Theory]
        [InlineData("11985107295", "(11)98510-7295")]
        [InlineData("11 985107295", "(11)98510-7295")]
        [InlineData("11 98510-7295", "(11)98510-7295")]
        [InlineData("(11)98510-7295", "(11)98510-7295")]
        [InlineData("21987654321", "(21)98765-4321")]
        [InlineData("119851072", "")]
        [InlineData("", "")]
        [InlineData("   ", "")]
        [InlineData(null, "")]
        public void FormatPhone_ShouldFormatPhoneCorrectly(string phone, string expectedFormat)
        {
            try
            {
                var phoneFormat = Phone.Create(phone).Format();

                Assert.Equal(phoneFormat, expectedFormat);
            }
            catch (Exception ex)
            {
                Assert.True(ex is DomainException);
            }
        }

        [Theory]
        [InlineData("44207833706", true)]
        [InlineData("442.078.337-06", true)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        public void IsValidCPF_ShouldReturnTrueForValidCPF(string cpf, bool expected)
        {
            // Act
            var result = Cpf.IsValid(cpf);
            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("77238254000177", true)]
        [InlineData("77.238.254/0001-77", true)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        public void IsValidCNPJ_ShouldReturnTrueForValidCNPJ(string cnpj, bool expected)
        {
            // Act
            var result = Cnpj.IsValid(cnpj);
            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("OR311229", true)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        public void IsValidPassport_ShouldReturnTrueForValidPassport(string passport, bool expected)
        {
            // Act
            var result = Passport.IsValid(passport);
            // Assert
            Assert.Equal(expected, result);
        }
    }
}
