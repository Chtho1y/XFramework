using System.IO;
using System.Linq;
using UnityEditor;
using XEngine.Engine;


namespace XEngine.Editor
{
    // 打包 Lua 和 Protobuf 文件到 Res2Bundle 目录下
    internal static class Res2BundleBuilder
    {
        public static void BuildRes2Bundle()
        {
            EncryptLuaToRes2Bundle();
            //EncryptProtosToRes2Bundle();
        }

        // 把sourceDir目录下的文件拷贝到destDir, 保留子目录结构
        // 只拷贝extensions结尾的文件并加密，自动加上.txt后缀
        private static void EncryptAndCopy(string sourceDir, string destDir, string[] extensions)
        {

            string[] allFilesFromDirectory = GameUtil.GetAllFilesFromDirectory(sourceDir);
            foreach (string filePath in allFilesFromDirectory)
            {
                if (extensions.Any(ext => filePath.EndsWith(ext)))
                {
                    // 如果是以extensions结尾的文件，就加密并拷贝到destDir目录下

                    // 从源代码目录拷贝到 Res2Bundle 目录下的文件名后面加上 .txt 后缀(来让UnityEditor识别)
                    var fileName = Path.GetFileName(filePath);
                    var relativePath = Path.GetRelativePath(sourceDir, filePath);
                    var subDirPath = Path.GetDirectoryName(relativePath);
                    var savePath = Path.Combine(destDir, subDirPath, fileName + ".txt");
                    var saveDir = Path.GetDirectoryName(savePath);
                    if (!Directory.Exists(saveDir))
                    {
                        Directory.CreateDirectory(saveDir);
                    }
                    var encryptedLuaFile = CryptoTool.Encrypt(GameUtil.File2UTF8(filePath));
                    GameUtil.Write2Disk(savePath, encryptedLuaFile);
                }
            }
            AssetDatabase.Refresh();
        }

        private static void EncryptLuaToRes2Bundle()
        {
            EncryptAndCopy(PathProtocol.LuaSourceCodeDir, PathProtocol.Res2BundleLuasDirPath, new string[] { ".lua" });

        }

        private static void EncryptProtosToRes2Bundle()
        {
            EncryptAndCopy(PathProtocol.ProtoSourceCodeDir, PathProtocol.Res2BundleProtosDirPath, new string[] { ".proto" });
        }
    }
}