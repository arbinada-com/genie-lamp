using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Patterns;

namespace GenieLamp.Genies.NHibernate
{
	public class CodeGenDomainSupport : CodeGenBase
	{
		public const string ClassName_SessionManager = "SessionManager";
		public const string ClassName_QueryFactory = "QueryFactory";
		public const string ClassName_EntityAuditor = "EntityAuditListener";
		public const string ClassName_CommonEntityInterceptor = "CommonEntityInterceptor";
		public const string FunctionName_GetSession = "GetSession";
		public const string FunctionName_CloseSession = "CloseSession";
		public const string InterfaceName_PersistentObject = "IPersistentObject";
		// Events
		public const string InterfaceName_OnSave = "IOnSave";
		public const string InterfaceName_OnDelete = "IOnDelete";
		public const string InterfaceName_OnFlush = "IOnFlush";
		public const string InterfaceName_OnValidatable = "NHibernate.Classic.IValidatable";
		public const string ClassName_EventHandlerBase = "EventHandlerBase";
		public const string ClassName_DomainEventHandler = "DomainEventHandler";
		// Patterns
		public const string InterfaceName_UsesRegistry = "IUsesRegistry";
		public const string IUsesRegistry_Property_Registry = "Registry";
		public const string InterfaceName_UsesAudit = "IUsesAudit";

		/// <summary>
		/// Generates domain infrastructure classes, interfaces and helpers.
		/// </summary>
		/// <param name='genie'>
		/// NHibernate genie.
		/// </param>
		public CodeGenDomainSupport(NHibernateGenie genie)
			: base(genie)
		{
			outFileName = "DomainSupport.cs";
			genie.Config.Macro.SetMacro("%InterfaceName_PersistentObject%", InterfaceName_PersistentObject);
			genie.Config.Macro.SetMacro("%ClassName_CommonEntityInterceptor%", ClassName_CommonEntityInterceptor);
			// Quering
			genie.Config.Macro.SetMacro("%ClassName_QueryFactory%", ClassName_QueryFactory);
			genie.Config.Macro.SetMacro("%ClassName_DomainQueryParams%", DomainLayerConfig.GetClassName_QueryParams(false));
			genie.Config.Macro.SetMacro("%ClassName_SortOrder%", DomainLayerConfig.GetClassName_SortOrder(false));
			// Events
			genie.Config.Macro.SetMacro("%InterfaceName_OnSave%", InterfaceName_OnSave);
			genie.Config.Macro.SetMacro("%InterfaceName_OnDelete%", InterfaceName_OnDelete);
			genie.Config.Macro.SetMacro("%InterfaceName_OnFlush%", InterfaceName_OnFlush);
			genie.Config.Macro.SetMacro("%ClassName_EventHandlerBase%", ClassName_EventHandlerBase);
			genie.Config.Macro.SetMacro("%ClassName_DomainEventHandler%", ClassName_DomainEventHandler);
			// Patterns
			genie.Config.Macro.SetMacro("%ClassName_EntityAuditor%", ClassName_EntityAuditor);
			genie.Config.Macro.SetMacro("%InterfaceName_UsesRegistry%", InterfaceName_UsesRegistry);
			genie.Config.Macro.SetMacro("%IUsesRegistry_Property_Registry%", IUsesRegistry_Property_Registry);
			genie.Config.Macro.SetMacro("%InterfaceName_UsesAudit%", InterfaceName_UsesAudit);
		}


		protected override void InternalRun()
		{
			WriteTemplate("GenieLamp.Genies.NHibernate.Templates.Persistence.cs");

			cw.WriteText(
				this.GetType().Assembly,
				"GenieLamp.Genies.NHibernate.Templates.Querying.cs",
				genie.Config.Macro);
			cw.WriteLine();

			WriteTemplate("GenieLamp.Genies.NHibernate.Templates.Patterns.cs");
		}


		private void WriteTemplate(string resourceName)
		{
			Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
			List<string> patternNames = new List<string>();
			foreach(IPatternConfig pattern in Patterns)
				patternNames.Add(pattern.Name);
			data.Add("patterns", patternNames);

			if (Patterns.Registry != null)
			{
				List<string> registryDetails = new List<string>();
				registryDetails.Add(environment.ToTypeName(Patterns.Registry.RegistryEntity.Entity, true));
				registryDetails.Add(environment.ToTypeName(Patterns.Registry.TypesEntity.Entity, true));
				registryDetails.Add(environment.ToTypeName(Patterns.Registry.TypesEntity.Entity, false));
				registryDetails.Add(Patterns.Registry.TypesEntity.FullNameAttribute.Name);
				registryDetails.Add(Patterns.Registry.RegistryEntity.Entity.Name);
				data.Add("pattern.Registry", registryDetails);
			}

			if (Patterns.Audit != null)
			{
				List<string> auditDetails = new List<string>();
				auditDetails.Add(Patterns.Audit.CreatedByAttributeName);
				auditDetails.Add(Patterns.Audit.CreatedDateAttributeName);
				auditDetails.Add(Patterns.Audit.LastModifiedByAttributeName);
				auditDetails.Add(Patterns.Audit.LastModifiedDateAttributeName);
				data.Add("pattern.Audit", auditDetails);
			}

			if (Patterns.Localization != null)
			{
				List<string> localizationDetails = new List<string>();
				string satelliteAssemblyName = DomainLayerConfig.LocalizationParams.ValueByName("SatelliteAssemblyName", String.Empty);
				if (!String.IsNullOrEmpty(satelliteAssemblyName))
					satelliteAssemblyName = "\"" + satelliteAssemblyName + "\"";
				localizationDetails.Add(satelliteAssemblyName);
				data.Add("pattern.Localization", localizationDetails);
			}

			string content = GenieLamp.Core.Utils.Text.ReadFromResource(
				this.GetType().Assembly,
				resourceName);
			var template = new TextTemplate.Template< Dictionary<string, List<string>> >("data");
			template.Content = genie.Config.Macro.Subst(content); // Substitute all macros before compiling template to avoid errors
			cw.WriteText(template.Execute(data));
			cw.WriteLine();
		}

		/// <summary>
		/// Writes registry pattern event listener
		/// </summary>
		private void WriteRegistryImpl()
		{
			if (Patterns.Registry == null)
				return;
			// Use OnPreInsert: OnSaveOrUpdate is not working 
			// under SQL Server in transaction scope when multiples objects being created
		    cw.BeginClass(AccessLevel.Public, false,
			              "EntityRegistryListener",
			              "NHibernate.Event.IPreInsertEventListener, NHibernate.Event.IFlushEntityEventListener");

			cw.BeginFunction("public bool OnPreInsert(NHibernate.Event.PreInsertEvent e)");
		    cw.WriteLine("if (!(e.Entity is {0}) || (e.Entity as {0}).Registry != null) return false;",
			             InterfaceName_UsesRegistry);
			cw.WriteLine("ISession childSession = e.Session.GetSession(EntityMode.Poco);");
			cw.WriteLine("{0} r = new {0}();", environment.ToTypeName(Patterns.Registry.RegistryEntity.Entity, true));
			cw.WriteLine("r.{0} = childSession.CreateCriteria<{1}>()",
			             environment.ToTypeName(Patterns.Registry.TypesEntity.Entity, false),
			             environment.ToTypeName(Patterns.Registry.TypesEntity.Entity, true));
			cw.Indent++;
			cw.WriteLine(".Add(NHibernate.Criterion.Expression.Eq(\"{0}\", e.Entity.GetType().FullName))",
			             Patterns.Registry.TypesEntity.FullNameAttribute.Name);
			cw.WriteLine(".UniqueResult<{0}>();", 
			             environment.ToTypeName(Patterns.Registry.TypesEntity.Entity, true));
			cw.Indent--;
			cw.WriteLine("childSession.Save(r);");
			cw.WriteLine("childSession.Flush();");
			cw.WriteLine("e.Session.Refresh(r);");
			cw.WriteLine("int i = Array.IndexOf(e.Persister.PropertyNames, \"{0}\");",
			             Patterns.Registry.RegistryEntity.Entity.Name);
			cw.WriteLine("e.State[i] = r;");
			cw.WriteLine("(e.Entity as IUsesRegistry).{0} = r;", IUsesRegistry_Property_Registry);
			cw.WriteLine("return false;");
			cw.EndFunction();
			cw.WriteLine();

			cw.BeginFunction("public void OnFlushEntity(NHibernate.Event.FlushEntityEvent e)");
		    cw.WriteLine("if (e.EntityEntry.Status != NHibernate.Engine.Status.Deleted || !(e.Entity is {0}) || (e.Entity as {0}).Registry == null) return;",
			             InterfaceName_UsesRegistry);
			cw.WriteLine();
			cw.WriteLine("(e.Entity as {0}).{1}.Delete(e.Session.Transaction);",
			             InterfaceName_UsesRegistry,
			             IUsesRegistry_Property_Registry);
			cw.EndFunction();

			cw.EndClass();
		}
	}
}

