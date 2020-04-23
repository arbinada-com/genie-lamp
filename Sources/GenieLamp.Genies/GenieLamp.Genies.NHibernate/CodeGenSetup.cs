using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Patterns;
using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.CodeWriters.CSharp;

namespace GenieLamp.Genies.NHibernate
{
	public class CodeGenSetup : CodeGenBase
	{
		private const string ClassName_DomainSetup = "DomainSetup";
		private const string MethodName_Init = "Init";
		private const string MethodName_Cleanup = "Cleanup";
		private const string VarName_EntityType = "entityType";

		public CodeGenSetup(NHibernateGenie genie)
			: base(genie)
		{
			outFileName = "DomainSetup.cs";
		}

		protected override void InternalRun()
		{
			cw.WriteUsing("System");
			cw.WriteLine();
			cw.WriteUsing(DomainLayerConfig.PersistenceNamespace);
			cw.WriteLine();

			nswriter.BeginScope(DomainLayerConfig.BaseNamespace);

			cw.BeginStaticClass(AccessLevel.Public,
			                        false,
			                        ClassName_DomainSetup,
			                        null);
			WriteInit();
			cw.WriteLine();
			WriteCleanup();
			cw.WriteLine();
			cw.EndClass();

			nswriter.EndScope();
		}


		private void WriteInit()
		{
			cw.WriteLine("public static void {0}()", MethodName_Init);
			cw.BeginScope();
			int i = 1;
			foreach(IEntity entity in Model.Entities)
			{
				if (Patterns.Registry != null && Patterns.Registry.AppliedToEntityOrSupertypes(entity))
				{
					string varName = String.Format("{0}{1}", VarName_EntityType, i++);
					cw.WriteLine("{0} {1} = {0}.GetBy{2}(typeof({3}).FullName);",
					             environment.ToTypeName(Patterns.Registry.TypesEntity.Entity, true),
					             varName,
					             Patterns.Registry.TypesEntity.FullNameAttribute.Name,
					             environment.ToTypeName(entity, true));
					cw.If("{0} == null", varName);
					cw.WriteLine("{0} = new {1}();",
					             varName,
					             environment.ToTypeName(Patterns.Registry.TypesEntity.Entity, true));
					cw.WriteLine("{0}.{1} = typeof({2}).FullName;",
					             varName,
					             Patterns.Registry.TypesEntity.FullNameAttribute.Name,
					             environment.ToTypeName(entity, true));
					cw.WriteLine("{0}.{1} = typeof({2}).Name;",
					             varName,
					             Patterns.Registry.TypesEntity.ShortNameAttribute.Name,
					             environment.ToTypeName(entity, true));
					if (!entity.HasDoc)
						cw.WriteLine("{0}.Save();", varName);
					cw.EndIf();
					if (entity.HasDoc)
					{
						cw.WriteLine("{0}.{1} = {2};",
						             varName,
						             Patterns.Registry.TypesEntity.DescriptionAttribute.Name,
						             cw.ToConst(entity.Doc.GetLabel()));
						cw.WriteLine("{0}.Save();", varName);
					}
				}
			}
			cw.EndScope();
		}

		private void WriteCleanup()
		{
			cw.WriteLine("public static void {0}()", MethodName_Cleanup);
			cw.BeginScope();
			cw.EndScope();
		}
	}
}

