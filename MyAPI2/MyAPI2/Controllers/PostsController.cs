using Microsoft.AspNet.Identity;
using MyAPI2.Biz;
using MyAPI2.Infrastructure;
using MyAPI2.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WeedCSharpClient;

namespace MyAPI2.Controllers
{   
    [RoutePrefix("api/posts")]
    public class PostsController : ApiController
    {
        // GET: api/Posts
        [ClaimsAuthorization(ClaimType = "Test", ClaimValue = "False")]
        public ArrayList Get()
        {
            return PostsLogic.getNewestPosts(4);
        }

        [Authorize]
        public IHttpActionResult Get(int id)
        {
            var post = PostsLogic.getPostById(id);

            return Json<Models.Posts>(post);
        }

        // POST: api/Posts
        [Authorize]
        [HttpPost]
        public IHttpActionResult newPost(PostBindingModel newPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tempPost = new Models.Posts()
            {
                AspNetUser = PostsLogic.postContext.AspNetUsers.First(u => u.Id == User.Identity.GetUserId()),
                Content = newPost.Content,
               // PostDate = DateTime.Now,
                JsonPicture = newPost.JsonPicture
            };

            PostsLogic.postContext.Posts.InsertOnSubmit(tempPost);
            PostsLogic.postContext.SubmitChanges();

            return Ok();
        }

        [Authorize]
        [HttpPost]
        public IHttpActionResult updatePost(PostBindingModel aPost)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
 //           ApplicationDbContext db = new ApplicationDbContext();

            return Ok();
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
                var result = weedProxy.Upload(stPicture,null,null,ReplicationStrategy.OnceOnDifferentRack);

                var index = result.url.LastIndexOf('/') + 1;
                var fid = result.url.Substring(index);

                return Ok(fid);
            }

            return BadRequest();
        }


    }
}
