#Product performance
Below experiments intendent to proof technology choice and justificate possibility to use application on production environment. Tests on cheapest environments will come later. Experimennts explan latency difference between calling app directly and calling via Gateway, but not real performannce\load of the solution.
## Experiment 1
Measure cheap pre-production and production-grade configuration. Key parts:

**Gateway service**: Protects HTTPs calls to endpoint with API key. Once API key is checked, call passed with http (TLS Termination) to protected endpoint (Echo Service). As a "Consumer action" gateway adds custom header into response.

**Echo service**: Service under Gateway protection. Configured to respond predefined message ~3kb with 300 ms delay (mimics some workload) 

**Jmeter script**: plays role of client. It calls endpoint with ~1kb payload. URL path ending is auto generated, to reduce possible caching side effects. Only one api key is used, so potentially it could be subject for caching on Azure Storage Account side. 200 threads, Loop count: 200 

### Results:

Scenario A: Jmeter calls via HTTPs Echo Service directly. Numbers are used as a performance base line, and overal measure of whole infrastructure overhead.

Scenario B: Jmeter execute HTTPs calls via Gateway. Gateway is hosted on 3 instances (smaller and cheaper production grade infra)

Scenario C: Same as scenario B, but Gateway hosted on 1 instance. Kind of PPE environment

| Scenario | Duration(s) | Median(ms) | P90(ms) | P95(ms) | P99(ms) | Throughput(req/s) |
|--------------|-----|------|---------|--------|----------|----------|
| Scenario A | 64 | 306 | 310 | 312 | 324 | 625 |
| Scenario B | 69 | 330 | 342 | 349 | 368 | 579 |
| Scenario C | 77 | 359 | 412 | 436 | 511 | 519 |

### Environment

Azure region: West Europe

Azure Storage Account, Kind: V2

App service plans: SKU P0V3, Linux. 

Api Gateway and Echo Service deployed to own app service plans, deployed with docker image

Apps deployed with ```az webapp deploy ...```
