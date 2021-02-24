using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Vaquinha.Unit.Tests.Extensions
{
    public static class IActionExtensions
    {
        public static void StatusCodeShouldBe(this Task<IActionResult> objectResult, HttpStatusCode httpStatusCode)
        {
            (objectResult.Result as ObjectResult).StatusCode.Should().Be((int)httpStatusCode);
        }
    }
}