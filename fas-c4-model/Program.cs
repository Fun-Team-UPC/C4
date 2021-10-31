using Structurizr;
using Structurizr.Api;
using Structurizr.Core.Util;
using System.Linq;

namespace fas_c4_model
{
    class Program
    {
        static void Main(string[] args)
        {
            Banking();
        }
        
        static void Banking()
        {
            const long workspaceId = 69480;
            const string apiKey = "cf0dbe78-8c8a-45c0-b0c3-ae22de02f062";
            const string apiSecret = "20828a8f-e85a-4bb5-9990-4e3d8079e645";

            StructurizrClient structurizrClient = new StructurizrClient(apiKey, apiSecret);
            Workspace workspace = new Workspace("ILanguage C4 Model - Sistema de Tutorías", "Sistema de Tutorías para aprender/perfeccionar algún idioma");
            Model model = workspace.Model;

            SoftwareSystem tutoringSystem = model.AddSoftwareSystem(Location.Internal, "Tutorias para aprender/perfeccionar algún idioma", "Permite la reserva de sesiones entre estudiante y tutor");
            SoftwareSystem zoom = model.AddSoftwareSystem("Zoom", "Plataforma que ofrece una REST API de videollamadas.");
            SoftwareSystem paypal = model.AddSoftwareSystem("Paypal", "Plataforma que el servicio de medios de pago");
            SoftwareSystem googleCalendar = model.AddSoftwareSystem("Google Calendar", "Permite transmitir información en tiempo real por el avión del vuelo a nuestro sistema");

            Person student = model.AddPerson("Estudiante", "Persona que busca aprender/mejorar algún idioma.");
            Person tutor = model.AddPerson("Tutor", "Tutor en busca de trabajo que ayuda a aprender/mejorar algún idioma.");
            Person admin = model.AddPerson(Location.Internal, "Admin", "Admin - Open Data.");


            student.Uses(tutoringSystem, "Realiza consultas para progresar en su aprendizaje de un idioma");
            tutor.Uses(tutoringSystem, "Realiza consultas para realizar su trabajo de tutor en un idioma");
            admin.Uses(tutoringSystem, "Realiza consultas a la REST API para mantenerse al tanto de los planes de suscripción y monitoreo del desenvolvimiento de los usuarios");

            tutoringSystem.Uses(googleCalendar, "Envía información para la reserva de un evento");
            tutoringSystem.Uses(zoom, "Se comunica para la programación de una reunión");
            tutoringSystem.Uses(paypal, "Se comunica para la realizacion de pagos en transacciones");

            ViewSet viewSet = workspace.Views;


            //---------------------------//---------------------------//
            // 1. System Context Diagram
            //---------------------------//---------------------------//

            SystemContextView contextView = viewSet.CreateSystemContextView(tutoringSystem, "Contexto", "Diagrama de contexto");
            contextView.PaperSize = PaperSize.A3_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            // Tags
            tutoringSystem.AddTags("SistemaMonitoreo");
            zoom.AddTags("Zoom");
            googleCalendar.AddTags("AircraftSystem");
            paypal.AddTags("Paypal");
            student.AddTags("Estudiante");
            tutor.AddTags("Tutor");
            admin.AddTags("Admin");

            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle("Estudiante") { Background = "#0a60ff", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Tutor") { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Admin") { Background = "#facc2e", Shape = Shape.Robot });
            styles.Add(new ElementStyle("SistemaMonitoreo") { Background = "#008f39", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("Zoom") { Background = "#90714c", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("Paypal") { Background = "#828412", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("AircraftSystem") { Background = "#2f95c7", Color = "#ffffff", Shape = Shape.RoundedBox });


            //---------------------------//---------------------------//
            // 2. Conteiner Diagram
            //---------------------------//---------------------------//

            Container mobileApplication = tutoringSystem.AddContainer("Mobile App", "Permite a los usuarios visualizar un dashboard con las funcionalidades que brinda la aplicación.", "Flutter");
            Container webApplication = tutoringSystem.AddContainer("Web App", "Permite a los usuarios visualizar un dashboard con las funcionalidades que brinda la aplicación.", "Flutter Web");
            Container landingPage = tutoringSystem.AddContainer("Landing Page", "", "Flutter Web");

            Container apiGateway = tutoringSystem.AddContainer("API Gateway", "API Gateway", "Spring Boot port 8080");
            Container messageBus = tutoringSystem.AddContainer("Bus de Mensajes en Cluster de Alta Disponibilidad", "Transporte de eventos del dominio.", "RabbitMQ");

            Container sessionContext = tutoringSystem.AddContainer("Session Bounded Context", "Bounded Context para gestión de sesiones", "Spring Boot port 8085");
            Container userContext = tutoringSystem.AddContainer("User Bounded Context", "Bounded Context para gestión de usuarios", "Spring Boot port 8081");
            Container subscriptionContext = tutoringSystem.AddContainer("Subscription Bounded Context", "Bounded Context para gestión de suscripciones", "Spring Boot port 8089");
            Container externalToolsContext = tutoringSystem.AddContainer("External Tools Bounded Context", "Bounded Context para gestión de herramientas externas", "Spring Boot port 8082");
            Container paymentContext = tutoringSystem.AddContainer("Payment Bounded Context", "Bounded Context para gestión de pagos", "Spring Boot port 8081");

            Container userContextDatabase = tutoringSystem.AddContainer("User Context DB", "", "MySQL");
            Container sessionContextDatabase = tutoringSystem.AddContainer("Session Context DB", "", "MySQL");
            Container subscriptionContextDatabase = tutoringSystem.AddContainer("Subscription Context DB", "", "MySQL");
            Container paymentContextDatabase = tutoringSystem.AddContainer("Payment Context DB", "", "MySQL");
            Container externalToolContextDatabase = tutoringSystem.AddContainer("External Tool Context DB", "", "MySQL");


            student.Uses(mobileApplication, "Consulta");
            student.Uses(webApplication, "Consulta");
            student.Uses(landingPage, "Consulta");

            tutor.Uses(mobileApplication, "Consulta");
            tutor.Uses(webApplication, "Consulta");
            tutor.Uses(landingPage, "Consulta");

            mobileApplication.Uses(apiGateway, "API Request", "JSON/HTTPS");
            webApplication.Uses(apiGateway, "API Request", "JSON/HTTPS");
            admin.Uses(apiGateway, "API Request", "JSON/HTTPS");

            apiGateway.Uses(sessionContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(userContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(subscriptionContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(externalToolsContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(paymentContext, "API Request", "JSON/HTTPS");

            sessionContext.Uses(sessionContextDatabase, "Lee desde y escribe hasta", "JDBC");
            sessionContext.Uses(messageBus, "Envia registro de acciones", "JDBC");

            userContext.Uses(userContextDatabase, "Lee desde y escribe hasta", "JDBC");
            userContext.Uses(messageBus, "Envia registro de acciones", "JDBC");

            subscriptionContext.Uses(subscriptionContextDatabase, "Lee desde y escribe hasta", "JDBC");
            subscriptionContext.Uses(messageBus, "Envia registro de acciones", "JDBC");

            paymentContext.Uses(paypal, "", "JDBC");
            paymentContext.Uses(paymentContextDatabase, "Lee desde y escribe hasta", "JDBC");
            paymentContext.Uses(messageBus, "Envia registro de acciones", "JDBC");

            externalToolsContext.Uses(googleCalendar, "Reserva de evento", "JSON");
            externalToolsContext.Uses(zoom, "Programa reunión", "JSON");
            externalToolsContext.Uses(externalToolContextDatabase, "Lee desde y escribe hasta", "JDBC");
            externalToolsContext.Uses(messageBus, "Envia registro de acciones", "JDBC");

            zoom.Uses(externalToolsContext, "Retorna link de la reunión", "JSON");

            // Tags
            mobileApplication.AddTags("MobileApp");
            webApplication.AddTags("WebApp");
            landingPage.AddTags("LandingPage");
            apiGateway.AddTags("APIGateway");
            messageBus.AddTags("MessageBus");

            sessionContext.AddTags("BoundedContext");
            userContext.AddTags("BoundedContext");
            subscriptionContext.AddTags("BoundedContext");
            externalToolsContext.AddTags("BoundedContext");
            paymentContext.AddTags("BoundedContext");

            userContextDatabase.AddTags("DataBase");
            sessionContextDatabase.AddTags("DataBase");
            subscriptionContextDatabase.AddTags("DataBase");
            paymentContextDatabase.AddTags("DataBase");
            externalToolContextDatabase.AddTags("DataBase");



            styles.Add(new ElementStyle("MobileApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.MobileDevicePortrait, Icon = "" });
            styles.Add(new ElementStyle("WebApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("LandingPage") { Background = "#929000", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("APIGateway") { Shape = Shape.RoundedBox, Background = "#0000ff", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("MessageBus") { Width = 850, Background = "#fd8208", Color = "#ffffff", Shape = Shape.Pipe, Icon = "" });
            styles.Add(new ElementStyle("BoundedContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("DataBase") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });


            ContainerView containerView = viewSet.CreateContainerView(tutoringSystem, "Contenedor", "Diagrama de contenedores");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();


            //---------------------------//---------------------------//
            // 3. Component Diagrams
            //---------------------------//---------------------------//

            // Components Diagram - Session Bounded Context
            Component sessionQueryController = sessionContext.AddComponent("Session Query Controller", "REST API endpoints de Session Details", "Spring Boot REST Controller");
            Component sessionsCommandController = sessionContext.AddComponent("Sessions Command Controller", "REST API endpoints de Session", "Spring Boot REST Controller");
            
            Component sessionComandService = sessionContext.AddComponent("Session Command Service", "Provee métodos para Session, pertenece a la capa Application de DDD", "Spring Component");

            Component sessionRepository = sessionContext.AddComponent("Session Repository", "Provee los métodos para la persistencia de datos de Session", "Spring Component");

            Component domainLayer = sessionContext.AddComponent("Domain Layer", "Contiene las entidades Core del microservicio", "Spring Component");


            // Components Diagram - User Bounded Context
            Component languageCommandController = userContext.AddComponent("Language Command Controller", "REST API endpoints de monitoreo.", "Spring Boot REST Controller");
            Component roleCommandController = userContext.AddComponent("Role Command Controller", "REST API endpoints de Roles", "Spring Boot REST Controller");
            Component topicCommandController = userContext.AddComponent("Topic Command Controller", "REST API endpoints de Topic of Interest", "Spring Boot REST Controller");
            Component userCommandController = userContext.AddComponent("User Command Controller", "REST API endpoints de User", "Spring Boot REST Controller");
            
            Component languageQueryController = userContext.AddComponent("Language Query Controller", "REST API endpoints de monitoreo.", "Spring Boot REST Controller");
            Component roleQueryController = userContext.AddComponent("Role Query Controller", "REST API endpoints de Roles", "Spring Boot REST Controller");
            Component topicQueryController = userContext.AddComponent("Topic Query Controller", "REST API endpoints de Topic of Interest", "Spring Boot REST Controller");
            Component userQueryController = userContext.AddComponent("User Query Controller", "REST API endpoints de User", "Spring Boot REST Controller");

            Component languageApplicationService = userContext.AddComponent("Language Application Service", "Provee métodos para el monitoreo, pertenece a la capa Application de DDD", "Spring Component");
            Component roleApplicationService = userContext.AddComponent("Role Application Service", "Provee métodos para Role, pertenece a la capa Application de DDD", "Spring Component");
            Component topicApplicationService = userContext.AddComponent("Topic Application Service", "Provee métodos para Topic of Interest, pertenece a la capa Application de DDD", "Spring Component");
            Component userApplicationService = userContext.AddComponent("User Application Service", "Provee métodos para User, pertenece a la capa Application de DDD", "Spring Component");
            
            Component languageRepository = userContext.AddComponent("Language Repository", "Provee los métodos para la persistencia de datos de Laguage of interest", "Spring Component");
            Component roleRepository = userContext.AddComponent("Role Repository", "Provee los métodos para la persistencia de datos de Role", "Spring Component");
            Component topicRepository = userContext.AddComponent("Topic Repository", "Provee los métodos para la persistencia de datos de Topic of Interest", "Spring Component");
            Component userRepository = userContext.AddComponent("User Repository", "Provee los métodos para la persistencia de datos de Laguage of User", "Spring Component");
            

            // Components Diagram - Subscription Bounded Context
            Component subscriptionCommandController = subscriptionContext.AddComponent("Subscription Command Controller", "REST API endpoints de Payment", "Spring Boot REST Controller");
            Component subscriptionQueryController = subscriptionContext.AddComponent("Subscriptions Controller", "REST API endpoints de Subscription", "Spring Boot REST Controller");

            Component subscriptionApplicationService = subscriptionContext.AddComponent("Subscription Application Service", "Provee métodos para Subscription, pertenece a la capa Application de DDD", "Spring Component");

            Component subscriptionRepository = subscriptionContext.AddComponent("Subscription Repository", "Provee los métodos para la persistencia de datos de Subscription", "Spring Component");

            // Components Diagram - External Tools Bounded Context
            Component externalToolController = externalToolsContext.AddComponent("External Tool Command Controller", "REST API ", "Spring Boot REST Controller");
            Component externalToolRespository = externalToolsContext.AddComponent("External Tool Repository", "", "Spring Component");
            Component externalQueryController = externalToolsContext.AddComponent("External Tool Query Controller", "REST API ", "Spring Boot REST Controller");
            Component externalApplicationService = externalToolsContext.AddComponent("External Tool Application Service", "", "Spring Component");

            // Components Diagram - Payment Bounded Context
            Component paymentCommandController = paymentContext.AddComponent("Payment Command Controller", "REST API ", "Spring Boot REST Controller");
            Component paymentQueryController = paymentContext.AddComponent("Payment Query Controller", "REST API ", "Spring Boot REST Controller");

            Component transactionCommandController = paymentContext.AddComponent("Transaction Command Controller", "REST API ", "Spring Boot REST Controller");
            Component transactionQueryController = paymentContext.AddComponent("Transaction Query Controller", "REST API ", "Spring Boot REST Controller");

            Component paymentApplicationService = paymentContext.AddComponent("Payment Application Service", "", "Spring Component");
            Component transactionApplicationService = paymentContext.AddComponent("Transaction Application Service", "", "Spring Component");

            Component paypalFacade = paymentContext.AddComponent("Paypal Facade", "", "Spring Component");

            Component paymentRepository = paymentContext.AddComponent("Payment Repository", "", "Spring Component");
            Component transactionRepository = paymentContext.AddComponent("Transaction Repository", "", "Spring Component");

            // Tags
            languageCommandController.AddTags("Controller");
            languageQueryController.AddTags("Controller");
            languageApplicationService.AddTags("Service");
            languageRepository.AddTags("Repository");

            roleCommandController.AddTags("Controller");
            roleQueryController.AddTags("Controller");
            roleApplicationService.AddTags("Service");
            roleRepository.AddTags("Repository");

            sessionQueryController.AddTags("Controller");

            sessionsCommandController.AddTags("Controller");
            sessionComandService.AddTags("Service");
            sessionRepository.AddTags("Repository");
            domainLayer.AddTags("Repository");

            subscriptionCommandController.AddTags("Controller");
            subscriptionQueryController.AddTags("Controller");
            subscriptionApplicationService.AddTags("Service");
            subscriptionRepository.AddTags("Repository");

            topicCommandController.AddTags("Controller");
            topicQueryController.AddTags("Controller");
            topicApplicationService.AddTags("Service");
            topicRepository.AddTags("Repository");

            userCommandController.AddTags("Controller");
            userQueryController.AddTags("Controller");
            userApplicationService.AddTags("Service");
            userRepository.AddTags("Repository");

            externalToolController.AddTags("Controller");
            externalToolRespository.AddTags("Repository");

            externalQueryController.AddTags("Controller");
            externalApplicationService.AddTags("Service");

            paymentCommandController.AddTags("Controller");
            paymentQueryController.AddTags("Controller");
            transactionCommandController.AddTags("Controller");
            transactionQueryController.AddTags("Controller");
            paymentApplicationService.AddTags("Service");
            transactionApplicationService.AddTags("Service");
            paymentRepository.AddTags("Repository");
            transactionRepository.AddTags("Repository");
            paypalFacade.AddTags("Service");


            styles.Add(new ElementStyle("Controller") { Shape = Shape.Component, Background = "#FDFF8B", Icon = "" });
            styles.Add(new ElementStyle("Service") { Shape = Shape.Component, Background = "#FEF535", Icon = "" });
            styles.Add(new ElementStyle("Repository") { Shape = Shape.Component, Background = "#FFC100", Icon = "" });



            //Component connection: Language
            apiGateway.Uses(languageCommandController, "", "JSON/HTTPS");
            apiGateway.Uses(languageQueryController, "", "JSON/HTTPS");
            languageCommandController.Uses(languageApplicationService, "Llama a los métodos del Service");
            languageApplicationService.Uses(domainLayer, "Usa");
            languageQueryController.Uses(languageRepository, "Usa");
            /**/
            languageQueryController.Uses(userRepository, "Usa");
            languageRepository.Uses(userContextDatabase, "Lee desde y escribe hasta");

            //Component connection: Role 
            apiGateway.Uses(roleCommandController, "", "JSON/HTTPS");
            apiGateway.Uses(roleQueryController, "", "JSON/HTTPS");
            roleCommandController.Uses(roleApplicationService, "Llama a los métodos del Service");
            roleApplicationService.Uses(domainLayer, "Usa");
            roleQueryController.Uses(roleRepository, "Usa");
            roleRepository.Uses(userContextDatabase, "Lee desde y escribe hasta");

            //Component connection: Session Details 
            apiGateway.Uses(sessionQueryController, "", "JSON/HTTPS");
            sessionQueryController.Uses(sessionRepository, "Usa");
            /**/
            sessionRepository.Uses(sessionContextDatabase, "Lee desde y escribe hasta");

            //Component connection: Session 
            apiGateway.Uses(sessionsCommandController, "", "JSON/HTTPS");
            sessionsCommandController.Uses(sessionComandService, "Llama a los métodos del Service");
            sessionComandService.Uses(domainLayer, "Usa");
            /**/

            //Component connection: Subscription 
            apiGateway.Uses(subscriptionQueryController, "", "JSON/HTTPS");
            apiGateway.Uses(subscriptionCommandController, "", "JSON/HTTPS");
            subscriptionCommandController.Uses(subscriptionApplicationService, "Usa");
            subscriptionApplicationService.Uses(domainLayer, "Usa");
            subscriptionQueryController.Uses(subscriptionRepository, "Usa");
            subscriptionRepository.Uses(subscriptionContextDatabase, "Lee desde y escribe hasta");

            //Component connection: Topic Of Interest 
            apiGateway.Uses(topicCommandController, "", "JSON/HTTPS");
            apiGateway.Uses(topicQueryController, "", "JSON/HTTPS");
            topicCommandController.Uses(topicApplicationService, "Llama a los métodos del Service");
            topicApplicationService.Uses(domainLayer, "Usa");
            topicQueryController.Uses(topicRepository, "Usa");
            /**/
            topicRepository.Uses(userRepository, "Usa");
            topicRepository.Uses(userContextDatabase, "Lee desde y escribe hasta");

            //Component connection: User 
            apiGateway.Uses(userCommandController, "", "JSON/HTTPS");
            apiGateway.Uses(userQueryController, "", "JSON/HTTPS");
            userCommandController.Uses(userApplicationService, "Llama a los métodos del Service");
            userApplicationService.Uses(domainLayer, "Usa");
            userQueryController.Uses(userRepository, "Usa");
            /**/
            userQueryController.Uses(roleRepository, "Usa");
            /**/
            userQueryController.Uses(languageRepository, "Usa");
            /**/
            userQueryController.Uses(topicRepository, "Usa");
            userRepository.Uses(userContextDatabase, "Lee desde y escribe hasta");


            //Component connection: External
            //Google Calendar
            apiGateway.Uses(externalQueryController, "", "JSON/HTTPS");
            externalQueryController.Uses(externalToolRespository, "Llama a los métodos del Service");
            externalToolRespository.Uses(externalToolContextDatabase, "Lee desde y escribe hasta");
            //userSubscriptionRepository.Uses(subscriptionContextDatabase, "Lee desde y escribe hasta");

            //Zoom
            apiGateway.Uses(externalToolController, "", "JSON/HTTPS");
            externalToolController.Uses(externalApplicationService, "Llama a los métodos del Service");
            externalApplicationService.Uses(domainLayer, "Usa");
            externalApplicationService.Uses(zoom, "Usa");
            externalApplicationService.Uses(googleCalendar, "Usa");

            //Component connection: Payment
            apiGateway.Uses(paymentCommandController, "", "JSON/HTTPS");
            apiGateway.Uses(paymentQueryController, "", "JSON/HTTPS");
            apiGateway.Uses(transactionCommandController, "", "JSON/HTTPS");
            apiGateway.Uses(transactionQueryController, "", "JSON/HTTPS");
            transactionCommandController.Uses(transactionApplicationService, "", "Llama a los métodos del Service");
            transactionQueryController.Uses(transactionRepository, "", "JSON/HTTPS");
            transactionRepository.Uses(paymentContextDatabase, "", "Lee desde y escribe hasta");
            paymentQueryController.Uses(paymentRepository, "", "JSON/HTTPS");
            paymentCommandController.Uses(paymentApplicationService, "", "Llama a los métodos del Service");
            paymentApplicationService.Uses(paypalFacade, "", "Usa");
            transactionApplicationService.Uses(domainLayer, "", "Usa");
            paymentApplicationService.Uses(domainLayer, "", "Usa");
            paypalFacade.Uses(paypal, "", "Usa");
            paymentRepository.Uses(paymentContextDatabase, "", "Lee desde y escribe hasta");



            // View - Components Diagram - Session Bounded Context
            ComponentView sessionComponentView = viewSet.CreateComponentView(sessionContext, "Session Bounded Context's Components", "Component Diagram");
            sessionComponentView.PaperSize = PaperSize.A4_Landscape;
            sessionComponentView.Add(mobileApplication);
            sessionComponentView.Add(webApplication);
            sessionComponentView.Add(apiGateway);
            sessionComponentView.Add(sessionContextDatabase);
            sessionComponentView.AddAllComponents();

            // View - Components Diagram - User Bounded Context
            ComponentView userComponentView = viewSet.CreateComponentView(userContext, "User Bounded Context's Components", "Component Diagram");
            userComponentView.PaperSize = PaperSize.A3_Landscape;
            userComponentView.Add(mobileApplication);
            userComponentView.Add(webApplication);
            userComponentView.Add(apiGateway);
            userComponentView.Add(domainLayer);
            userComponentView.Add(userContextDatabase);
            userComponentView.AddAllComponents();

            // View - Components Diagram - Subscription Bounded Context
            ComponentView subscriptionComponentView = viewSet.CreateComponentView(subscriptionContext, "Subscription Bounded Context's Components", "Component Diagram");
            subscriptionComponentView.PaperSize = PaperSize.A4_Landscape;
            subscriptionComponentView.Add(mobileApplication);
            subscriptionComponentView.Add(webApplication);
            subscriptionComponentView.Add(apiGateway);
            subscriptionComponentView.Add(subscriptionContextDatabase);
            subscriptionComponentView.Add(domainLayer);
            subscriptionComponentView.AddAllComponents();

            // View - Components Diagram - External Bounded Context
            ComponentView externalToolsComponentView = viewSet.CreateComponentView(externalToolsContext, "External Tools Bounded Context's Components", "Component Diagram");
            externalToolsComponentView.PaperSize = PaperSize.A4_Landscape;
            externalToolsComponentView.Add(mobileApplication);
            externalToolsComponentView.Add(webApplication);
            externalToolsComponentView.Add(apiGateway);
            externalToolsComponentView.Add(googleCalendar);
            externalToolsComponentView.Add(zoom);
            externalToolsComponentView.Add(externalToolContextDatabase);
            externalToolsComponentView.Add(domainLayer);
            externalToolsComponentView.AddAllComponents();

            // View - Components Diagram - External Payment Context
            ComponentView paymentComponentView = viewSet.CreateComponentView(paymentContext, "Payment Bounded Context's Components", "Component Diagram");
            paymentComponentView.Add(mobileApplication);
            paymentComponentView.PaperSize = PaperSize.A3_Landscape;
            paymentComponentView.Add(webApplication);
            paymentComponentView.Add(apiGateway);
            paymentComponentView.Add(paymentCommandController);
            paymentComponentView.Add(paypalFacade);
            paymentComponentView.Add(paymentRepository);
            paymentComponentView.Add(paypal);
            paymentComponentView.Add(paymentContextDatabase);
            paymentComponentView.Add(domainLayer);
            paymentComponentView.AddAllComponents();


            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            DeploymentNode liveWebServer = model.AddDeploymentNode("IlanguageWebApp", "A web server residing in the web server farm, accessed via F5 BIG-IP LTMs.", "Firebase", 1, DictionaryUtils.Create("Location=London"));
            liveWebServer.AddDeploymentNode("Web Application", "An open source Java EE web server.", "Spring Boot", 1, DictionaryUtils.Create("Xmx=512M", "Xms=1024M", "Java Version=8"))
                .Add(webApplication);

            DeploymentNode landingPageNode = model
                .AddDeploymentNode("IlanguageLP", "The primary database server.", "Ubuntu 16.04 LTS", 1, DictionaryUtils.Create("Location=London"))
                .AddDeploymentNode("Navegador Web", "The primary, live database server.", "Chrome");
            landingPageNode.Add(landingPage);

            DeploymentNode mobileNode = model
                .AddDeploymentNode("IlanguageMobileApp", "The primary database server.", "Ubuntu 16.04 LTS", 1, DictionaryUtils.Create("Location=London"))
                .AddDeploymentNode("Dispositivo móvil del usuario", "The primary, live database server.", "Android");
            mobileNode.Add(mobileApplication);

            DeploymentNode newNode = model
                .AddDeploymentNode("AWS-Cloud Diagram", "The primary, live database server.", "AWS Cloud");
            newNode.AddDeploymentNode("API Gateway", "The primary, live database server.", "Docker").Add(apiGateway);

            newNode.AddDeploymentNode("User Context", "The primary, live database server.", "Docker").Add(userContext);
            newNode.AddDeploymentNode("Payment Context", "The primary, live database server.", "Docker").Add(paymentContext);
            newNode.AddDeploymentNode("Subscription Context", "The primary, live database server.", "Docker").Add(subscriptionContext);
            newNode.AddDeploymentNode("Session Context", "The primary, live database server.", "Docker").Add(sessionContext);
            newNode.AddDeploymentNode("External Tool Context", "The primary, live database server.", "Docker").Add(externalToolsContext);

            newNode.AddDeploymentNode("User Persistance", "The primary, live database server.", "MySQL").Add(userContextDatabase);
            newNode.AddDeploymentNode("Payment Persistance", "The primary, live database server.", "MySQL").Add(paymentContextDatabase);
            newNode.AddDeploymentNode("Subscription Persistance", "The primary, live database server.", "MySQL").Add(subscriptionContextDatabase);
            newNode.AddDeploymentNode("Session Persistance", "The primary, live database server.", "MySQL").Add(sessionContextDatabase);
            newNode.AddDeploymentNode("External Tool Persistance", "The primary, live database server.", "MySQL").Add(externalToolContextDatabase);

            

            DeploymentNode deployedLandingPage = model
                .AddDeploymentNode("Oracle - Primary", "The primary, live database server.", "Oracle 12c");

            //model.Relationships.Where(r => r.Destination.Equals(secondaryDatabase)).ToList().ForEach(r => r.AddTags("Failover"));
            Relationship dataReplicationRelationship = landingPageNode.Uses(liveWebServer, "Call Action To", "");
            //secondaryDatabase.AddTags("Failover");

            //model.Relationships.Where(r => r.Destination.Equals(secondaryDatabase)).ToList().ForEach(r => r.AddTags("Failover"));
            //Relationship dataPersistanceRelationship = liveWebServer.Uses(landingPageNode, "Escribe desde y lee hasta", "JDBC");

            DeploymentView liveDeploymentView = viewSet.CreateDeploymentView(tutoringSystem, "Deployment Diagram", "ILanguage Deployment Diagram");
            liveDeploymentView.Add(liveWebServer);
            liveDeploymentView.Add(newNode);
            liveDeploymentView.Add(mobileNode);
            liveDeploymentView.Add(landingPageNode);
            liveDeploymentView.PaperSize = PaperSize.A3_Landscape;

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}