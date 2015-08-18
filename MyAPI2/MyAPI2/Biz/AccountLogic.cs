using MyAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyAPI2.Biz
{
    public static class AccountLogic
    {
        /*
        - Get user profile
        - Update profile / Update Picture**
        - 
        */
        private static DataClasses1DataContext dc = new DataClasses1DataContext();

       public static Array getOtherPeople(string userId)
        {
            Array people;

            people = dc.AspNetUsers.Select( u => new { u.Id, u.FirstName, u.LastName, u.JoinDate}).Where(u => u.Id != userId).ToArray();
            
            return people;
        }
        

       //public static Array getProfilePicture()
       // {
       //     Array profiles;
       //     profiles = dc.AspNetUsers.Select(p => p.ProfilePicture).ToArray();

       //     return profiles;
       // }

    }
}