﻿using IpLocations.Api.Constants;
using IpLocations.Api.Misc;
using Microsoft.AspNetCore.Mvc.Routing;
using StackExchange.Redis;
using System;
using System.Net.WebSockets;

namespace IpLocations.Api.Infra
{
    public class IpRangeStore
    {
        private readonly IDatabase _db;

        public IpRangeStore(IConnectionMultiplexer connectionMultiplexer)
        {
            _db = connectionMultiplexer.GetDatabase();
        }

        public async Task<string> GetCountryAsync(string ip)
        {
            var ipNum = IpConverter.IpToInt(ip);

            if (ipNum == -1) return string.Empty;

            var result = await _db.SortedSetRangeByScoreAsync(GetRedisKey(ipNum), double.NegativeInfinity, ipNum, Exclude.None, Order.Descending, 0, 1);

            if (result.Any())
            {
                var rangeInfo = result.First().ToString();
                var parts = rangeInfo.Split(':');
                var ips = parts[0].Split('-');
                var startIp = IpConverter.IpToInt(ips[0]);
                var endIp = IpConverter.IpToInt(ips[1]);

                if (startIp <= ipNum && ipNum <= endIp)
                {
                    return parts[1];
                }
            }

            return string.Empty;
        }

        public void WarmUp()
        {
            //TODO: 可以調整為檔案載入方式
            AddIpRange("1.160.0.0", "1.175.255.255", "TW");
            AddIpRange("100.42.20.0", "100.42.23.255", "CA");
            AddIpRange("1.0.1.0", "1.0.3.255", "CN");
            AddIpRange("1.0.32.0", "1.0.63.255", "CN");
        }

        private void AddIpRange(string startIp, string endIp, string country)
        {
            var startIpNum = IpConverter.IpToInt(startIp);
            _db.SortedSetAdd(GetRedisKey(startIpNum), $"{startIp}-{endIp}:{country}", startIpNum);
        }

        private string GetRedisKey(long value)
        {
            //Note: 最大數字 4294967295
            var part = value.ToString()
                    .PadLeft(10, '0')
                    .Substring(0, 3);

            return string.Format(RedisKeyConst.IpToCountryRange, part);
        }
    }
}