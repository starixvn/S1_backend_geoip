using System;
using System.Collections.Generic;

using FluentAssertions;

using GeoIP.Core;
using GeoIP.Core.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

namespace GeoIP.UnitTest
{
    [TestFixture]
    public class FileTest
    {
        [Test]
        public void LoadCsv_succeed()
        {
            var provider = Substitute.For<IServiceProvider>();

            provider.GetService<ILoggerFactory>()
                .Returns(Substitute.For<ILoggerFactory>());
            IFile file = new FileLoader(provider);

            var actual = file.Read("./Data/IPInfo_WhiteList.csv");

            var expectation = new List<IPRange>() { new IPRange() { Description = "辦公室VPN 125.227.148.102", To = 2112066657, From = 2112066657 } };

            actual.Should().BeEquivalentTo(expectation);
        }

        [Test]
        public void Should_ignore_empty_value()
        {
            var provider = Substitute.For<IServiceProvider>();

            provider.GetService<ILoggerFactory>()
                .Returns(Substitute.For<ILoggerFactory>());
            IFile file = new FileLoader(provider);

            var actual = file.Read("./Data/IPInfo_WhiteList_with_EmptyValue.csv");

            var expectation = new List<IPRange>
                {new IPRange() {Description = "辦公室VPN 125.227.148.102", To = 2112066657, From = 2112066657}};

            actual.Should().BeEquivalentTo(expectation);
        }

        [Test]
        public void Should_ignore_duplicate_value()
        {
            var provider = Substitute.For<IServiceProvider>();

            provider.GetService<ILoggerFactory>()
                .Returns(Substitute.For<ILoggerFactory>());
            IFile file = new FileLoader(provider);

            var actual = file.Read("./Data/IPInfo_WhiteList_with_DuplicatedIPs.csv");

            var expectation = new List<IPRange>
                {new IPRange() {Description = "辦公室VPN 125.227.148.102", To = 2112066657, From = 2112066657}};

            actual.Should().BeEquivalentTo(expectation);
        }
    }
}