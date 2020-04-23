namespace %QUERING_NAMESPACE%
{
	using NHibernate.Criterion;

	public class %ClassName_SortOrder%
	{
		public %ClassName_SortOrder%(string propertyName, bool ascending)
		{
			this.PropertyName = propertyName;
			this.Ascending = ascending;
		}
	
		public string PropertyName { get; set; }
		public bool Ascending { get; set; }
	}
	
	public class HqlParam
	{
	    public HqlParam(string name, object value)
	    {
	        this.Name = name;
	        this.Value = value;
	    }
	    public string Name { get; set; }
	    public object Value { get; set; }
	}
	
	public class %ClassName_DomainQueryParams% : System.Collections.Generic.List<HqlParam>
	{
		public static %ClassName_DomainQueryParams% CreateParams()
		{
			return new %ClassName_DomainQueryParams%();
		}
	
		public %ClassName_DomainQueryParams% AddParam(string name, object value)
		{
			this.Add(new HqlParam(name, value));
			return this;
		}
	}
	
	public class %ClassName_QueryFactory%
	{
	    public static IQuery CreateQuery(string hql, %ClassName_DomainQueryParams% hqlParams)
	    {
	        IQuery query = %PERSISTENCE_NAMESPACE%.SessionManager.GetSession().CreateQuery(hql);
	        if (hqlParams != null)
	        {
	            foreach (HqlParam param in hqlParams)
	            {
	                query = query.SetParameter(param.Name, param.Value);
	            }
	        }
	        return query;
	    }
	}
}