

using api.artpixxel.data.Features.Common;
using api.artpixxel.repo.Extensions.Models;
using Microsoft.AspNetCore.StaticFiles;
using MimeKit;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Extensions
{
    public static class StringExtension
    {

       

        private static readonly byte[] BMP = { 66, 77 };
        private static readonly byte[] DOC = { 208, 207, 17, 224, 161, 177, 26, 225 };
        private static readonly byte[] EXE_DLL = { 77, 90 };
        private static readonly byte[] GIF = { 71, 73, 70, 56 };
        private static readonly byte[] ICO = { 0, 0, 1, 0 };
        private static readonly byte[] JPG = { 255, 216, 255 };
        private static readonly byte[] MP3 = { 255, 251, 48 };
        private static readonly byte[] OGG = { 79, 103, 103, 83, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] PDF = { 37, 80, 68, 70, 45, 49, 46 };
        private static readonly byte[] PNG = { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 };
        private static readonly byte[] RAR = { 82, 97, 114, 33, 26, 7, 0 };
        private static readonly byte[] SWF = { 70, 87, 83 };
        private static readonly byte[] TIFF = { 73, 73, 42, 0 };
        private static readonly byte[] TORRENT = { 100, 56, 58, 97, 110, 110, 111, 117, 110, 99, 101 };
        private static readonly byte[] TTF = { 0, 1, 0, 0, 0 };
        private static readonly byte[] WAV_AVI = { 82, 73, 70, 70 };
        private static readonly byte[] WMV_WMA = { 48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108 };
        private static readonly byte[] ZIP_DOCX = { 80, 75, 3, 4 };


        


        /// <summary>
        /// To demonstrate extraction of file extension from base64 string.
        /// </summary>
        /// <param name="base64String">base64 string.</param>
        /// <returns>Henceforth file extension from string.</returns>
        public static string Base64FileType(this string base64String)
        {


            string base64 = string.Empty;

            //string filetype = Regex.Match(base64String, "data:(?<filetype>.*?)/").Groups["filetype"].Value;
            //string subfiletype = Regex.Match(base64String, "/(?<subfiletype>.*?);base64").Groups["subfiletype"].Value;
            //string type = Regex.Match(base64String, ":(?<type>.*?);base64,").Groups["type"].Value;
            if (base64String.Contains("data:image"))
            {
                base64 = Regex.Replace(base64String, @"^data:image\/[a-zA-Z]+;base64,", string.Empty); //Regex.Match(base64String, "base64,(?<base64>.*?),").Groups["base64"].Value;
            }
            else if (base64String.Contains("data:video"))
            {
                base64 = Regex.Replace(base64String, @"^data:video\/[a-zA-Z0-9]+;base64,", string.Empty);
            }
                

           

            string data = base64.Substring(0, 5);
            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "R0LGO":
                    return "gif";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "AAAAG":
                    return "mp4";
                case "AAAAH":
                    return "mp4";
                case "AAAAI":
                    return "mp4";
                case "SUQZB":
                    return "mp3";
                case "UKLGR":
                    return "wav";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }



        public static async Task<string> GetImageFileBase64(this string filePath)
        {
            if(await FileExistsAsync(filePath))
            {
                byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

                string base64String = Convert.ToBase64String(fileBytes);
                string ext = Path.GetExtension(filePath).Replace(".", string.Empty);
                //string fileName = Path.GetFileName(ImageFullPath);
                //var mimeType = GetMimeType(imageBytes, fileName);

                return string.Format("data:image/{0};base64," + base64String, ext);
            }


            return string.Empty;
        }

        public static async Task<FileMeta> RenameFile(this string filePath, string base64String, string outputPath)
        {
            try
            {
                if (File.Exists(filePath))
                {

                   

                    string fileExtension = base64String.Base64FileType();
                    byte[] imageBytes = Convert.FromBase64String(base64String[(base64String.LastIndexOf(',') + 1)..]);
                    string fullURL = outputPath + "_" + Guid.NewGuid().ToString("N") +"." + fileExtension;
                    string base64Img = await Base64FromImage(filePath);
                    if ((base64Img != base64String) || (filePath != fullURL))
                    {
                        await filePath.DeleteFileFromPathAsync();
                        if (await fullURL.FileExistsAsync())
                        {
                            fullURL = outputPath + "_" + DateTime.Today.Ticks.ToString() + "." + fileExtension;
                        }
                        return  await imageBytes.SaveByteArrayAsImage(fullURL);
                    }



                    return new FileMeta
                    {
                        ImageByte = imageBytes,
                        Path = fullURL,
                        FileName = Path.GetFileName(fullURL)
                    };





                }


                else
                {
                    return await base64String.SaveBase64AsImage(outputPath);
                }



              
            }
            catch (Exception)
            {

                throw;
            }
        }




        public static Task<bool> FileExistsAsync(this string filePath)
        {
            try
            {
                return Task.Run(() => File.Exists(filePath));
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static Task<string> GetFileName(this string filePath)
        {
            try
            {
                return Task.Run(() => Path.GetFileNameWithoutExtension(filePath));

            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Task<string> GetFilTypeAsync(this string filePath)
        {
            try
            {
                return Task.Run(() => Path.GetExtension(filePath)[1..]);
              
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static Task<string> GetFileFullNameAsync(this string filePath)
        {
            try
            {
                return Task.Run(() => Path.GetFileName(filePath));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static Task<string> GetFileNameAsync(this string filePath)
        {
            try
            {
                return Task.Run(() => Path.GetFileNameWithoutExtension(filePath));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static Task<string> GetFileExtensionAsync(this string filePath)
        {
            try
            {
                return Task.Run(() => Path.GetExtension(filePath));
                
            }
            catch (Exception)
            {

                throw;
            }
        }



        public static async Task CreateDirectoryAsync(this string filePath)
        {
            try
            {
                 await Task.Run(() => Directory.CreateDirectory(filePath));

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static Task<bool> DirectoryExistAsync(this string filePath)
        {
            try
            {
                return Task.Run(() => Directory.Exists(filePath));

            }
            catch (Exception)
            {

                throw;
            }
        }



        public static async Task  DeleteFileFromPathAsync(this string filePath)
        {
            try
            {

                if (await filePath.FileExistsAsync())
                {
                    await  Task.Run(() => File.Delete(filePath));
                }


               
            }
            catch (Exception)
            {

                throw;
            }
           
           
            
        }
        public static async Task<FileMeta> SaveBase64AsImage(this string base64String, string outputPath)
        {
            try
            {
                string fileExtension = base64String.Base64FileType();
                byte[] imageBytes = Convert.FromBase64String(base64String[(base64String.LastIndexOf(',') + 1)..]);
                string fullURL = outputPath+ "_" + Guid.NewGuid().ToString("N") + "." + fileExtension;
                if (await fullURL.FileExistsAsync())
                {
                    fullURL = outputPath + "_" + DateTime.Today.Ticks.ToString() + "_" + Guid.NewGuid().ToString("N") + "." + fileExtension;
                }
                return  await imageBytes.SaveByteArrayAsImage(fullURL);

              
            }
            catch (Exception)
            {

                return new FileMeta
                {
                    ImageByte = null,
                    Path = null,
                    FileName = null,
                };
            }

           

            
        }


        public static string EmptyIfNull(this object value)
        {
            return value == null ? "" : value.ToString();
        }


        public static async Task<string> Base64FromImage(this string ImageFullPath)
        {

            if (await ImageFullPath.FileExistsAsync())
            {
                byte[] imageBytes = await File.ReadAllBytesAsync(ImageFullPath);
                string base64String = Convert.ToBase64String(imageBytes);
                string ext = Path.GetExtension(ImageFullPath).Replace(".", string.Empty);
                //string fileName = Path.GetFileName(ImageFullPath);
                //var mimeType = GetMimeType(imageBytes, fileName);

                return string.Format("data:image/{0};base64," + base64String, ext);
            }



            return AssetDefault.DefaultImagePNG;
          
           
           
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }



        public static bool IsBase64String(this string base64)
        {
            base64 = base64.Contains("data:image") ?  Regex.Replace(base64, @"^data:image\/[a-zA-Z]+;base64,", string.Empty) : Regex.Replace(base64, @"^data:video\/[a-zA-Z0-9]+;base64,", string.Empty);
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);


        }


        public static bool IsGuid(this string guid)
        {
            Guid x;
            return Guid.TryParse(guid, out x);
        }

        public static string FirstFromSplit(this string source, char delimiter)
        {
            var i = source.IndexOf(delimiter);

            return i == -1 ? source : source.Substring(0, i);
        }

        public static string LastFromSplit(this string input, char delimiter)
            => input.Substring(input.LastIndexOf(delimiter) + 1);


        public static string RandomString()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");

            return GuidString;
        }


        public static async Task<FileContentInfo> FileContent(this string filePath)
        {
            try
            {

              

                string mime = GetMimeTypeFromFile(filePath);
                string fileName = await GetFileFullNameAsync(filePath);
                string extension = fileName.LastFromSplit('.');
                byte[] fileByte = await File.ReadAllBytesAsync(filePath);


                return new FileContentInfo
                {
                    FileName = fileName,
                    Bytes = fileByte,
                    ContentType = new ContentType(extension, mime)
                };

              



            }
            catch (Exception)
            {

                throw;
            }
        }


        public static void AddOrUpdateMapping( this FileExtensionContentTypeProvider provider, string fileExtension, string contentType)
        {
            // usage:
            //FileExtensionContentTypeProvider prv = new();
            //prv.AddOrUpdateMapping(".csv", "text/csv");

            if (!provider.Mappings.TryGetValue(fileExtension, out var _))
            {
                provider.Mappings.Add(fileExtension, contentType);
            }
            else
            {
                provider.Mappings[fileExtension] = contentType;
            }
        }

        public static string GetMimeTypeFromFile(this string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }

        public static string GetMimeType( byte[] file, string fileName)
        {

            string mime = "application/octet-stream"; //DEFAULT UNKNOWN MIME TYPE

            //Ensure that the filename isn't empty or null
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return mime;
            }

            //Get the file extension
            string extension = Path.GetExtension(fileName) == null
                                   ? string.Empty
                                   : Path.GetExtension(fileName).ToUpper();

            //Get the MIME Type
            if (file.Take(2).SequenceEqual(BMP))
            {
                mime = "image/bmp";
            }
            else if (file.Take(8).SequenceEqual(DOC))
            {
                mime = "application/msword";
            }
            else if (file.Take(2).SequenceEqual(EXE_DLL))
            {
                mime = "application/x-msdownload"; //both use same mime type
            }
            else if (file.Take(4).SequenceEqual(GIF))
            {
                mime = "image/gif";
            }
            else if (file.Take(4).SequenceEqual(ICO))
            {
                mime = "image/x-icon";
            }
            else if (file.Take(3).SequenceEqual(JPG))
            {
                mime = "image/jpeg";
            }
            else if (file.Take(3).SequenceEqual(MP3))
            {
                mime = "audio/mpeg";
            }
            else if (file.Take(14).SequenceEqual(OGG))
            {
                if (extension == ".OGX")
                {
                    mime = "application/ogg";
                }
                else if (extension == ".OGA")
                {
                    mime = "audio/ogg";
                }
                else
                {
                    mime = "video/ogg";
                }
            }
            else if (file.Take(7).SequenceEqual(PDF))
            {
                mime = "application/pdf";
            }
            else if (file.Take(16).SequenceEqual(PNG))
            {
                mime = "image/png";
            }
            else if (file.Take(7).SequenceEqual(RAR))
            {
                mime = "application/x-rar-compressed";
            }
            else if (file.Take(3).SequenceEqual(SWF))
            {
                mime = "application/x-shockwave-flash";
            }
            else if (file.Take(4).SequenceEqual(TIFF))
            {
                mime = "image/tiff";
            }
            else if (file.Take(11).SequenceEqual(TORRENT))
            {
                mime = "application/x-bittorrent";
            }
            else if (file.Take(5).SequenceEqual(TTF))
            {
                mime = "application/x-font-ttf";
            }
            else if (file.Take(4).SequenceEqual(WAV_AVI))
            {
                mime = extension == ".AVI" ? "video/x-msvideo" : "audio/x-wav";
            }
            else if (file.Take(16).SequenceEqual(WMV_WMA))
            {
                mime = extension == ".WMA" ? "audio/x-ms-wma" : "video/x-ms-wmv";
            }
            else if (file.Take(4).SequenceEqual(ZIP_DOCX))
            {
                mime = extension == ".DOCX" ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" : "application/x-zip-compressed";
            }

            return mime;
        }






        public static string ToTitleCase(this string input)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(input);
        }


        


        public static DateTime DDMMYYYY(this string date)
        {
           
            DateTime dateResult;
            if (DateTime.TryParseExact(date, DefaultDateFormat.ddMMyyyy, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateResult))
            {
                return DateTime.ParseExact(date, DefaultDateFormat.ddMMyyyy, CultureInfo.InvariantCulture);
            }

            return new DateTime();
        }



        public static DateTime DDMMYYYYHHMMSStt(this string date)
        {
            DateTime dateResult;
            if (DateTime.TryParseExact(date, DefaultDateFormat.ddMMyyyyhhmmsstt, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateResult))
            {
                return DateTime.ParseExact(date, DefaultDateFormat.ddMMyyyyhhmmsstt, CultureInfo.InvariantCulture);
            }

            return new DateTime();


           
        }




        public static string Encrypt(this string encr, string appkey)
        {

            byte[] data = UTF8Encoding.UTF8.GetBytes(encr);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(appkey));
                using (TripleDESCryptoServiceProvider tripDes = new() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }


        public static string Decrypt(this string decr, string appkey)
        {

            byte[] data = Convert.FromBase64String(decr); // decrypt the incrypted text
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(appkey));
                using (TripleDESCryptoServiceProvider tripDes = new() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return UTF8Encoding.UTF8.GetString(results);
                }
            }
        }



      





    }
}
