using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3LifeBackup.Core.Communication.Files
{
    public class AddFileResponse
    {
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}
