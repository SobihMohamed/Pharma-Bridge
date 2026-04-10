using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Resolver
{
    public class PictureResolver<TSource, TDestination> (IConfiguration configuration) : IMemberValueResolver<TSource, TDestination, string, string>
    {
        public string Resolve(TSource source, TDestination destination, string sourceMember, string destMember, ResolutionContext context)
        {
            if(string.IsNullOrEmpty(sourceMember))
                return string.Empty;

            var baseUrl = configuration["ApiBaseUrl"];
            return $"{baseUrl}/{sourceMember.TrimStart('/')}"; // http://localhost:5000/images/product1.jpg
        }
    }
}
