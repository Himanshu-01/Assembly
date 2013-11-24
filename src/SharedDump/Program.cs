﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Blamite.Blam;
using Blamite.Blam.Resources;
using Blamite.Flexibility;
using Blamite.Flexibility.Settings;
using Blamite.IO;
using Blamite.Util;

namespace SharedDump
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("Usage: SharedDump <map file(s)>");
				return;
			}

			// Locate the Formats folder and SupportedBuilds.xml
			string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string formatsDir = Path.Combine(exeDir, "Formats");
			string supportedBuildsPath = Path.Combine(formatsDir, "Engines.xml");
			EngineDatabase db = XMLEngineDatabaseLoader.LoadDatabase(supportedBuildsPath);

			// Dump each map file
			foreach (string arg in args)
			{
				Console.WriteLine("{0}...", arg);
				DumpSharedResources(arg, db);
			}
		}

		private static void DumpSharedResources(string mapPath, EngineDatabase db)
		{
			ICacheFile cacheFile;
			ResourceTable resources;
			using (var reader = new EndianReader(File.OpenRead(mapPath), Endian.BigEndian))
			{
				cacheFile = CacheFileLoader.LoadCacheFile(reader, db);
				resources = cacheFile.Resources.LoadResourceTable(reader);
			}

			using (var output = new StreamWriter(Path.ChangeExtension(mapPath, ".txt")))
			{
				output.WriteLine("Shared resources referenced by {0}:", Path.GetFileName(mapPath));
				output.WriteLine();
				output.WriteLine("Rsrc Datum  Map File      Class  Tag");
				output.WriteLine("----------  --------      -----  ---");

				foreach (Resource resource in resources.Resources.Where(r => r.Location != null && r.ParentTag != null))
				{
					// If either page has a null file path, then it's shared
					ResourcePointer loc = resource.Location;
					string primaryFile = (loc.PrimaryPage != null) ? loc.PrimaryPage.FilePath : null;
					string secondaryFile = (loc.SecondaryPage != null) ? loc.SecondaryPage.FilePath : null;
					if (primaryFile != null || secondaryFile != null)
					{
						string className = CharConstant.ToString(resource.ParentTag.Class.Magic);
						string name = cacheFile.FileNames.GetTagName(resource.ParentTag) ?? resource.ParentTag.Index.ToString();
						string fileName = primaryFile ?? secondaryFile;
						fileName = fileName.Substring(fileName.IndexOf('\\') + 1);
						output.WriteLine("{0}  {1, -12}  {2}   {3}", resource.Index, fileName, className, name);
					}
				}
			}
		}
	}
}