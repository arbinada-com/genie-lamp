using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using GenieLamp.Core;
using GenieLamp.Core.Metamodel;
using GenieLamp.Core.Exceptions;
using GenieLamp.Core.CodeWriters;
using GenieLamp.Core.CodeWriters.CSharp;
using GenieLamp.Core.Layers;

namespace GenieLamp.Genies.NHibernate
{
	public class CodeGenDomain : CodeGenBase
	{
		public CodeGenDomain(NHibernateGenie genie)
			: base(genie)
		{
			outFileName = genie.Config.OutFileName;
		}

		private string Call_GetSession
		{
			get { return String.Format(
					"{0}.{1}.{2}()",
					DomainLayerConfig.PersistenceNamespace,
					CodeGenDomainSupport.ClassName_SessionManager,
					CodeGenDomainSupport.FunctionName_GetSession); }
		}

		protected override void InternalRun()
		{
			cw.WriteUsing("System");
			cw.WriteUsing("System.Linq");
			cw.WriteUsing("System.Text");
			cw.WriteUsing("System.IO");
			cw.WriteUsing("System.Collections.Generic");
            cw.WriteLine();
			cw.WriteUsing("NHibernate");
			cw.WriteUsing("NHibernate.Criterion");
			cw.WriteLine();
			CreateMappingDoc();

			nswriter.BeginScope(environment.BaseNamespace);
			GenerateClasses();
			nswriter.EndScope();
			
			string mapFileName = Path.Combine(genie.Config.OutDir, Path.GetFileNameWithoutExtension(outFileName));
			map.Document.Save(
                String.Format("{0}.hbm.xml", Path.Combine(genie.Config.OutDir, mapFileName)));
		}
		
		private void CreateMappingDoc()
		{
			map = genie.Lamp.GenieLampUtils.Xml.CreateDocHelper("hibernate-mapping");
			map.AddAttribute("xmlns", "urn:nhibernate-mapping-2.2");
			map.AddAttribute("assembly", targetAssemblyName);
			map.AddAttribute("namespace", DomainLayerConfig.BaseNamespace);
		}
		
		private void GenerateClasses()
		{
			ProcessEnumerations();
			ProcessEntities();
		}
		
		
		#region Enumerations
		private void ProcessEnumerations()
		{
			cw.BeginRegion("Enumerations");
			ICSharpNamespaceWriter nswriter = cw.CreateNamespaceWriter();
			foreach (IEnumeration enumeration in Model.Enumerations)
			{
				nswriter.BeginOrContinueScope(enumeration.Schema);
				ProcessEnumeration(enumeration);
			}
			nswriter.EndScope();
			cw.EndRegion();
		}
		
		private void ProcessEnumeration(IEnumeration enumeration)
		{
			
			cw.WriteLine("public enum {0}", enumeration.Name);
			cw.BeginScope();
			for (int i = 0; i < enumeration.Items.Count; i++)
			{
				IEnumerationItem item = enumeration.Items[i];
				cw.WriteLine("{0} = {1}{2}", 
				                 item.Name, 
				                 item.Value,
				                 i + 1 == enumeration.Items.Count ? "" : ",");
			}
			cw.EndScope();

			if (Patterns.Localization != null)
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
					             DomainLayerConfig.PatternsNamespace,
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
		
		#region Entities
		private void ProcessEntities()
		{
			cw.BeginRegion("Entities classes");
			ICSharpNamespaceWriter nswriter = cw.CreateNamespaceWriter();
			foreach (IEntity entity in Model.Entities)
			{
				nswriter.BeginOrContinueScope(entity.Schema);
				ProcessEntity(entity);
			}
			nswriter.EndScope();
			cw.EndRegion();
		}

		private void ProcessEntity(IEntity entity)
		{
			// EntityCollection
			cw.WriteLine("public interface {0} : ISet<{1}> {{ }}",
			             environment.ToCollectionIntfName(entity, false),
			             environment.ToTypeName(entity, true));
			cw.WriteLine();

			WriteEntityInterface(entity);

			// Entity
			List<string> interfaces = new List<string>();
			interfaces.Add(environment.ToIntfName(entity, false));
			if (entity.Persistence.Persisted)
				interfaces.Add(String.Format("{0}.{1}", DomainLayerConfig.PersistenceNamespace, CodeGenDomainSupport.InterfaceName_PersistentObject));

			// Handlers
			foreach(IEntityEventHandler handler in entity.EventHandlers)
			{
				switch (handler.Type)
				{
				case EntityEventHandlerType.Save:
					interfaces.Add(String.Format("{0}.{1}", DomainLayerConfig.PatternsNamespace, CodeGenDomainSupport.InterfaceName_OnSave));
					break;
				case EntityEventHandlerType.Delete:
					interfaces.Add(String.Format("{0}.{1}", DomainLayerConfig.PatternsNamespace, CodeGenDomainSupport.InterfaceName_OnDelete));
					break;
				case EntityEventHandlerType.Flush:
					interfaces.Add(String.Format("{0}.{1}", DomainLayerConfig.PatternsNamespace, CodeGenDomainSupport.InterfaceName_OnFlush));
					break;
				case EntityEventHandlerType.Validate:
					interfaces.Add(String.Format("{0}", CodeGenDomainSupport.InterfaceName_OnValidatable));
					break;
				}
			}

			if (Patterns.Registry != null && Patterns.Registry.AppliedTo(entity))
				interfaces.Add(String.Format("{0}.{1}", DomainLayerConfig.PatternsNamespace, CodeGenDomainSupport.InterfaceName_UsesRegistry));
			if (Patterns.Audit != null && Patterns.Audit.AppliedTo(entity))
				interfaces.Add(String.Format("{0}.{1}", DomainLayerConfig.PatternsNamespace, CodeGenDomainSupport.InterfaceName_UsesAudit));

			cw.WriteDoc(entity);
			if (entity.HasSupertype)
				cw.BeginClass(AccessLevel.Public, true, entity.Name, entity.Supertype.FullName, cw.ToSeparatedString(interfaces, ", "));
			else
				cw.BeginClass(AccessLevel.Public, true, entity.Name, cw.ToSeparatedString(interfaces, ", "));

			cw.WriteLine("public {0}()", entity.Name);
			cw.BeginScope();
			cw.EndScope();
			cw.WriteLine();

			XmlNode classNode = null;
			if (entity.Persistence.Persisted)
			{
				if (!entity.HasSupertype)
				{
					classNode = map.CreateElement("class");
				}
				else
				{
					switch(DomainLayerConfig.MappingStrategy)
					{
					case GenieLamp.Core.Layers.MappingStrategy.TablePerSubclass:
						// Implementing joined subclass strategy
						// A subclass that is persisted to its own table (table-per-subclass mapping strategy) is declared using a <joined-subclass> element
						classNode = map.CreateElement("joined-subclass");
						break;
					case GenieLamp.Core.Layers.MappingStrategy.TablePerClass:
						// Implementing union-subclass strategy
						// Each table defines columns for all properties of the class, including inherited properties.
						classNode = map.CreateElement("union-subclass");
						break;
					default:
						throw new GenieLamp.Core.Exceptions.GlException("Mapping strategy is not supported: '{0}'",
						                                                DomainLayerConfig.MappingStrategy);
					}
				}
				map.AddAttribute("name", environment.ToTypeName(entity, true));
				map.AddAttribute("table", entity.Persistence.Name);
				if (!String.IsNullOrWhiteSpace(entity.Persistence.Schema))
					map.AddAttribute("schema", entity.Persistence.Schema);
				if(!entity.HasSupertype)
					map.AddAttribute("polymorphism",
					                 DomainLayerConfig.Params.ValueByName("explicitPolymorphism", false) ?
					                 "explicit" : "implicit");

				ProcessStateVersion(entity, classNode);
				Logger.ProgressStep();
				ProcessConstraints(entity, classNode);
				Logger.ProgressStep();
			}

			ProcessRelations(entity, classNode);
			Logger.ProgressStep();
			ProcessAttributes(entity, classNode);
			Logger.ProgressStep();
			cw.WriteLine();

			if (entity.Persistence.Persisted)
			{
				WriteUtilsFunctions(entity);
				ImplementPatternInterfaces(entity);

				XmlNode toAppendClass;
				if (entity.HasSupertype)
				{
					string query;
					if (String.IsNullOrWhiteSpace(entity.Supertype.Persistence.Schema))
	                    query = String.Format("//class[@name='{0}']",
					                      environment.ToTypeName(entity.Supertype, true));
					else
	                    query = String.Format("//class[@name='{0}' and @schema='{1}']",
					                      environment.ToTypeName(entity.Supertype, true), entity.Supertype.Persistence.Schema);
					XmlNode parentClassNode = map.Document.SelectSingleNode(query);
					if (parentClassNode == null)
						throw new ApplicationException(
							String.Format("Cannot find parent class node. Name: {0}", entity.Supertype.FullName));
					toAppendClass = parentClassNode;
				}
				else
					toAppendClass =	map.Document.DocumentElement;

				toAppendClass.AppendChild(map.Document.CreateComment(String.Format("{0}", entity.FullName)));
				toAppendClass.AppendChild(classNode);
			}
			Logger.ProgressStep();

			cw.EndClass();
			cw.WriteLine();
		}


		private void WriteEntityInterface(IEntity entity)
		{
			cw.WriteLine("public interface {0}", environment.ToIntfName(entity, false));
			cw.BeginScope();
			foreach(IAttribute attribute in entity.Attributes)
			{
				if (!attribute.Persistence.Persisted)
				{
					cw.WriteDoc(attribute);
					cw.WriteLine("{0} {1} {{ get; }}", environment.ToTypeName(attribute, true), attribute.Name);
				}
			}
			foreach(IEntityOperation operation in entity.Operations)
			{
				cw.WriteDoc(operation);
				cw.WriteLine("{0};", environment.ToOperationSignature(operation, true));
			}
			cw.EndScope();
			cw.WriteLine();
		}

		
		private void WriteUtilsFunctions(IEntity entity)
		{
			// GetById
			cw.WriteLine("public static {0}{1} {2}",
			                 entity.HasSupertype ? "new " : "",
			                 entity.Name,
			                 DomainLayerConfig.Methods.GetById(entity.Constraints.PrimaryId, environment).Signature);
			cw.BeginScope();
			cw.WriteLine("return {0}.CreateCriteria<{1}>()",
			                 Call_GetSession,
			                 entity.Name);
			cw.Indent++;
			foreach(IAttribute a in entity.Constraints.PrimaryId.Attributes)
			{
				cw.WriteLine(".Add(Expression.Eq(\"{0}\", {1}))",
				                 a.Name, environment.ToParamName(a));
			}
			cw.WriteLine(".UniqueResult<{0}>();", entity.Name);
			cw.Indent--;
			cw.EndScope();
			cw.WriteLine();

			// GetByUid
			foreach(IUniqueId uid in entity.Constraints.UniqueIds)
			{
				cw.WriteLine("public static {0} {1}",
				                 entity.Name,
				                 DomainLayerConfig.Methods.GetByUniqueId(uid, environment).Signature);
				cw.BeginScope();
				cw.WriteLine("return {0}.CreateCriteria<{1}>()",
				                 Call_GetSession,
				                 entity.Name);
				cw.Indent++;
				foreach(IAttribute a in uid.Attributes)
				{
					string propertyName = a.Name;
					if (a.IsUsedInRelations)
					foreach(IRelation r in a.UsedInRelations)
					{
						if (r.ChildEntity == entity)
						{
							propertyName =String.Format("{0}.{1}", r.ChildName, r.ParentEntity.PrimaryId.Attributes[0].Name);
						}
					}

					cw.Write(".Add(Expression.Eq(\"{0}\", ", propertyName);
					if (a.TypeDefinition.Required || !environment.IsNullable(a))
						cw.Write(environment.ToParamName(a));
					else
						cw.Write("{0}.HasValue ? {0}.Value : ({1})null",
						         environment.ToParamName(a),
						         environment.ToTypeName(a, true));
					cw.WriteLine("))");
				}
				cw.WriteLine(".UniqueResult<{0}>();", entity.Name);
				cw.Indent--;
				cw.EndScope();
				cw.WriteLine();
			}

			// Get by HQL
			cw.WriteLine("public static {0}System.Collections.Generic.IList<{1}> {2}",
			             entity.HasSupertype ? "new " : "",
			             entity.Name,
			             DomainLayerConfig.Methods.GetByHQL().Signature);
			cw.BeginScope();
            cw.WriteLine("return {0}.{1}.CreateQuery(hql, hqlParams)",
			             DomainLayerConfig.QueriesNamespace,
			             CodeGenDomainSupport.ClassName_QueryFactory);
			cw.Indent++;
            cw.WriteLine(".SetFirstResult(firstResult < 0 ? 0 : firstResult)");
            cw.WriteLine(".SetMaxResults(maxResult < firstResult ? 100 : maxResult)");
            cw.WriteLine(".List<{0}>();", entity.Name);
			cw.Indent--;
			cw.EndScope();
			cw.WriteLine();

			cw.WriteLine("public static {0}System.Collections.Generic.IList<{1}> {2}",
			             entity.HasSupertype ? "new " : "",
			             entity.Name,
			             DomainLayerConfig.Methods.GetPageByHQL().Signature);
			cw.BeginScope();
			cw.WriteLine("return {0}.{1}(hql, hqlParams, page * pageSize, pageSize);",
			             entity.Name,
			             DomainLayerConfig.Methods.GetByHQL().Name);
			cw.EndScope();
			cw.WriteLine();

			// Get by SQL
			cw.WriteLine("public static {0}System.Collections.Generic.IList<{1}> {2}",
			             entity.HasSupertype ? "new " : "",
			             entity.Name,
			             DomainLayerConfig.Methods.GetBySQL().Signature);
			cw.BeginScope();
            cw.WriteLine("return {0}.CreateSQLQuery(sql)", Call_GetSession);
			cw.Indent++;
            cw.WriteLine(".AddEntity(typeof({0}))", entity.Name);
            cw.WriteLine(".SetFirstResult(firstResult < 0 ? 0 : firstResult)");
            cw.WriteLine(".SetMaxResults(maxResult < firstResult ? 100 : maxResult)");
            cw.WriteLine(".List<{0}>();", entity.Name);
			cw.Indent--;
			cw.EndScope();
			cw.WriteLine();

			cw.WriteLine("public static {0}System.Collections.Generic.IList<{1}> {2}",
			             entity.HasSupertype ? "new " : "",
			             entity.Name,
			             DomainLayerConfig.Methods.GetPageBySQL().Signature);
			cw.BeginScope();
			cw.WriteLine("return {0}.{1}(sql, page * pageSize, pageSize);",
			             entity.Name,
			             DomainLayerConfig.Methods.GetPageBySQL().Name);
			cw.EndScope();
			cw.WriteLine();

			// DeleteById
			if (!entity.HasSupertype)
			{
				cw.WriteLine("public static void {0}",
				                 DomainLayerConfig.Methods.DeleteById(entity.Constraints.PrimaryId, environment).Signature);
				cw.BeginScope();
				cw.WriteLine("{0} o = {1}({2});",
				                 entity.Name,
				                 DomainLayerConfig.Methods.GetById(entity.Constraints.PrimaryId, environment).Name,
				                 environment.ToArguments(entity.Constraints.PrimaryId.Attributes));
	
				cw.WriteLine("if (o != null) o.Delete();");
				cw.EndScope();
				cw.WriteLine();
			}

			// Refresh
			cw.WriteLine("public {0} void {1}",
			             entity.HasSupertype ? "override" : "virtual",
			             DomainLayerConfig.Methods.Refresh().Signature);
			cw.BeginScope();
			cw.WriteLine("{0}.Refresh(this);",
			             Call_GetSession);
			cw.EndScope();
			cw.WriteLine();

			// Criteria
			cw.WriteLine("public static {0}ICriteria CreateCriteria()",
			             entity.HasSupertype ? "new " : "");
			cw.BeginScope();
			cw.WriteLine("return {0}.CreateCriteria<{1}>();",
			             Call_GetSession,
			             entity.Name);
			cw.EndScope();
			cw.WriteLine();

			// Get by foreign keys
			foreach(IRelation r in entity.Relations)
			{
				if (r is ISubtypeRelation
				    || r.ParentSide == RelationSide.None
				    || !r.IsChild(entity))
					continue;

				cw.WriteCommentLine("{0}", r);
				cw.WriteLine("public static System.Collections.Generic.IList<{0}> {1}",
				                 entity.Name,
				                 DomainLayerConfig.Methods.GetByRelationParent(r, environment).Signature);
				cw.BeginScope();
				cw.WriteLine("return CreateCriteria()");
				cw.Indent++;
				foreach(IRelationAttributeMatch am in r.AttributesMatch)
				{
					cw.WriteLine(".Add(Expression.Eq(\"{0}.{1}\", {2}))",
					                 r.ChildName,
					                 (r.ParentSide == RelationSide.Left ? am.Attribute : am.Attribute2).Name,
					                 environment.ToParamName(r.ParentSide == RelationSide.Left ? am.Attribute2 : am.Attribute)
					                 );
				}
                cw.WriteLine(".List<{0}>();", entity.Name);
				cw.Indent--;
				cw.EndScope();
				cw.WriteLine();
			}

			// Pagination
			cw.BeginFunction("public static {0}System.Collections.Generic.IList<{1}> {2}",
			                 entity.HasSupertype ? "new " : "",
			                 entity.Name,
			                 DomainLayerConfig.Methods.GetPage().Signature);
			cw.WriteLine("int firstResult = (pageSize < 1 ? 1 : pageSize) * (page < 0 ? 0 : page);");
			cw.WriteLine("ICriteria criteria = CreateCriteria().SetFirstResult(firstResult).SetMaxResults(pageSize);");
			cw.WriteLine("if (sortOrders != null)");
			cw.Indent++;
			cw.WriteLine("foreach({0} sort in sortOrders)",
			             DomainLayerConfig.GetClassName_SortOrder(true));
			cw.Indent++;
			cw.WriteLine("criteria = criteria.AddOrder(new Order(sort.PropertyName, sort.Ascending));");
			cw.Indent--;
			cw.Indent--;
			cw.WriteLine("return criteria.List<{0}>();", entity.Name);
			cw.EndFunction();
			cw.WriteLine();

			// IPersistentObject implementation
			if (!entity.HasSupertype)
			{
				WritePersistenceMethod("Save", "SaveOrUpdate", entity);
				WritePersistenceMethod("Delete", null, entity);
			}

			// Lambda expression queries
			cw.BeginFunction("public static System.Collections.Generic.IList<{0}> GetList(System.Linq.Expressions.Expression<Func<{0}, bool>> predicate)",
			                 environment.ToTypeName(entity, false));
			cw.WriteLine("return {0}.CreateCriteria().Add(Expression.Where<{0}>(predicate)).List<{0}>();",
			             environment.ToTypeName(entity, false));
			cw.EndFunction();
			cw.WriteLine();

			cw.BeginFunction("public static {0} GetUnique(System.Linq.Expressions.Expression<Func<{0}, bool>> predicate)",
			                 environment.ToTypeName(entity, false));
			cw.WriteLine("return {0}.CreateCriteria().Add(Expression.Where<{0}>(predicate)).UniqueResult<{0}>();",
			             environment.ToTypeName(entity, false));
			cw.EndFunction();
			cw.WriteLine();
		}


		private void WritePersistenceMethod(string methodName, string nhMethodName, IEntity entity)
		{
			if (String.IsNullOrEmpty(nhMethodName))
				nhMethodName = methodName;
			cw.WriteLine("public virtual void {0}(ITransaction outer = null)", methodName);
			cw.BeginScope();

			cw.WriteLine("ITransaction tx = null;");
			cw.WriteLine("if (outer == null) tx = {0}.BeginTransaction();",
			                 Call_GetSession);
			cw.BeginTry();
			cw.WriteLine("{0}.{1}(this);",
			                 Call_GetSession,
			                 nhMethodName);
			cw.WriteLine("if (outer == null) tx.Commit();");
			cw.EndTry();
			cw.BeginCatch("Exception");
			cw.WriteLine("if (outer == null) tx.Rollback();");
			cw.WriteLine("throw;");
			cw.EndCatch();
			cw.EndScope();
			cw.WriteLine();
		}

		/// <summary>
		/// Implements domain class interfaces provided by patterns
		/// </summary>
		/// <param name='entity'>
		/// Entity to implement
		/// </param>
		void ImplementPatternInterfaces(IEntity entity)
		{
			if (Patterns.Registry != null && Patterns.Registry.AppliedTo(entity))
			{
				cw.BeginRegion("Implementation of " + CodeGenDomainSupport.InterfaceName_UsesRegistry);
				cw.BeginProperty("{0} {1}.{2}.{3}",
				                 environment.ToTypeName(Patterns.Registry.RegistryEntity.Entity, true),
				                 DomainLayerConfig.PatternsNamespace,
				                 CodeGenDomainSupport.InterfaceName_UsesRegistry,
				                 CodeGenDomainSupport.IUsesRegistry_Property_Registry);
				cw.WriteLine("get {{ return this.{0}; }}", Patterns.Registry.GetRelation(entity).Name);
				cw.WriteLine("set {{ this.{0} = value; }}", Patterns.Registry.GetRelation(entity).Name);
				cw.EndProperty();
				cw.WriteLine();
				cw.EndRegion();
			}

			if (Patterns.Audit != null && Patterns.Audit.AppliedTo(entity))
			{
			}
		}
		#endregion
		
		
		#region Constraints
		private void ProcessConstraints(IEntity entity, XmlNode classNode)
		{
			if (entity.Constraints.PrimaryId == null || entity.Constraints.PrimaryId.Attributes.Count == 0)
				throw new ApplicationException(
					String.Format("No primary id defined. Entity {0}", entity));
			
			ProcessPrimaryId(entity.Constraints.PrimaryId, classNode);
			
			foreach (IUniqueId constraint in entity.Constraints.UniqueIds)
			{
				ProcessUniqueId(constraint, classNode);
			}
		}
		
		private void ProcessPrimaryId(IPrimaryId primaryId, XmlNode classNode)
		{
			XmlNode propNode = null;
			if (primaryId.Composite)
			{
				propNode = map.CreateElement("composite-id");
				foreach (IAttribute pidAttribute in primaryId.Attributes)
				{
					IRelation manyToOne = pidAttribute.GetManyToOne();
					if (manyToOne != null)
					{
						map.CreateElement("key-many-to-one");
						map.AddAttribute("name", manyToOne.ChildName);
						map.AddAttribute("class", environment.ToTypeName(manyToOne.ParentEntity, true));
						map.AddAttribute("column", pidAttribute.Persistence.Name);
						propNode.AppendChild(map.CurrentNode);
						DeclareManyToOneProperty(manyToOne, null);
						manyToOne.Processed = true;
					}
					else
					{
						map.CreateElement("key-property");
						map.AddAttribute("name", pidAttribute.Name);
						map.AddAttribute("type", environment.ToTypeName(pidAttribute, true));
						map.AddAttribute("column", pidAttribute.Persistence.Name);
						propNode.AppendChild(map.CurrentNode);
						DeclareProperty(pidAttribute, null);
					}
					pidAttribute.Processed = true;
				}

				cw.BeginRegion("Composite id override methods");
				cw.BeginFunction("public override bool Equals(object o)");
				cw.WriteLine("return o != null && (o as {0}) != null && {1};",
				             environment.ToTypeName(primaryId.Entity, false),
				             cw.ToSeparatedString(primaryId.Attributes.ToList(),
				                     " && ",
				                     delegate(object item, int count)
				                    {
										string name = (item as IAttribute).Name;
										IRelation manyToOne = (item as IAttribute).GetManyToOne();
										if (manyToOne != null)
										{
											IAttribute related = (item as IAttribute).GetManyToOneRelatedAttribute();
											name = String.Format("{0}.{1}", manyToOne.ChildName, related.Name);
										}
										return String.Format("this.{0} == (o as {1}).{0}",
										                     name,
										                     environment.ToTypeName(primaryId.Entity, false));
									})
				);
				cw.EndFunction();
				cw.WriteLine();

				cw.BeginFunction("public override int GetHashCode()");
				cw.WriteLine("return ({0}).GetHashCode();",
				             cw.ToSeparatedString(primaryId.Attributes.ToList(),
				                     " + \"|\" + ",
				                     delegate(object item, int count)
				                    {
										string name = (item as IAttribute).Name;
										IRelation manyToOne = (item as IAttribute).GetManyToOne();
										if (manyToOne != null)
										{
											IAttribute related = (item as IAttribute).GetManyToOneRelatedAttribute();
											name = String.Format("{0}.{1}", manyToOne.ChildName, related.Name);
										}
										return String.Format("this.{0}.ToString()", name);
									})
				);
				cw.EndFunction();
				cw.EndRegion();
				cw.WriteLine();
			}
			else
			{
				if (primaryId.Entity.HasSupertype)
				{
					if (DomainLayerConfig.MappingStrategy == GenieLamp.Core.Layers.MappingStrategy.TablePerSubclass)
						propNode = map.CreateElement("key");
					else
						propNode = null;
				}
				else
				{
					propNode = map.CreateElement("id");
					map.AddAttribute("name", primaryId.Attributes[0].Name);
					map.AddAttribute("access", "property");
					
					if (primaryId.HasGenerator)
					{
						XmlNode generatorNode = map.CreateElement("generator");
						propNode.AppendChild(generatorNode);
						switch (primaryId.Generator.Type)
						{
						case GeneratorType.Sequence:
							map.AddAttribute("class", "native"); // "native" supports both sequeces and identity
							XmlNode genParam = map.CreateElement("param");
							generatorNode.AppendChild(genParam);
							map.AddAttribute("name", "sequence");
							genParam.InnerText = primaryId.Generator.Persistence.FullName;
							break;
						case GeneratorType.Guid:
							map.AddAttribute("class", "guid");
							break;
						case GeneratorType.GuidHex:
							map.AddAttribute("class", "uuid.hex");
							break;
						}
					}
				}
				if (propNode != null)
				{
					map.CurrentNode = propNode;
					map.AddAttribute("column", primaryId.Attributes[0].Persistence.Name);
				}
			}

			foreach (IAttribute attribute in primaryId.Attributes)
			{
				if (!primaryId.Entity.HasSupertype && !attribute.Processed)
					DeclareProperty(attribute, null);
				attribute.Processed = true;
			}

			if (propNode != null)
			{
				map.CurrentNode = classNode;
				map.AddFirstChild(propNode);
			}
		}

		private void ProcessUniqueId(IUniqueId uniqueId, XmlNode classNode)
		{
			bool canDeclare = true;
			foreach (IAttribute attribute in uniqueId.Attributes)
			{
				canDeclare = canDeclare && 
					!attribute.Processed &&
				    (attribute.UsedInRelations.Count == 0);
				if (!canDeclare)
					break;
			}
			
			//?? The element 'class' in namespace 'urn:nhibernate-mapping-2.2' has invalid child element 'natural-id'
//			if (canDeclare && false) { 
//	            XmlNode constraintNode = map.CreateElement("natural-id");
//				foreach(IAttribute attribute in uniqueId.Attributes) {
//					DeclareProperty(attribute, constraintNode);
//					processedAttributes.Add(attribute);
//				}
//	            classNode.AppendChild(constraintNode);
//			}
		}
		#endregion

		private void ProcessStateVersion(IEntity entity, XmlNode classNode)
		{
			// The <version> element needs to go above all <property> elements in mapping file
			foreach(IAttribute attribute in entity.Attributes)
			{
				if (Patterns.StateVersion != null && Patterns.StateVersion.IsUsed(attribute))
				{
					DeclareProperty(attribute, null);
					map.CreateElement("version");
					map.AddAttribute("name", attribute.Name);
					map.AddAttribute("column", attribute.Persistence.Name);
					map.AddAttribute("type", environment.ToTypeName(attribute, false));
					map.AddAttribute("access", "property");
					if (Patterns.StateVersion.UnsavedValue !=  Const.EmptyValue)
						map.AddAttribute("unsaved-value", Patterns.StateVersion.UnsavedValue);
					classNode.AppendChild(map.CurrentNode);
					attribute.Processed = true;
				}
			}
		}
		
		
		#region Relations
		private void ProcessRelations(IEntity entity, XmlNode classNode)
		{
			foreach (IRelation relation in entity.Relations)
			{
				if (relation is ISubtypeRelation || relation.Processed)
				{
					relation.Processed = false; // Relation may be processed in composite primary id so unmark it for processing in related class
					continue;
				}
				ProcessRelation(relation, entity, classNode);
				foreach (IRelationAttributeMatch attrMatch in relation.AttributesMatch)
				{
					if (relation.Entity == entity)
						attrMatch.Attribute.Processed = true;
					if (relation.Entity2 == entity)
						attrMatch.Attribute2.Processed = true;
				}
			}
		}
		
		private void ProcessRelation(IRelation relation, IEntity entity, XmlNode classNode)
		{
			if (relation.Cardinality == Cardinality.RM_M)
			{
				throw new ApplicationException("Many-to-many relation implementation is not supported. Use association entity with two 1:M relations instead.");
			}
			else if (relation.Cardinality == Cardinality.R1_1)
			{
				DeclareOneToOneProperty(relation, entity, classNode);
			}
			else
			{
				if (relation.IsChild(entity) && relation.ChildNavigate)
				{
					DeclareManyToOneProperty(relation, classNode);
				}
				// May be parent and child when self-related
				if (relation.IsParent(entity) && relation.ParentNavigate)
				{
					DeclareOneToManyProperty(relation, classNode);
				}
			}
		}
		#endregion
		
		
		#region Attributes
		private void ProcessAttributes(IEntity entity, XmlNode classNode)
		{
			foreach (IAttribute attribute in entity.Attributes)
			{
				if (!attribute.Processed)
				{
					ProcessAttribute(attribute, classNode);
					attribute.Processed = true;
				}
			}
		}

		private void ProcessAttribute(IAttribute attribute, XmlNode mapIntoNode)
		{
			if (attribute.Persistence.Persisted)
				DeclareProperty(attribute, mapIntoNode);
		}
		#endregion
		
		
		private void DeclareOneToOneProperty(IRelation relation, IEntity entity, XmlNode mapIntoNode)
		{
			IEntity entity2 = entity == relation.Entity ? relation.Entity2 : relation.Entity;
			IAttributes attributes =
				entity == relation.Entity ?
					relation.AttributesMatch.Attributes :
					relation.AttributesMatch.Attributes2;
			bool navigate = entity == relation.Entity ? relation.Navigate : relation.Navigate2;
			string propertyName = entity == relation.Entity ? relation.Name : relation.Name2;
			string propertyName2 = entity == relation.Entity ? relation.Name2 : relation.Name;
			bool primaryKeySide = entity.Constraints.PrimaryId.ContainsAttribute(attributes[0]);

			if (navigate)
			{
				if (Patterns.Registry != null && Patterns.Registry.RegistryEntity.Entity == entity2)
				{
					string memberName = String.Format("m_{0}", propertyName);
					cw.WriteLine("private {0} {1} = null;",
					             environment.ToTypeName(entity2, true),
					             memberName);
					cw.BeginProperty("public virtual {0} {1}",
					                 environment.ToTypeName(entity2, true),
					                 propertyName);
					cw.WriteLine("get {{ return {0}; }}", memberName);
					cw.WriteLine("set {{ if ({0} == null) {0} = value; }}", memberName);
					cw.EndProperty();
				}
				else
				{
					cw.SimpleProperty(
						AccessLevel.Public,
						VirtualisationLevel.Virtual,
						environment.ToTypeName(entity2, true),
						propertyName,
						true,
						true);
				}

				if (entity.Persistence.Persisted && entity2.Persistence.Persisted)
				{
					XmlNode refNode;
					if (primaryKeySide)
					{
						refNode = map.CreateElement("one-to-one");
						map.AddAttribute("name", propertyName);
						map.AddAttribute("class", environment.ToTypeName(entity2.Type, true));
						map.AddAttribute("property-ref", propertyName2);
						map.AddAttribute("cascade", RelationCascadeToNHValue(relation.Cascade));
					}
					else
					{
						refNode = map.CreateElement("many-to-one");
						map.AddAttribute("name", propertyName);
						map.AddAttribute("class", environment.ToTypeName(entity2.Type, true));
						map.AddAttribute("unique", "true");
						foreach (IAttribute a in attributes)
						{
							XmlNode colNode = InitColumnNode(a);
							refNode.AppendChild(colNode);
						}
					}
					mapIntoNode.AppendChild(refNode);
				}
			}
		}
		
		private void DeclareManyToOneProperty(IRelation relation, XmlNode mapIntoNode)
		{
			cw.SimpleProperty(
				AccessLevel.Public,
				VirtualisationLevel.Virtual,
				environment.ToTypeName(relation.ParentEntity.Type, true),
				relation.ChildName,
				true, true);
			// Mapping
			if (relation.Entity.Persistence.Persisted && relation.Entity2.Persistence.Persisted)
			{
				if (mapIntoNode == null)
					return;
				XmlNode refNode = map.CreateElement("many-to-one");
				map.AddAttribute("name", relation.ChildName);
				map.AddAttribute("class", environment.ToTypeName(relation.ParentEntity.Type, true));
				map.AddAttribute("cascade", RelationCascadeToNHValue(relation.Cascade));
				foreach (IAttribute a in relation.ChildAttributes)
				{
					XmlNode colNode = InitColumnNode(a);
					refNode.AppendChild(colNode);
				}
				mapIntoNode.AppendChild(refNode);
			}
		}
		
		private void DeclareOneToManyProperty(IRelation relation, XmlNode mapIntoNode)
		{
			cw.SimpleProperty(
				AccessLevel.Public,
				VirtualisationLevel.Virtual,
				String.Format("Iesi.Collections.Generic.ISet<{0}>", environment.ToTypeName(relation.ChildEntity, true)),
				relation.ParentName,
				true, true);
			// Mapping
			if (relation.Entity.Persistence.Persisted && relation.Entity2.Persistence.Persisted)
			{
				if (mapIntoNode == null)
					return;
				XmlNode refNode = map.CreateElement("set");
				map.AddAttribute("name", relation.ParentName);
				map.AddAttribute("table", relation.ChildEntity.Persistence.FullName);
				map.AddAttribute("inverse", "true");
				map.AddAttribute("lazy", "true");
				map.AddAttribute("cascade", RelationCascadeToNHValue(relation.Cascade));
				foreach (IAttribute a in relation.ChildAttributes)
				{
					XmlNode keyNode = map.CreateElement("key");
					map.AddAttribute("column", a.Persistence.Name);
					refNode.AppendChild(keyNode);
				}
				XmlNode relNode = map.CreateElement("one-to-many");
				map.AddAttribute("class", environment.ToTypeName(relation.ChildEntity.Type, true));
				refNode.AppendChild(relNode);
				mapIntoNode.AppendChild(refNode);
			}
		}


		private void DeclareProperty(IAttribute attribute, XmlNode mapIntoNode)
		{
			cw.WriteProperty(AccessLevel.Public,
			                 VirtualisationLevel.Virtual,
			                 attribute,
			                 environment,
			                 null);

			if (mapIntoNode == null)
				return;
			
			XmlNode propNode = map.CreateElement("property");
			map.AddAttribute("name", attribute.Name);
			map.AddAttribute("access", "property");
			if (attribute.Type is IScalarType)
			{
				switch((attribute.Type as IScalarType).BaseType)
				{
				case BaseType.TypeDateTime:
					map.AddAttribute("type", "timestamp"); // NHibernate cut milliseconds in DateTime
					break;
				case BaseType.TypeBool:
					switch(PersistenceLayerConfig.BooleanValuePersistence)
					{
					case BooleanValuePersistence.TrueFalse:
						map.AddAttribute("type", "TrueFalse");
						break;
					case BooleanValuePersistence.YesNo:
						map.AddAttribute("type", "YesNo");
						break;
					default:
						map.AddAttribute("type", "boolean");
						break;
					}
					break;
				}
			}
			if (attribute.ReadOnly)
			{
				map.AddAttribute("insert", "false");
				map.AddAttribute("update", "false");
			}
            
			XmlNode colNode = InitColumnNode(attribute);
			propNode.AppendChild(colNode);
			mapIntoNode.AppendChild(propNode);
		}


		private XmlNode InitColumnNode(IAttribute attribute)
		{
			XmlNode colNode = map.CreateElement("column");
			map.AddAttribute("name", attribute.Persistence.Name);
			map.AddAttribute("not-null", attribute.TypeDefinition.Required ? "true" : "false");
			if (attribute.Persistence.TypeDefined)
				map.AddAttribute("sql-type", attribute.Persistence.TypeName);
			return colNode;
		}
		
	}
}

