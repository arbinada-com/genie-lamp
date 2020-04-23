using System;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;

namespace GenieLamp.Genies.ServicesLayer
{
	public class CodeGenDTO : CodeGenBase
	{
		#region Constructors
		public CodeGenDTO(GenieBase genie)
			: base(genie)
		{
			outFileName = "DomainDTO.cs";
		}
		#endregion

		protected override void Write()
		{
			cw.WriteUsing("System.Runtime.Serialization");
			cw.WriteUsing("System.Collections.Generic");
			cw.WriteUsing("System.Text");
			cw.WriteLine();

			environment.BaseNamespace = ServicesLayerConfig.ServicesInterfacesNamespace;
			ns.BeginScope(environment.BaseNamespace);

			// Utils classes
            cw.WriteText(this.GetType().Assembly,
			             "Templates.DomainDTO.cs",
			             genie.Config.Macro);

			WriteDomainTypesEnum();
			cw.WriteLine();
			WriteDomainObjectDTO();
			cw.WriteLine();
			WritePersistentObjectDTO();
			cw.WriteLine();
			ProcessEnumerations();
			cw.WriteLine();
			ProcessEntities();
			cw.WriteLine();

			ns.EndScope();
		}


		private void WriteDomainTypesEnum()
		{
			cw.WriteLine("[DataContract]");
			cw.WriteLine("public enum {0}", NamingHelper.EnumName_DomainTypes);
			cw.BeginScope();
			for (int i = 0; i < Model.Entities.Count; i++)
			{
	    		cw.WriteLine("[EnumMember] {0} = {1}{2}",
				             NamingHelper.ToDomainTypesEnumItemName(Model.Entities[i]),
				             Model.Entities[i].Id,
				             i + 1 == Model.Entities.Count ? "" : ",");
			}
			cw.EndScope();
		}


		private void WriteDomainObjectDTO()
		{
    		cw.WriteLine("[DataContract]");
    		cw.WriteLine("[KnownType(typeof({0}))]", NamingHelper.ClassName_PersistentObjectDTO);
			foreach(IEntity entity in Model.Entities)
			{
				if (entity.Persistence.Persisted)
				{
		    		cw.WriteLine("[KnownType(typeof({0}))]", NamingHelper.ToDTOTypeName(entity, environment));
				}
			}
			cw.BeginAbstractClass(AccessLevel.Public,
			                      false,
			                      NamingHelper.ClassName_DomainObjectDTO,
			                      Const.EmptyName);
    		cw.WriteLine("[DataMember]");
			cw.SimpleProperty(AccessLevel.Public,
			                  VirtualisationLevel.None,
			                  "string",
			                  NamingHelper.PropertyName_InternalObjectId,
			                  true, true);
			cw.WriteLine("public abstract int Get{0}();",
			             NamingHelper.PropertyName_DomainTypeId);
    		cw.WriteLine("[DataMember]");
			cw.SimpleProperty(AccessLevel.Public,
			                  VirtualisationLevel.None,
			                  "bool",
			                  NamingHelper.PropertyName_DTOChanged,
			                  true, true);
    		cw.WriteLine("public {0}()", NamingHelper.ClassName_DomainObjectDTO);
			cw.BeginScope();
    		cw.WriteLine("this.{0} = Guid.NewGuid().ToString();", NamingHelper.PropertyName_InternalObjectId);
			cw.EndScope();
			cw.EndClass();
		}

		private void WritePersistentObjectDTO()
		{
    		cw.WriteLine("[DataContract]");
			foreach(IEntity entity in Model.Entities)
			{
				if (entity.Persistence.Persisted)
				{
		    		cw.WriteLine("[KnownType(typeof({0}))]", NamingHelper.ToDTOTypeName(entity, environment));
				}
			}
			cw.WriteLine("public abstract class {0} : {1}",
			             NamingHelper.ClassName_PersistentObjectDTO,
			             NamingHelper.ClassName_DomainObjectDTO);
			cw.BeginScope();
			cw.EndScope();
		}

		#region Enumerations
		private void ProcessEnumerations()
		{
			cw.BeginRegion("Enumerations");
			ICSharpNamespaceWriter nswriter = cw.CreateNamespaceWriter();
			foreach (IEnumeration enumeration in genie.Model.Enumerations)
			{
				nswriter.BeginOrContinueScope(enumeration.Schema);
				ProcessEnumeration(enumeration);
				cw.WriteLine();
			}
			nswriter.EndScope();
			cw.EndRegion();
		}

		private void ProcessEnumeration(IEnumeration enumeration)
		{
			cw.WriteLine("[DataContract]");
			cw.WriteLine("public enum {0}", enumeration.Name);
			cw.BeginScope();
			for (int i = 0; i < enumeration.Items.Count; i++)
			{
				IEnumerationItem item = enumeration.Items[i];
				cw.WriteLine("[EnumMember] {0} = {1}{2}",
				             item.Name,
				             item.Value,
				             i + 1 == enumeration.Items.Count ? "" : ",");
			}
			cw.EndScope();

			if (this.genie.Lamp.Config.Patterns.Localization != null)
			{
				cw.WriteLine("public static class {0}L10n", enumeration.Name);
				cw.BeginScope();
				cw.BeginFunction("public static string GetName({0} value)", enumeration.Name);
				cw.WriteLine("switch(value)");
				cw.BeginScope();
				for (int i = 0; i < enumeration.Items.Count; i++)
				{
					IEnumerationItem item = enumeration.Items[i];
					string caption = item.HasDoc ?
						item.Doc.GetLabel(new System.Globalization.CultureInfo("en")) : item.Name;
					cw.WriteLine("case {0}.{1}: return {2}.L.Catalog.GetString(\"{3}\");", 
					             enumeration.Name,
					             item.Name, 
					             ServicesLayerConfig.ServicesInterfacesNamespace,
					             String.IsNullOrWhiteSpace(caption) ? item.Name : caption);
				}
				cw.EndScope();
				cw.WriteLine("throw new Exception(String.Format(\"Unsupported value: {{0}}. Enum: {0}\", value));", enumeration.Name);
				cw.EndFunction();
				cw.WriteLine();
				string enumItemClassName = String.Format("{0}Item", enumeration.Name);
				cw.BeginClass(AccessLevel.Public, false, enumItemClassName, null);
				cw.WriteLine("public {0} Id  {{ get; set; }}", enumeration.Name);
				cw.WriteLine("public string Name { get; set; }");
				cw.EndClass();
				cw.WriteLine();
				cw.BeginFunction("public static IList<{0}> GetList()", enumItemClassName);
				cw.WriteLine("List<{0}> list = new List<{0}>();", enumItemClassName);
				cw.WriteLine("{0}[] items = ({0}[])Enum.GetValues(typeof({0}));", enumeration.Name);
				cw.WriteLine("for(int i = 0; i < items.Length; i++ )");
				cw.BeginScope();
				cw.WriteLine("{0} item = new {0}() {{ Id = items[i], Name = GetName(items[i]) }};", enumItemClassName);
				cw.WriteLine("list.Add(item);");
				cw.EndScope();
				cw.WriteLine("return list;");
				cw.EndFunction();
				cw.EndScope();
				cw.WriteLine();
			}
		}
		#endregion

		protected override void ProcessEntity(IEntity entity)
		{
    		cw.WriteLine("[DataContract]");
			string supertypeName = NamingHelper.ClassName_PersistentObjectDTO;
			if (entity.HasSupertype)
				supertypeName = NamingHelper.ToDTOTypeName(entity.Supertype, environment);
			else if (!entity.Persistence.Persisted)
				supertypeName = NamingHelper.ClassName_DomainObjectDTO;
			cw.BeginClass(AccessLevel.Public,
			              true,
			              NamingHelper.ToDTOTypeName(entity, null),
			              supertypeName);

			cw.WriteLine("[DataMember]");
			cw.WriteLine("public {0}const int {1} = {2}; // {3}.{4}",
			             entity.HasSupertype ? "new " : "",
			             NamingHelper.PropertyName_DomainTypeId,
			             entity.Id,
			             NamingHelper.EnumName_DomainTypes,
			             NamingHelper.ToDomainTypesEnumItemName(entity) );
			cw.WriteLine("public override int Get{0}() {{ return {1}.{0}; }}",
			             NamingHelper.PropertyName_DomainTypeId,
			             NamingHelper.ToDTOTypeName(entity, null));

			ProcessAttributes(entity);
    		cw.WriteLine();

			ProcessRelations(entity);

			OnProcessEntity(entity);

			cw.EndClass();
			cw.WriteLine();

		}

		protected virtual void OnProcessEntity(IEntity entity)
		{
		}


		private void ProcessAttributes(IEntity entity)
		{
			foreach(IAttribute a in entity.Attributes)
			{
				if (a.ProcessInRelations)
					continue;
				cw.WriteProperty(AccessLevel.Public,
				                 VirtualisationLevel.None,
				                 a,
				                 environment,
				                 "[DataMember]");
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
			    		cw.WriteLine("[DataMember]");
						cw.SimpleProperty(AccessLevel.Public,
						                  VirtualisationLevel.None,
						                  environment.ToTypeName(a, false),
						                  NamingHelper.ToDTOPropertyName(a),
						                  true, true);
					}
				}
			}
		}
	}
}

