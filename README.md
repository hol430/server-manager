# APSIM Server Manager

Web API for managing [apsim server](https://apsimnextgeneration.netlify.app/usage/server/) instances. This project contains a rest api with one endpoint, and a web page which wraps the API for convenience. The endpoint asks for a single .apsimx file, and will start an apsim server instance with this .apsimx file. Any existing apsim servers will be killed when this happens.

## Remarks

This tool is fairly clunky at the moment. Each apsim server is started as a separate process, because the apsim server implementation does not support cancellation at all. As a result, we run each apsim server as a process and kill it when we want to start a new one. This approach works well with docker; each apsim server runs in the same container as the server manager, so killing the docker container will kill any apsim server processes spawned by the server manager. When debugging outside of docker, you'll need to verify that any apsim-servers spawned by the server manager are also killed. They should be killed when killing the server manager at the console by Ctrl+C, but **not** when a debug session is aborted in vscode.

The apsim server could be refactored to support cancellation, in which case we could do away with separate processes altogether, but in the longer run, it may be better to integrate this tool with the [bootstrapper](https://github.com/hol430/APSIM.Bootstrapper), in order to create each apsim server as a container/pod running in the cluster. Each apsim server in this scenario could be a relay server in order to spread the load more evenly across the cluster.

Should also write some unit tests for this at some point. There's not much code at the moment, but that means that writing tests shouldn't be much work!

## Deployment

The repository contains a dockerfile and docker-compose file. To deploy the website, login to the production server, clone/navigate to this repository, and run:

```bash
cd docker
docker-compose build --no-cache
docker-compose down # (if container is already running)
docker-compose up -d
```
