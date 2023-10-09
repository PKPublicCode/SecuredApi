#Product performance
Below experiments intendent to proof technology choice and justificate possibility to use application on production environment. Tests on cheapest environments will come later. Experimennts explan latency difference between calling app directly and calling via Gateway, but not real performannce\load of the solution.
## Experiment 1
Measure production-like configuration
### Environment
Azure region: West Europe

Azure Storage Account, Kind: V2

App service plans: Gateway: SKU P0V3, 3 instances, Echo: SKU P0V3, 1 instance

Api Gateway and Echo Service deployed to own app service plans.

Api Gateway accepts Https only, configured to forward calls to Echo Server by http (tls termination), calls protected by Api Key. Gateway url protected by api key.

Echo Service configured to respond pre-configured string after 300 ms delay (mimic some load)

Apps deployed with ```az webapp deploy ...```

JMeter client executed from Azure Load Test in west europe region, 200 threads, Loop count: 200 

Both Api Gateway and Echo Server are called with random suffix in url, to minimize caching impact. Request body ~1 kb

Only one subscription key (and so only one Consumer entity) is used during the experiments. Blobs configured with cache-control=no-store.

### Results

#### JMeter -> Api Gateway -> Echo Service
| Label        | # Samples | Average | Median | 90% Line | 95% Line | 99% Line | Min | Max   | Error % | Throughput | Received KB/sec | Sent KB/sec |
|--------------|-----------|---------|--------|----------|----------|----------|-----|-------|---------|------------|-----------------|-------------|
| HTTP Request | 20000     | 478     | 397    | 559      | 741      | 1098     | 344 | 18933 | 0.000%  | 331.21905  | 704.81          | 441.16      |
| TOTAL        | 20000     | 478     | 397    | 559      | 741      | 1098     | 344 | 18933 | 0.000%  | 331.21905  | 704.81          | 441.16      |

**Result**: 20'000 requests, duration <60 seconds, ~330 request per second.

#### JMeter -> Echo Service
| Label        | # Samples | Average | Median | 90% Line | 95% Line | 99% Line | Min | Max  | Error % | Throughput | Received KB/sec | Sent KB/sec |
|--------------|-----------|---------|--------|----------|----------|----------|-----|------|---------|------------|-----------------|-------------|
| HTTP Request | 20000     | 412     | 353    | 405      | 424      | 1194     | 326 | 9544 | 0.000%  | 436.76705  | 926.85          | 545.49      |
| TOTAL        | 20000     | 412     | 353    | 405      | 424      | 1194     | 326 | 9544 | 0.000%  | 436.76705  | 926.85          | 545.49      |

**Result**: 20'000 requests, duration 47 seconds, ~425 request per second.

Cost of Api Gateway is <100 ms latency in avarage.