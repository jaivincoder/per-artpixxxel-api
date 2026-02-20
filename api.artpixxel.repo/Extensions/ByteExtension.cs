
using api.artpixxel.data.Features.Common;
using System.IO;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Extensions
{
   public static class ByteExtension
    {
        public static async Task<FileMeta> SaveByteArrayAsImage(this byte[] imageBytes, string fullOutputPath)
        {
          

              await File.WriteAllBytesAsync(fullOutputPath, imageBytes);
              
              if (await fullOutputPath.FileExistsAsync())
            {
                return new FileMeta
                {
                    ImageByte = imageBytes,
                    Path = fullOutputPath,
                    FileName = Path.GetFileName(fullOutputPath)
                };
            }

            else
            {
                return new FileMeta
                {
                    ImageByte = null,
                    Path = null,
                    FileName = null
                };
            }
        }
    }
}
