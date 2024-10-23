using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections.Specialized;
using SkiaSharp;

namespace AvaloniaDemo.Utils
{
	public static class FilePathDirUtil
	{
		public static Encoding GBK = Encoding.GetEncoding("gbk");
		public static Encoding UTF8 = new UTF8Encoding(false);

		public static bool CheckFileOccupied(string path)
		{
			if (string.IsNullOrEmpty(path) || !File.Exists(path))
				return false;
			bool isOccupied = true;
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) {
				isOccupied = false;
			}
			return isOccupied;
		}
		public static bool CanRead(string path)
		{
			if (string.IsNullOrEmpty(path) || !File.Exists(path))
				return false;
			bool cr = false;
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None)) {
				cr = true;
			}
			return cr;
		}
		public static bool CanWrite(string path)
		{
			if (string.IsNullOrEmpty(path) || !File.Exists(path))
				return false;
			bool cw = false;
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None)) {
				cw = true;
			}
			return cw;
		}
		public static string[] ReadAllLines(string path, Encoding encoding)
		{
			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			return ReadAllLines(stream, encoding);
		}
		public static string[] ReadAllLines(FileStream stream, Encoding encoding)
		{
			string? line;
			List<string> lines = new List<string>();
			using StreamReader sr = new StreamReader(stream, encoding);
			while ((line = sr.ReadLine()) != null) {
				lines.Add(line);
			}
			return lines.ToArray();
		}

		// 有现成的：Path.GetRelativePath(to, path)
		// https://blog.walterlv.com/post/make-relative-file-path.html
		public static string MakeRelativePath(string fromPath, string toPath)
		{
			if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
			if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));
			var fromUri = new Uri(fromPath);
			var toUri = new Uri(toPath);
			if (fromUri.Scheme != toUri.Scheme) {
				// 不是同一种路径，无法转换成相对路径。
				return toPath;
			}
			if (fromUri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase)
				&& !fromPath.EndsWith("/", StringComparison.OrdinalIgnoreCase)
				&& !fromPath.EndsWith("\\", StringComparison.OrdinalIgnoreCase)) {
				// 如果是文件系统，则视来源路径为文件夹。
				fromUri = new Uri(fromPath + Path.DirectorySeparatorChar);
			}
			var relativeUri = fromUri.MakeRelativeUri(toUri);
			var relativePath = Uri.UnescapeDataString(relativeUri.ToString());
			if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase)) {
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}
			return relativePath;
		}

		public static string AppendDirectorySeparatorChar(string path)
		{
			if (!Path.HasExtension(path) && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
				return path + Path.DirectorySeparatorChar;
			return path;
		}
		// Creates a relative path from one file or folder to another.
		// https://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
		public static string GetRelativePath(string fromPath, string toPath)
		{
			if (string.IsNullOrEmpty(fromPath))
				throw new ArgumentNullException(nameof(fromPath));
			if (string.IsNullOrEmpty(toPath))
				throw new ArgumentNullException(nameof(toPath));
			var fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
			var toUri = new Uri(AppendDirectorySeparatorChar(toPath));
			if (fromUri.Scheme != toUri.Scheme)
				return toPath;
			var relativeUri = fromUri.MakeRelativeUri(toUri);
			var relativePath = Uri.UnescapeDataString(relativeUri.ToString());
			if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			return relativePath;
		}
		
		public static string GetPathWithoutExtension(string path)
		{
			return Path.Join(Path.GetDirectoryName(path.AsSpan()), Path.GetFileNameWithoutExtension(path.AsSpan()));
		}

	}
}
