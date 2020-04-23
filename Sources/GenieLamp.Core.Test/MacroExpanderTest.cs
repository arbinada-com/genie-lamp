using NUnit.Framework;
using System;
using System.Text;

using GenieLamp.Core.Utils;

namespace GenieLamp.Core.Test
{
	[TestFixture()]
	public class MacroExpanderTest
	{
		[Test()]
		public void TestCase()
		{
			string envVarName = "GL_TEST";
			string envVar = "%" + envVarName + "%";
			string envVarValue = "/RootTest";

			string source = String.Format("Output to: {0}/%PROJECT_DIR%/%PROJECT_NAME%.sql", envVar);
			string projectName = "TestProject";
			string projectDir = "TestDir/Subdir";
			string target = String.Format("Output to: {0}/{1}/{2}.sql", envVarValue, projectDir, projectName);

			Environment.SetEnvironmentVariable(envVarName, envVarValue);
			Assert.AreEqual("%%", System.Environment.ExpandEnvironmentVariables("%%"));
			Assert.AreEqual(envVarValue, System.Environment.ExpandEnvironmentVariables(envVar));
			Assert.AreEqual("%" + envVar + "%", Utils.Sys.ExpandEnvironmentVariables("%" + envVar + "%"));

			Assert.AreEqual("%TEST123%", Utils.Sys.ExpandEnvironmentVariables("%TEST123%"));
			Assert.AreEqual("%TEST123%_%TEST456%", Utils.Sys.ExpandEnvironmentVariables("%TEST123%_%TEST456%"));
			Assert.AreEqual(envVarValue, Utils.Sys.ExpandEnvironmentVariables(envVar));
			Assert.AreEqual(String.Format("{0}_{0}", envVarValue),
			                String.Format("{0}_{0}", Utils.Sys.ExpandEnvironmentVariables(envVar)));

				
			MacroExpander m = new MacroExpander();
			m.SetMacro("%PROJECT_NAME%", projectName);
			m.SetMacro("%PROJECT_DIR%", projectDir);
			Assert.AreEqual(2, m.Count);
			
			Assert.AreEqual(target, m.Subst(source));
			m.Clear();
			Assert.AreEqual(0, m.Count);

			string unchanged = "IX%COUNTER%_%TABLE%_%COLUMNS%";
			Assert.AreEqual(unchanged, m.Subst(unchanged));
		}
		
	}
}

