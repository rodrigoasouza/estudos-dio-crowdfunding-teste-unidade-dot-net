using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NToastNotify;
using Vaquinha.Domain;
using Vaquinha.Domain.Entities;
using Vaquinha.Domain.ViewModels;
using Vaquinha.MVC.Controllers;
using Vaquinha.Service;
using Vaquinha.Tests.Common.Fixtures;
using Xunit;

namespace Vaquinha.Unit.Tests.ControllerTests
{
    [Collection(nameof(DoacaoFixtureCollection))]
    public class DoacaoControllerTests : IClassFixture<DoacaoFixture>,
                                        IClassFixture<EnderecoFixture>,
                                        IClassFixture<CartaoCreditoFixture>
    {
        private readonly Mock<IDoacaoRepository> _DoacaoRepository;
        private readonly DoacaoFixture _DoacaoFixture;
        private readonly DoacaoViewModel _DoacaoModelValida;
        private readonly Mock<IMapper> _Mapper;
        private readonly IDomainNotificationService _DomainNotificationService;
        private readonly Mock<IToastNotification> _ToastNotification;
        private readonly IDoacaoService _DoacaoService;

        public DoacaoControllerTests(DoacaoFixture doacaoFixture, EnderecoFixture enderecoFixture, CartaoCreditoFixture cartaoCreditoFixture)
        {
            _DoacaoRepository = new Mock<IDoacaoRepository>();
            _DoacaoFixture = doacaoFixture;

            var doacaoValida = doacaoFixture.DoacaoValida();
            doacaoValida.AdicionarEnderecoCobranca(enderecoFixture.EnderecoValido());
            doacaoValida.AdicionarFormaPagamento(cartaoCreditoFixture.CartaoCreditoValido());

            _DoacaoModelValida = doacaoFixture.DoacaoModelValida();
            _DoacaoModelValida.EnderecoCobranca = enderecoFixture.EnderecoModelValido();
            _DoacaoModelValida.FormaPagamento = cartaoCreditoFixture.CartaoCreditoModelValido();

            _Mapper = new Mock<IMapper>();
            _Mapper.Setup(a => a.Map<DoacaoViewModel, Doacao>(_DoacaoModelValida)).Returns(doacaoValida);

            _DomainNotificationService = new DomainNotificationService();
            _ToastNotification = new Mock<IToastNotification>();
            _DoacaoService = new DoacaoService(_Mapper.Object, _DoacaoRepository.Object, _DomainNotificationService);
        }

        #region HTTPPOST

        [Trait("DoacaoController", "DoacaoController_Adicionar_RetornaDadosComSucesso")]
        [Fact]
        public void DoacaoController_Adicionar_RetornaDadosComSucesso()
        {
            // Arrange            
            var doacaoController = new DoacoesController(_DoacaoService, _DomainNotificationService, _ToastNotification.Object);

            // Act
            var retorno = doacaoController.Create(_DoacaoModelValida);

            _Mapper.Verify(a => a.Map<DoacaoViewModel, Doacao>(_DoacaoModelValida), Times.Once);
            _ToastNotification.Verify(a => a.AddSuccessToastMessage(It.IsAny<string>(), It.IsAny<LibraryOptions>()), Times.Once);

            retorno.Should().BeOfType<RedirectToActionResult>();

            ((RedirectToActionResult)retorno).ActionName.Should().Be("Index");
            ((RedirectToActionResult)retorno).ControllerName.Should().Be("Home");
        }

        [Trait("DoacaoController", "DoacaoController_AdicionarDadosInvalidos_BadRequest")]
        [Fact]
        public void DoacaoController_AdicionarDadosInvalidos_BadRequest()
        {
            // Arrange          
            var doacao = _DoacaoFixture.DoacaoInvalida();
            var doacaoModelInvalida = new DoacaoViewModel();
            _Mapper.Setup(a => a.Map<DoacaoViewModel, Doacao>(doacaoModelInvalida)).Returns(doacao);

            var doacaoController = new DoacoesController(_DoacaoService, _DomainNotificationService, _ToastNotification.Object);

            // Act
            var retorno = doacaoController.Create(doacaoModelInvalida);

            // Assert                   
            retorno.Should().BeOfType<ViewResult>();

            _Mapper.Verify(a => a.Map<DoacaoViewModel, Doacao>(doacaoModelInvalida), Times.Once);
            _DoacaoRepository.Verify(a => a.AdicionarAsync(doacao), Times.Never);
            _ToastNotification.Verify(a => a.AddErrorToastMessage(It.IsAny<string>(), It.IsAny<LibraryOptions>()), Times.Once);

            var viewResult = ((ViewResult)retorno);

            viewResult.Model.Should().BeOfType<DoacaoViewModel>();

            ((DoacaoViewModel)viewResult.Model).Should().Be(doacaoModelInvalida);
        }

        #endregion
    }
}

