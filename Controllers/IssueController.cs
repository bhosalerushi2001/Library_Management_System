using Library_Management_System.EntityDto;
using Library_Management_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Library_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class IssueController : Controller
    {
        private Container container;

        public string URI = "https://localhost:8081";
        public string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD" +
            "4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public string DatabaseName = "centralogic";
        public string ContainerName = "issue";

        public IssueController()
        {
            container = GetContainer();

        }

        private Container GetContainer()
        {

            CosmosClient cosmosClient = new CosmosClient(URI, PrimaryKey);
            Database database = cosmosClient.GetDatabase(DatabaseName);
            return database.GetContainer(ContainerName);

        }


        [HttpPost("IssueBook")]
        public async Task<ActionResult> IssueBook(IssueModel issueModel)
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
            model.IsReturned = response.Resource.IsReturned;

            return Ok(model);
        }

        [HttpGet("GetIssueByUId/{UId}")]
        public async Task<ActionResult<IssueModel>> GetIssueByUId(string UId)
        {
            var issueEntity = container.GetItemLinqQueryable<IssueEntityDto>(true)
                .Where(x => x.UId == UId).FirstOrDefault();

            IssueModel model = new IssueModel();

            model.UId = issueEntity.UId;
            model.BookId = issueEntity.BookId;
            model.MemberId = issueEntity.MemberId;
            model.IssueDate = issueEntity.IssueDate;
            model.ReturnDate = issueEntity.ReturnDate;
            model.IsReturned = issueEntity.IsReturned;
            

            return Ok(model);
        }

        [HttpPut("UpdateIssue/{UId}")]
        public async Task<IActionResult> UpdateIssue(string UId, IssueModel updatedIssue)
        {
            var issueEntity = container.GetItemLinqQueryable<IssueEntityDto>(true)
                .Where(x => x.UId == UId).FirstOrDefault();

            issueEntity.BookId = updatedIssue.BookId;
            issueEntity.MemberId = updatedIssue.MemberId;
            issueEntity.IssueDate = updatedIssue.IssueDate;
            issueEntity.ReturnDate = updatedIssue.ReturnDate;
            issueEntity.IsReturned = updatedIssue.IsReturned;

            await container.ReplaceItemAsync(issueEntity, issueEntity.id);

            return Ok(updatedIssue);
        }
    }


}
