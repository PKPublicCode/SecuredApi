# Product performance
Below experiments intended to proof technology choice and justify possibility to use application on production environment. Tests on cheapest environments will come later. Experiments explain latency difference between calling app directly and calling via Gateway, but not real performance\load of the solution.

## Experiment 1
Measure cheap pre-production and production-grade configuration. Key parts:

### Setup
#### Gateway service:
Protects HTTPs calls to endpoint with API key. Once API key is checked, call passed with http (TLS Termination) to protected endpoint (Echo Service). As a "Consumer action" gateway adds custom header into response.

#### Echo service: 
Service under Gateway protection. Configured to respond predefined message ~3kb with 300 ms delay (mimics some workload).
**Important:** Task.Delay method is not stable on linux, and sometimes can be faster then specified timeout. So 300ms delay is very rough number.


#### Jmeter script:
Plays role of client. It calls endpoint with ~1kb payload. URL path ending is auto generated to reduce possible caching side effects. For api key authentication only one api key is used, so potentially it could be subject for caching on Azure Storage Account side. For JWT (Entra) authentication same token (so same SPI) is used for all calls. Scenario uses 200 threads, loop count 200 

### Results:

Scenario A: Jmeter calls via HTTPs Echo Service directly. Numbers are used as a performance base line, and overall measure of whole infrastructure overhead. Hosted on 1 instance

#### Api key authentication:
Scenario B: Jmeter execute HTTPs calls via Gateway. Gateway is hosted on 3 instances

Scenario C: Same as scenario B, but Gateway hosted on 1 instance. Kind of PPE environment

#### Jwt (Entra) authentication
Scenario D: Jmeter execute HTTPs calls via Gateway. Gateway is hosted on 3 instances

Scenario E: Same as scenario D, but Gateway hosted on 1 instance. Kind of PPE environment

| Scenario | Duration(s) | Median(ms) | P90(ms) | P95(ms) | P99(ms) | Throughput(req/s) |
|--------------|-----|------|---------|--------|----------|----------|
| Scenario A | 63 | 305 | 310 | 313 | 326 | 615 |
| Scenario B | 69 | 325 | 333 | 337 | 356 | 580 |
| Scenario C | 72 | 332 | 355 | 382 | 467 | 571 |
| Scenario D | 65 | 309 | 315 | 318 | 330 | 615 |
| Scenario E | 66 | 312 | 318 | 323 | 340 | 606 |

### Environment

Azure region: West Europe

Azure Storage Account, Kind: V2

App service plans: SKU P0V3, Linux. Chosen as small and cheap production grade sku as possible

Api Gateway and Echo Service deployed to own app service plans, deployed with docker image
