# IpLocations.Api

## 關於
主要功能是使用IPV4查出對應的國家

## 資料
- [圖資](https://lite.ip2location.com/ip2location-lite)


## 未來
- 增加IPV6



## 效能
### IpToInt
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
13th Gen Intel Core i9-13900H, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.302
  [Host]   : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 8.0 : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0

| Method | Mean       | Error     | StdDev    | Gen0   | Allocated |
|------- |-----------:|----------:|----------:|-------:|----------:|
| Default |  0.2392 ns | 0.0281 ns | 0.0402 ns |      - |         - |
| LinqWithAggregate | 10.1143 ns | 0.2257 ns | 0.5186 ns | 0.0025 |      32 B |
| LeftShitOperator |  0.1970 ns | 0.0251 ns | 0.0235 ns |      - |         - |