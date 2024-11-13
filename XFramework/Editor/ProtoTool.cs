using System.IO;
using XEngine.Engine;
using UnityEditor;
using UnityEngine;


namespace XEngine.Editor
{
	internal class ProtoTool : UnityEditor.Editor
	{
		private const string protoFileExtension = ".proto";
		private const string annotationFileExtension = ".lua.txt";
		private const string protoSourceCodeDirectory = PathProtocol.AssetsPathRes2BundleDir + "Protos/";
		private const string generateAnnotationDirectory = PathProtocol.LuaSourceCodeDir + "Annotations/";
		private const string generateLuaProtobufDirectory = PathProtocol.LuaSourceCodeDir + "protobuf/";

		private static bool IsFileType(string fileName, string extension)
		{
			return Path.GetExtension(fileName) == extension;
		}

		private static void PrepareOutputDirectory(params string[] directory)
		{
			foreach (var dir in directory)
			{
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
			}
		}

		[MenuItem("Tools/Protos/Generate Annotations")]
		private static void GenerateProtobufAnnotations()
		{
			string[] protoFileDirectory = GameUtil.GetAllFilesFromDirectory(protoSourceCodeDirectory);
			PrepareOutputDirectory(generateAnnotationDirectory, generateLuaProtobufDirectory);
			foreach (var fileName in protoFileDirectory)
			{
				if (IsFileType(fileName, protoFileExtension))
				{
					GenerateAnnotationForProtoFile(fileName);
				}
			}
		}

		private static void GenerateAnnotationForProtoFile(string protoFile)
		{
			UnityEngine.Assertions.Assert.IsTrue(IsFileType(protoFile, protoFileExtension));
			string[] lines = File.ReadAllLines(protoFile);
			if (lines.Length == 0)
			{
				Debug.LogFormat("Empty file(ignored): {protoFile}");
				return;
			}
			var protoContent = string.Join("\n", lines);

			var fileName = Path.GetFileName(protoFile);
			var annotationFileName = fileName.Replace(protoFileExtension, annotationFileExtension);
			var annotationFilePath = Path.Combine(generateAnnotationDirectory, annotationFileName);
			GenerateAnnotationsForProto(protoContent, out var annotations);
			File.WriteAllText(annotationFilePath, annotations);
			Debug.LogFormat($"Generated annotation file for {protoFile} to {annotationFilePath}");
		}

		private static void GenerateAnnotationsForProto(string proto, out string annotations)
		{
			// TODO: 在这里解析 proto 语法，生成 emmylua 注释
			annotations = proto;
			return;
		}
	}
}