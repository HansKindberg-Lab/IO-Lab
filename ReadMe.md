# IO-Lab

Web-application with the function to look at IO, directories and files. That way we can investigate directories and files when running in a container.

![.github/workflows/Docker-deploy.yml](https://github.com/HansKindberg-Lab/IO-Lab/actions/workflows/Docker-deploy.yml/badge.svg)

Web-application, without configuration, pushed to Docker Hub: https://hub.docker.com/r/hanskindberg/io-lab

## Important

This application is only for investigating, laborating and testing. To expose this application may be a security-risk. The user can send any path, browse any directory and read any file.

You can read about the security-risks here:
- [CA3003: Review code for file path injection vulnerabilities](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca3003)