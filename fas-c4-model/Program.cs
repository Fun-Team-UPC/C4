using Structurizr;
using Structurizr.Api;

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
            SoftwareSystem googleCalendar = model.AddSoftwareSystem("Google Calendar", "Permite transmitir información en tiempo real por el avión del vuelo a nuestro sistema");

            Person student = model.AddPerson("Estudiante", "Persona que busca aprender/mejorar algún idioma.");
            Person tutor = model.AddPerson("Tutor", "Tutor en busca de trabajo que ayuda a aprender/mejorar algún idioma.");
            Person admin = model.AddPerson(Location.Internal, "Admin", "Admin - Open Data.");
            

            student.Uses(tutoringSystem, "Realiza consultas para progresar en su aprendizaje de un idioma");
            tutor.Uses(tutoringSystem, "Realiza consultas para realizar su trabajo de tutor en un idioma");
            admin.Uses(tutoringSystem, "Realiza consultas a la REST API para mantenerse al tanto de los planes de suscripción y monitoreo del desenvolvimiento de los usuarios");
            
            tutoringSystem.Uses(googleCalendar, "Envía información para la reserva de un evento");
            tutoringSystem.Uses(zoom, "Se comunica para la programación de una reunión");

            ViewSet viewSet = workspace.Views;


            //---------------------------//---------------------------//
            // 1. Diagrama de Contexto
            //---------------------------//---------------------------//

            SystemContextView contextView = viewSet.CreateSystemContextView(tutoringSystem, "Contexto", "Diagrama de contexto");
            contextView.PaperSize = PaperSize.A3_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();
            
            // Tags
            tutoringSystem.AddTags("SistemaMonitoreo");
            zoom.AddTags("Zoom");
            googleCalendar.AddTags("AircraftSystem");
            student.AddTags("Estudiante");
            tutor.AddTags("Tutor");
            admin.AddTags("Admin");
            
            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle("Estudiante") { Background = "#0a60ff", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Tutor") { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Admin") { Background = "#facc2e", Shape = Shape.Robot });
            styles.Add(new ElementStyle("SistemaMonitoreo") { Background = "#008f39", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("Zoom") { Background = "#90714c", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("AircraftSystem") { Background = "#2f95c7", Color = "#ffffff", Shape = Shape.RoundedBox });


            //---------------------------//---------------------------//
            // 2. Diagrama de Contenedores
            //---------------------------//---------------------------//

            Container mobileApplication = tutoringSystem.AddContainer("Mobile App", "Permite a los usuarios visualizar un dashboard con las funcionalidades que brinda la aplicación.", "Flutter");
            Container webApplication = tutoringSystem.AddContainer("Web App", "Permite a los usuarios visualizar un dashboard con las funcionalidades que brinda la aplicación.", "Flutter Web");
            Container landingPage = tutoringSystem.AddContainer("Landing Page", "", "Flutter Web");
            Container apiGateway = tutoringSystem.AddContainer("API Gateway", "API Gateway", "Spring Boot port 8080");
            Container businessContext = tutoringSystem.AddContainer("Business Context", "Bounded Context del Monolito de ILanguage", "Spring Boot port 8081");
            Container businessContextDatabase = tutoringSystem.AddContainer("Business Context DB", "", "MySQL");
            
            
            student.Uses(mobileApplication, "Consulta");
            student.Uses(webApplication, "Consulta");
            student.Uses(landingPage, "Consulta");
            
            tutor.Uses(mobileApplication, "Consulta");
            tutor.Uses(webApplication, "Consulta");
            tutor.Uses(landingPage, "Consulta");
            
            mobileApplication.Uses(apiGateway, "API Request", "JSON/HTTPS");
            webApplication.Uses(apiGateway, "API Request", "JSON/HTTPS");
            admin.Uses(apiGateway, "API Request", "JSON/HTTPS");
            
            apiGateway.Uses(businessContext, "API Request", "JSON/HTTPS");

            businessContext.Uses(businessContextDatabase, "", "JDBC");
            businessContext.Uses(googleCalendar, "Reserva de evento", "JSON");
            businessContext.Uses(zoom, "Programa reunión", "JSON");

            zoom.Uses(businessContext, "Retorna link de la reunión", "JSON");

            // Tags
            mobileApplication.AddTags("MobileApp");
            webApplication.AddTags("WebApp");
            landingPage.AddTags("LandingPage");
            apiGateway.AddTags("APIGateway");
            
            businessContext.AddTags("businessContext");
            businessContextDatabase.AddTags("businessContextDatabase");



            styles.Add(new ElementStyle("MobileApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.MobileDevicePortrait, Icon = "" });
            styles.Add(new ElementStyle("WebApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("LandingPage") { Background = "#929000", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("APIGateway") { Shape = Shape.RoundedBox, Background = "#0000ff", Color = "#ffffff", Icon = "" });
            
            styles.Add(new ElementStyle("businessContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("businessContextDatabase") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });


            ContainerView containerView = viewSet.CreateContainerView(tutoringSystem, "Contenedor", "Diagrama de contenedores");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();


            //---------------------------//---------------------------//
            // 3. Diagrama de Componentes
            //---------------------------//---------------------------//
            
            Component languageOfInterestController = businessContext.AddComponent("Language Of Interest Controller", "REST API endpoints de monitoreo.", "Spring Boot REST Controller");
            Component paymentsController = businessContext.AddComponent("Payment Controller", "REST API endpoints de Payment", "Spring Boot REST Controller");
            Component rolesController = businessContext.AddComponent("Roles Controller", "REST API endpoints de Roles", "Spring Boot REST Controller");
            Component scheduleController = businessContext.AddComponent("Schedule Controller", "REST API endpoints de Schedule", "Spring Boot REST Controller");
            Component sessionDetailsController = businessContext.AddComponent("Session Details Controller", "REST API endpoints de Session Details", "Spring Boot REST Controller");
            Component sessionsController = businessContext.AddComponent("Sessions Controller", "REST API endpoints de Session", "Spring Boot REST Controller");
            Component subscriptionsController = businessContext.AddComponent("Subscriptions Controller", "REST API endpoints de Subscription", "Spring Boot REST Controller");
            Component topicOfInterestController = businessContext.AddComponent("Topic Of Interest Controller", "REST API endpoints de Topic of Interest", "Spring Boot REST Controller");
            Component userController = businessContext.AddComponent("User Controller", "REST API endpoints de User", "Spring Boot REST Controller");
            Component userLanguageController = businessContext.AddComponent("Language Controller", "REST API endpoints de User Language", "Spring Boot REST Controller");
            Component userScheduleController = businessContext.AddComponent("user Schedule Controller", "REST API endpoints de User Schedule", "Spring Boot REST Controller");
            Component userSubscriptionController = businessContext.AddComponent("User Subscription Controller", "REST API endpoints de User Subscription", "Spring Boot REST Controller");
            Component userTopicsController = businessContext.AddComponent("User Topics Controller", "REST API endpoints de User Topics", "Spring Boot REST Controller");

            Component languageOfInterestService = businessContext.AddComponent("Language Of Interest Service", "Provee métodos para el monitoreo, pertenece a la capa Application de DDD", "Spring Component");
            Component paymentsService = businessContext.AddComponent("Payments Service", "Provee métodos para Payment, pertenece a la capa Application de DDD", "Spring Component");
            Component roleService = businessContext.AddComponent("Role Service", "Provee métodos para Role, pertenece a la capa Application de DDD", "Spring Component");
            Component scheduleService = businessContext.AddComponent("Schedule Service", "Provee métodos para Schedule, pertenece a la capa Application de DDD", "Spring Component");
            Component sessionDetailService = businessContext.AddComponent("Session Detail Service", "Provee métodos para Schedule Detail, pertenece a la capa Application de DDD", "Spring Component");
            Component sessionService = businessContext.AddComponent("Session Service", "Provee métodos para Session, pertenece a la capa Application de DDD", "Spring Component");
            Component subscriptionService = businessContext.AddComponent("Subscription Service", "Provee métodos para Subscription, pertenece a la capa Application de DDD", "Spring Component");
            Component topicOfInterestService = businessContext.AddComponent("Topic Of Interes Service", "Provee métodos para Topic of Interest, pertenece a la capa Application de DDD", "Spring Component");
            Component userService = businessContext.AddComponent("User Service", "Provee métodos para User, pertenece a la capa Application de DDD", "Spring Component");
            Component userScheduleService = businessContext.AddComponent("User Schedule Service", "Provee métodos para User Schedule, pertenece a la capa Application de DDD", "Spring Component");
            Component userSubscriptionService = businessContext.AddComponent("User Subscription Service", "Provee métodos para User Subscription, pertenece a la capa Application -de DDD", "Spring Component");

            Component languageOfInterestRepository = businessContext.AddComponent("Language Of Interest Repository", "Provee los métodos para la persistencia de datos de Laguage of interest", "Spring Component");
            Component roleRepository = businessContext.AddComponent("Role Repository", "Provee los métodos para la persistencia de datos de Role", "Spring Component");
            Component scheduleRepository = businessContext.AddComponent("Schedule Repository", "Provee los métodos para la persistencia de datos de Schedule", "Spring Component");
            Component sessionDetailRepository = businessContext.AddComponent("Session Details Repository", "Provee los métodos para la persistencia de datos de Session Detail", "Spring Component");
            Component sessionRepository = businessContext.AddComponent("Session Repository", "Provee los métodos para la persistencia de datos de Session", "Spring Component");
            Component subscriptionRepository = businessContext.AddComponent("Subscription Repository", "Provee los métodos para la persistencia de datos de Subscription", "Spring Component");
            Component topicOfInterestRepository = businessContext.AddComponent("Topic Of Interes Repository", "Provee los métodos para la persistencia de datos de Topic of Interest", "Spring Component");
            Component userRepository = businessContext.AddComponent("User Repository", "Provee los métodos para la persistencia de datos de Laguage of User", "Spring Component");
            Component userScheduleRepository = businessContext.AddComponent("User Schedule Repository", "Provee los métodos para la persistencia de datos de User Schedule", "Spring Component");
            Component userSubscriptionRepository = businessContext.AddComponent("User Subscription Repository", "Provee los métodos para la persistencia de datos de User Schedule", "Spring Component");

            Component googleCalendarController = businessContext.AddComponent("Google Calendar Controller", "REST API ", "Spring Boot REST Controller");
            Component googleCalendarFacade = businessContext.AddComponent("Google Calendar Facade", "", "Spring Component");
            Component zoomController = businessContext.AddComponent("Zoom Controller", "REST API ", "Spring Boot REST Controller");
            Component zoomFacade = businessContext.AddComponent("Zoom Facade", "", "Spring Component");


            // Tags
            languageOfInterestController.AddTags("Controller");
            languageOfInterestService.AddTags("Service");
            languageOfInterestRepository.AddTags("Repository");

            paymentsController.AddTags("Controller");
            paymentsService.AddTags("Service");

            rolesController.AddTags("Controller");
            roleService.AddTags("Service");
            roleRepository.AddTags("Repository");

            scheduleController.AddTags("Controller");
            scheduleService.AddTags("Service");
            scheduleRepository.AddTags("Repository");

            sessionDetailsController.AddTags("Controller");
            sessionDetailService.AddTags("Service");
            sessionDetailRepository.AddTags("Repository");

            sessionsController.AddTags("Controller");
            sessionService.AddTags("Service");
            sessionRepository.AddTags("Repository");

            subscriptionsController.AddTags("Controller");
            subscriptionService.AddTags("Service");
            subscriptionRepository.AddTags("Repository");

            topicOfInterestController.AddTags("Controller");
            topicOfInterestService.AddTags("Service");
            topicOfInterestRepository.AddTags("Repository");

            userController.AddTags("Controller");
            userService.AddTags("Service");
            userRepository.AddTags("Repository");

            userLanguageController.AddTags("Controller");

            userScheduleController.AddTags("Controller");
            userScheduleService.AddTags("Service");
            userScheduleRepository.AddTags("Repository");

            userSubscriptionController.AddTags("Controller");
            userSubscriptionService.AddTags("Service");
            userSubscriptionRepository.AddTags("Repository");

            userTopicsController.AddTags("Controller");

            googleCalendarController.AddTags("Controller");
            googleCalendarFacade.AddTags("Service");

            zoomController.AddTags("Controller");
            zoomFacade.AddTags("Service");


            styles.Add(new ElementStyle("Controller") { Shape = Shape.Component, Background = "#FDFF8B", Icon = "" });
            styles.Add(new ElementStyle("Service") { Shape = Shape.Component, Background = "#FEF535", Icon = "" });
            styles.Add(new ElementStyle("Repository") { Shape = Shape.Component, Background = "#FFC100", Icon = "" });


            //styles.Add(new ElementStyle("LanguageOfInterestController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("LanguageOfInterestService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("LanguageOfInterestRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("PaymentsController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("PaymentsService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("RolesController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("RoleService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("RoleRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("ScheduleController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("ScheduleService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("ScheduleRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("SessionDetailsController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("SessionDetailService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("SessionDetailRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("SessionsController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("SessionService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("SessionRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("SubscriptionsController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("SubscriptionService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("SubscriptionRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("TopicOfInterestController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("TopicOfInterestService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("TopicOfInterestRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("UserController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("UserService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("UserRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("UserLanguageController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("UserScheduleController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("UserScheduleService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("UserScheduleRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("UserSubscriptionController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("UserSubscriptionService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("UserSubscriptionRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("UserTopicsController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            //styles.Add(new ElementStyle("GoogleCalendarController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            //styles.Add(new ElementStyle("ZoomController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });



            //Component connection: Language Of Interest 
            apiGateway.Uses(languageOfInterestController, "", "JSON/HTTPS");
            languageOfInterestController.Uses(languageOfInterestService, "Llama a los métodos del Service");
            languageOfInterestService.Uses(languageOfInterestRepository, "Usa");
            /**/languageOfInterestService.Uses(userRepository, "Usa");
            languageOfInterestRepository.Uses(businessContextDatabase , "Lee desde y escribe hasta");

            //Component connection: Payment
            apiGateway.Uses(paymentsController, "", "JSON/HTTPS");
            paymentsController.Uses(paymentsService, "Llama a los métodos del Service");

            //Component connection: Role 
            apiGateway.Uses(rolesController, "", "JSON/HTTPS");
            rolesController.Uses(roleService, "Llama a los métodos del Service");
            roleService.Uses(roleRepository, "Usa");
            roleRepository.Uses(businessContextDatabase, "Lee desde y escribe hasta");

            //Component connection: Schedule 
            apiGateway.Uses(scheduleController, "", "JSON/HTTPS");
            scheduleController.Uses(scheduleService, "Llama a los métodos del Service");
            scheduleService.Uses(scheduleRepository, "Usa");
            scheduleRepository.Uses(businessContextDatabase, "Lee desde y escribe hasta");

            //Component connection: Session Details 
            apiGateway.Uses(sessionDetailsController, "", "JSON/HTTPS");
            sessionDetailsController.Uses(sessionDetailService, "Llama a los métodos del Service");
            sessionDetailService.Uses(sessionDetailRepository, "Usa");
            /**/sessionDetailService.Uses(sessionRepository, "Usa");
            sessionDetailRepository.Uses(businessContextDatabase, "Lee desde y escribe hasta");

            //Component connection: Session 
            apiGateway.Uses(sessionsController, "", "JSON/HTTPS");
            sessionsController.Uses(sessionService, "Llama a los métodos del Service");
            sessionService.Uses(sessionRepository, "Usa");
            /**/sessionService.Uses(userScheduleRepository, "Usa");
            sessionRepository.Uses(businessContextDatabase, "Lee desde y escribe hasta");

            //Component connection: Subscription 
            apiGateway.Uses(subscriptionsController, "", "JSON/HTTPS");
            subscriptionsController.Uses(subscriptionService, "Llama a los métodos del Service");
            subscriptionService.Uses(subscriptionRepository, "Usa");
            subscriptionRepository.Uses(businessContextDatabase, "Lee desde y escribe hasta");

            //Component connection: Topic Of Interest 
            apiGateway.Uses(topicOfInterestController, "", "JSON/HTTPS");
            topicOfInterestController.Uses(topicOfInterestService, "Llama a los métodos del Service");
            topicOfInterestService.Uses(topicOfInterestRepository, "Usa");
            /**/topicOfInterestService.Uses(userRepository, "Usa");
            topicOfInterestRepository.Uses(businessContextDatabase, "Lee desde y escribe hasta");

            //Component connection: User 
            apiGateway.Uses(userController, "", "JSON/HTTPS");
            userController.Uses(userService, "Llama a los métodos del Service");
            userService.Uses(userRepository, "Usa");
            /**/userService.Uses(roleRepository, "Usa");
            /**/userService.Uses(languageOfInterestRepository, "Usa");
            /**/userService.Uses(topicOfInterestRepository, "Usa");
            userRepository.Uses(businessContextDatabase, "Lee desde y escribe hasta");

            //Component connection: User Languages
            apiGateway.Uses(userLanguageController, "", "JSON/HTTPS");
            userLanguageController.Uses(languageOfInterestService, "Llama a los métodos del Service");
            /**/userLanguageController.Uses(userService, "Llama a los métodos del Service");

            //Component connection: User Topics
            apiGateway.Uses(userTopicsController, "", "JSON/HTTPS");
            userTopicsController.Uses(userService, "Llama a los métodos del Service");
            userTopicsController.Uses(topicOfInterestService, "Llama a los métodos del Service");

            //Component connection: User Schedule
            apiGateway.Uses(userScheduleController, "", "JSON/HTTPS");
            userScheduleController.Uses(userScheduleService, "Llama a los métodos del Service");
            userScheduleService.Uses(userScheduleRepository, "Usa");
            /**/userScheduleService.Uses(userRepository, "Usa");
            /**/userScheduleService.Uses(scheduleRepository, "Usa");
            userScheduleRepository.Uses(businessContextDatabase, "Lee desde y escribe hasta");

            //Component connection: User subscription
            apiGateway.Uses(userSubscriptionController, "", "JSON/HTTPS");
            userSubscriptionController.Uses(userSubscriptionService, "Llama a los métodos del Service");
            userSubscriptionService.Uses(userSubscriptionRepository, "Usa");
            /**/userSubscriptionService.Uses(userRepository, "Usa");
            /**/userSubscriptionService.Uses(subscriptionRepository, "Usa");
            userSubscriptionRepository.Uses(businessContextDatabase, "Lee desde y escribe hasta");

            //Component connection: External
            //Google Calendar
            apiGateway.Uses(googleCalendarController, "", "JSON/HTTPS");
            googleCalendarController.Uses(googleCalendarFacade, "Llama a los métodos del Service");
            googleCalendarFacade.Uses(googleCalendar, "Usa");

            //Zoom
            apiGateway.Uses(zoomController, "", "JSON/HTTPS");
            zoomController.Uses(zoomFacade, "Llama a los métodos del Service");
            zoomFacade.Uses(zoom, "Usa");

            ComponentView componentView = viewSet.CreateComponentView(businessContext, "Components", "Component Diagram");
            componentView.PaperSize = PaperSize.A2_Landscape;
            componentView.Add(mobileApplication);
            componentView.Add(webApplication);
            componentView.Add(apiGateway);
            componentView.Add(businessContextDatabase);
            componentView.Add(googleCalendar);
            componentView.Add(zoom);
            componentView.AddAllComponents();

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}