using Microsoft.AspNetCore.Mvc.Testing;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebClient;
using WebClient.Services;
using Xunit;

namespace TestProject2
{
    public class UnitTest1 :  IClassFixture<WebApplicationFactory<WebClient.Startup>>
    {
        private WebApplicationFactory<Startup> _factory;

        public UnitTest1(WebApplicationFactory<WebClient.Startup> factory)
        {
            _factory = factory;
        }


        [Theory]
        [InlineData("/")]
        [InlineData("/Index")]
        [InlineData("/api/category")]
        [InlineData("/Privacy")]
        [InlineData("/Contactssss")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var stringData = await response.Content.ReadAsStringAsync();
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }



        [Theory]
        [InlineData("/api/product/stock")]
        public async Task GetStock(string url)
        {

            // Arrange
            var client = _factory.CreateClient();

            var service = _factory.Services.GetService(typeof(IProductService));
           
            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var stringData = await response.Content.ReadAsStringAsync();

           var result =  System.Text.Json.JsonSerializer.Deserialize<List<ProductStock>>(stringData, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive=true });

            var res = result.Where(x => x.Stock > 0).ToList();

            Assert.Equal(21, res.Count);

        }


    }
}
