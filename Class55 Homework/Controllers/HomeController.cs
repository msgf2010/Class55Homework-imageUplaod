using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Class55_Homework.Data;
using Class55_Homework.Models;

namespace Class55_Homework.Controllers
{
    //Create an application where users can upload images and share
    //it with their friends. However, when an image is uploaded,
    //the user will be prompted to create a "password" which will
    //protect the image from being seen by anyone that doesn't have 
    //the password.

    //Here's the flow of the application: 

    //On the home page, there should be a textbox and a file upload
    //input. The user will then put in a "password" into the textbox
    //and choose an image to upload. When they hit submit, they should
    //get taken to a page that says:

    //"Thank you for uploading your images, here's the link to share with your friends:

    //http://localhost:123/images/view?id=14

    //Make sure to give them the password of 'foobar'"

    //When a user tries to visit an images page, they should first be presented with a
    //textbox where they need to put the password saved by the image uploader. If they
    //enter it correctly, the page should refresh (same url) and they should see the image.
    //Underneath the image, they should also see a little number that displays how many
    //times this image has already been viewed (just store this number in the database
    //and keep updating it every time it's viewed). If they put the password in incorrectly,
    //the page should refresh with an error message saying "please try again".

    //Once they've put in the password, they should never have to put in the password again
    //for that image.

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase image, Image imageSpecs)
        {
            string ext = Path.GetExtension(image.FileName);
            string fileName = $"{Guid.NewGuid()}{ext}";
            string fullPath = $"{Server.MapPath("/UploadedImages")}\\{fileName}";
            image.SaveAs(fullPath);
            var mgr = new ImageManager(Properties.Settings.Default.ConStr);

            imageSpecs.FileName = fileName;
            mgr.SaveImage(imageSpecs);

            return Redirect($"/home/upload?id={imageSpecs.Id}");
        }

        public ActionResult Upload(int id)
        {
            var mgr = new ImageManager(Properties.Settings.Default.ConStr);
            ImageLinkModelView mv = new ImageLinkModelView();
            mv.ImageLink = $"http://localhost:54900/home/image?id={id}";

            return View(mv);
        }

        public ActionResult Image(int id, string password)
        {
            var mgr = new ImageManager(Properties.Settings.Default.ConStr);
            ImageModelView mv = new ImageModelView();
            mv.Image = mgr.GetSingleImage(id);

            HttpCookie fromRequest = Request.Cookies[$"PasswordForImageId{mv.Image.Id}"];
            HttpCookie cookie = new HttpCookie($"PasswordForImageId{mv.Image.Id}", $"{ mv.Image.Password }");

            if (fromRequest != null)
            {
                mv.PassedInPassword = fromRequest.Value;
                mv.Image.Views++;
                mgr.UpdateImageViews(id, mv.Image.Views);
            }
            else
            {
                if (password == mv.Image.Password)
                {
                    Response.Cookies.Add(cookie);
                    mv.PassedInPassword = mv.Image.Password;
                    mv.Image.Views++;
                    mgr.UpdateImageViews(id, mv.Image.Views);
                }
                else
                {
                    mv.PassedInPassword = password;
                }
            }

            return View(mv);
        }

        public ActionResult Images()
        {
            var mgr = new ImageManager(Properties.Settings.Default.ConStr);
            ImagesModelView mv = new ImagesModelView();
            mv.Images = mgr.Get();

            return View(mv);
        }
    }
}