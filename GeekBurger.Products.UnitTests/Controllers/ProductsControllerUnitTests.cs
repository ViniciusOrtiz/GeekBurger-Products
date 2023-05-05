using AutoMapper;
using FluentAssertions;
using GeekBurger.Products.Contract;
using GeekBurger.Products.Controllers;
using GeekBurger.Products.Model;
using GeekBurger.Products.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GeekBurger.Products.UnitTests.Controllers
{

    public class ProductsControllerUnitTests
    {
        private readonly ProductsController _productsController;
        private Mock<IProductsRepository> _productRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private MockRepository _mockRepository;

        public ProductsControllerUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _productRepositoryMock = _mockRepository.Create<IProductsRepository>(MockBehavior.Strict);
            _mapperMock = _mockRepository.Create<IMapper>();
            _productsController = new ProductsController(_productRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void OnGetProductsByStoreName_WhenListIsEmpty_ShouldReturnNotFound()
        {
            //arrange
            var storeName = "Paulista";
            var productList = new List<Product>();
            _productRepositoryMock.Setup( _ => _.GetProductsByStoreName(storeName)).Returns(productList);
            var expected = new NotFoundObjectResult("Nenhum dado encontrado");

            //act
            var response = _productsController.GetProductsByStoreName(storeName);

            //assert            
            Assert.IsType<NotFoundObjectResult>(response);
            response.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetProductsByStoreName_WhenListHasProducts_ShouldReturnOkObjectResultWithProductList()
        {
            //arrange
            var storeName = "Paulista";
            var guid = Guid.NewGuid();

            var product = new Product
            {
                ProductId = guid
            };
            var productToGet = new ProductToGet()
            {
                ProductId = guid
            };

            var productList = new List<Product> { product };
            var productToGetList = new List<ProductToGet> { productToGet };

            _mapperMock.Setup(m => m.Map<IEnumerable<ProductToGet>>(productList)).Returns(productToGetList);

            _productRepositoryMock.Setup( _ => _.GetProductsByStoreName(storeName)).Returns(productList);

            //act
            var response = _productsController.GetProductsByStoreName(storeName);

            //assert
            response.Should().BeOfType<OkObjectResult>();
            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            _mockRepository.VerifyAll();
        }

    }
}
