using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Bot.JsonData
{
    public class PhotosGallery
    {
        private List<string> photoNames;
        public PhotosGallery(params string[] photoNames)
        {
            this.photoNames = photoNames.ToList<string>();
        }

        public void AddPhoto(string photoName)
        {
            if (!photoNames.Contains(photoName))
            {
                photoNames.Add(photoName);
            }
        }
    }
}
