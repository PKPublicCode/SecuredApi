## Build
cd %SolutionDir%
#### Gateway
docker build -t pkruglov/securedapi.gateway:0.1 -t pkruglov/securedapi.gateway:latest -f ../Build/Docker/Gateway/dockerfile .
docker push pkruglov/securedapi.gateway --all-tags 

docker run -p 80:80 --name securedapi.gateway securedapi.gateway

#### Echo
docker build -t pkruglov/test:gateway.echo.latest -f ..\Build\Gateway.Echo\dockerfile .
docker push pkruglov/test:gateway.echo.latest 