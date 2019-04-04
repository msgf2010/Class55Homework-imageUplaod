using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Class55_Homework.Data;

namespace Class55_Homework.Models
{
    public class ImagesModelView
    {
        public IEnumerable<Image> Images { get; set; }
    }

    public class ImageModelView
    {
        public Image Image { get; set; }
        public string PassedInPassword { get; set; }
    }
}