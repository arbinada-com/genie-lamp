using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

using Arbinada.GenieLamp.Warehouse;

using log4net;


namespace Arbinada.GenieLamp.Warehouse.Domain.Test
{
	[SetUpFixture]
	public class Environment
	{
        public static readonly ILog log = LogManager.GetLogger(typeof(MainClass));

		public const string DbName = "Warehouse.db";
		public const string ScriptName_CreateDb = "CRE_Warehouse.sql";

		public static void Init()
		{
			(new Environment()).TextFixtureSetUp();
		}

		[SetUp]
		public void TextFixtureSetUp()
		{
            log4net.Config.XmlConfigurator.Configure();
            log.Info("Init()");


			string path = Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
				"../../../SQL/Sqlite-3/").Replace('/', Path.DirectorySeparatorChar);
			string fileDb = Path.Combine(path, DbName);
			File.Delete(fileDb);
			if (File.Exists(fileDb))
				log.Error("Database file was not deleted");
			string fileSql = Path.Combine(path, ScriptName_CreateDb).Replace('\\', '/');
			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = "sqlite3";
			p.StartInfo.Arguments = fileDb;
			p.Start();
			string cmd = String.Format(".read \"{0}\"", fileSql);
			log.Info(cmd);
			//p.StandardInput.WriteLine(";");
			p.StandardInput.WriteLine(cmd);
			p.StandardInput.WriteLine(";");
			p.StandardInput.WriteLine(".exit");
			p.WaitForExit(10000);
			if (p.HasExited)
			{
				if (p.ExitCode != 0)
				{
					log.WarnFormat("Database may was not created. ExitCode: {0}", p.ExitCode);
					log.Warn(p.StandardError.ReadToEnd());
				}
			}
			else
			{
				p.Close();
				p.Kill();
			}

			try
			{
				DomainSetup.Init();
			}
			catch (Exception e)
			{
				log.Error(e.ToString());
			}
		}

		[TearDown]
		public void TestFixtureTearDown()
		{
		}
	}
}

