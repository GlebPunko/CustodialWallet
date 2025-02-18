# CustodialWallet
### This tutorial will help you run the CustodialWallet Web API application either through Visual Studio or Docker by using command line.
## Launch via Visual Studio
1. Clone the repository:
`git clone https://github.com/GlebPunko/CustodialWallet.git`
2. Open the solution in Visual Studio.
3. In Visual Studio, press F5 or select Debug > Start Debugging. You should choose `docker-compose` as solution to run.
4. The application will run and a browser will automatically open with Swagger UI at https://localhost:8081/swagger.
5. Use Swagger UI to test the API.
## Launch via Docker by using command line
1. Clone the repository:
`git clone https://github.com/GlebPunko/CustodialWallet.git`
2. Go to the root folder of the project, where the docker-compose.yaml file is located:
`cd <path-to-root-folder>`
3. Build and run containers:
- `docker-compose build`
- `docker-compose up -d`
#### If you got exception that you can`t install postgres or pgAdmin images, use VPN for installing and building docker-compose.
4. Open Swagger UI:
Go to https://localhost:8081/swagger in your browser.
