using Microsoft.AspNet.Identity;
using MyAPI2.Biz;
using MyAPI2.Infrastructure;
using MyAPI2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WeedCSharpClient;

namespace MyAPI2.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        
        [Authorize]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
          //  this.AppUserManager.AddClaim(User.Identity.GetUserId(), new System.Security.Claims.Claim("Test", "1"));
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Authorize]
        [Route("myProfile")]
        public IHttpActionResult GetUser()
        {
            var user = PostsLogic.postContext.AspNetUsers.Select( u => new { u.Id,u.FirstName,u.LastName,u.Email } ).First(u => u.Id == User.Identity.GetUserId());

            if( user != null)
            {
                return Ok(user);
            }

            return BadRequest("User not found");
        }

        [Authorize]
        [HttpPost]
        [Route("uploadPicture")]
        public async Task<IHttpActionResult> uploadPicture()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                var streamProvider = new MultipartMemoryStreamProvider();
                streamProvider = await Request.Content.ReadAsMultipartAsync(streamProvider);


               var item = streamProvider.Contents.Where(c => !string.IsNullOrEmpty(c.Headers.ContentDisposition.FileName)).First();
                
                Stream stPicture = new MemoryStream(await item.ReadAsByteArrayAsync());

                WeedCSharpClientProxy weedProxy = new WeedCSharpClientProxy();
               var result = weedProxy.Upload(stPicture);

                var index = result.url.LastIndexOf('/') + 1;
                var fid = result.url.Substring(index);

                var tempUser = PostsLogic.postContext.AspNetUsers.First(u => u.Id == User.Identity.GetUserId());
             //   tempUser.ProfilePicture = fid;
                PostsLogic.postContext.SubmitChanges();
                return Ok(fid);
            }

            return BadRequest();
        }

        [Authorize]       
        [HttpGet]
        [Route("getPeople")]
        public IHttpActionResult GetPeople()
        {
            var result = AccountLogic.getOtherPeople(User.Identity.GetUserId());
            if (result != null) return Ok(result);

            return BadRequest();
        }

        [Authorize]
        [HttpPost]
        [Route("updateProfile")]
        public IHttpActionResult updateProfile(UserUpdateModel userModel)
        {
            if (ModelState.IsValid)
            {
                var user = Biz.PostsLogic.postContext.AspNetUsers.First(u => u.Id == User.Identity.GetUserId());
                user.FirstName = userModel.FirstName;
                user.LastName = userModel.LastName;
                user.Email = userModel.Email;

                Biz.PostsLogic.postContext.SubmitChanges();

                return Ok();

            }
            return BadRequest(ModelState);
        }

        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }


        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FirstName = createUserModel.FirstName,
                LastName = createUserModel.LastName,
                JoinDate = DateTime.Now.Date,
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }

        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }
    }
}
