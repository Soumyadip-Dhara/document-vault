# Document Vault

A secure document management system built with ASP.NET Core and Angular.

## Project Structure

```
document-vault/
│
├── documentvaultapi/        # ASP.NET Core Web API Backend
├── documentvaultui/         # Angular Frontend Application
├── docker/                  # Docker Configuration
│   └── docker-compose.yml   # Docker Compose Orchestration
│
└── README.md               # This file
```

## Technology Stack

### Backend (documentvaultapi)
- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **API Type**: RESTful Web API

### Frontend (documentvaultui)
- **Framework**: Angular (Latest)
- **Language**: TypeScript
- **Styling**: CSS

### Infrastructure
- **Containerization**: Docker
- **Orchestration**: Docker Compose
- **Web Server**: Nginx (for Angular app)

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v20 or later)
- [npm](https://www.npmjs.com/) (comes with Node.js)
- [Docker](https://www.docker.com/get-started) and Docker Compose (optional, for containerized deployment)

## Getting Started

### Running Locally

#### Backend API

```bash
cd documentvaultapi
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5000` or `https://localhost:5001`

#### Frontend UI

```bash
cd documentvaultui
npm install
npm start
```

The UI will be available at `http://localhost:4200`

### Running with Docker

Build and run both services using Docker Compose:

```bash
cd docker
docker-compose up --build
```

Services will be available at:
- **API**: http://localhost:5000
- **UI**: http://localhost:4200

To run in detached mode:

```bash
docker-compose up -d
```

To stop the services:

```bash
docker-compose down
```

## Development

### Backend Development

```bash
cd documentvaultapi
dotnet build          # Build the project
dotnet test           # Run tests
dotnet watch run      # Run with hot reload
```

### Frontend Development

```bash
cd documentvaultui
npm run build         # Build for production
npm test              # Run tests
npm run lint          # Lint the code
```

## API Documentation

Once the API is running, you can access the Swagger/OpenAPI documentation at:
- http://localhost:5000/swagger

## Architecture

This application follows a client-server architecture:

- **Frontend (Angular)**: Single Page Application (SPA) that provides the user interface
- **Backend (ASP.NET Core)**: RESTful API that handles business logic and data management
- **Communication**: HTTP/HTTPS using JSON for data exchange

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

For questions or support, please open an issue in the repository.