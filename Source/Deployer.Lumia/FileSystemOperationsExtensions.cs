using System.IO;
using System.Threading.Tasks;

namespace Deployer.Lumia
{
    public static class FileSystemOperationsExtensions
    {
        public static Task WriteAllTextToFile(this IFileSystemOperations fileSystemOperations, string path, string contents)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return Task.Run(() => File.WriteAllText(path, contents));
        }
    }
}

