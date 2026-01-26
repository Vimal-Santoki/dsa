namespace DSA.Api.Common.AuthZ.Dto
{
    // The Helper Record for "Strongly Typed" Route Selection
    internal record RouteParam(string Name)
    {
        public static RouteParam From(string name) => new(name);
    }
}
