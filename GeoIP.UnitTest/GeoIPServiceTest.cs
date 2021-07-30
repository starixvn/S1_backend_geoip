using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using GeoIP.DAL;
using GeoIP.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GeoIP.UnitTest
{
    [TestFixture]
    public class GeoIpServiceTest
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public async Task IP_from_TW_Should_return_forbidden_when_applied_ForbiddenCountries()
        {
            var forbiddenCountry = new ForbiddenCountries(new[] {"SE", "SG", "TR", "TW", "US", "VG"});
            var logger = Substitute.For<ILogger<GeoIpService>>();
            var file = Substitute.For<IFile>();
            var ipCountryDal = Substitute.For<IIpCountryDataLayers>();
            ipCountryDal.GetIpCountry(Arg.Is<uint>(618989922)).Returns(x => new IPCountry() {Code = "TW"});
            var geoIp = new GeoIpService(file, logger, ipCountryDal);
            geoIp.ApplyForbidden(forbiddenCountry);
            var country = await geoIp.Check("36.229.9.98");

            country.Forbidden.Should().Be(true);
        }
        
        [Test]
        public void Illegal_IP_should_handle_IPFormatException()
        {
            var logger = Substitute.For<ILogger<GeoIpService>>();
            var file = Substitute.For<IFile>();
            var ipCountryDal = Substitute.For<IIpCountryDataLayers>();
            ipCountryDal.GetIpCountry(Arg.Is<uint>(618989922)).Returns(x => new IPCountry() {Code = "TW"});
            var geoIp = new GeoIpService(file, logger, ipCountryDal);
            geoIp.Invoking(g => g.Check("61").Result
                    .Message.Should()
                    .BeEquivalentTo("IP format error"))
                .Should()
                .NotThrow<IPFormatException>();
        }

        [Ignore("RedundantArgumentMatcherException")]
        [Test]
        public void Should_write_log_if_any_exception_occured()
        {
            var logger = Substitute.For<ILogger<GeoIpService>>();
            var file = Substitute.For<IFile>();
            var ipCountryDal = Substitute.For<IIpCountryDataLayers>();
            var service = new GeoIpService(file, logger, ipCountryDal);

            service.Check("61");

            logger.Received().LogWarning(Arg.Any<IPFormatException>(), Arg.Any<string>());
        }
        
        [Test]
        public async Task IPv6_IP_should_return_not_support()
        {
            var ipv6 = "2001:288:12c4::1";
            var forbiddenCountries = new ForbiddenCountries(new List<string>
            {
                "TW",
                "HK"
            });

            var logger = Substitute.For<ILogger<GeoIpService>>();
            var file = Substitute.For<IFile>();
            var ipCountryDal = Substitute.For<IIpCountryDataLayers>();
            var service = new GeoIpService(file, logger, ipCountryDal);
            service.ApplyForbidden(forbiddenCountries);

            var country = await service.Check(ipv6);
            country.Code.Should()
                .BeEmpty();
            country.Message.Should()
                .Be("no support ipv6");
        }
        
        [Test]
        public async Task Empty_CountryCode_should_be_forbidden()
        {
            var ipv6 = "192.168.1.1";
            var forbiddenCountries = new ForbiddenCountries(new List<string>
            {
                "TW",
                "HK",
                "-"
            });

            var logger = Substitute.For<ILogger<GeoIpService>>();
            var file = Substitute.For<IFile>();
            var ipCountryDal = Substitute.For<IIpCountryDataLayers>();
            ipCountryDal.GetIpCountry(Arg.Is(3232235777)).Returns(x => new IPCountry() {Code = "-"});
            var service = new GeoIpService(file, logger, ipCountryDal);
            service.ApplyForbidden(forbiddenCountries);

            var country = await service.Check(ipv6);
            country.Code.Should().Be("-");
            country.Forbidden.Should()
                .Be(true);
        }
        [Test]
        public async Task Taipei_Office_IP_should_be_allowed()
        {
            var forbiddendCountries = new ForbiddenCountries(new List<string>
            {
                "TW",
                ""
            });
            var allowedIPRange = new List<IPRange>()
            {
                new IPRange
                {
                    Description = "ailab VPN",
                    To = 2112066662,
                    From = 2112066662
                }
            };
            var whiteList = new AllowedAreas(allowedIPRange);
            var twIP = "125.227.148.102";

            var logger = Substitute.For<ILogger<GeoIpService>>();
            var file = Substitute.For<IFile>();
            var ipCountryDal = Substitute.For<IIpCountryDataLayers>();
            ipCountryDal.GetIpCountry(Arg.Is<uint>(2112066662)).Returns(x => new IPCountry() {Code = "TW"});
            var service = new GeoIpService(file, logger, ipCountryDal);
            service.ApplyForbidden(forbiddendCountries);
            service.ApplyAllowedAreas(whiteList);

            var country = await service.Check(twIP);

            country.Code.Should()
                .Be("TW");
            country.Forbidden.Should()
                .Be(false);
        }
        
        [Test]
        public void Load_whitelist_file_should_return_AllowedAreas()
        {
            var logger = Substitute.For<ILogger<GeoIpService>>();
            var file = Substitute.For<IFile>();
            var ipRanges = new List<IPRange>()
            {
                new IPRange
                {
                    Description = "ailab VPN",
                    To = 2112066662,
                    From = 2112066662
                }
            };

            file.Read(Arg.Any<string>())
                .Returns(ipRanges);
            var ipCountryDal = Substitute.For<IIpCountryDataLayers>();
            ipCountryDal.GetIpCountry(Arg.Is<uint>(2112066662)).Returns(x => new IPCountry() {Code = "TW"});
            var service = new GeoIpService(file, logger, ipCountryDal);
            string whitelistFile = "./Data/IPInfo_WhiteList.csv";
            var allowedAreas = service.LoadCsv(whitelistFile);

            file.Received(1)
                .Read(Arg.Any<string>());
            allowedAreas.Exists(2112066662)
                .Should()
                .Be(true);
        }
    }
}