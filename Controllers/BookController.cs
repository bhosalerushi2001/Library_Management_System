using Library_Management_System.Entity;
using Library_Management_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace Library_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BookController : Controller
    {

        private Container container;

        public string URI = "https://localhost:8081";
        public string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD" +
            "4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public string DatabaseName = "centralogic";
        public string ContainerName = "book";

        public BookController() 
        {
            container = GetContainer();
                   
        }

        private Container GetContainer()
        {
            CosmosClient cosmosClient=new CosmosClient(URI,PrimaryKey);
            Database database=cosmosClient.GetDatabase(DatabaseName);
            return database.GetContainer(ContainerName);

        }

        [HttpPost("AddBookToTheLibrary")]
        public async Task<ActionResult> AddBookToTheLibrary(BookModel bookModel)
        {
            BookEntityDto book = new BookEntityDto();

            book.id=Guid.NewGuid().ToString();
            book.UId=Guid.NewGuid().ToString();
            book.Title = bookModel.Title;
            book.PublishedDate= bookModel.PublishedDate;
            book.Author = bookModel.Author;

            ItemResponse<BookEntityDto> response = await container.CreateItemAsync(book);

            BookModel model = new BookModel();

            model.UId = book.UId;
            model.Author= response.Resource.Author;
            model.Title = response.Resource.Title;
            model.PublishedDate= DateTime.Now;
            model.ISBN = response.Resource.ISBN;
            model.IsIssued = true;

            return Ok(model);

        }

        [HttpGet("RetrieveBookByUId/{UId}")]
        public async Task<BookModel> RetrieveBookByUId(string UId)
        {
            var bookId= container.GetItemLinqQueryable<BookModel>
                (true).Where(x=>x.UId==UId).FirstOrDefault();

            BookModel model= new BookModel();

            model.UId=bookId.UId;
            model.Title = bookId.Title;
            model.PublishedDate= bookId.PublishedDate;
            model.ISBN = bookId.ISBN;
            model.Author= bookId.Author;
            model.IsIssued = true;
                
            return model;
        }

        [HttpGet("RetrieveBookByName")]
        public async Task<BookModel> RetrieveBookByName(string name)
        {
            var bookName= container.GetItemLinqQueryable<BookModel>(true).Where(x=>x.Title==name).FirstOrDefault();

            BookModel model= new BookModel();

            model.UId = bookName.UId;
            model.Title = bookName.Title;
            model.ISBN= bookName.ISBN;
            model.PublishedDate = bookName.PublishedDate;
            model.Author = bookName.Author;
            model.IsIssued = false;
           
            return model;
        }

        [HttpGet("RetrieveAllBooks")]
        public async Task<List<BookModel>> RetrieveAllBooks()
        {
            var listofBook=container.GetItemLinqQueryable<BookModel>(true).ToList();

            List<BookModel> books=new List<BookModel>();

            foreach(var book in listofBook)
            {
                BookModel model=new BookModel();
                model.UId = book.UId;
                model.Title = book.Title;
                model.ISBN= book.ISBN;
                model.PublishedDate= book.PublishedDate;
                model.Author = book.Author;
                model.IsIssued = false;

                books.Add(model);

            }
            return books;

        }

        [HttpGet("RetrieveAllNotIssueBooks")]
        public async Task<ActionResult<List<BookModel>>> RetrieveAllNotIssueBooks()
        {
            var availableBooks = container.GetItemLinqQueryable<BookEntityDto>(true)
                .Where(book => !book.IsIssued).ToList();

            List<BookModel> books = availableBooks.Select(book => new BookModel
            {
                UId = book.UId,
                Title = book.Title,
                ISBN = book.ISBN,
                PublishedDate = book.PublishedDate,
                Author = book.Author,
                IsIssued = book.IsIssued
            }).ToList();

            return Ok(books);
        }


        [HttpGet("RetrieveAllIssuedBooks")]
        public async Task<ActionResult<List<BookModel>>> RetrieveAllIssuedBooks()
        {
            var issuedBooks = container.GetItemLinqQueryable<BookEntityDto>(true)
                .Where(book => book.IsIssued).ToList();

            List<BookModel> books = issuedBooks.Select(book => new BookModel
            {
                UId = book.UId,
                Title = book.Title,
                ISBN = book.ISBN,
                PublishedDate = book.PublishedDate,
                Author = book.Author,
                IsIssued = book.IsIssued
            }).ToList();

            return Ok(books);
        }

        [HttpPut("UpdateBook/{UId}")]
        public async Task<IActionResult> UpdateBook(string UId, BookModel updatedBook)
        {
            var book = container.GetItemLinqQueryable<BookEntityDto>(true)
                .Where(x => x.UId == UId).FirstOrDefault();

            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.PublishedDate = updatedBook.PublishedDate;
            book.ISBN = updatedBook.ISBN;
            book.IsIssued = updatedBook.IsIssued;

            await container.ReplaceItemAsync(book, book.id);

            return Ok(updatedBook);
        }
    }





}

