# Testing Strategy
Unit tests are good, but they are not about testing in broad sense, and they are expensive from dev effort perspective, especially for the dynamic project.

Automated Integration Tests are good too, and they test nearly e2e scenarious, but execution time is consuming, require extra effort to setup\maintain environment, infrastructure, need permanent care.

This project can't afford neither of above on the current stage.

Idea is to rely on Component Tests that's between UT and IT - something that can be executed in-process, don't need environment\infra setup, but at the same time test integration of units. Component tests use mocks for infrastructure code (so don't work with real storages, etc), however instantiate whole chain of dependencies (classes), so can test whole piece of logic.

So, strategy can be described as:
* Use Unit Tests for units with complex algorithms only
* Main happy paths and main negative cases are tested with Component Tests
* Integration tests are manual (semi-automated) testing using JMeter, or Rest Client. Later will be substituted by fully automated tests
