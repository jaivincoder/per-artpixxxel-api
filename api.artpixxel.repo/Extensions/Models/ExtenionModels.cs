using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.artpixxel.repo.Extensions.Models
{
    public class FileContentInfo
    {
        public byte[] Bytes { get; set; }
        public ContentType ContentType { get; set; }
        public string FileName { get; set; }
    }
}
