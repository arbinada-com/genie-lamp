using System;
using System.Text;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;
using System.Collections.Generic;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Genies.ServicesLayer.ServiceStack
{
	public class CodeGenDomainDTOAdapters : CodeGenBase
	{
		private const string ClassName_WebClientFactory = "WebClientFactory";
		private const string ClassName_DTOAdapterCollection = "DTOAdapterCollection";
		private const string ClassName_PersistentDTOAdapterCollection = "PersistentDTOAdapterCollection";
		private const string ClassName_DomainObjectDTOAdapter = "DomainObjectDTOAdapter";
		private const string MethodName_GetPage = "GetPage";
		private const string MethodName_GetByQuery = "GetByQuery";
		private const string MemberName_DTO = "dto";
		private const string VarName_DTO = "dto";
		private const string VarName_DTOCollection = "dtoCollection";
		private const string VarName_DTOAdapterCollection = "collection";
		private const string VarName_Request = "request";
		private const string VarName_Response = "response";

		public CodeGenDomainDTOAdapters(GenieBase genie)
			: base(genie)
		{
			outFileName = "DomainDTOAdapters.cs";
			genie.Config.Macro.SetMacro("%ClassName_WebClientFactory%", ClassName_WebClientFactory);
			genie.Config.Macro.SetMacro("%ClassName_DTOAdapterCollection%", ClassName_DTOAdapterCollection);
			genie.Config.Macro.SetMacro("%ClassName_PersistentDTOAdapterCollection%", ClassName_PersistentDTOAdapterCollection);
			genie.Config.Macro.SetMacro("%VarName_DTOAdapterCollection%", VarName_DTOAdapterCollection);
			genie.Config.Macro.SetMacro("%ClassName_DomainObjectDTO%", NamingHelperStack.ClassName_DomainObjectDTO);
			genie.Config.Macro.SetMacro("%ClassName_PersistentObjectDTO%", NamingHelperStack.ClassName_PersistentObjectDTO);
			genie.Config.Macro.SetMacro("%ClassName_DomainObjectDTOAdapter%", ClassName_DomainObjectDTOAdapter);
			genie.Config.Macro.SetMacro("%ClassName_PersistentObjectDTOAdapter%", NamingHelperStack.ClassName_PersistentObjectDTOAdapter);
			genie.Config.Macro.SetMacro("%AuthRequired%", 
			                            genie.Lamp.Config.Patterns.Security == null ? "false" : "true");
		}

		#region implemented abstract members of GenieLamp.Genies.ServicesLayer.CodeGenBase
		protected override void Write()
		{
			cw.WriteUsing("System.Collections.Generic");
			cw.WriteCommentLine("Assembly required: ServiceStack.Interfaces.dll");
			cw.WriteUsing("ServiceStack.Service");
			cw.WriteCommentLine("Assembly required: ServiceStack.Common.dll");
			cw.WriteUsing("ServiceStack.ServiceClient.Web");
			cw.WriteCommentLine("Assembly required: ServiceStack.ServiceInterface");
			cw.WriteUsing("ServiceStack.ServiceInterface.Auth");
			cw.WriteLine();

			environment.BaseNamespace = ServicesLayerConfig.ServicesAdaptersNamespace;
			ns.BeginScope(environment.BaseNamespace);

			cw.WriteText(this.GetType().Assembly,
			             "Templates.StackDomainDTOAdapters.cs",
			             genie.Config.Macro);
			cw.WriteLine();

			ProcessEntities();

			ns.EndScope();
		}
		#endregion

		protected override void ProcessEntity(IEntity entity)
		{
			WriteEntityDTOAdapter(entity);
		    cw.WriteLine();
			WriteEntityDTOAdapterCollection(entity);
		}

		private void WriteEntityDTOAdapter(IEntity entity)
		{
		    cw.BeginClass(AccessLevel.Public,
			              true,
			              NamingHelperStack.ToDTOAdapterTypeName(entity, null),
			              entity.HasSupertype ?
			              NamingHelperStack.ToDTOAdapterTypeName(entity.Supertype, environment)
			              :
			              entity.Persistence.Persisted ? NamingHelperStack.ClassName_PersistentObjectDTOAdapter : ClassName_DomainObjectDTOAdapter);

			cw.BeginProperty(AccessLevel.Public,
			                 VirtualisationLevel.New,
			                 NamingHelper.ToDTOTypeName(entity, dtoEnvironment),
			                 NamingHelper.PropertyName_AdapterDTO);
			cw.WritePropertyGet("return base.{0} as {1};",
			                    NamingHelper.PropertyName_AdapterDTO,
			                    NamingHelper.ToDTOTypeName(entity, dtoEnvironment));
			cw.WriteLine("internal set {{ base.{0} = value; }}", NamingHelper.PropertyName_AdapterDTO);
			cw.EndProperty();

			cw.BeginRegion("Constructors");
		    cw.BeginFunction("public {0}()", NamingHelperStack.ToDTOAdapterTypeName(entity, null));
			cw.WriteLine("this.{0} = new {1}();",
			             MemberName_DTO,
			             NamingHelper.ToDTOTypeName(entity, dtoEnvironment));
			cw.EndFunction();
		    cw.WriteLine();

			cw.BeginFunction("public {0}({1} {2}) : base({2})",
				                 NamingHelperStack.ToDTOAdapterTypeName(entity, null),
				                 NamingHelper.ToDTOTypeName(entity, dtoEnvironment),
				                 VarName_DTO);
		    cw.EndFunction();
			cw.EndRegion();
		    cw.WriteLine();

			if (entity.Persistence.Persisted)
			{
				WriteGetById(entity);
		    	cw.WriteLine();
				WriteGetByUid(entity);
		    	cw.WriteLine();
				WriteRefresh(entity);
		    	cw.WriteLine();
				WriteDTOPersistenceFunctions(entity);
			    cw.WriteLine();
			}
			cw.BeginRegion("Operations");
			WriteOperations(entity);
			cw.EndRegion();

			ProcessAttributes(entity);
		    cw.WriteLine();
			ProcessRelations(entity);

		    cw.EndClass();
		}

		private void ProcessAttributes(IEntity entity)
		{
			foreach(IAttribute a in entity.Attributes)
			{
				if (a.ProcessInRelations)
					continue;
				WriteTransitionProperty(a);
			}
		}


		private void ProcessRelations(IEntity entity)
		{
			foreach(IRelation r in entity.Relations)
			{
				if (!r.IsParent(entity) && r.ChildNavigate)
				{
					foreach(IAttribute a in r.ChildAttributes)
					{
						WriteTransitionProperty(a);
					}
				}
			}
		}

		private void WriteTransitionProperty(IAttribute attribute)
		{
			cw.BeginProperty(AccessLevel.Public,
			                 VirtualisationLevel.None,
			                 interfacesEnvironment.ToTypeName(attribute, true),
			                 NamingHelper.ToDTOPropertyName(attribute));
			cw.WritePropertyGet("return this.{0}.{1};",
			                    NamingHelper.PropertyName_AdapterDTO,
			                    NamingHelper.ToDTOPropertyName(attribute));
			cw.WritePropertySet("this.{0}.{1} = value; NotifyPropertyChanged(\"{1}\");",
			                    NamingHelper.PropertyName_AdapterDTO,
			                    NamingHelper.ToDTOPropertyName(attribute));
			cw.EndProperty();
		}


		private void WriteGetById(IEntity entity)
		{
			cw.BeginFunction("public static {0}{1} {2}",
			                 entity.HasSupertype ? "new " : "",
			                 NamingHelperStack.ToDTOAdapterTypeName(entity, null),
			                 ServicesLayerConfig.Methods.GetById(entity.PrimaryId, environment).Signature);
			WriteBodyGetByAttributes(entity, entity.PrimaryId.Attributes);
		    cw.EndFunction();
		}


		private void WriteGetByUid(IEntity entity)
		{
			foreach(IUniqueId uid in entity.Constraints.UniqueIds)
			{
				cw.BeginFunction("public static {0} {1}",
				                 NamingHelperStack.ToDTOAdapterTypeName(entity, null),
				                 ServicesLayerConfig.Methods.GetByUniqueId(uid, interfacesEnvironment).Signature);
				WriteBodyGetByAttributes(entity, uid.Attributes);
			    cw.EndFunction();
				cw.WriteLine();
			}
		}

		private void WriteRefresh(IEntity entity)
		{
			cw.BeginFunction("public {0}void Refresh()",
			                 entity.HasSupertype ? "override " : "virtual ");
			List<string> args = new List<string>();
			foreach(IAttribute a in entity.PrimaryId.Attributes)
			{
				args.Add("this." + NamingHelper.ToDTOPropertyName(a));
			}
		    cw.WriteLine("{0} o = {0}.{1};",
			             NamingHelperStack.ToDTOAdapterTypeName(entity, null),
			             ServicesLayerConfig.Methods.GetById(entity.PrimaryId, environment).Call(args.ToArray()));
		    cw.WriteLine("this.{0} = o != null && o.{0} != null ? o.{0} : new {1}();",
			             NamingHelper.PropertyName_AdapterDTO,
			             NamingHelperStack.ToDTOTypeName(entity, dtoEnvironment));
		    cw.EndFunction();
		}


		private void WriteBodyGetByAttributes(IEntity entity, IAttributes attributes)
		{
		    cw.WriteLine("{0} {1} = WebClientFactory.GetJsonClient()",
			             NamingHelperStack.ToDTOTypeName(entity, dtoEnvironment),
			             VarName_DTO);
			cw.Indent++;
		    cw.WriteLine(".Get<{0}>(String.Format(\"/{1}/{2}\", {3}))",
			             NamingHelperStack.ToServiceResponseName(entity, interfacesEnvironment),
			             NamingHelperStack.ToServiceName(entity, null),
			             cw.ToSeparatedString(attributes.ToList(),
			                     "/",
			                     delegate(object item, int count)
			                    {return String.Format("{0}/{{{1}}}", (item as IAttribute).Name, count);}),
			             environment.ToArguments(attributes)
			             );
		    cw.WriteLine(".{0};",
			             NamingHelper.ToDTOPropertyName(entity));
			cw.Indent--;

		    cw.WriteLine("return {0} == null ? null : new {1}({0});",
			             VarName_DTO,
			             NamingHelperStack.ToDTOAdapterTypeName(entity, null));
		}


		private void WriteDTOPersistenceFunctions(IEntity entity)
		{
			cw.BeginFunction("public override {0}.{1} Save(bool throwException = false)",
			                 interfacesEnvironment.BaseNamespace,
			                 NamingHelper.ClassName_CommitResult);
			cw.WriteLine("return PersistenceAction<{0}>({1}.{2}.Action.Save, throwException);",
			             NamingHelperStack.ToDTOTypeName(entity, dtoEnvironment),
			             interfacesEnvironment.BaseNamespace,
			             NamingHelper.ClassName_UnitOfWorkDTO);
			cw.EndFunction();
			cw.WriteLine();

			cw.BeginFunction("public override {0}.{1} Delete(bool throwException = false)",
			                 interfacesEnvironment.BaseNamespace,
			                 NamingHelper.ClassName_CommitResult);
			cw.WriteLine("return PersistenceAction<{0}>({1}.{2}.Action.Delete, throwException);",
			             NamingHelperStack.ToDTOTypeName(entity, dtoEnvironment),
			             interfacesEnvironment.BaseNamespace,
			             NamingHelper.ClassName_UnitOfWorkDTO);
			cw.EndFunction();
		}


		private void WriteOperations(IEntity entity)
		{
			OperationHelper oph = new OperationHelper(cw, interfacesEnvironment);

			foreach(IEntityOperation operation in entity.Operations)
			{
				if (operation.Access != EntityOperationAccess.Public)
					continue;
				cw.BeginFunction("public " + environment.ToOperationSignature(operation, true));
				cw.WriteLine("{0} {1} = new {0}();",
				             NamingHelperStack.ToServiceRequestName(entity, interfacesEnvironment),
				             VarName_Request);
				cw.WriteLine("{0}.{1} = new {2}();",
				             VarName_Request,
				             OperationHelper.GetParamClassPropertyName(operation),
				             oph.GetParamClassName(operation, true));
				foreach(IEntityOperationParam param in operation.Params)
				{
					cw.WriteLine("{0}.{1}.{2} = {3};",
					             VarName_Request,
					             OperationHelper.GetParamClassPropertyName(operation),
					             oph.GetParamPropertyName(param),
					             param.Name);
				}
				cw.WriteLine("{0}.{1} = this.{2};",
				             VarName_Request,
				             NamingHelper.ToDTOPropertyName(entity),
				             NamingHelper.PropertyName_AdapterDTO);

				cw.WriteLine("{0} {1} = WebClientFactory.GetJsonClient()",
				             NamingHelperStack.ToServiceResponseName(entity, interfacesEnvironment),
				             VarName_Response);
				cw.Indent++;
			    cw.WriteLine(".Post<{0}>(\"{1}\", {2});",
				             NamingHelperStack.ToServiceResponseName(entity, interfacesEnvironment),
				             OperationHelper.GetRestServicePath(operation),
				             VarName_Request);
				cw.Indent--;

				cw.If("{0}.ResponseStatus.ErrorCode == \"{1}\"", VarName_Response, RestServiceHelper.ErrorCode);
				cw.WriteLine("throw new Exception({0}.ResponseStatus.Message);", VarName_Response);
				cw.EndIf();
				cw.WriteLine("this.{0} = {1}.{2};",
				             NamingHelper.PropertyName_AdapterDTO,
				             VarName_Response,
				             NamingHelper.ToDTOPropertyName(entity));


				if (operation.Params.HasReturns)
				{
					cw.WriteLine("return {0}.{1}.{2};",
					             VarName_Response,
					             OperationHelper.GetParamClassPropertyName(operation),
					             oph.GetParamPropertyName(operation.Params.Returns));
				}
				cw.EndFunction();
			}

		}


		private void WriteEntityDTOAdapterCollection(IEntity entity)
		{
			string baseClass = String.Format("{0}<{1}",
				              entity.Persistence.Persisted ? ClassName_PersistentDTOAdapterCollection : ClassName_DTOAdapterCollection,
				              NamingHelperStack.ToDTOAdapterTypeName(entity, environment));
			if (entity.Persistence.Persisted)
				baseClass += ", " + NamingHelper.ToDTOTypeName(entity, dtoEnvironment) + ">";
			else
				baseClass += ">";

			cw.BeginClass(AccessLevel.Public,
			              true,
			              NamingHelperStack.ToDTOAdapterCollectionTypeName(entity, null),
			              baseClass);

			cw.BeginRegion("Constructors");
			cw.BeginFunction("public {0} ()",
			                 NamingHelperStack.ToDTOAdapterCollectionTypeName(entity, null));
			cw.EndFunction();
			cw.BeginFunction("public {0} ({1} {2})",
			                 NamingHelperStack.ToDTOAdapterCollectionTypeName(entity, null),
			                 NamingHelperStack.ToDTOCollectionTypeName(entity, dtoEnvironment),
			                 VarName_DTOCollection);
            cw.WriteLine("foreach ({0} {1} in {2})",
			             NamingHelper.ToDTOTypeName(entity, dtoEnvironment),
			             VarName_DTO,
			             VarName_DTOCollection);
			cw.BeginScope();
            cw.WriteLine("this.InternalAdd(new {0}({1}));",
			             NamingHelperStack.ToDTOAdapterTypeName(entity, environment),
			             VarName_DTO);
            cw.EndScope();
			cw.EndFunction();
			cw.EndRegion();
		    cw.WriteLine();

			// Get page
//			cw.BeginFunction("public static {0} {1}(int pageNum = 0, int pageSize = 20)",
//			                 NamingHelperStack.ToDTOAdapterCollectionTypeName(entity, null),
//			                 MethodName_GetPage);
//		    cw.WriteLine("return null;");
//			cw.EndFunction();
//		    cw.WriteLine();

			// Get by query
			cw.BeginFunction("public static {0} {1}(string query, {2}.{3} queryParams, int pageNum = 0, int pageSize = {4})",
			                 NamingHelperStack.ToDTOAdapterCollectionTypeName(entity, null),
			                 MethodName_GetByQuery,
			                 interfacesEnvironment.BaseNamespace,
			                 NamingHelper.ClassName_ServicesQueryParams,
			                 RestServiceHelper.MaxPageSize);
            cw.WriteLine("{0} request = new {0}();", NamingHelperStack.ToServiceRequestName(entity, interfacesEnvironment));
            cw.WriteLine("request.{0} = query;", NamingHelperStack.ParamName_Query);
            cw.WriteLine("request.{0} = queryParams;", NamingHelperStack.ParamName_QueryParams);
            cw.WriteLine("request.{0} = pageNum;", NamingHelperStack.ParamName_PageNumber);
            cw.WriteLine("request.{0} = pageSize;", NamingHelperStack.ParamName_PageSize);
            cw.WriteLine("{0} {1} =",
			             NamingHelperStack.ToServiceResponseCollectionName(entity, interfacesEnvironment),
			             VarName_Response);
			cw.Indent++;
			cw.WriteLine("WebClientFactory.GetJsonClient()");
		    cw.WriteLine(".Post<{0}>(\"/{1}\", request);",
			             NamingHelperStack.ToServiceResponseCollectionName(entity, interfacesEnvironment),
			             NamingHelperStack.ToServiceName(entity, null));
			cw.Indent--;
			cw.WriteLine("WebClientFactory.CheckResponseStatus({0}.ResponseStatus);",
			             VarName_Response);
		    cw.WriteLine("return new {0}({1}.{2});",
			             NamingHelperStack.ToDTOAdapterCollectionTypeName(entity, null),
			             VarName_Response,
			             NamingHelperStack.ToDTOCollectionPropertyName(entity));
			cw.EndFunction();
			cw.WriteLine();

			// Get by relations
			foreach (IRelation r in entity.Parents)
			{
				cw.BeginFunction("public static {0} {1}",
				                 NamingHelperStack.ToDTOAdapterCollectionTypeName(entity, null),
				                 ServicesLayerConfig.Methods.GetByRelationParent(r, interfacesEnvironment).Signature);
				cw.WriteLine("{0} request = new {0}();", NamingHelperStack.ToServiceRequestName(entity, interfacesEnvironment));
				cw.WriteLine("{0} {1} = WebClientFactory.GetJsonClient()",
				             NamingHelperStack.ToServiceResponseCollectionName(entity, interfacesEnvironment),
				             VarName_Response);
				cw.Indent++;
				cw.WriteLine(".Get<{0}>(String.Format(\"/{1}/{2}\", {3}));",
				             NamingHelperStack.ToServiceResponseCollectionName(entity, interfacesEnvironment),
				             NamingHelperStack.ToServiceName(entity, null),
				             cw.ToSeparatedString(r.ChildAttributes.ToList(),
				                     "/",
				                     delegate(object item, int count)
				                     {return String.Format("{0}/{{{1}}}", (item as IAttribute).Name, count);}),
				             environment.ToArguments(r.ChildAttributes)
				             );
				cw.Indent--;
				cw.WriteLine("WebClientFactory.CheckResponseStatus({0}.ResponseStatus);",
				             VarName_Response);
				cw.WriteLine("return new {0}({1}.{2});",
				             NamingHelperStack.ToDTOAdapterCollectionTypeName(entity, null),
				             VarName_Response,
				             NamingHelperStack.ToDTOCollectionPropertyName(entity));
				cw.EndFunction();
				cw.WriteLine();
			}

		    cw.EndClass();
		}
	}
}

