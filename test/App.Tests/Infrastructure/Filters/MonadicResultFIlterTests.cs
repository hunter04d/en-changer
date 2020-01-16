using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnChanger.Infrastructure.Exceptions;
using EnChanger.Infrastructure.Filters;
using FluentAssertions;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;
using static LanguageExt.Prelude;

namespace EnChanger.Tests.Infrastructure.Filters
{
    public class MonadicResultFilterTests
    {
        private readonly MonadicResultFilterAttribute _filter = new MonadicResultFilterAttribute();

        [Fact]
        public void PassingSome_ReturnsOk()
        {
            var opt = Some(42);
            var context = ContextFromObject(opt);

            _filter.OnResultExecuting(context);

            context.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(42);
        }

        [Fact]
        public void PassingRight_ReturnsOk()
        {
            var either = Right(42);
            var context = ContextFromObject(either);

            _filter.OnResultExecuting(context);

            context.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(42);
        }

        [Fact]
        public void PassingNone_ReturnsNotFound()
        {
            var either = None;
            var context = ContextFromObject(either);

            _filter.OnResultExecuting(context);

            context.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void PassingLeft_ReturnsBadRequest()
        {
            var either = Left(new BadRequestException("Test Error Message"));
            var context = ContextFromObject(either);

            _filter.OnResultExecuting(context);

            context.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void PassingLeft_WhichIsNotAnException_Throws()
        {
            var either = Left(42);
            var context = ContextFromObject(either);

            _filter.Invoking(_ => _.OnResultExecuting(context)).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void PassingLeft_WhichNotHandledException_ThrowsThatException()
        {
            var either = Left(new Exception());
            var context = ContextFromObject(either);

            _filter.Invoking(_ => _.OnResultExecuting(context)).Should().ThrowExactly<Exception>();
        }


        private static ResultExecutingContext ContextFromObject(object o) =>
            new ResultExecutingContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(), new ObjectResult(o), null);
    }
}
