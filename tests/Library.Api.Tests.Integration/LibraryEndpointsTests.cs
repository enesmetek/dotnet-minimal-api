using FluentAssertions;
using Library.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Library.Api.Tests.Integration
{
    public class LibraryEndpointsTests(WebApplicationFactory<IApiMarker> factory) : IClassFixture<WebApplicationFactory<IApiMarker>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<IApiMarker> _factory = factory;
        private readonly List<string> _createdIsbns = [];

        [Fact]
        public async Task CreateBook_CreatesBook_WhenDataIsCorrect()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var book = GenerateBook();

            // Act
            var result = await httpClient.PostAsJsonAsync("/books", book);
            _createdIsbns.Add(book.Isbn);
            var createdBook = await result.Content.ReadFromJsonAsync<Book>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            createdBook.Should().BeEquivalentTo(book);
            result.Headers.Location.Should().Be($"/books/{book.Isbn}");
        }

        [Fact]
        public async Task CreateBook_Fails_WhenIsbnIsNotValid()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var book = GenerateBook();
            book.Isbn = "INVALID";

            // Act
            var result = await httpClient.PostAsJsonAsync("/books", book);
            _createdIsbns.Add(book.Isbn);
            var errors = await result.Content.ReadFromJsonAsync<ValidationError[]>();
            var error = errors!.Single();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            error.PropertyName.Should().Be("Isbn");
            error.ErrorMessage.Should().Be("Value was not a valid ISBN-13");
        }

        [Fact]
        public async Task CreateBook_Fails_WhenBookExists()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var book = GenerateBook();

            // Act
            await httpClient.PostAsJsonAsync("/books", book);
            _createdIsbns.Add(book.Isbn);
            var result = await httpClient.PostAsJsonAsync("/books", book);
            var errors = await result.Content.ReadFromJsonAsync<ValidationError[]>();
            var error = errors!.Single();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            error.PropertyName.Should().Be("Isbn");
            error.ErrorMessage.Should().Be("A book with this ISBN-13 already exists.");
        }

        [Fact]
        public async Task GetBook_ReturnsBook_WhenBookExists()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var book = GenerateBook();
            await httpClient.PostAsJsonAsync("/books", book);
            _createdIsbns.Add(book.Isbn);

            // Act
            var result = await httpClient.GetAsync($"/books/{book.Isbn}");
            var returnedBook = await result.Content.ReadFromJsonAsync<Book>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            returnedBook.Should().BeEquivalentTo(book);
        }

        [Fact]
        public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            string isbn = GenerateIsbn();

            // Act
            var result = await httpClient.GetAsync($"/books/{isbn}");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private static Book GenerateBook(string title = "The Clean Coder")
        {
            return new Book
            {
                Isbn = GenerateIsbn(),
                Title = title,
                Author = "Enes Mete Kafali",
                ShortDescription = "Test Description",
                PageCount = 100,
                ReleaseDate = DateTime.Now
            };
        }

        [Fact]
        public async Task GetAllBooks_ReturnsAllBooks_WhenBooksExist()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var book = GenerateBook();
            await httpClient.PostAsJsonAsync("/books", book);
            _createdIsbns.Add(book.Isbn);

            // Act
            var result = await httpClient.GetAsync("/books");
            var returnedBooks = await result.Content.ReadFromJsonAsync<List<Book>>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            returnedBooks.Should().ContainEquivalentOf(book);
        }

        [Fact]
        public async Task GetAllBooks_ReturnsMatchedBooks_WhenSearchTermExists()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var book = GenerateBook();
            await httpClient.PostAsJsonAsync("/books", book);
            _createdIsbns.Add(book.Isbn);

            // Act
            var result = await httpClient.GetAsync($"/books?searchTerm={book.Title}");
            var returnedBooks = await result.Content.ReadFromJsonAsync<List<Book>>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            returnedBooks.Should().ContainEquivalentOf(book);
        }

        [Fact]
        public async Task UpdateBook_UpdatesBook_WhenDataIsCorrect()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var book = GenerateBook();
            await httpClient.PostAsJsonAsync("/books", book);
            _createdIsbns.Add(book.Isbn);

            // Act
            book.Title = "The Clean Coder 2";
            var result = await httpClient.PutAsJsonAsync($"/books/{book.Isbn}", book);
            var updatedBook = await result.Content.ReadFromJsonAsync<Book>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedBook.Should().BeEquivalentTo(book);
        }

        [Fact]
        public async Task UpdateBook_DoesNotUpdatesBook_WhenDataIsNotValid()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var book = GenerateBook();
            await httpClient.PostAsJsonAsync("/books", book);
            _createdIsbns.Add(book.Isbn);
            book.Isbn = "INVALID";

            // Act
            var result = await httpClient.PutAsJsonAsync($"/books/{book.Isbn}", book);
            var errors = await result.Content.ReadFromJsonAsync<ValidationError[]>();
            var error = errors!.Single();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            error.PropertyName.Should().Be("Isbn");
            error.ErrorMessage.Should().Be("Value was not a valid ISBN-13");
        }

        [Fact]
        public async Task UpdateBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var book = GenerateBook();

            // Act
            var result = await httpClient.PutAsJsonAsync($"/books/{book.Isbn}", book);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteBook_DeletesBook_WhenBookExists()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            var book = GenerateBook();
            await httpClient.PostAsJsonAsync("/books", book);
            _createdIsbns.Add(book.Isbn);

            // Act
            var result = await httpClient.DeleteAsync($"/books/{book.Isbn}");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var httpClient = _factory.CreateClient();
            string isbn = GenerateIsbn();

            // Act
            var result = await httpClient.DeleteAsync($"/books/{isbn}");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private static string GenerateIsbn()
        {
            return $"{Random.Shared.Next(100, 999)}-" + $"{Random.Shared.Next(1000000000, 2100999999)}";
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public async Task DisposeAsync()
        {
            var httpClient = _factory.CreateClient();
            foreach (var isbn in _createdIsbns)
            {
                await httpClient.DeleteAsync($"/books/{isbn}");
            }
        }
    }
}
