using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApplication4.Models;

namespace WebApplication4.Tests
{
    public class BooksApiIntegrationTests : IClassFixture<WebApplicationFactory<WebApplication4.Program>>
    {
        private readonly HttpClient _client;

        public BooksApiIntegrationTests(WebApplicationFactory<WebApplication4.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetBook_ReturnsOk_WhenBookExists()
        {
            var response = await _client.GetAsync("/api/books/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var book = await response.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(book);
            Assert.Equal(1, book.Id);
        }
    }
}
