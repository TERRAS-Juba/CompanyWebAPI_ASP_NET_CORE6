namespace Entities.RequestFeatures;

public abstract class RequestParameters
{
    public const int MaxPageSize = 50;
    public int pageNumber { get; set; } = 1;
    private int _pageSize = 10;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
    public string OrderBy { get; set; }
    public string Fields { get; set; }
    public string SearchTerm { get; set; }

}