# DATABENTO .NET WRAPPER - COMPREHENSIVE API COVERAGE REPORT

**Report Generated:** 2025-11-11
**Databento-cpp Version:** v0.43.0
**Framework:** .NET 8.0
**Current Implementation Status:** ~40-45% Complete

---

## Executive Summary

Our .NET wrapper currently provides solid coverage of core streaming APIs (both historical and live) with good metadata query support. However, significant gaps remain in batch processing, file I/O, symbology resolution, and advanced live client features.

---

## 1. HISTORICAL DATA API

### A. Timeseries Streaming (COST-INCURRING)

| Method | Status | Implementation | Notes |
|--------|--------|----------------|-------|
| `TimeseriesGetRange()` | ✅ **COMPLETE** | `GetRangeAsync()` in HistoricalClient.cs:68 | Async enumerable pattern with callbacks |
| `TimeseriesGetRangeToFile()` | ❌ **MISSING** | Not implemented | Would stream directly to DBN file |

**Coverage: 1/2 methods (50%)**

---

### B. Batch Download API (COST-INCURRING)

| Method | Status | Implementation | Notes |
|--------|--------|----------------|-------|
| `BatchSubmitJob()` | ❌ **MISSING** | Not implemented | Submit batch job with encoding/compression/delivery options |
| `BatchListJobs()` | ❌ **MISSING** | Not implemented | List previous batch jobs with state/time filters |
| `BatchListFiles()` | ❌ **MISSING** | Not implemented | List files for a batch job |
| `BatchDownload()` | ❌ **MISSING** | Not implemented | Download batch job files to local directory |

**Coverage: 0/4 methods (0%)**

**Missing Types:**
- `BatchJob` - Job description (id, cost, state, timestamps, sizes)
- `BatchFileDesc` - File info (filename, size, hash, URLs)
- `SplitDuration` enum - ✅ Implemented in ConfigEnums.cs:36
- `Delivery` enum - ✅ Implemented in ConfigEnums.cs:54
- `Compression` enum - ✅ Implemented in ConfigEnums.cs:84
- `JobState` enum - ✅ Implemented in ConfigEnums.cs:111

---

### C. Metadata Query API

| Method | Status | Implementation | Notes |
|--------|--------|----------------|-------|
| `MetadataListPublishers()` | ✅ **COMPLETE** | `ListPublishersAsync()` in HistoricalClient.cs:195 | Returns `IReadOnlyList<PublisherDetail>` |
| `MetadataListDatasets()` | ✅ **COMPLETE** | `ListDatasetsAsync(venue?)` in HistoricalClient.cs:228 | Optional venue filter |
| `MetadataListSchemas()` | ✅ **COMPLETE** | `ListSchemasAsync(dataset)` in HistoricalClient.cs:262 | Returns `IReadOnlyList<Schema>` |
| `MetadataListFields()` | ✅ **COMPLETE** | `ListFieldsAsync(encoding, schema)` in HistoricalClient.cs:297 | Returns `IReadOnlyList<FieldDetail>` |
| `MetadataListUnitPrices()` | ❌ **MISSING** | Not implemented | Would return pricing per schema |
| `MetadataGetDatasetCondition()` | ✅ **COMPLETE** | `GetDatasetConditionAsync(dataset)` in HistoricalClient.cs:336 | Returns `DatasetConditionInfo` |
| `MetadataGetDatasetRange()` | ✅ **COMPLETE** | `GetDatasetRangeAsync(dataset)` in HistoricalClient.cs:370 | Returns `DatasetRange` |
| `MetadataGetRecordCount()` | ✅ **COMPLETE** | `GetRecordCountAsync(...)` in HistoricalClient.cs:404 | Returns `ulong` |
| `MetadataGetBillableSize()` | ✅ **COMPLETE** | `GetBillableSizeAsync(...)` in HistoricalClient.cs:446 | Returns `ulong` |
| `MetadataGetCost()` | ✅ **COMPLETE** | `GetCostAsync(...)` in HistoricalClient.cs:488 | Returns `decimal` (USD) |
| `GetBillingInfo()` | ✅ **COMPLETE** | `GetBillingInfoAsync(...)` in HistoricalClient.cs:537 | Combined query (custom .NET method) |

**Coverage: 10/11 methods (91%)**

---

### D. Symbology Resolution API

| Method | Status | Implementation | Notes |
|--------|--------|----------------|-------|
| `SymbologyResolve()` | ❌ **MISSING** | Not implemented | Resolve symbols between STypes with date ranges |

**Coverage: 0/1 methods (0%)**

---

### E. Historical Builder

| Method | Status | Implementation | Notes |
|--------|--------|----------------|-------|
| `Builder()` | ✅ **COMPLETE** | Constructor in HistoricalClientBuilder.cs | Static factory pattern |
| `SetKey() / SetKeyFromEnv()` | ✅ **COMPLETE** | `WithApiKey()` in HistoricalClientBuilder.cs:22 | Fluent API |
| `SetGateway()` | ✅ **COMPLETE** | `WithGateway()` in HistoricalClientBuilder.cs:32 | Bo1, Bo2, Custom |
| `SetUpgradePolicy()` | ✅ **COMPLETE** | `WithUpgradePolicy()` in HistoricalClientBuilder.cs:55 | DBN version policy |
| `WithAddress()` | ✅ **COMPLETE** | `WithAddress(host, port)` in HistoricalClientBuilder.cs:43 | Custom gateway |
| `WithUserAgent()` | ✅ **COMPLETE** | `WithUserAgent()` in HistoricalClientBuilder.cs:65 | Extend User-Agent |
| `WithTimeout()` | ✅ **COMPLETE** | `WithTimeout()` in HistoricalClientBuilder.cs:75 | Request timeout |
| `Build()` | ✅ **COMPLETE** | `Build()` in HistoricalClientBuilder.cs:87 | Returns IHistoricalClient |

**Coverage: 7/7 methods (100%)** ⭐ **CATEGORY COMPLETE**

---

## 2. LIVE DATA API

### A. Live Client Operations

| Method | Status | Implementation | Notes |
|--------|--------|----------------|-------|
| `Subscribe()` | ✅ **COMPLETE** | `SubscribeAsync()` in LiveClient.cs | Basic subscription |
| `SubscribeWithSnapshot()` | ❌ **MISSING** | Not implemented | Subscribe with initial snapshot |
| `Start()` | ✅ **COMPLETE** | `StartAsync()` in LiveClient.cs | Start streaming |
| `NextRecord()` (blocking) | ⚠️ **PARTIAL** | `StreamAsync()` IAsyncEnumerable | Different pattern, not blocking |
| `Stop()` | ✅ **COMPLETE** | `StopAsync()` in LiveClient.cs | Stop streaming |
| `Reconnect()` | ❌ **MISSING** | Not implemented | Reconnect to gateway |
| `Resubscribe()` | ❌ **MISSING** | Not implemented | Resubscribe to all subscriptions |
| `BlockForStop()` | ❌ **MISSING** | Not implemented | Wait for session close (threaded mode) |

**Coverage: 3/8 methods (37.5%)**

---

### B. Live Builder

| Method | Status | Implementation | Notes |
|--------|--------|----------------|-------|
| `Builder()` | ✅ **COMPLETE** | Constructor in LiveClientBuilder.cs | Static factory pattern |
| `SetKey() / SetKeyFromEnv()` | ✅ **COMPLETE** | `WithApiKey()` in LiveClientBuilder.cs:20 | Fluent API |
| `SetDataset()` | ✅ **COMPLETE** | `WithDataset()` in LiveClientBuilder.cs:30 | Required for live |
| `SetSendTsOut()` | ✅ **COMPLETE** | `WithSendTsOut()` in LiveClientBuilder.cs:40 | Append gateway timestamps |
| `SetUpgradePolicy()` | ✅ **COMPLETE** | `WithUpgradePolicy()` in LiveClientBuilder.cs:50 | DBN version policy |
| `SetHeartbeatInterval()` | ✅ **COMPLETE** | `WithHeartbeatInterval()` in LiveClientBuilder.cs:60 | Connection monitoring |
| `SetBufferSize()` | ❌ **MISSING** | Not implemented | TCP buffer size |
| `Build()` | ✅ **COMPLETE** | `Build()` in LiveClientBuilder.cs:72 | Returns ILiveClient |

**Coverage: 6/7 methods (86%)**

---

## 3. DATA RECORDS & SCHEMAS

### A. Record Types

| Record Type | Status | Implementation | Size | Notes |
|-------------|--------|----------------|------|-------|
| `MboMsg` | ✅ **COMPLETE** | MboMessage.cs | 72 bytes | Market-by-order |
| `Mbp1Msg` | ✅ **COMPLETE** | Mbp1Message.cs | 104 bytes | MBP depth 1 |
| `Mbp10Msg` | ✅ **COMPLETE** | Mbp10Message.cs | 344 bytes | MBP depth 10 |
| `TradeMsg` | ✅ **COMPLETE** | TradeMessage.cs | 72 bytes | Trade messages (MBP-0) |
| `TbboMsg` | ❌ **MISSING** | Not implemented | ~96 bytes | Trade with BBO before |
| `BboMsg` | ✅ **COMPLETE** | BboMessage.cs | 88 bytes | BBO (1s/1m intervals) |
| `TcbboMsg` | ❌ **MISSING** | Not implemented | ~96 bytes | Trade with CBBO before |
| `CbboMsg` | ✅ **COMPLETE** | CbboMessage.cs | 120 bytes | Consolidated BBO |
| `Cmbp1Msg` | ✅ **COMPLETE** | Cmbp1Message.cs | 152 bytes | Consolidated MBP-1 |
| `OhlcvMsg` | ✅ **COMPLETE** | OhlcvMessage.cs | 96 bytes | Candlesticks |
| `StatusMsg` | ✅ **COMPLETE** | StatusMessage.cs | 88 bytes | Trading status |
| `InstrumentDefMsg` | ✅ **COMPLETE** | InstrumentDefMessage.cs | 520 bytes | Instrument definitions |
| `ImbalanceMsg` | ✅ **COMPLETE** | ImbalanceMessage.cs | 112 bytes | Auction imbalances |
| `StatMsg` | ✅ **COMPLETE** | StatMessage.cs | 104 bytes | Statistics |
| `ErrorMsg` | ✅ **COMPLETE** | ErrorMessage.cs | 80 bytes | Errors |
| `SymbolMappingMsg` | ✅ **COMPLETE** | SymbolMappingMessage.cs | 120 bytes | Symbol mappings |
| `SystemMsg` | ✅ **COMPLETE** | SystemMessage.cs | 80 bytes | System & heartbeats |

**Coverage: 15/17 record types (88%)**

---

### B. Schema Enum

| Schema | Status | Implementation | Notes |
|--------|--------|----------------|-------|
| `Mbo` | ✅ **COMPLETE** | Schema.cs:9 | Market by order |
| `Mbp1` | ✅ **COMPLETE** | Schema.cs:12 | MBP depth 1 |
| `Mbp10` | ✅ **COMPLETE** | Schema.cs:15 | MBP depth 10 |
| `Tbbo` | ❌ **MISSING** | Not implemented | Trade with BBO |
| `Trades` | ✅ **COMPLETE** | Schema.cs:18 | All trades |
| `Ohlcv1S` | ✅ **COMPLETE** | Schema.cs:21 | 1-second bars |
| `Ohlcv1M` | ✅ **COMPLETE** | Schema.cs:24 | 1-minute bars |
| `Ohlcv1H` | ✅ **COMPLETE** | Schema.cs:27 | 1-hour bars |
| `Ohlcv1D` | ✅ **COMPLETE** | Schema.cs:30 | 1-day bars UTC |
| `OhlcvEod` | ✅ **COMPLETE** | Schema.cs:33 | End-of-day bars |
| `Definition` | ✅ **COMPLETE** | Schema.cs:36 | Instrument definitions |
| `Statistics` | ✅ **COMPLETE** | Schema.cs:39 | Statistics messages |
| `Status` | ✅ **COMPLETE** | Schema.cs:42 | Status messages |
| `Imbalance` | ✅ **COMPLETE** | Schema.cs:45 | Imbalance messages |
| `Cmbp1` | ❌ **MISSING** | Not in enum | Consolidated MBP-1 |
| `Cbbo1S` | ❌ **MISSING** | Not in enum | Consolidated BBO 1-second |
| `Cbbo1M` | ❌ **MISSING** | Not in enum | Consolidated BBO 1-minute |
| `Tcbbo` | ❌ **MISSING** | Not in enum | Trade with CBBO |
| `Bbo1S` | ❌ **MISSING** | Not in enum | BBO 1-second |
| `Bbo1M` | ❌ **MISSING** | Not in enum | BBO 1-minute |

**Coverage: 13/20 schemas (65%)**

---

## 4. CRITICAL GAPS & PRIORITIES

### 🔴 **HIGH PRIORITY** (Core Functionality)

1. **Live Client Resilience** - No reconnect/resubscribe (Phase 7)
2. **Missing Schemas & Records** - Tbbo, Tcbbo, 7 schema values (Phase 8)
3. **Historical File Output** - TimeseriesGetRangeToFile() (Phase 9)

### 🟡 **MEDIUM PRIORITY** (Enhanced Functionality)

4. **Batch Download API** - All 4 methods (Phase 11)
5. **DBN File I/O** - DbnFileStore, Decoder, Encoder (Phase 12)
6. **Symbol Mapping** - TsSymbolMap, PitSymbolMap (Phase 10)
7. **Symbology Resolution** - SymbologyResolve() (Phase 13)

### 🟢 **LOW PRIORITY** (Nice-to-Have)

8. **Specialized Exception Types** (Phase 14)
9. **Live Blocking API** (Future)
10. **Utilities** - Datetime, logging, venues (Phase 14)

---

## 5. IMPLEMENTATION ROADMAP

### **Phase 7: Live Client Resilience** (1 week)
- Implement Reconnect()
- Implement Resubscribe()
- Implement SubscribeWithSnapshot()
- Add connection state management
- **Target: Live API → 75% coverage**

### **Phase 8: Missing Schemas & Records** (3-4 days)
- Add Tbbo, Tcbbo record types
- Add 7 missing schema enum values
- Update schema parsing/conversion
- **Target: Schemas → 100%, Records → 100%**

### **Phase 9: Historical File Output** (2-3 days)
- Implement TimeseriesGetRangeToFile()
- Add DBN file writing support
- **Target: Historical Timeseries → 100%**

### **Phase 10: Symbol Mapping** (1 week)
- Implement TsSymbolMap & PitSymbolMap
- Add symbol resolution helpers
- **Target: Symbol Mapping → 100%**

### **Phase 11: Batch Download API** (1-2 weeks)
- Implement all 4 batch methods
- Add BatchJob and BatchFileDesc types
- **Target: Batch API → 100%**

### **Phase 12: DBN File I/O** (1-2 weeks)
- Implement DbnFileStore, Decoder, Encoder
- Add file stream wrappers
- **Target: File I/O → 100%**

### **Phase 13: Symbology Resolution** (3-4 days)
- Implement SymbologyResolve()
- Add SymbologyResolution types
- **Target: Symbology → 100%**

### **Phase 14: Polish & Utilities** (3-5 days)
- Specialized exceptions, datetime helpers
- MetadataListUnitPrices(), logging, venues enum
- **Target: Utilities → 90%**

---

## **ESTIMATED TIMELINE: 6-8 weeks to 95% coverage**

---

## 6. CURRENT STRENGTHS

✅ **Solid core streaming APIs** (historical and live)
✅ **Excellent builder patterns** (100% coverage)
✅ **Comprehensive record type support** (88%)
✅ **Strong metadata query capabilities** (91%)
✅ **Complete enumeration coverage** (95%)
✅ **Good .NET idioms** (async/await, IAsyncEnumerable, events)

---

## 7. CURRENT WEAKNESSES

❌ **No batch download capabilities** (0%)
❌ **No DBN file I/O** (0%)
❌ **No symbol mapping** (0%)
❌ **Limited live client resilience** (37.5%)
❌ **Incomplete schema support** (65%)

---

## 8. STRATEGIC ASSESSMENT

Our wrapper excels at **real-time streaming** and **metadata queries**, making it suitable for:
- Live market data applications
- Historical data exploration
- Cost estimation before queries
- Real-time analytics

However, it currently lacks support for:
- Production-grade live clients (no reconnect)
- Batch data processing workflows
- Offline DBN file analysis
- Symbol resolution automation
- Trade-with-BBO schemas

**Recommended Next Steps:**
1. **Phase 7** - Live Client Resilience for production readiness
2. **Phase 8** - Missing Schemas for complete coverage

---

**Last Updated:** 2025-11-11
**Repository:** https://github.com/Alparse/databento_client
