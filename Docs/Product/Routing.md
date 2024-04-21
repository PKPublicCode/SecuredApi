# Routing

To understand logic behind the routing configuration purpose of the each action it's important to understand basics of SecuredAPI. 

SecuredAPI logic can be explained as following steps:
* As a first step gateway receives http(s) request from client (client request object).
* As a last step gateway sends back response to the client (client response object).

Between these two steps SecuredAPI executes operations that modify request object, populate and alter response object. Operations provided by SecuredAPI are called _actions_. Sequence of actions and their parameters depends on the path of the client's request URL and its method. Url path and method is called _route_. Sequence of actions for each route is called _route configuration_. Once last action is executed, SecuredAPI sends final state of the response object to the client.

In SecuredAPI actions can be split into two groups: fallible and infallible. Fallible actions implement conditions and in some circumstances 'fails' and stop the success execution sequence. Infallible doesn't have any conditions and has no effect to the further execution.

Routing configuration defines two action execution flows for every route: successful flow and error flow. Successful flow is a flow that SecuredAPI processing starts with. This is the happy path scenario and implements main gateway logic. If all actions are succeeded, once last action in success flow is completed, SecuredAPI returns response object to the client. If during success flow execution fallible actions identify error, successful flow stops and SecuredAPI starts executing error flow. Routing configuration has tree-like structure and so, when SecuredAPI switches to error flow, execution starts on the level where error occurred.

Read more about [Routing configuration](./RoutingConfiguration.md)

See list of [available actions](./Actions.md)

