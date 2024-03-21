# Testing Strategy
Unit tests are good and powerful tool to keep app well designed and properly functioning. However, with modern tech stacks and having properly designed application framework, application logic is implemented as set of small built-in blocks, that depend on each other and on the framework itself. Meaningful functional parts primarily depends on the DI and application configuration. In this case value of proper unit tests is reduced, but cost of their maintenance is decreased.

To address maintenance cost issues, this project introduces component tests. Components tests are in memory tests (same as Unit Tests), and so they are fast, flexible, with high potential of using mocking approaches. However they test units connected to meaningful scenario, and cover units, appropriate part of DI and application configuration. This approach decreases costs of tests maintenance if in case of design changes and is focused rather on functional testing, then testing of code itself. Due lack of resources this project rely on these types of tests.


#### Strategy:
* Unit Tests: Units and methods with complex logic or algorithms only
* Component Tests: Most of the scenarios, including happy path and negative test cases.
* Integration tests: Happy path with E2E scenarios, covering main infra-integration capabilities.
* Performance tests: Few main happypath scenarios on jmeter to bench-mark performance of the app deployed to cloud.

#### Naming convention:
##### Unit tests:
MethodName_TestCaseDetails_ExpectedBehavior

###### Component and Integration tests
Scenario_TestCaseDetails_ExpectedBehavior 
