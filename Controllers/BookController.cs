using Library_Management_System.Entity;
using Library_Management_System.EntityDto;
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
        public async Task<BookModel> AddBookToTheLibrary(BookModel bookModel)
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

            return model;

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
            var bookName= container.GetItemLinqQueryable<BookModel>(true).Where
                (x=>x.Title==name).FirstOrDefault();

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
        public async Task<List<BookModel>> RetrieveAllNotIssueBooks()
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

            return books;
        }


        [HttpGet("RetrieveAllIssuedBooks")]
        public async Task<List<BookModel>> RetrieveAllIssuedBooks()
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

            return books;
        }

        [HttpPut("UpdateBook/{UId}")]
        public async Task<BookModel> UpdateBook(string UId, BookModel updatedBook)
        {
            var book = container.GetItemLinqQueryable<BookEntityDto>(true)
                .Where(x => x.UId == UId).FirstOrDefault();

            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.PublishedDate = updatedBook.PublishedDate;
            book.ISBN = updatedBook.ISBN;
            book.IsIssued = updatedBook.IsIssued;

            await container.ReplaceItemAsync(book, book.id);

            return updatedBook;
        }


        //issueController


        [HttpPost("IssueBook")]
        public async Task<IssueModel> IssueBook(IssueModel issueModel)
        {
            IssueEntityDto issueEntity = new IssueEntityDto();

            issueEntity.id = Guid.NewGuid().ToString();
            issueEntity.UId = Guid.NewGuid().ToString();
            issueEntity.BookId = issueModel.BookId;
            issueEntity.MemberId = issueModel.MemberId;
            issueEntity.IssueDate = issueModel.IssueDate;
            issueEntity.ReturnDate = issueModel.ReturnDate;
            issueEntity.IsReturned = issueModel.IsReturned;


            ItemResponse<IssueEntityDto> response = await container.CreateItemAsync(issueEntity);

            IssueModel model = new IssueModel();

            model.UId = response.Resource.UId;
            model.BookId = response.Resource.BookId;
            model.MemberId = response.Resource.MemberId;
            model.IssueDate = response.Resource.IssueDate;
            model.ReturnDate = response.Resource.ReturnDate;
            model.IsReturned = true;

            return model;
        }

        [HttpGet("GetIssueByUId/{UId}")]
        public async Task<IssueModel> GetIssueByUId(string UId)
        {
            var issueEntity = container.GetItemLinqQueryable<IssueEntityDto>(true)
                .Where(x => x.UId == UId).FirstOrDefault();

            IssueModel model = new IssueModel();

            model.UId = issueEntity.UId;
            model.BookId = issueEntity.BookId;
            model.MemberId = issueEntity.MemberId;
            model.IssueDate = issueEntity.IssueDate;
            model.ReturnDate = issueEntity.ReturnDate;
            model.IsReturned = true;


            return model;
        }

        [HttpPut("UpdateIssue/{UId}")]
        public async Task<IssueModel> UpdateIssue(string UId, IssueModel updatedIssue)
        {
            var issueEntity = container.GetItemLinqQueryable<IssueEntityDto>(true)
                .Where(x => x.UId == UId).FirstOrDefault();

            issueEntity.BookId = updatedIssue.BookId;
            issueEntity.MemberId = updatedIssue.MemberId;
            issueEntity.IssueDate = updatedIssue.IssueDate;
            issueEntity.ReturnDate = updatedIssue.ReturnDate;
            issueEntity.IsReturned = updatedIssue.IsReturned;

            await container.ReplaceItemAsync(issueEntity, issueEntity.id);

            return updatedIssue;
        }



        //MemberController

        [HttpPost]
        public async Task<MemberModel> Addnewmember(MemberModel newMembers)
        {
            string newUId = Guid.NewGuid().ToString();

            MemberEntityDto member = new MemberEntityDto();

            member.id = newUId;
            member.UId = newUId;
            member.Name = newMembers.Name;
            member.DateOfBirth = newMembers.DateOfBirth;
            member.Email = newMembers.Email;


            ItemResponse<MemberEntityDto> response = await container.CreateItemAsync(member);

            MemberModel ResponseModel = new MemberModel();

            ResponseModel.UId = member.UId;
            ResponseModel.Name = response.Resource.Name;
            ResponseModel.Email = response.Resource.Email;
            ResponseModel.DateOfBirth = response.Resource.DateOfBirth;


            return ResponseModel;
        }




        [HttpGet("RetrieveMemberById/{UId}")]
        public async Task<MemberModel> RetrieveMemberById(string UId)
        {
            var member = container.GetItemLinqQueryable<MemberEntityDto>(true).
                Where(x => x.UId == UId).FirstOrDefault();

            MemberModel memberModel = new MemberModel();
            memberModel.Name = member.Name;
            memberModel.Email = member.Email;
            memberModel.DateOfBirth = member.DateOfBirth;

            return memberModel;

        }


        [HttpGet("GetAllMembers")]
        public async Task<List<MemberModel>> GetAllMembers()
        {
            var members = container.GetItemLinqQueryable<MemberEntityDto>(true).ToList();

            List<MemberModel> listOfMembers = new List<MemberModel>();

            foreach (var member in members)
            {
                MemberModel Model = new MemberModel();
                Model.Name = member.Name;
                Model.Email = member.Email;
                Model.DateOfBirth = member.DateOfBirth;

                listOfMembers.Add(Model);

            }
            return listOfMembers;

        }


        [HttpPut("UpdateMember")]
        public async Task<MemberModel> UpdateMember(string UId, MemberModel updatedMember)
        {

            var member = container.GetItemLinqQueryable<MemberEntityDto>(true)
                .Where(x => x.UId == UId).FirstOrDefault();

            member.Name = updatedMember.Name;
            member.DateOfBirth = updatedMember.DateOfBirth;
            member.Email = updatedMember.Email;

            await container.ReplaceItemAsync(member, member.id);

            return updatedMember;
        }


    }





}

