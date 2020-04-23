namespace %PATTERNS_NAMESPACE%
{
	#region Interfaces
	public interface %InterfaceName_OnSave%
	{
		void OnSave(%ClassName_EventHandlerBase% sender, NHibernate.Event.SaveOrUpdateEvent e);
	}

	public interface %InterfaceName_OnDelete%
	{
		void OnDelete(%ClassName_EventHandlerBase% sender, NHibernate.Event.DeleteEvent e, Iesi.Collections.ISet transientEntities);
	}

	public interface %InterfaceName_OnFlush%
	{
		void OnFlush(%ClassName_EventHandlerBase% sender, NHibernate.Event.FlushEntityEvent e);
	}

<%@ language C# %>
<%@ using System.Collections.Generic %>
<% if (data.ContainsKey("patterns") && data["patterns"].Contains("Registry")) { %>
	public interface %InterfaceName_UsesRegistry%
	{
		<%=data["pattern.Registry"][0] %> %IUsesRegistry_Property_Registry% { get; set; }
	}

<% }
   if (data.ContainsKey("patterns") && data["patterns"].Contains("Audit")) { %>
	public interface %InterfaceName_UsesAudit%
	{
		string <%=data["pattern.Audit"][0] %> { get; set; }
		DateTime? <%=data["pattern.Audit"][1] %> { get; set; }
		string <%=data["pattern.Audit"][2] %> { get; set; }
		DateTime? <%=data["pattern.Audit"][3] %> { get; set; }
	}
<% } %>
	#endregion

	public class %ClassName_EventHandlerBase%
	{
		public bool HasDirtyProperties(NHibernate.Event.FlushEntityEvent e)
		{
			if (!e.EntityEntry.RequiresDirtyCheck(e.Entity) || !e.EntityEntry.ExistsInDatabase || e.EntityEntry.LoadedState == null)
				return false;
			object[] currentState = e.EntityEntry.Persister.GetPropertyValues(e.Entity, e.Session.EntityMode);
			object[] loadedState = e.EntityEntry.LoadedState;
			for(int i = 0; i < e.EntityEntry.Persister.EntityMetamodel.Properties.Length; i++)
			{
				if(!NHibernate.Intercept.LazyPropertyInitializer.UnfetchedProperty.Equals(currentState[i])
				   && e.EntityEntry.Persister.EntityMetamodel.Properties[i].Type.IsDirty(loadedState[i], currentState[i], e.Session))
					return true;
			}
			return false;
		}
	}


	public class %ClassName_DomainEventHandler% :
		%ClassName_EventHandlerBase%,
		NHibernate.Event.ISaveOrUpdateEventListener,
		NHibernate.Event.IFlushEntityEventListener,
		NHibernate.Event.IDeleteEventListener
	{
		public void OnSaveOrUpdate(NHibernate.Event.SaveOrUpdateEvent e)
		{
			if (e.Entity is %InterfaceName_OnSave%)
				(e.Entity as %InterfaceName_OnSave%).OnSave(this, e);
		}

		public void OnFlushEntity(NHibernate.Event.FlushEntityEvent e)
		{
			if (e.EntityEntry.Status == NHibernate.Engine.Status.Deleted)
				return;
			if (e.Entity is %InterfaceName_OnFlush%)
				(e.Entity as %InterfaceName_OnFlush%).OnFlush(this, e);
		}

		#region IDeleteEventListener implementation
		public void OnDelete(NHibernate.Event.DeleteEvent e)
		{
			if (e.Entity is %InterfaceName_OnDelete%)
				(e.Entity as %InterfaceName_OnDelete%).OnDelete(this, e, null);
		}

		public void OnDelete(NHibernate.Event.DeleteEvent e, Iesi.Collections.ISet transientEntities)
		{
			if (e.Entity is %InterfaceName_OnDelete%)
				(e.Entity as %InterfaceName_OnDelete%).OnDelete(this, e, transientEntities);
		}
		#endregion
	}


	public class %ClassName_CommonEntityInterceptor% : EmptyInterceptor
	{
<% if (data.ContainsKey("patterns") && data["patterns"].Contains("Registry")) { %>
		private IList<Medasys.BusinessModules.Accounting.Domain.Core.EntityRegistry> toUnRegister = 
			new List<Medasys.BusinessModules.Accounting.Domain.Core.EntityRegistry>();
<% } %>
		private object locker = new object();
		private ISession session = null;
		
		public override void SetSession(ISession session)
		{
			base.SetSession(session);
			this.session = session;
		}
		
		public override bool OnSave(object entity, object id, object[] state,
		                            string[] propertyNames, NHibernate.Type.IType[] types)
		{
			//base.OnSave(entity, id, state, propertyNames, types);
			bool modified = false;
<% if (data.ContainsKey("patterns") && data["patterns"].Contains("Registry")) { %>
			%InterfaceName_UsesRegistry% registered = entity as %InterfaceName_UsesRegistry%;
			if (registered != null && registered.%IUsesRegistry_Property_Registry% == null)
			{
				ISession childSession = this.session.GetSession(EntityMode.Poco);
				<%=data["pattern.Registry"][0] %> r =
					new <%=data["pattern.Registry"][0] %>();
				r.<%=data["pattern.Registry"][2] %> = childSession.CreateCriteria<<%=data["pattern.Registry"][1] %>>()
					.Add(NHibernate.Criterion.Expression.Eq("<%=data["pattern.Registry"][3] %>", registered.GetType().FullName))
					.UniqueResult<<%=data["pattern.Registry"][1] %>>();
				childSession.Save(r);
				childSession.Flush();
				//this.session.Refresh(r);
				state[Array.IndexOf(propertyNames, "<%=data["pattern.Registry"][4] %>")] = r;
			}
<% } if (data.ContainsKey("patterns") && data["patterns"].Contains("Audit")) { %>
			%InterfaceName_UsesAudit% auditable = entity as %InterfaceName_UsesAudit%;
			if (auditable != null)
			{
				state[Array.IndexOf(propertyNames, "<%=data["pattern.Audit"][0] %>")] = GetActorName();
				state[Array.IndexOf(propertyNames, "<%=data["pattern.Audit"][1] %>")] = DateTime.Now;
				modified = true;
			}
<% } %>
			return modified;
		}

<% if (data.ContainsKey("patterns") && data["patterns"].Contains("Audit")) { %>
		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, 
		                                  string[] propertyNames, NHibernate.Type.IType[] types)
		{
			base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
			bool modified = false;
			IUsesAudit auditable = entity as IUsesAudit;
			if (auditable != null)
			{
				currentState[Array.IndexOf(propertyNames, "<%=data["pattern.Audit"][2] %>")] = GetActorName();
				currentState[Array.IndexOf(propertyNames, "<%=data["pattern.Audit"][3] %>")] = DateTime.Now;
				modified = true;
			}
			return modified;
		}
<% } %>
		public string GetActorName()
		{
			return String.IsNullOrEmpty(System.Threading.Thread.CurrentPrincipal.Identity.Name)
				? "?" : System.Threading.Thread.CurrentPrincipal.Identity.Name;
		}

<% if (data.ContainsKey("patterns") && data["patterns"].Contains("Registry")) { %>
		public override void OnDelete(object entity, object id, object[] state, 
		                              string[] propertyNames, NHibernate.Type.IType[] types)
		{
			base.OnDelete(entity, id, state, propertyNames, types);
			IUsesRegistry registered = entity as IUsesRegistry;
			if (registered != null && registered.Registry != null)
			{
				lock(locker)
				{
					toUnRegister.Add(registered.Registry);
				}
			}
		}

		public override void PostFlush(System.Collections.ICollection entities)
		{
			base.PostFlush(entities);
			lock (locker)
			{
				foreach (Medasys.BusinessModules.Accounting.Domain.Core.EntityRegistry r in toUnRegister)
				{
					this.session.Delete(r);
				}
				toUnRegister.Clear();
			}
		}
<% } %>
	}


<% if (data.ContainsKey("patterns") && data["patterns"].Contains("Localization")) { %>
		
	public class L
	{
		public static GNU.Gettext.GettextResourceManager Catalog
		{
			get { return new GNU.Gettext.GettextResourceManager(<%=data["pattern.Localization"][0] %>); }
		}
	}
<% } %>
}

