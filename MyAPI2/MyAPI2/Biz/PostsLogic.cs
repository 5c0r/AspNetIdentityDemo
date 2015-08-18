using MyAPI2.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MyAPI2.Biz
{
    public static class PostsLogic
    {
        public static DataClasses1DataContext postContext = new DataClasses1DataContext();
        public static ArrayList getNewestPosts(int count)
        {
            ArrayList listPost = new ArrayList();

            listPost = new ArrayList(postContext.Posts.Select(p => new { p.Id, p.AspNetUser.FirstName,p.AspNetUser.LastName, p.UserId, p.Content, p.JsonPicture }).OrderByDescending( p => p.Id).Take( count ).ToArray());
            return listPost;
        }

        public static Posts getPostById(int id)
        {
            Posts targetPost = postContext.Posts.Where(i => i.Id == id).Single();

            Posts newPost = new Posts()
            {
                Id = targetPost.Id,
                UserId = targetPost.UserId,
                Content = targetPost.Content,
                JsonPicture = targetPost.JsonPicture
            };

            return newPost;
        }

        public static ArrayList getPostsByUser(string userId)
        {
            ArrayList listPost = new ArrayList();

            listPost = new ArrayList(postContext.Posts.OrderBy(i => i.Id).Where(i => i.UserId == userId).ToArray());

            return listPost;
        }


    }
}