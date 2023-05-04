using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3LifeBackup.Core.Communication.Files
{
    public class AddBase64FileRequest
    {
        public string FileName { get; set; }
        public string Base64Content { get; set; }
    }
}
