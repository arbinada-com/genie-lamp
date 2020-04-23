using System;
using System.Text;
using System.Collections.Generic;

using GenieLamp.Core;
using GenieLamp.Core.Exceptions;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;
using GenieLamp.Core.Patterns;

namespace GenieLamp.Genies.ServicesLayer.ServiceStack
{
	public class CodeGenStackServicesInterfaces : CodeGenBase
	{
		private OperationHelper oph;
		private ServicesInterfacesHelper intfh;

		public CodeGenStackServicesInterfaces(GenieBase genie)
			: base(genie)
		{
			outFileName = "ServicesInterfaces.cs";

			genie.Config.Macro.SetMacro("%ServiceRequest_Persistence%",
			                            NamingHelperStack.ToServiceRequestName(NamingHelper.ServiceName_Persistence));
			genie.Config.Macro.SetMacro("%ServiceResponse_Persistence%",
			                            NamingHelperStack.ToServiceResponseName(NamingHelper.ServiceName_Persistence));
		}

		#region implemented abstract members of GenieLamp.Genies.ServicesLayer.CodeGenBase
		protected override void Write()
		{
			cw.WriteUsing("System.Runtime.Serialization");
			cw.WriteUsing("System.Collections.Generic");
			cw.WriteCommentLine("Assembly required: ServiceStack.Interfaces.dll");
			cw.WriteUsing("ServiceStack.ServiceHost");
			cw.WriteUsing("ServiceStack.ServiceInterface");
			cw.WriteUsing("ServiceStack.ServiceInterface.ServiceModel");
			cw.WriteLine();

			environment.BaseNamespace = ServicesLayerConfig.ServicesInterfacesNamespace;

			oph = new OperationHelper(cw, environment);
			intfh = new ServicesInterfacesHelper(cw, environment);

			ns.BeginScope(environment.BaseNamespace);

			Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
			List<string> patternNames = new List<string>();
			foreach(IPatternConfig pattern in this.genie.Lamp.Config.Patterns)
				patternNames.Add(pattern.Name);
			data.Add("patterns", patternNames);
			if (this.genie.Lamp.Config.Patterns.Localization != null)
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
				"Templates.ServicesInterfaces.cs");
			var template = new TextTemplate.Template< Dictionary<string, List<string>> >("data");
			template.Content = genie.Config.Macro.Subst(content); // Substitute all macros before compiling template to avoid errors
			cw.WriteText(template.Execute(data));

//			cw.WriteText(this.GetType().Assembly,
//			             "Templates.ServicesInterfaces.cs",
//			             genie.Config.Macro);
			cw.WriteLine();

			cw.WriteText(this.GetType().Assembly,
			             "Templates.StackServicesInterfaces.cs",
			             genie.Config.Macro);
			cw.WriteLine();

			ProcessEntities();

			ns.EndScope();
		}
		#endregion

		protected override void ProcessEntity(IEntity entity)
		{
			WriteRequestClass(entity);
			cw.WriteLine();
			WriteScalarResponse(entity);
			cw.WriteLine();
			WriteCollectionPesponse(entity);
			cw.WriteLine();
		}

		private void WriteRequestClass(IEntity entity)
		{
    		cw.WriteLine("[DataContract]");
			cw.WriteLine(RestServiceHelper.GetRestServiceSpec(entity, String.Empty));
			if (entity.Persistence.Persisted)
			{
				cw.WriteLine(RestServiceHelper.GetRestServiceSpec(entity, entity.Constraints.PrimaryId.Attributes));
				foreach(IUniqueId uid in entity.Constraints.UniqueIds)
				{
					cw.WriteLine(RestServiceHelper.GetRestServiceSpec(entity, uid.Attributes));
				}
				foreach (IRelation r in entity.Parents)
				{
					cw.WriteLine(RestServiceHelper.GetRestServiceSpec(entity, r.ChildAttributes));
				}

				cw.WriteLine(RestServiceHelper.GetRestServiceSpec(
					entity,
					String.Format("/{0}/{{{0}}}/{1}/{{{1}}}/{2}/{{{2}}}/{3}/{{{3}}}",
				              NamingHelperStack.ParamName_PageNumber,
				              NamingHelperStack.ParamName_PageSize,
				              NamingHelperStack.ParamName_SortOrderProperty,
				              NamingHelperStack.ParamName_SortOrderAsc)));
			}

			oph.WriteOperationsSpec(entity);

			cw.BeginClass(AccessLevel.Public,
			              true,
			              NamingHelperStack.ToServiceRequestName(entity, null),
			              Const.EmptyName);

			intfh.WriteProperties(entity.Constraints.PrimaryId.Attributes);
			foreach(IUniqueId uid in entity.Constraints.UniqueIds)
			{
				intfh.WriteProperties(uid.Attributes);
			}
			foreach (IRelation r in entity.Parents)
			{
				intfh.WriteProperties(r.ChildAttributes);
			}

			intfh.WriteProperty("string", NamingHelperStack.ParamName_Query);
			intfh.WriteProperty(NamingHelper.ClassName_ServicesQueryParams, NamingHelperStack.ParamName_QueryParams);
			intfh.WritePaginationProperties(entity);

			intfh.WriteProperty(NamingHelper.ToDTOTypeName(entity, null),
			              NamingHelper.ToDTOPropertyName(entity));

			oph.WriteParamClassesProperties(entity);
			cw.WriteLine();

			cw.EndClass();

			cw.WriteLine();
			oph.WriteParamClasses(entity);
		}

		private void WriteScalarResponse(IEntity entity)
		{
    		cw.WriteLine("[DataContract]");
			cw.BeginClass(AccessLevel.Public,
			              true,
			              NamingHelperStack.ToServiceResponseName(entity, null),
			              Const.EmptyName);

			intfh.WriteProperty(NamingHelper.ToDTOTypeName(entity, null),
			              NamingHelper.ToDTOPropertyName(entity));

			intfh.WriteProperty(NamingHelper.ClassName_CommitResult,
			              NamingHelper.PropertyName_CommitResult);

			intfh.WriteProperty("ResponseStatus", "ResponseStatus");

			oph.WriteParamClassesProperties(entity);

			cw.WriteLine();
            cw.BeginRegion("Constructors");
			cw.WriteLine("public {0}()", NamingHelperStack.ToServiceResponseName(entity, null));
			cw.BeginScope();
            cw.WriteLine("this.ResponseStatus = new ResponseStatus();");
            cw.WriteLine("this.{0} = new {0}();",
			             NamingHelper.ClassName_CommitResult,
			             NamingHelper.PropertyName_CommitResult);

			cw.EndScope();
			cw.EndRegion();

			cw.EndClass();
		}

		private void WriteCollectionPesponse(IEntity entity)
		{
    		cw.WriteLine("[DataContract]");
			cw.BeginClass(AccessLevel.Public,
			              true,
			              NamingHelperStack.ToServiceResponseCollectionName(entity, null),
			              Const.EmptyName);

			intfh.WriteProperty(NamingHelperStack.ToDTOCollectionTypeName(entity, null),
			              NamingHelperStack.ToDTOCollectionPropertyName(entity));

			intfh.WriteProperty("ResponseStatus", "ResponseStatus");

			cw.WriteLine();
            cw.BeginRegion("Constructors");
			cw.WriteLine("public {0}()", NamingHelperStack.ToServiceResponseCollectionName(entity, null));
			cw.BeginScope();
            cw.WriteLine("this.ResponseStatus = new ResponseStatus();");
            cw.WriteLine("this.{0} = new {1}();",
			             NamingHelperStack.ToDTOCollectionPropertyName(entity),
			             NamingHelperStack.ToDTOCollectionTypeName(entity, null));
			cw.EndScope();
			cw.EndRegion();

			cw.EndClass();
		}

	}
}

