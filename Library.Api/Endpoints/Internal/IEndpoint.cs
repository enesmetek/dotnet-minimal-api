namespace Library.Api.Endpoints.Internal
{
    public interface IEndpoint
    {
        static abstract void DefineEndpoints(IEndpointRouteBuilder app);

        static abstract void AddServices(IServiceCollection services, IConfiguration configuration);
    }
}
