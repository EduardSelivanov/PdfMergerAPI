namespace PdfMergerAPI.Services
{
    public class PathsService
    {
        private List<string>? _paths;

        public void SetPaths(List<string> paths)
        {
            _paths = paths;
        }
        public List<string>? GetPaths()
        {
                return _paths;
        }
    }
}
