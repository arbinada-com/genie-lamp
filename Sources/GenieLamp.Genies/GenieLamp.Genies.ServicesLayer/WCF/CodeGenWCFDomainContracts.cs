using System;
using System.IO;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;

namespace GenieLamp.Genies.ServicesLayer.WCF
{
	public class CodeGenWCFDomainContracts : CodeGenBase
	{
		#region Constructors
		public CodeGenWCFDomainContracts(ServicesGenie genie)
			: base(genie)
		{
			outFileName = "DomainServicesContracts.cs";
		}
		#endregion

		protected override void Write()
		{
			cw.WriteUsing("System.ServiceModel");
			cw.WriteUsing("System.ServiceModel.Description");
			cw.WriteLine();

			ns.BeginScope(ServicesLayerConfig.ServicesNamespace);

			cw.WriteLine("[ServiceContract]");
    		cw.WriteLine("public interface {0}", NamingHelperWCF.IntfName_PersistenceService);
			cw.BeginScope();
	        cw.WriteLine("[OperationContract]");
        	cw.WriteLine("{0} {1};",
			             NamingHelper.ToDTOTypeName(NamingHelper.ClassName_UnitOfWorkDTO),
			             NamingHelperWCF.MethodSignature_Create(NamingHelper.ClassName_UnitOfWorkDTO));
        	cw.WriteLine("[OperationContract]");
        	cw.WriteLine("CommitResult Commit({0} {1});",
			             NamingHelper.ToDTOTypeName(NamingHelper.ClassName_UnitOfWorkDTO),
			             NamingHelper.VarName_UnitOfWork);
    		cw.EndScope();
    		cw.WriteLine();

			foreach(IEntity entity in Model.Entities)
			{
				WriteEntityServiceContract(entity);
			}

			ns.EndScope();
		}
		
		private void WriteEntityServiceContract(IEntity entity)
		{
			cw.WriteLine("[ServiceContract]");
    		cw.WriteLine("public interface {0}", NamingHelperWCF.GetEntityServiceName(entity));
			cw.BeginScope();
	        cw.WriteLine("[OperationContract]");
        	cw.WriteLine("{0} {1};",
			             NamingHelper.ToDTOTypeName(entity, environment),
			             NamingHelperWCF.MethodSignature_Create(entity));

	        cw.WriteLine("[OperationContract]");
        	cw.WriteLine("{0} {1};",
			             NamingHelper.ToDTOTypeName(entity, environment),
			             ServicesLayerConfig.Methods.GetProxyById(entity.Constraints.PrimaryId, environment).Signature);

			foreach(IUniqueId uid in entity.Constraints.UniqueIds)
			{
		        cw.WriteLine("[OperationContract]");
				cw.WriteLine("{0} {1};",
				             NamingHelper.ToDTOTypeName(entity, environment),
				             ServicesLayerConfig.Methods.GetProxyByUniqueId(uid, environment).Signature);
			}

    		cw.EndScope();
    		cw.WriteLine();
		}

		#region implemented abstract members of GenieLamp.Genies.ServicesLayer.CodeGenBase
		protected override void ProcessEntity(IEntity entity)
		{
		}
		#endregion
	}
}

