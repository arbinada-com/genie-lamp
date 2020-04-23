[DataContract]
[Route("/%ServiceName_Persistence%")]
public class %ServiceRequest_Persistence%
{
    [DataMember]
    public %ClassName_UnitOfWorkDTO% UnitOfWork { get; set; }
}

[DataContract]
public class UpdatedObjects :
	Dictionary<string, %ClassName_PersistentObjectDTO%>,
	IEnumerable<%ClassName_PersistentObjectDTO%>
{
	public bool Update<T>(ref T dto) where T : DomainObjectDTO
	{
		if (dto != null)
		{
			%ClassName_PersistentObjectDTO% found = null;
			if (this.TryGetValue(dto.%PropertyName_InternalObjectId%, out found))
			{
			    dto = found as T;
                return true;
            }
		}
        return false;
	}

    public new IEnumerator<%ClassName_PersistentObjectDTO%> GetEnumerator()
    {
        return this.Values.GetEnumerator();
    }
}


[DataContract]
public class %ServiceResponse_Persistence% : IHasResponseStatus
{
    public %ServiceResponse_Persistence%()
    {
        this.ResponseStatus = new ResponseStatus();
        this.CommitResult = new CommitResult();
        this.UpdatedObjects = new UpdatedObjects();
    }

    [DataMember]
    public UpdatedObjects UpdatedObjects { get; set; }
    [DataMember]
    public CommitResult CommitResult { get; set; }

    [DataMember]
    public ResponseStatus ResponseStatus { get; set; }
}


[DataContract]
public class %ClassName_QueryParam%
{
    public %ClassName_QueryParam%(string name, object value)
    {
        this.Name = name;
        this.Value = value;
    }

	[DataContract]
	public enum QueryParamType
	{
	    [EnumMember]ParamTypeObject,
	    [EnumMember]ParamTypeString,
	    [EnumMember]ParamTypeInt,
	    [EnumMember]ParamTypeDecimal,
	    [EnumMember]ParamTypeFloat,
	    [EnumMember]ParamTypeBool,
	    [EnumMember]ParamTypeDateTime
	}

    [DataMember]
    public string Name { get; set; }
	[DataMember]
	public QueryParamType ParamType { get; set; }
	[DataMember]
	public object ObjectValue { get; set; }
	[DataMember]
	public string StringValue { get; set; }
	[DataMember]
	public int IntValue { get; set; }
	[DataMember]
	public decimal DecimalValue { get; set; }
	[DataMember]
	public float FloatValue { get; set; }
	[DataMember]
	public bool BoolValue { get; set; }
	[DataMember]
	public DateTime DateTimeValue { get; set; }

	public object Value
	{
	    get
	    {
	        switch (this.ParamType)
	        {
	            case QueryParamType.ParamTypeString:
	                return StringValue;
	            case QueryParamType.ParamTypeInt:
	                return IntValue;
	            case QueryParamType.ParamTypeDecimal:
	                return DecimalValue;
	            case QueryParamType.ParamTypeFloat:
	                return FloatValue;
	            case QueryParamType.ParamTypeBool:
	                return BoolValue;
	            case QueryParamType.ParamTypeDateTime:
	                return DateTimeValue;
	            default:
	                return ObjectValue;
	        }
	    }
	    private set
	    {
	        if (value is int)
	        {
	            IntValue = (int)value;
	            this.ParamType = QueryParamType.ParamTypeInt;
	        }
	        else if (value is string)
	        {
	            StringValue = value.ToString();
	            this.ParamType = QueryParamType.ParamTypeString;
	        }
	        else if (value is decimal)
	        {
	            DecimalValue = (decimal)value;
	            this.ParamType = QueryParamType.ParamTypeDecimal;
	        }
	        else if (value is float)
	        {
	            FloatValue = (float)value;
	            this.ParamType = QueryParamType.ParamTypeFloat;
	        }
	        else if (value is bool)
	        {
	            BoolValue = (bool)value;
	            this.ParamType = QueryParamType.ParamTypeBool;
	        }
	        else if (value is DateTime)
	        {
	            DateTimeValue = (DateTime)value;
	            this.ParamType = QueryParamType.ParamTypeDateTime;
	        }
	        else
	        {
	            ObjectValue = value;
	            this.ParamType = QueryParamType.ParamTypeObject;
	        }
	    }
	}
}

public class %ClassName_QueryParams% : System.Collections.Generic.List<%ClassName_QueryParam%>
{
	public static %ClassName_QueryParams% CreateParams()
	{
		return new %ClassName_QueryParams%();
	}

	public %ClassName_QueryParams% AddParam(string name, object value)
	{
		this.Add(new %ClassName_QueryParam%(name, value));
		return this;
	}
}

