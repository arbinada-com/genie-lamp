public class %ClassName_DomainQueryFactory%
{
	public static %ClassFullName_DomainQueryParams% ToDomainQueryParams(%ClassName_QueryParams% queryParams)
	{
		%ClassFullName_DomainQueryParams% domainQueryParams = null;
        if (queryParams != null)
        {
            domainQueryParams = new %ClassFullName_DomainQueryParams%();
            foreach (%ClassName_QueryParam% param in queryParams)
			{
                domainQueryParams.AddParam(param.Name, param.Value);
			}
        }
		return domainQueryParams;
	}
}
