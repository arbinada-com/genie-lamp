using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;

namespace GenieLamp.Genies.ServicesLayer.WCF
{
	public class CodeGenWCFDomainServices : CodeGenBase
	{
		public const string ClassName_DomainServices = "DomainServices";
		
		#region Constructors
		public CodeGenWCFDomainServices(ServicesGenie genie)
			: base(genie)
		{
			outFileName = String.Format("{0}.cs", ClassName_DomainServices);
		}
		#endregion

		protected override void Write()
		{
			cw.WriteUsing("System.Runtime.Serialization");
			cw.WriteUsing("System.Collections.Generic");
			cw.WriteUsing("System.Text");
			cw.WriteUsing("System.ServiceModel");
			cw.WriteUsing("System.ServiceModel.Description");
			cw.WriteLine();

			ns.BeginScope(ServicesLayerConfig.ServicesNamespace);

			cw.WriteLine("[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]");
        	cw.WriteLine("public partial class {0} :", ClassName_DomainServices);
			cw.Indent++;
        	cw.WriteLine("{0},", NamingHelperWCF.IntfName_PersistenceService);
			int count = 1;
			foreach(IEntity entity in Model.Entities)
			{
	        	cw.WriteLine("{0}{1}",
				             NamingHelperWCF.GetEntityServiceName(entity),
				             count == Model.Entities.Count ? "" : ",");
				count++;
			}
			cw.Indent--;
			cw.BeginScope();

			cw.BeginRegion(NamingHelperWCF.IntfName_PersistenceService);
	        cw.WriteLine("{0} {1}.{2}",
			             NamingHelper.ToDTOTypeName(NamingHelper.ClassName_UnitOfWorkDTO),
			             NamingHelperWCF.IntfName_PersistenceService,
			             NamingHelperWCF.MethodSignature_Create(NamingHelper.ClassName_UnitOfWorkDTO));
	        cw.BeginScope();
	        cw.WriteLine("return new {0}();", NamingHelper.ToDTOTypeName(NamingHelper.ClassName_UnitOfWorkDTO));
	        cw.EndScope();
			cw.WriteLine();
	        cw.WriteLine("CommitResult {0}.Commit({1} {2})",
			             NamingHelperWCF.IntfName_PersistenceService,
			             NamingHelper.ToDTOTypeName(NamingHelper.ClassName_UnitOfWorkDTO),
			             NamingHelper.VarName_UnitOfWork);
	        cw.BeginScope();
	        cw.WriteLine("return {0}.Commit();", NamingHelper.VarName_UnitOfWork);
	        cw.EndScope();
			cw.EndRegion();
			cw.WriteLine();

			foreach(IEntity entity in Model.Entities)
			{
				WriteEntityService(entity);
			}

			cw.EndScope();
			ns.EndScope();
		}

		private void WriteEntityService(IEntity entity)
		{
			cw.BeginRegion(NamingHelperWCF.GetEntityServiceName(entity));
			// Create
        	cw.WriteLine("{0} {1}.{2}",
			             NamingHelper.ToDTOTypeName(entity, environment),
			             NamingHelperWCF.GetEntityServiceName(entity),
			             NamingHelperWCF.MethodSignature_Create(entity));
			cw.BeginScope();
			cw.WriteLine("return new {0}();", NamingHelper.ToDTOTypeName(entity, environment));
			cw.EndScope();
			cw.WriteLine();

			// GetById
        	cw.WriteLine("{0} {1}.{2}",
			             NamingHelper.ToDTOTypeName(entity, environment),
			             NamingHelperWCF.GetEntityServiceName(entity),
			             ServicesLayerConfig.Methods.GetProxyById(entity.Constraints.PrimaryId, environment).Signature);
			cw.BeginScope();
			cw.WriteLine("return {0}.{1}({2});",
			             NamingHelper.ToDTOTypeName(entity, environment),
			             ServicesLayerConfig.Methods.GetById(entity.Constraints.PrimaryId, environment).Name,
			             environment.ToArguments(entity.Constraints.PrimaryId.Attributes));
			cw.EndScope();
			cw.WriteLine();

			// Get by unique id
			foreach(IUniqueId uid in entity.Constraints.UniqueIds)
			{
	        	cw.WriteLine("{0} {1}.{2}",
				             NamingHelper.ToDTOTypeName(entity, environment),
				             NamingHelperWCF.GetEntityServiceName(entity),
				             ServicesLayerConfig.Methods.GetProxyByUniqueId(uid, environment).Signature);
				cw.BeginScope();
				cw.WriteLine("return {0}.{1}({2});",
				             NamingHelper.ToDTOTypeName(entity, environment),
				             ServicesLayerConfig.Methods.GetByUniqueId(uid, environment).Name,
				             environment.ToArguments(uid.Attributes));
				cw.EndScope();
			}

			cw.EndRegion();
			cw.WriteLine();
		}
		
		#region implemented abstract members of GenieLamp.Genies.ServicesLayer.CodeGenBase
		protected override void ProcessEntity(IEntity entity)
		{
		}
		#endregion
	}
}

