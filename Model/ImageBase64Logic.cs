using System;
using System.Drawing;
using System.IO;

namespace _06_24InformationSystem.Model
{
    public class ImageBase64Logic
    {

        /// <summary>
        /// Converts Images to Base64
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }
}
