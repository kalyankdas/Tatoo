using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tatoo.Azure.BlobStorage.Test.Integration
{
    [TestClass]
    public class ImageManagerTest
    {
       

       [TestInitialize]
        public void SetUp()
        {
            var imageManager = new ImageManager();




        }
        [TestMethod]
        public void test_image_upload_successful()
        {
           // Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, "..\\..\\"));
            var path = Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, "image.jpg"));

            var imageManager = new ImageManager();
            string imagePath;
            using (var fs = new FileStream(path, FileMode.Open))
            {
                imagePath = imageManager.UploadImage(fs).Result;


            }
            var imageContent = imageManager.GetImage(imagePath).Result;
            Assert.IsNotNull(imageContent);
            Assert.IsTrue(imageContent.Length> 0);
           

        }
    }

    /* public static class StorageEmulator

     {

         public static void Start()

         {

             // check if emulator is already running

             var processes = Process.GetProcesses().OrderBy(p => p.ProcessName).ToList();

             if (processes.Any(process => process.ProcessName.Contains("DSServiceLDB")))

             {

                 return;

             }



             //var command = Environment.GetEnvironmentVariable("PROGRAMFILES") + @"\Microsoft SDKs\Windows Azure\Emulator\csrun.exe";

             const string command = @"c:\Program Files\Microsoft SDKs\Windows Azure\Emulator\csrun.exe";



             using (var process = Process.Start(command, "/devstore:start"))

             {

                 process.WaitForExit();

             }

         }

     }*/
}