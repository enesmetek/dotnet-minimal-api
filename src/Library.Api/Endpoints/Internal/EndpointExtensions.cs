﻿using System.Reflection;

namespace Library.Api.Endpoints.Internal
{
    public static class EndpointExtensions
    {
        public static void AddEndpoints<TMarker>(this IServiceCollection services,
            IConfiguration configuration)
        {
            AddEndpoints(services, typeof(TMarker), configuration);
        }

        public static void AddEndpoints(this IServiceCollection services,
            Type typeMarker, IConfiguration configuration)
        {
            var endpointTypes = GetEndpointTypesFromAssemblyContaining(typeMarker);

            foreach (var endpointType in endpointTypes)
            {
                endpointType.GetMethod(nameof(IEndpoint.AddServices))!
                    .Invoke(null, [services, configuration]);
            }
        }

        public static void UseEndpoints<TMarker>(this IApplicationBuilder app)
        {
            UseEndpoints(app, typeof(TMarker));
        }

        public static void UseEndpoints(this IApplicationBuilder app, Type typeMarker)
        {
            var endpointTypes = GetEndpointTypesFromAssemblyContaining(typeMarker);

            foreach (var endpointType in endpointTypes)
            {
                endpointType.GetMethod(nameof(IEndpoint.DefineEndpoints))!
                    .Invoke(null, [app]);
            }
        }

        private static IEnumerable<TypeInfo> GetEndpointTypesFromAssemblyContaining(Type typeMarker)
        {
            var endpointTypes = typeMarker.Assembly.DefinedTypes
                .Where(x => !x.IsAbstract && !x.IsInterface &&
                            typeof(IEndpoint).IsAssignableFrom(x));
            return endpointTypes;
        }
    }
}
