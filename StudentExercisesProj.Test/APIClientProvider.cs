﻿//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace StudentExercisesProj.Test
//{
//    class APIClientProvider
//    {
//    }
//}

using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using Xunit;
using StudentExercisesPart5;

namespace StudentExercisesProj.Test
{
    class APIClientProvider : IClassFixture<WebApplicationFactory<Startup>>
{
    public HttpClient Client { get; private set; }
    private readonly WebApplicationFactory<Startup> _factory = new WebApplicationFactory<Startup>();

    public APIClientProvider()
    {
        Client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _factory?.Dispose();
        Client?.Dispose();
    }
}
}