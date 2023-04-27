using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3LifeBackup.Core.Communication.Bucket
{
    public class CreateBucketResponse
    {
        public CreateBucketResponse(string requestId, string buctetName)
        {
            RequestId = requestId;
            BucketName = buctetName;
        }

        public string RequestId { get; set; }
        public string BucketName { get; set; }
    }
}
