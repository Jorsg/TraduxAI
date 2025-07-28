# AI Document Processor

A comprehensive document processing solution that leverages OpenAI's powerful APIs to extract text from images, convert PDFs to text, and translate content between multiple languages. Built with a modern .NET architecture featuring a Web API backend and Blazor Server frontend.

## 🚀 Features

- **Image to Text (OCR)**: Extract text from images using AI-powered optical character recognition
- **PDF to Text**: Convert PDF documents to editable text format
- **Text Translation**: Translate text between multiple languages (Spanish ↔ English and more)
- **Real-time Processing**: Live updates during document processing
- **Responsive UI**: Modern, mobile-friendly interface

## 🛠️ Tech Stack

### Backend
- **.NET 9.0** - Core framework
- **ASP.NET Core Web API** - RESTful API services
- **Entity Framework Core** - ORM for data persistence
- **MongDB** - Database (LocalDB for development)
- **HttpClient** - HTTP communication

### Frontend
- **Blazor Server** - Interactive web UI framework
- **Bootstrap 5** - CSS framework for responsive design
- **JavaScript Interop** - Browser API integration

### External Services
- **OpenAI API** - AI-powered text processing
  - GPT-4 for text translation and processing
  - GPT-4 Vision for image text extraction

### Architecture & Patterns
- **Clean Architecture** - Separation of concerns
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - IoC container
- **SOLID Principles** - Object-oriented design principles
- **RESTful API Design** - Standard HTTP methods and status codes

### Development Tools
- **Visual Studio 2022** / **VS Code** - IDE
- **Git** - Version control
- **NuGet** - Package management

## 🏗️ Project Structure

```
TraduxAI/
├── 📁 TraduxAI.Server/                    # Web API Backend
│   ├── 📁 Controllers/                   # API endpoints
│   ├── 📄 Program.cs                     # Application entry point
│   └── 📄 appsettings.json               # Configuration
│
├── 📁 TraduxAI.Core/                     # Business Logic Layer
│   ├── 📁 Repositorio/                        # Domain models & DTOs
│   ├── 📁 Interfaces/                    # Service contracts
│   ├── 📁 Services/                      # Business logic implementation
│   
│
├── 📁 TraduxAI.Share/                    # Data Access Layer
│   ├── 📁 DbContext/                     # Entity Framework context
│   ├── 📁 Repositories/                  # Repository implementations
│   └── 📁 DTOs/                          # Data Transformation Object
    └── 📁 Errors/                        # Erros handle
│
├── 📁 TraduxAI.Client/                   # Blazor Server Frontend
│   ├── 📁 Pages/                         # Blazor pages/components
│   ├── 📁 Shared/                        # Shared UI components
│   ├── 📁 Services/                      # Client-side services
│   └── 📁 wwwroot/                       # Static web assets
│
```

## 🚦 Getting Started

2. **Configure JWTSettings**
   
   "JwtSettings": {
    "Issuer": "",
    "Audience": "",
    "ExpiryMinutes": 
  },
   ```


3. **Build the solution**
   ```bash
   dotnet build
   ```

5. **Run the applications**
   
   Terminal 1 (API):
   ```bash
   cd TraduxAI.API
   dotnet run
   ```
   
   Terminal 2 (Client):
   ```bash
   cd TraduxAI.Client
   dotnet run
   ```

6. **Access the applications**
   - API: https://localhost:7001
   - Client: https://localhost:7100


## 📋 Roadmap

- [ ] **v1.1**: User authentication and authorization
- [ ] **v1.2**: Batch processing capabilities
- [ ] **v1.3**: Additional file format support (Word, Excel)
- [ ] **v1.4**: Advanced OCR with layout preservation
- [ ] **v1.5**: Multi-language UI support
- [ ] **v2.0**: Machine learning model training for custom documents

## 🔒 Security

- API keys are stored securely in configuration
- Input validation on all endpoints
- Error messages don't expose sensitive information

