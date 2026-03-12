using System;
using FunTranslationClient;
//using FunTranslationClient;
using Microsoft.Extensions.DependencyInjection;

namespace FunTranslationsClient
{
    public static class FunTranslationClientExtensions
    {
        public static void AddFunTranslationClient(this IServiceCollection services)
        {
            services.AddScoped<ITranslationClient, TranslationClient>();
        }
    }
}