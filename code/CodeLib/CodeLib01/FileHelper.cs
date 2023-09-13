using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLib01;

public static class FileHelper
{
    /// <summary>
    /// 删除掉空文件夹<br/>
    /// 所有没有子“文件系统”的都将被删除<br/>
    /// 2023-9-13 Ciaran
    /// </summary>
    /// <param name="path"></param>
    public static void DelEmptyDirectory(string path, bool del_current)
    {
        var dir = new DirectoryInfo(path);
        var sub_dirs = dir.GetDirectories();

        foreach (var sub_dir1 in sub_dirs)
        {
            if (sub_dir1.EnumerateFileSystemInfos().Any())
            {
                DelEmptyDirectory(sub_dir1.FullName, true);
            }
            else
            {
                // 删除空文件夹
                sub_dir1.Delete();
            }
        }

        // 如果当前文件夹为空, 判断删除
        if (!dir.EnumerateFileSystemInfos().Any())
        {
            if (del_current)
                dir.Delete();
            return;
        }
    }
}
