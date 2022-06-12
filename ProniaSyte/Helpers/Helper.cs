using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProniaSyte.Helpers
{
    public static class Helper
    {
        public static void DeleteImg(string path,string root,string imageName)
        {
           string fullPath = Path.Combine(path, root, imageName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
    public enum UserRoles
    {
        Admin,
        Member,
        Moderator
    }
}
