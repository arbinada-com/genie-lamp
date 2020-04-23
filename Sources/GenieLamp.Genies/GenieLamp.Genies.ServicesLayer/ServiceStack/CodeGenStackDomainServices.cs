using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;
using System.Collections.Generic;
using GenieLamp.Core.Exceptions;

namespace GenieLamp.Genies.ServicesLayer.ServiceStack
{
	public class CodeGenStackDomainServices : CodeGenBase
	{
		private const string ClassName_UpdatedObjects = "UpdatedObjects";
		private const string ClassName_DomainQueryFactory = "DomainQueryFactory";
		private const string MethodName_DomainObjectToDTO = "DomainObjectToDTO";
		private const string MethodName_DTOToDomainObject = "DTOToDomainObject";
		private const string MethodName_DomainCollectionToDTO = "DomainCollectionToDTO";
		private const string PropertyName_UpdatedObjects = "UpdatedObjects";
		private const string VarName_DTO = "dto";
		private const string VarName_DTOCollection = "dtoCollection";
		private const string VarName_DomainObject = "domain";
		private const string VarName_DomainCollection = "domainCollection";
		private const string VarName_Request = "request";
		private const string VarName_Response = "response";
		private const string VarName_ResponseList = "responseList";
		private const string VarName_UpdatedObjects = "updatedObjects";

		public CodeGenStackDomainServices(GenieBase genie)
			: base(genie)
		{
			outFileName = "DomainServices.cs";
			genie.Config.Macro.SetMacro("%ClassName_DomainQueryFactory%", ClassName_DomainQueryFactory);
			genie.Config.Macro.SetMacro("%ClassFullName_DomainQueryParams%", DomainLayerConfig.GetClassName_QueryParams(true));
		}

		#region Local naming helper functions
		private string ToMethodSignature_DomainObjectToDTO(IEntity entity)
		{
			return String.Format("{0}({1} {2}, {3} {4})",
			                     MethodName_DomainObjectToDTO,
			                     NamingHelper.ToDomainTypeName(entity, domainEnvironment),
			                     VarName_DomainObject,
			                     NamingHelper.ToDTOTypeName(entity, interfacesEnvironment),
			                     VarName_DTO);
		}

		private string ToMethodSignature_DTOToDomainObject(IEntity entity)
		{
			return String.Format("{0}({1} {2}, {3} {4})",
			                     MethodName_DTOToDomainObject,
			                     NamingHelper.ToDTOTypeName(entity, interfacesEnvironment),
			                     VarName_DTO,
			                     NamingHelper.ToDomainTypeName(entity, domainEnvironment),
			                     VarName_DomainObject);
		}

		private string ToMethodSignature_DomainCollectionToDTO(IEntity entity)
		{
			return String.Format("{0}(IList<{1}> {2})",
			                     MethodName_DomainCollectionToDTO,
			                     NamingHelper.ToDomainTypeName(entity, domainEnvironment),
			                     VarName_DomainCollection);
		}
		#endregion

		protected override void Write()
		{
			cw.WriteUsing("System.Runtime.Serialization");
			cw.WriteUsing("System.Collections.Generic");
			cw.WriteCommentLine("Assembly required: ServiceStack.dll");
			cw.WriteCommentLine("Assembly required: ServiceStack.Common.dll");
			cw.WriteCommentLine("Assembly required: ServiceStack.Interfaces.dll");
			cw.WriteCommentLine("Assembly required: ServiceStack.ServiceInterfaces.dll");
			cw.WriteUsing("ServiceStack.ServiceHost");
			cw.WriteUsing("ServiceStack.ServiceInterface");
			cw.WriteUsing("ServiceStack.ServiceInterface.ServiceModel");
			cw.WriteLine();
			cw.WriteUsing(ServicesLayerConfig.ServicesInterfacesNamespace);
			cw.WriteLine();

			ns.BeginScope(environment.BaseNamespace);

			cw.WriteText(this.GetType().Assembly,
			             "Templates.StackDomainServices.cs",
			             genie.Config.Macro);

			WritePersistence();
			cw.WriteLine();
			ProcessEntities();

			ns.EndScope();
		}


		private void WritePersistence()
		{
			if (genie.Lamp.Config.Patterns.Security != null)
				cw.WriteLine("[Authenticate]");
			cw.WriteLine("public partial class {0} : RestServiceBase<{1}>, IRequiresRequestContext",
			             NamingHelperStack.ToServiceName(NamingHelper.ServiceName_Persistence),
			             NamingHelperStack.ToServiceRequestName(NamingHelper.ServiceName_Persistence));
			cw.BeginScope();
			cw.WriteLine("public override object OnGet({0} {1})",
			             NamingHelperStack.ToServiceRequestName(NamingHelper.ServiceName_Persistence),
			             VarName_Request);
			cw.BeginScope();
			cw.WriteLine("return new {0}();",
			             NamingHelperStack.ToServiceResponseName(NamingHelper.ServiceName_Persistence));
			cw.EndScope();
			cw.WriteLine();

			cw.WriteLine("public override object OnPost({0} {1})",
			             NamingHelperStack.ToServiceRequestName(NamingHelper.ServiceName_Persistence),
			             VarName_Request);
			cw.BeginScope();
			cw.WriteLine("{0} {1} = new {0}();",
			             NamingHelperStack.ToServiceResponseName(NamingHelper.ServiceName_Persistence),
			             VarName_Response);
			cw.WriteLine("{0}.{1} = {2}.Commit({3}.{4}, {0}.{5});",
			             VarName_Response,
			             NamingHelper.PropertyName_CommitResult,
			             NamingHelperStack.ToServiceName(NamingHelper.ServiceName_Persistence),
			             VarName_Request,
			             NamingHelper.PropertyName_UnitOfWorkDTO,
			             PropertyName_UpdatedObjects);
			cw.WriteLine("return {0};", VarName_Response);
			cw.EndScope();
			cw.WriteLine();
			// Commit
			cw.BeginFunction("public static {0} Commit({1} {2}, {3} {4})",
			             NamingHelper.ClassName_CommitResult,
			             NamingHelper.ClassName_UnitOfWorkDTO,
			             NamingHelper.VarName_UnitOfWork,
			             ClassName_UpdatedObjects,
			             VarName_UpdatedObjects);
			cw.WriteLine("{0} commitResult = new {0}();", NamingHelper.ClassName_CommitResult);
			cw.WriteLine("{0}.{1} uow = new {0}.{1}();",
			             DomainLayerConfig.PersistenceNamespace,
			             NamingHelper.ClassName_UnitOfWorkDomain);
			cw.WriteLine("foreach({0}.WorkItem wi in unitOfWork.WorkItems)",
			             NamingHelper.ClassName_UnitOfWorkDTO);
			cw.BeginScope();
			cw.WriteLine("InitDomain(wi);");
			cw.If("wi.Action == {0}.Action.Save",
			      NamingHelper.ClassName_UnitOfWorkDTO);
			cw.WriteLine("uow.Save(wi.DomainObject);");
			cw.Else();
			cw.WriteLine("uow.Delete(wi.DomainObject);");
			cw.EndIf();
			cw.EndScope();
			cw.BeginTry();
			cw.WriteLine("uow.Commit();");
			cw.WriteLine("if ({0} != null) {0}.Clear();", VarName_UpdatedObjects);
			cw.WriteLine("foreach({0}.WorkItem wi in unitOfWork.WorkItems)",
			             NamingHelper.ClassName_UnitOfWorkDTO);
			cw.BeginScope();
			cw.If("wi.Action != {0}.Action.Delete", NamingHelper.ClassName_UnitOfWorkDTO);
			cw.WriteLine("InitDTO(wi);");
			cw.WriteLine("if ({0} != null) {0}.Add(wi.Item.Internal_ObjectId, wi.Item);", VarName_UpdatedObjects);
			cw.EndIf();
			cw.EndScope();
			cw.EndTry();
			cw.BeginCatch("Exception e");
			cw.WriteLine("commitResult.HasError = true;", VarName_Response);
			cw.WriteLine("commitResult.Message = e.Message;", VarName_Response);
			cw.WriteLine("commitResult.ExceptionString = e.ToString();", VarName_Response);
			cw.EndCatch();
			cw.WriteLine("return commitResult;");
			cw.EndFunction();
			cw.WriteLine();

			// InitDomain
			cw.BeginFunction("private static void InitDomain({0}.WorkItem wi)",
			                 NamingHelper.ClassName_UnitOfWorkDTO);
			cw.WriteLine("switch(({0})wi.Item.Get{1}())",
			             NamingHelper.EnumName_DomainTypes,
			             NamingHelper.PropertyName_DomainTypeId);
			cw.BeginScope();
			foreach(IEntity entity in Model.Entities)
			{
				if (!entity.Persistence.Persisted)
					continue;
				cw.WriteLine("case {0}.{1}:",
					             NamingHelper.EnumName_DomainTypes,
					             NamingHelper.ToDomainTypesEnumItemName(entity));
				cw.BeginScope();
				List<string> getByIdArgs = new List<string>();
				foreach(IAttribute a in entity.PrimaryId.Attributes)
				{
					getByIdArgs.Add(String.Format("(wi.Item as {0}).{1}",
					                              NamingHelperStack.ToDTOTypeName(entity, interfacesEnvironment),
					                              a.Name));
				}
                cw.WriteLine("wi.DomainObject = {0}.{1};",
				             domainEnvironment.ToTypeName(entity, true),
				             DomainLayerConfig.Methods.GetById(entity.PrimaryId, domainEnvironment).Call(getByIdArgs.ToArray()) );
                cw.If("wi.DomainObject == null");
				cw.WriteLine("wi.DomainObject = new {0}();",
				             domainEnvironment.ToTypeName(entity, true));
				cw.EndIf();
				cw.WriteLine("{0}.{1}(wi.Item as {2}, wi.DomainObject as {3});",
				             NamingHelperStack.ToServiceName(entity, environment),
				             MethodName_DTOToDomainObject,
				             NamingHelperStack.ToDTOTypeName(entity, interfacesEnvironment),
				             domainEnvironment.ToTypeName(entity, true));
				cw.WriteLine("break;");
				cw.EndScope();
			}
			cw.WriteLine("default: throw new ApplicationException(\"Cannot save non-persistent object\");");
			cw.EndScope();
			cw.EndFunction();
			cw.WriteLine();
			// InitDTO
			cw.WriteLine("private static void InitDTO({0}.WorkItem wi)",
			             NamingHelper.ClassName_UnitOfWorkDTO);
			cw.BeginScope();
			cw.WriteLine("switch(({0})wi.Item.Get{1}())",
			             NamingHelper.EnumName_DomainTypes,
			             NamingHelper.PropertyName_DomainTypeId);
			cw.BeginScope();
			foreach(IEntity entity in Model.Entities)
			{
				if (!entity.Persistence.Persisted)
					continue;
				cw.WriteLine("case {0}.{1}:",
					             NamingHelper.EnumName_DomainTypes,
					             NamingHelper.ToDomainTypesEnumItemName(entity));
				cw.BeginScope();
				cw.WriteLine("(wi.DomainObject as {0}).Refresh();",
				             domainEnvironment.ToTypeName(entity, true));
				cw.WriteLine("{0}.{1}(wi.DomainObject as {2}, wi.Item as {3});",
				             NamingHelperStack.ToServiceName(entity, environment),
				             MethodName_DomainObjectToDTO,
				             domainEnvironment.ToTypeName(entity, true),
				             NamingHelperStack.ToDTOTypeName(entity, interfacesEnvironment));
				cw.WriteLine("break;");
				cw.EndScope();
			}
			cw.WriteLine("default: throw new ApplicationException(\"Cannot process non-persistent object\");");
			cw.EndScope();
			cw.EndScope();

			cw.EndScope(); // Class
		}



		protected override void ProcessEntity(IEntity entity)
		{
			if (genie.Lamp.Config.Patterns.Security != null)
				cw.WriteLine("[Authenticate]");
			cw.BeginClass(AccessLevel.Public,
			              true,
			              NamingHelperStack.ToServiceName(entity, null),
			              String.Format("RestServiceBase<{0}>", NamingHelperStack.ToServiceRequestName(entity, interfacesEnvironment)),
			              "IRequiresRequestContext");

			if (entity.Persistence.Persisted)
			{
				WriteOnGet(entity);
				cw.WriteLine();
				WriteOnDelete(entity);
				cw.WriteLine();
				WriteDomainCollectionToDTOCollection(entity);
				cw.WriteLine();
			}
			WriteOnPost(entity);
			cw.WriteLine();
			WriteDomainObjectToDTO(entity);
			cw.WriteLine();
			WriteDTOToDomainObject(entity);
			cw.WriteLine();

			cw.EndClass();
			cw.WriteLine();

		}


		private void WriteInitResponse(string responseVariableName)
		{
			cw.WriteLine("{0}.ResponseStatus.ErrorCode = \"200\";", responseVariableName);
		}

		private void WriteCatch(string responseVariableName)
		{
			cw.BeginCatch("Exception e");
			WriteResponceError(responseVariableName, "e.Message + '\\n' + e.ToString()", "e.ToString()");
			cw.EndCatch();
		}

		private void WriteResponceError(string responseVariableName, string messageExpression, string stackTraceExpression)
		{
			cw.WriteLine("{0}.ResponseStatus.ErrorCode = \"{1}\";", responseVariableName, RestServiceHelper.ErrorCode);
            cw.WriteLine("{0}.ResponseStatus.Message = {1};",
			             responseVariableName,
			             messageExpression);
			if (!String.IsNullOrEmpty(stackTraceExpression))
			{
	            cw.WriteLine("{0}.ResponseStatus.StackTrace = {1};",
				             responseVariableName,
				             stackTraceExpression);
			}
		}

		private void WriteOnGet(IEntity entity)
		{
			cw.BeginFunction("public override object OnGet({0} {1})",
			                 NamingHelperStack.ToServiceRequestName(entity, interfacesEnvironment),
			                 VarName_Request);
	        cw.WriteLine("{0} {1} = new {0}();",
			             NamingHelperStack.ToServiceResponseName(entity, interfacesEnvironment),
			             VarName_Response);
			WriteInitResponse(VarName_Response);
	        cw.WriteLine("{0} {1} = null;",
			             domainEnvironment.ToTypeName(entity, true),
			             VarName_DomainObject);
			cw.BeginTry();

	        cw.If(entity.Constraints.PrimaryId.Attributes.ToNamesString(" && ", VarName_Request + ".", " != null") ); // Global
			// Request by primary id
	        cw.WriteLine("{0} = {1}.{2}({3});",
			             VarName_DomainObject,
			             domainEnvironment.ToTypeName(entity, true),
			             DomainLayerConfig.Methods.GetById(entity.Constraints.PrimaryId, domainEnvironment).Name,
			             cw.ToSeparatedString(entity.Constraints.PrimaryId.Attributes.ToList(), ", ",
			                        delegate(object item, int count) {
									return String.Format("{0}.{1}{2}",
				                     VarName_Request,
				                     (item as IAttribute).Name,
				                     environment.IsNullable(item as IAttribute) ? ".Value" : ""); })
			             );

			// Request by uid
			foreach (IUniqueId uid in entity.Constraints.UniqueIds)
			{
		        cw.ElseIf(uid.Attributes.ToNamesString(" && ", VarName_Request + ".", " != null") );
		        cw.WriteLine("{0} = {1}.{2}({3});",
				             VarName_DomainObject,
				             domainEnvironment.ToTypeName(entity, true),
				             DomainLayerConfig.Methods.GetByUniqueId(uid, domainEnvironment).Name,
				             cw.ToSeparatedString(uid.Attributes.ToList(), ", ",
				                     delegate(object item, int count) {
										string arg = String.Empty;
										if ((item as IAttribute).Type is IEnumerationType)
										{
											arg = String.Format("({0})",
											                    domainEnvironment.ToTypeName((item as IAttribute), true));
										}
										arg += String.Format("{0}.{1}{2}",
										                     VarName_Request,
										                     (item as IAttribute).Name,
										                     environment.IsNullable(item as IAttribute) ? ".Value" : "");
										return arg;	})
				             );
			}

			// Request by relation
			foreach (IRelation r in entity.Parents)
			{
				cw.ElseIf(r.ChildAttributes.ToNamesString(" && ", VarName_Request + ".", " != null") );
				cw.WriteLine("{0} {1} = new {0}();",
				             NamingHelperStack.ToServiceResponseCollectionName(entity, interfacesEnvironment),
				             VarName_ResponseList);
				WriteInitResponse(VarName_ResponseList);
				cw.BeginTry();
				cw.WriteLine("IList<{0}> list = ",
				             domainEnvironment.ToTypeName(entity, true));
				cw.Indent++;
				cw.WriteLine("{0}.{1};",
				             domainEnvironment.ToTypeName(entity, true),
				             DomainLayerConfig.Methods.GetByRelationParent(r, domainEnvironment)
				             .Call(r.ChildAttributes.ToNamesString(", ", VarName_Request + ".", ".Value"))
				             );
				cw.Indent--;
				cw.WriteLine("{0}.{1} = {2}(list);",
				             VarName_ResponseList,
				             NamingHelperStack.ToDTOCollectionPropertyName(entity),
				             MethodName_DomainCollectionToDTO);
				cw.EndTry();
				WriteCatch(VarName_ResponseList);
				cw.WriteLine("return {0};", VarName_ResponseList);
			}

			cw.Else(); // Collection request TOP MaxPageSize or by page
			cw.WriteLine("{0} {1} = new {0}();",
			             NamingHelperStack.ToServiceResponseCollectionName(entity, interfacesEnvironment),
			             VarName_ResponseList);
			WriteInitResponse(VarName_ResponseList);
			cw.BeginTry();
			cw.WriteLine("IList<{0}> list = null;",
			             domainEnvironment.ToTypeName(entity, true));
			cw.If("{0}.{1} != null && {0}.{2} != null",
			      VarName_Request,
			      NamingHelperStack.ParamName_PageNumber,
			      NamingHelperStack.ParamName_PageSize);
			cw.If("{0}.{1} != null && {0}.{2} != null",
			      VarName_Request,
			      NamingHelperStack.ParamName_SortOrderProperty,
			      NamingHelperStack.ParamName_SortOrderAsc);
			cw.WriteLine("list = {0}.GetPage({1}.{2}.Value, {1}.{3}.Value, new {4}[] {{ new {4}({1}.{5}, {1}.{6}.Value) }});",
			             domainEnvironment.ToTypeName(entity, true),
			             VarName_Request,
			             NamingHelperStack.ParamName_PageNumber,
			             NamingHelperStack.ParamName_PageSize,
			             DomainLayerConfig.GetClassName_SortOrder(true),
			             NamingHelperStack.ParamName_SortOrderProperty,
			             NamingHelperStack.ParamName_SortOrderAsc);
			cw.Else();
			cw.WriteLine("list = {0}.GetPage({1}.{2}.Value, {1}.{3}.Value, null);",
			             domainEnvironment.ToTypeName(entity, true),
			             VarName_Request,
			             NamingHelperStack.ParamName_PageNumber,
			             NamingHelperStack.ParamName_PageSize);
			cw.EndIf();
			cw.Else();
			cw.WriteLine("list = {0}.GetPage(0, {1}, null);",
			             domainEnvironment.ToTypeName(entity, true),
			             RestServiceHelper.MaxPageSize);
			cw.EndIf();
			cw.WriteLine("{0}.{1} = {2}(list);",
			             VarName_ResponseList,
			             NamingHelperStack.ToDTOCollectionPropertyName(entity),
			             MethodName_DomainCollectionToDTO);
			cw.EndTry();
			WriteCatch(VarName_ResponseList);
			cw.WriteLine("return {0};", VarName_ResponseList);

			cw.EndIf(); // Global
			cw.WriteLine();


	        cw.If("{0} != null", VarName_DomainObject);
	        cw.WriteLine("{0} {1} = new {0}();",
			             NamingHelperStack.ToDTOTypeName(entity, interfacesEnvironment),
			             VarName_DTO);
	        cw.WriteLine("{0}.{1}({2}, {3});",
			             NamingHelperStack.ToServiceName(entity, null),
			             MethodName_DomainObjectToDTO,
			             VarName_DomainObject,
			             VarName_DTO);
            cw.WriteLine("{0}.{1} = {2};",
			             VarName_Response,
			             NamingHelperStack.ToDTOPropertyName(entity),
			             VarName_DTO);
			cw.EndIf();
			cw.EndTry();
			WriteCatch(VarName_Response);
            cw.WriteLine("return {0};", VarName_Response);

			cw.EndFunction();
		}

		private void WriteOnPost(IEntity entity)
		{
			cw.BeginFunction("public override object OnPost({0} {1})",
			                 NamingHelperStack.ToServiceRequestName(entity, interfacesEnvironment),
			                 VarName_Request);
			cw.WriteLine("{0} {1} = new {0}();",
			             NamingHelperStack.ToServiceResponseName(entity, interfacesEnvironment),
			             VarName_Response);
			WriteInitResponse(VarName_Response);
			string requestDto = String.Format("{0}.{1}", VarName_Request, NamingHelper.ToDTOPropertyName(entity));
			cw.BeginTry();

			bool declared = false;
			foreach(IEntityOperation operation in entity.Operations)
			{
				if (operation.Access != EntityOperationAccess.Public)
					continue;

				string condition = String.Format("{0}.{1} != null", VarName_Request, OperationHelper.GetParamClassPropertyName(operation));
				if (!declared)
				{
					cw.If(condition);
					declared = true;
				}
				else
		            cw.ElseIf(condition);
				if (entity.Persistence.Persisted)
				{
		            cw.WriteLine("{0} domain = null;", domainEnvironment.ToTypeName(entity, true));
		            cw.WriteLine("if ({0} != null) domain = {1}.{2}({3});",
					             requestDto,
					             domainEnvironment.ToTypeName(entity, true),
					             DomainLayerConfig.Methods.GetById(entity.PrimaryId, domainEnvironment).Name,
					             entity.PrimaryId.Attributes.ToNamesString(", ", requestDto + ".", ""));
		            cw.WriteLine("if (domain == null) domain = new {0}();",
					             domainEnvironment.ToTypeName(entity, true));
				}
				else
				{
		            cw.WriteLine("{0} domain = new {0}();",
					             domainEnvironment.ToTypeName(entity, true));
				}
	            cw.WriteLine("{0}({1}, domain);",
				             MethodName_DTOToDomainObject,
				             requestDto);
				if (entity.Persistence.Persisted)
		            cw.WriteLine("domain.Save();");
				if (operation.Params.HasReturns)
				{
		            cw.Write("{0}.{1}.{2} = ",
					             VarName_Request,
					             OperationHelper.GetParamClassPropertyName(operation),
					             operation.Params.Returns.Name,
					             operation.Name);
				}
	            cw.Write("domain.{0}(", operation.Name);
				for (int i = 0; i < operation.Params.Count; i++)
				{
					if (operation.Params[i].IsOut || operation.Params[i].IsRef)
						throw new GlException(
							"Output or reference parameters are not allowed on service layer. " +
							"Use ordinal parameters or define operation as internal. {0}",
							operation.Params[i].ToString());

		            cw.Write("{0}{1}.{2}.{3}",
					         i == 0 ? "" : ", ",
					         VarName_Request,
					         OperationHelper.GetParamClassPropertyName(operation),
					         operation.Params[i].Name);
				}
				cw.WriteLine(");");
	            cw.WriteLine("{0}(domain, {1});",
				             MethodName_DomainObjectToDTO,
				             requestDto);
				//oph.WriteParamClassesPropertiesInitialization(entity);
	            cw.WriteLine("{0}.{1} = {2}.{1};",
				             VarName_Response,
				             NamingHelper.ToDTOPropertyName(entity),
				             VarName_Request);
	            cw.WriteLine("{0}.{1} = {2}.{1};",
				             VarName_Response,
				             OperationHelper.GetParamClassPropertyName(operation),
				             VarName_Request);
			}

			if (entity.Persistence.Persisted)
			{
				string condition2 = String.Format("{0} != null", requestDto);
				if (!declared)
					cw.If(condition2);
				else
		            cw.ElseIf(condition2);
				declared = true;
				cw.WriteLine("{0} uow = new {0}();", NamingHelper.ClassName_UnitOfWorkDTO);
				cw.WriteLine("uow.Save({0}.{1});", VarName_Request, NamingHelper.ToDTOPropertyName(entity));
				cw.WriteLine("{0}.{1} = {2}.Commit(uow, null);",
				             VarName_Response,
				             NamingHelper.PropertyName_CommitResult,
				             NamingHelperStack.ToServiceName(NamingHelper.ServiceName_Persistence));
				cw.WriteLine("{0}.{1} = (uow.WorkItems[0].Item as {2});",
				             VarName_Response,
				             NamingHelper.ToDTOPropertyName(entity),
				             NamingHelper.ToDTOTypeName(entity, interfacesEnvironment));
				cw.If("{0}.{1}.HasError == true",
				      VarName_Response,
				      NamingHelper.PropertyName_CommitResult);
				WriteResponceError(VarName_Response, '"' + "Error saving object" + '"', null);
				cw.EndIf();
				cw.ElseIf("{0}.{1} != null", VarName_Request, NamingHelperStack.ParamName_Query);
				cw.WriteLine("{0} {1} = new {0}();",
				             NamingHelperStack.ToServiceResponseCollectionName(entity, interfacesEnvironment),
				             VarName_ResponseList);
				WriteInitResponse(VarName_ResponseList);
				cw.BeginTry();
				cw.WriteLine("IList<{0}> list = {0}.{1}({2}.{3}, {4}.ToDomainQueryParams({2}.{5}), {2}.{6} != null ? {2}.{6}.Value : 0, {2}.{7} != null ? {2}.{7}.Value : {8});",
				             domainEnvironment.ToTypeName(entity, true),
				             DomainLayerConfig.Methods.GetPageByHQL().Name,
				             VarName_Request,
				             NamingHelperStack.ParamName_Query,
				             ClassName_DomainQueryFactory,
				             NamingHelperStack.ParamName_QueryParams,
				             NamingHelperStack.ParamName_PageNumber,
				             NamingHelperStack.ParamName_PageSize,
				             RestServiceHelper.MaxPageSize);
				cw.WriteLine("{0}.{1} = {2}(list);",
				             VarName_ResponseList,
				             NamingHelperStack.ToDTOCollectionPropertyName(entity),
				             MethodName_DomainCollectionToDTO);
				cw.EndTry();
				WriteCatch(VarName_ResponseList);
				cw.WriteLine("return {0};", VarName_ResponseList);
			}
			if (declared)
				cw.EndIf();

			cw.EndTry();
			WriteCatch(VarName_Response);
			cw.WriteLine("return {0};", VarName_Response);
			cw.EndFunction();
		}


		private void WriteOnDelete(IEntity entity)
		{
			cw.BeginFunction("public override object OnDelete({0} {1})",
			                 NamingHelperStack.ToServiceRequestName(entity, interfacesEnvironment),
			                 VarName_Request);
			cw.WriteLine("{0} {1} = new {0}();",
			             NamingHelperStack.ToServiceResponseName(entity, interfacesEnvironment),
			             VarName_Response);
			WriteInitResponse(VarName_Response);
			cw.BeginTry();
			cw.If("{0}", entity.Constraints.PrimaryId.Attributes.ToNamesString(" && ", VarName_Request + ".", " != null"));
			cw.BeginTry();
			cw.WriteLine("{0}.{1}({2});",
			             domainEnvironment.ToTypeName(entity, true),
			             DomainLayerConfig.Methods.DeleteById(entity.Constraints.PrimaryId, domainEnvironment).Name,
			             entity.Constraints.PrimaryId.Attributes.ToNamesString(", ", VarName_Request + ".", ".Value"));
			cw.EndTry();
			cw.BeginCatch("Exception e");
			cw.WriteLine("{0}.CommitResult.HasError = true;", VarName_Response);
			cw.WriteLine("{0}.CommitResult.Message = e.Message;", VarName_Response);
			cw.WriteLine("{0}.CommitResult.ExceptionString = e.ToString();", VarName_Response);
			WriteResponceError(VarName_Response, '"' + "Error deleting object" + '"', "e.ToString()");
			cw.EndCatch();
			cw.Else();
			cw.WriteLine("{0}.CommitResult.HasError = true;", VarName_Response);
			cw.WriteLine("{0}.CommitResult.Message = \"Object primary id is empty\";", VarName_Response);
			WriteResponceError(VarName_Response, '"' + "Object primary id is empty" + '"', null);
			cw.EndIf();
			cw.EndTry();
			WriteCatch(VarName_Response);
			cw.WriteLine("return {0};", VarName_Response);
			cw.EndFunction();
		}


		private void WriteDomainObjectToDTO(IEntity entity)
		{
			// Domain --> DTO: init DTO from domain object
			cw.WriteLine("internal static void {0}",
			             ToMethodSignature_DomainObjectToDTO(entity));
			cw.BeginScope();
			if (entity.HasSupertype)
			{
				cw.WriteLine("{0}.{1}({2}, {3});",
				             NamingHelperStack.ToServiceName(entity.Supertype, environment),
				             MethodName_DomainObjectToDTO,
				             VarName_DomainObject,
				             VarName_DTO);
			}

			foreach(IAttribute a in entity.Attributes)
			{
				if (a.ProcessInRelations)
					continue;

				if (a.HasEnumerationType)
				{
					cw.WriteLine("{0}.{1} = ({2})((int){3}.{1});",
					             VarName_DTO,
					             a.Name,
					             interfacesEnvironment.ToTypeName(a, true),
					             VarName_DomainObject);
				}
				else
				{
					cw.WriteLine("{0}.{1} = {2}.{1};",
					             VarName_DTO,
					             a.Name,
					             VarName_DomainObject);
				}
			}

			foreach(IRelation r in entity.Relations)
			{
				if (!r.IsParent(entity) && r.ChildNavigate)
				{
					foreach(IRelationAttributeMatch am in r.AttributesMatch)
					{
						IAttribute childAttr = am.Attribute;
						IAttribute parentAttr = am.Attribute2;
						if (r.ParentSide == RelationSide.Left)
						{
							childAttr = am.Attribute2;
							parentAttr = am.Attribute;
						}
						cw.Write("{0}.{1} = ",
						         VarName_DTO,
						         NamingHelper.ToDTOPropertyName(childAttr) );
						if (!childAttr.TypeDefinition.Required)
							cw.Write("{0}.{1} == null ? ({2})null : ",
							         VarName_DomainObject,
							         r.ChildName,
							         environment.ToTypeName(childAttr, false, true));
						cw.WriteLine("{0}.{1}.{2};",
						             VarName_DomainObject,
						             r.ChildName,
						             parentAttr.Name);
					}
				}
			}

			cw.WriteLine("{0}.{1} = false;",
			             VarName_DTO,
			             NamingHelper.PropertyName_DTOChanged);

			cw.EndScope();
		}


		private void WriteDTOToDomainObject(IEntity entity)
		{
			// DTO --> Domain: init domain object from DTO
			cw.WriteLine("internal static void {0}", ToMethodSignature_DTOToDomainObject(entity));
			cw.BeginScope();
			if (entity.HasSupertype)
			{
				cw.WriteLine("{0}.{1}({2}, {3});",
				             NamingHelperStack.ToServiceName(entity.Supertype, environment),
				             MethodName_DTOToDomainObject,
				             VarName_DTO,
				             VarName_DomainObject);
			}
			foreach(IAttribute a in entity.Attributes)
			{
				if (a.ProcessInRelations || !a.Persistence.Persisted)
					continue;
				if (a.IsPrimaryId && entity.PrimaryId.HasGenerator)
					continue;
				if (a.HasEnumerationType)
				{
					cw.WriteLine("{0}.{1} = ({2})((int){3}.{1});",
					             VarName_DomainObject,
					             a.Name,
					             domainEnvironment.ToTypeName(a, true),
					             VarName_DTO);
				}
				else
				{
					cw.WriteLine("{0}.{1} = {2}.{1};",
					             VarName_DomainObject,
					             a.Name,
					             VarName_DTO);
				}
			}

			foreach(IRelation r in entity.Relations)
			{
				if (!r.IsParent(entity) && r.ChildNavigate)
				{
					foreach(IRelationAttributeMatch am in r.AttributesMatch)
					{
						IAttribute childAttr = am.Attribute;
						if (r.ParentSide == RelationSide.Left)
						{
							childAttr = am.Attribute2;
						}

						if (!childAttr.TypeDefinition.Required)
						{
							cw.If("{0}.{1} != null",
							         VarName_DTO,
							         NamingHelper.ToDTOPropertyName(childAttr));

							cw.WriteLine("{0}.{1} = {2}.{3}({4}.{5}{6});",
							             VarName_DomainObject,
							             r.ChildName,
							             domainEnvironment.ToTypeName(r.ParentEntity, true),
							             DomainLayerConfig.Methods.GetById(entity.Constraints.PrimaryId, domainEnvironment).Name,
							             VarName_DTO,
							             NamingHelper.ToDTOPropertyName(childAttr),
							             cw.AsAttributeValue(childAttr, environment) );
							cw.Else();
							cw.WriteLine("{0}.{1} = null;",
							             VarName_DomainObject,
							             r.ChildName);
							cw.EndIf();
						}
						else
						{
							cw.WriteLine("{0}.{1} = {2}.{3}({4}.{5});",
							             VarName_DomainObject,
							             r.ChildName,
							             domainEnvironment.ToTypeName(r.ParentEntity, true),
							             DomainLayerConfig.Methods.GetById(entity.Constraints.PrimaryId, domainEnvironment).Name,
							             VarName_DTO,
							             NamingHelper.ToDTOPropertyName(childAttr));
						}
					}
				}
			}

			cw.EndScope();
		}


		private void WriteDomainCollectionToDTOCollection(IEntity entity)
		{
			cw.WriteLine("protected {0} {1}",
			             NamingHelperStack.ToDTOCollectionTypeName(entity, interfacesEnvironment),
			             ToMethodSignature_DomainCollectionToDTO(entity));
			cw.BeginScope();
			cw.WriteLine("{0} {1} = new {0}();",
			             NamingHelperStack.ToDTOCollectionTypeName(entity, interfacesEnvironment),
			             VarName_DTOCollection);
			cw.WriteLine("foreach({0} {1} in {2})",
			             domainEnvironment.ToTypeName(entity, true),
			             VarName_DomainObject,
			             VarName_DomainCollection);
			cw.BeginScope();
			cw.WriteLine("{0} {1} = new {0}();",
			             NamingHelperStack.ToDTOTypeName(entity, interfacesEnvironment),
			             VarName_DTO);
			cw.WriteLine("{0}.{1}({2}, {3});",
			             NamingHelperStack.ToServiceName(entity, null),
			             MethodName_DomainObjectToDTO,
			             VarName_DomainObject,
			             VarName_DTO);
			cw.WriteLine("{0}.Add({1});",
			             VarName_DTOCollection,
			             VarName_DTO);
			cw.EndScope();
			cw.WriteLine("return {0};", VarName_DTOCollection);
			cw.EndScope();
		}

	}
}

