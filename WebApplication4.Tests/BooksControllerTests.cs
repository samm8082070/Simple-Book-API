using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication4.Controllers;
using WebApplication4.Models;
using WebApplication4.Repositories;

namespace WebApplication4.Tests
{
    public class BooksControllerTests
    {
        //[Fact]
        //public void GetBook_ReturnsOkResult_WhenBookExists()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<IBookRepository>();
        //    mockRepo.Setup(repo => repo.GetBook(1)).Returns(new Book { Id = 1, Title = "Test Book" });
        //    var controller = new BooksController(mockRepo.Object);

        //    // Act
        //    var result = controller.Get(1);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var book = Assert.IsType<Book>(okResult.Value);
        //    Assert.Equal(1, book.Id);
        //    Assert.Equal("Test Book", book.Title);
        //}

        //[Fact]
        //public void GetBook_ReturnsNotFoundResult_WhenBookDoesNotExist()
        //{
        //    //Arrange
        //    var mockRepo = new Mock<IBookRepository>();
        //    mockRepo.Setup(repo => repo.GetBook(1)).Returns((Book)null);
        //    var controller = new BooksController(mockRepo.Object);

        //    //Act
        //    var result = controller.Get(1);

        //    //Assert
        //    Assert.IsType<NotFoundResult>(result);
        //}

        //[Fact]
        //public void GetBooks_ReturnsOkResult_WithListOfBooks()
        //{
        //    //Arrange
        //    var mockRepo = new Mock<IBookRepository>();
        //    mockRepo.Setup(repo => repo.GetBooks()).Returns(new List<Book> { new Book { Id = 1, Title = "Book1" }, new Book { Id = 2, Title = "Book2" } });
        //    var controller = new BooksController(mockRepo.Object);

        //    //Act
        //    var result = controller.Get();

        //    //Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var books = Assert.IsType<List<Book>>(okResult.Value);
        //    Assert.Equal(2, books.Count);
        //}
    }
}