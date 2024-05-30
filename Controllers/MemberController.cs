using Library_Management_System.EntityDto;
using Library_Management_System.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Library_Management_System.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MemberController : Controller
    {
        private Container container;

        public string URI = "https://localhost:8081";
        public string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD" +
            "4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";    
        public string DatabaseName = "centralogic";
        public string ContainerName = "Member";

        public MemberController()
        {
            container = GetContainer();

        }

        private Container GetContainer()
        {
            CosmosClient cosmosClient = new CosmosClient(URI, PrimaryKey);
            Database database = cosmosClient.GetDatabase(DatabaseName);
            return database.GetContainer(ContainerName);

        }

        [HttpPost]
        public async Task<MemberModel> Addnewmember( MemberModel newMembers)
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
               var member=container.GetItemLinqQueryable<MemberEntityDto>(true).
                   Where(x=>x.UId==UId).FirstOrDefault();

               MemberModel memberModel = new MemberModel();
               memberModel.Name = member.Name;
               memberModel.Email = member.Email;
               memberModel.DateOfBirth = member.DateOfBirth;

               return memberModel;

           }


           [HttpGet("GetAllMembers")]
           public async Task<List<MemberModel>> GetAllMembers()
           {
               var members=container.GetItemLinqQueryable<MemberEntityDto>(true).ToList();

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
           public async Task<MemberModel> UpdateMember(string UId,  MemberModel updatedMember)
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
