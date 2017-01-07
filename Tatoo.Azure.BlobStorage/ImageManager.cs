
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;

namespace Tatoo.Azure.BlobStorage
{
    public interface IImageManager
    {
        /// <summary>
        /// Uploads a new image to a blob container.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<string> UploadImage(Stream image, string bucketName, string fileName);

        /// <summary>
        /// Lists of all the available images in the blob container
        /// </summary>
        /// <returns></returns>
        Task<string[]> ListImages();

        /// <summary>
        /// Gets an image from the blob container using the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<byte[]> GetImage(string name);
    }

    /// <summary>
    /// The image manager is responsible for uploading/downloading and listing images from the Blob Azure Storage
    /// </summary>
	public class ImageManager :IImageManager
    {
        private readonly IConfigurationRoot _configuration;

        public ImageManager(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

      

        /// <summary>
        /// Gets a reference to the container for storing the images
        /// </summary>
        /// <returns></returns>
        /// 
        /// 
        private CloudBlobContainer GetContainer()
        {
            // Parses the connection string for the WindowS Azure Storage Account
            var storageConnectionString = GetConnectionString();
            var account = CloudStorageAccount.Parse(storageConnectionString);



            var client = account.CreateCloudBlobClient();

            // Gets a reference to the images container
            var container = client.GetContainerReference("images");
           
            return container;
        }

        private CloudBlobContainer GetContainer(string bucketName)
        {
            // Parses the connection string for the WindowS Azure Storage Account
            var storageConnectionString = GetConnectionString();
            var account = CloudStorageAccount.Parse(storageConnectionString);
            var client = account.CreateCloudBlobClient();

            // Gets a reference to the images container
            var container = client.GetContainerReference(bucketName);

            return container;
        }

        private string GetConnectionString()
        {
            var storageConnectionString = _configuration["AppSettings:StorageConnectionString"];
            return storageConnectionString;
        }

        public async Task SetPublicContainerPermissions(CloudBlobContainer container)
        {
            var permissions = await container.GetPermissionsAsync();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            await container.SetPermissionsAsync(permissions).ConfigureAwait(false); 
        }

        /// <summary>
        /// Uploads a new image to a blob container.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> UploadImage(Stream image, string bucketName, string fileName)
        {
            var container = GetContainer(bucketName);
            
            
            // Creates the container if it does not exist
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null)
                .ConfigureAwait(false);

            var uniqueDateString = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
            var ext = Path.GetExtension(fileName);
            var file = Path.GetFileNameWithoutExtension(fileName);
            var name = string.Format("{0}_{1}{2}", file, uniqueDateString, ext);

            // Uses a random name for the new images
            //var name = RandomString(10);

           /* var randomNumberGenerator = new RNGCryptoServiceProvider();

            uploadedBytes = new byte[128];

            randomNumberGenerator.GetBytes(uploadedBytes);*/



            // Uploads the image the blob storage
            var imageBlob = container.GetBlockBlobReference(name);
            await imageBlob.UploadFromStreamAsync(image).ConfigureAwait(false);

            return imageBlob.Uri.ToString();
        }

        /// <summary>
        /// Lists of all the available images in the blob container
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> ListImages()
        {
            var container = GetContainer();

            // Iterates multiple times to get all the available blobs
            var allBlobs = new List<string>();
            BlobContinuationToken token = null;

            do
            {
                var result = await container.ListBlobsSegmentedAsync(token).ConfigureAwait(false);
                if (result.Results.Any())
                {
                    var blobs = result.Results.Cast<CloudBlockBlob>().Select(b => b.Name);
                    allBlobs.AddRange(blobs);
                }

                token = result.ContinuationToken;
            } while (token != null); // no more blobs to retrieve

            return allBlobs.ToArray();
        }

        /// <summary>
        /// Gets an image from the blob container using the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<byte[]> GetImage(string name)
        {
            var container = GetContainer();

            //Gets the block blob representing the image
            var blob = container.GetBlobReference(name);

            if (await blob.ExistsAsync())
            {
                // Gets the block blob length to initialize the array in memory
                await blob.FetchAttributesAsync().ConfigureAwait(false);

                byte[] blobBytes = new byte[blob.Properties.Length];

                // Downloads the block blob and stores the content in an array in memory
                await blob.DownloadToByteArrayAsync(blobBytes, 0).ConfigureAwait(false);

                return blobBytes;
            }

            return null;
        }

      
        /*private static readonly Random Random = new Random();
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }*/
    }
    
}
