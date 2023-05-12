using AwsS3LifeBackup.Core.Communication.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3LifeBackup.Core.Communication.Interfaces
{
    public interface IFolderRepository
    {
        Task<bool> CreateFolder(string folderName, string pathToFolder = "");
    }
}
