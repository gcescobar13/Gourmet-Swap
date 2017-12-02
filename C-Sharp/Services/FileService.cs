using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using System.IO;
using GSwap.Models.Domain.Files;
using GSwap.Data.Providers;
using System.Data.SqlClient;
using System.Web;
using GSwap.Data;
using System.Data;


namespace GSwap.Services
{
    public class FileService : IFileService
    {

        private IDataProvider _dataProvider;
        private ISiteConfigService _siteConfigService;

        public FileService(IDataProvider dataProvider, ISiteConfigService siteConfigService)
        {
            _dataProvider = dataProvider;
            _siteConfigService = siteConfigService;
        }

        private string ExistingBucketName
        {
            get { return _siteConfigService.AwsBucket; }
        }

        private string KeyName
        {
            get { return _siteConfigService.AwsFolder; }
        }
            
            
        

        public List<int> UploadMealPhotos(HttpFileCollection hfc, int mealId, int userId)
        {
            List<int> photoIds = null;

            AmazonS3Client client = new AmazonS3Client(_siteConfigService.AwsAccessKey, _siteConfigService.AwsSecretKey, Amazon.RegionEndpoint.USWest2);

            TransferUtility fileTransferUtility = new TransferUtility(client);

            List<string> photoFiles = null;

            for (int i = 0; i < hfc.Count; i++)
            {
                HttpPostedFile hpf = hfc[i];

                TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = ExistingBucketName,
                    InputStream = hpf.InputStream,  //where you get the stream
                    Key = KeyName + "/" + Guid.NewGuid().ToString() + Path.GetFileName(hpf.FileName),

                };

                fileTransferUtility.Upload(fileTransferUtilityRequest);

                if (photoFiles == null)
                {
                    photoFiles = new List<string>();
                }

                photoFiles.Add(fileTransferUtilityRequest.Key);

            }

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@MealId", mealId);
                paramCollection.AddWithValue("@UserId", userId);


                SqlParameter filesParam = new SqlParameter("@Files", SqlDbType.Structured);

                if (photoFiles != null && photoFiles.Any())
                {
                    NVarcharTable fileNames = new NVarcharTable(photoFiles);
                    filesParam.Value = fileNames;
                }

                paramCollection.Add(filesParam);

            };

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                if (photoIds == null)
                {
                    photoIds = new List<int>();
                }

                photoIds.Add(reader.GetSafeInt32(0));
            };

            _dataProvider.ExecuteCmd("dbo.Files_MealsPhoto_Insert", inputParamDelegate, singleRecMapper);

            return photoIds;

        }

        public List<Models.Domain.Files.File> GetFiles(List<int> ids)
        {
            List<Models.Domain.Files.File> files = null;



            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {

                SqlParameter fileIds = new SqlParameter("@Ids", SqlDbType.Structured);

                if (ids != null && ids.Any())
                {
                    IntIdTable idsTable = new IntIdTable(ids);
                    fileIds.Value = idsTable;
                }

                paramCollection.Add(fileIds);

            };

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                Models.Domain.Files.File filesList = new Models.Domain.Files.File();

                if (files == null)
                {
                    files = new List<Models.Domain.Files.File>();
                }
                int startingIndex = 0;
                filesList.Id = reader.GetSafeInt32(startingIndex++);
                filesList.UserId = reader.GetSafeInt32(startingIndex++);
                filesList.FileName = reader.GetSafeString(startingIndex++);
                filesList.FileTypeId = reader.GetSafeInt32(startingIndex++);
                filesList.DateAdded = reader.GetDateTime(startingIndex++);
                filesList.DateModified = reader.GetDateTime(startingIndex++);

                files.Add(filesList);
            };

            _dataProvider.ExecuteCmd("dbo.Files_SelectByMultiIds", inputParamDelegate, singleRecMapper);

            return files;

        }

        public List<Models.Domain.Files.File> GetAllPhotosById(int mealId, int userId)
        {
            List<Models.Domain.Files.File> files = null;

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@MealId", mealId);
                paramCollection.AddWithValue("@UserId", userId);

            };

            Action<IDataReader, short> singleRecMapper = delegate (IDataReader reader, short set)
            {
                Models.Domain.Files.File mealPhoto = new Models.Domain.Files.File();

                if (files == null)
                {
                    files = new List<Models.Domain.Files.File>();
                }
                int startingIndex = 0;
                mealPhoto.Id = reader.GetSafeInt32(startingIndex++);
                mealPhoto.UserId = reader.GetSafeInt32(startingIndex++);
                mealPhoto.FileName = reader.GetSafeString(startingIndex++);
                mealPhoto.FileTypeId = reader.GetSafeInt32(startingIndex++);
                mealPhoto.DateAdded = reader.GetDateTime(startingIndex++);
                mealPhoto.DateModified = reader.GetDateTime(startingIndex++);

                files.Add(mealPhoto);
            };

            _dataProvider.ExecuteCmd("dbo.MealPhotos_Files_SelectByMealId", inputParamDelegate, singleRecMapper);

            return files;

        }

        public int CookProfilePhoto(HttpPostedFile file, int userId)
        {
            int id = 0;

            AmazonS3Client client = new AmazonS3Client(_siteConfigService.AwsAccessKey, _siteConfigService.AwsSecretKey, Amazon.RegionEndpoint.USWest2);

            TransferUtility fileTransferUtility = new
                TransferUtility(client);
            
            // 4.Specify advanced settings/options.
            TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = ExistingBucketName,
                InputStream = file.InputStream,  //where you get the stream
                Key = KeyName + "/" + Guid.NewGuid().ToString() + Path.GetFileName(file.FileName),

            };

            fileTransferUtility.Upload(fileTransferUtilityRequest);
            Console.WriteLine("Upload completed");

            string fileName = fileTransferUtilityRequest.Key;

            Action<SqlParameterCollection> inputParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                
                paramCollection.AddWithValue("@UserId", userId);
                paramCollection.AddWithValue("@FileName", fileName);

                SqlParameter idParameter = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                idParameter.Direction = System.Data.ParameterDirection.Output;

                paramCollection.Add(idParameter);
            };

            Action<SqlParameterCollection> returnParamDelegate = delegate (SqlParameterCollection paramCollection)
            {
                Int32.TryParse(paramCollection["@Id"].Value.ToString(), out id);

            };

            string proc = "dbo.Files_Insert";
            _dataProvider.ExecuteNonQuery(proc, inputParamDelegate, returnParamDelegate);

            return id;

        }

        p

    }

}

