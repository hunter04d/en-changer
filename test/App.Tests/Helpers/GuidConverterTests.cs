using System;
using System.Collections.Generic;
using System.Linq;
using EnChanger.Helpers;
using FluentAssertions;
using Xunit;

namespace EnChanger.Tests.Helpers
{
    public class GuidConverterTests
    {
        public static IEnumerable<object[]> TestData = new[]
        {
            new object[] {new Guid("B99BE3CA-D1BC-4BF5-A113-58EEBB71A3D3")},
            new object[] {new Guid("4433ADCB-3EE6-4BAA-AA84-06B2BDE35F8E")},
            new object[] {new Guid("44B9D4CE-63B5-48C7-AD0D-6C56DF2FCFFE")},
            new object[] {new Guid(Enumerable.Repeat((byte) 'A', 16).ToArray())},
            new object[] {Guid.Empty}
        };

        [Theory]
        [MemberData(nameof(TestData))]
        public void GuidConverter_RoundTrip_ReturnsTheSameGuid(Guid guid)
        {
            guid.ToBase64().FromBase64()
                .Match(g => g.Should().Be(guid), e => throw e);
        }

        public static IEnumerable<object[]> MalformedTestData = new[]
        {
            new object[] {"Bad input"},
            new object[] {"Another one"},
            new object[] {new string('\t', 22)}
        };

        [Theory]
        [MemberData(nameof(MalformedTestData))]
        public void FromBase64_ShouldReturnLeft_IfInputIsMalformed(string input)
        {
            input.FromBase64().IsLeft.Should().BeTrue();
        }

        [Fact]
        public void FromBase64_Throws_OnInvalidInput()
        {
            string? input = null;
            Assert.Throws<ArgumentNullException>(() => input!.FromBase64());
        }
    }
}
