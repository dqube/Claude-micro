# Project Structure Plan for SQL Schema Services

Based on the `Sql/sqlstore.sql` analysis and following the PatientService pattern, here's the complete project structure for all 14 schemas:

## **1. SharedService** (shared schema)
**Tables**: Currencies, Countries

```
Services/SharedService/
├── API/
│   ├── Endpoints/
│   │   ├── CurrencyEndpoints.cs
│   │   └── CountryEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateCurrencyRequestValidator.cs
│   │   ├── CreateCountryRequestValidator.cs
│   │   ├── GetCurrenciesRequestValidator.cs
│   │   └── GetCountriesRequestValidator.cs
│   ├── SharedService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateCurrencyCommand.cs
│   │   ├── CreateCurrencyCommandHandler.cs
│   │   ├── CreateCountryCommand.cs
│   │   └── CreateCountryCommandHandler.cs
│   ├── DTOs/
│   │   ├── CurrencyDto.cs
│   │   └── CountryDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── CurrencyCreatedEventHandler.cs
│   │   └── CountryCreatedEventHandler.cs
│   ├── Queries/
│   │   ├── GetCurrencyByCodeQuery.cs
│   │   ├── GetCurrencyByCodeQueryHandler.cs
│   │   ├── GetCurrenciesQuery.cs
│   │   ├── GetCurrenciesQueryHandler.cs
│   │   ├── GetCountryByCodeQuery.cs
│   │   ├── GetCountryByCodeQueryHandler.cs
│   │   ├── GetCountriesQuery.cs
│   │   └── GetCountriesQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── SharedService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── Currency.cs
│   │   └── Country.cs
│   ├── Events/
│   │   ├── CurrencyCreatedEvent.cs
│   │   └── CountryCreatedEvent.cs
│   ├── Exceptions/
│   │   ├── CurrencyNotFoundException.cs
│   │   └── CountryNotFoundException.cs
│   ├── ValueObjects/
│   │   ├── CurrencyCode.cs
│   │   ├── CountryCode.cs
│   │   ├── CurrencyName.cs
│   │   └── CountryName.cs
│   ├── DependencyInjection.cs
│   └── SharedService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── CurrencyConfiguration.cs
│   │   └── CountryConfiguration.cs
│   ├── Persistence/
│   │   └── SharedDbContext.cs
│   ├── Repositories/
│   │   ├── CurrencyRepository.cs
│   │   ├── CountryRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── SharedService.Infrastructure.csproj
└── SharedService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **2. MessagingService** (messaging schema)
**Tables**: Outbox, Inbox

```
Services/MessagingService/
├── API/
│   ├── Endpoints/
│   │   ├── OutboxEndpoints.cs
│   │   └── InboxEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateOutboxMessageRequestValidator.cs
│   │   ├── CreateInboxMessageRequestValidator.cs
│   │   ├── GetOutboxMessagesRequestValidator.cs
│   │   └── GetInboxMessagesRequestValidator.cs
│   ├── MessagingService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateOutboxMessageCommand.cs
│   │   ├── CreateOutboxMessageCommandHandler.cs
│   │   ├── ProcessOutboxMessageCommand.cs
│   │   ├── ProcessOutboxMessageCommandHandler.cs
│   │   ├── CreateInboxMessageCommand.cs
│   │   ├── CreateInboxMessageCommandHandler.cs
│   │   ├── ProcessInboxMessageCommand.cs
│   │   └── ProcessInboxMessageCommandHandler.cs
│   ├── DTOs/
│   │   ├── OutboxMessageDto.cs
│   │   └── InboxMessageDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── OutboxMessageCreatedEventHandler.cs
│   │   ├── OutboxMessageProcessedEventHandler.cs
│   │   ├── InboxMessageCreatedEventHandler.cs
│   │   └── InboxMessageProcessedEventHandler.cs
│   ├── Queries/
│   │   ├── GetOutboxMessageByIdQuery.cs
│   │   ├── GetOutboxMessageByIdQueryHandler.cs
│   │   ├── GetUnprocessedOutboxMessagesQuery.cs
│   │   ├── GetUnprocessedOutboxMessagesQueryHandler.cs
│   │   ├── GetInboxMessageByIdQuery.cs
│   │   ├── GetInboxMessageByIdQueryHandler.cs
│   │   ├── GetUnprocessedInboxMessagesQuery.cs
│   │   └── GetUnprocessedInboxMessagesQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── MessagingService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── OutboxMessage.cs
│   │   └── InboxMessage.cs
│   ├── Events/
│   │   ├── OutboxMessageCreatedEvent.cs
│   │   ├── OutboxMessageProcessedEvent.cs
│   │   ├── InboxMessageCreatedEvent.cs
│   │   └── InboxMessageProcessedEvent.cs
│   ├── Exceptions/
│   │   ├── OutboxMessageNotFoundException.cs
│   │   └── InboxMessageNotFoundException.cs
│   ├── ValueObjects/
│   │   ├── OutboxMessageId.cs
│   │   ├── InboxMessageId.cs
│   │   ├── EventType.cs
│   │   └── MessagePayload.cs
│   ├── DependencyInjection.cs
│   └── MessagingService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── OutboxMessageConfiguration.cs
│   │   └── InboxMessageConfiguration.cs
│   ├── Persistence/
│   │   └── MessagingDbContext.cs
│   ├── Repositories/
│   │   ├── OutboxMessageRepository.cs
│   │   ├── InboxMessageRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── MessagingService.Infrastructure.csproj
└── MessagingService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **3. AuthService** (auth schema)
**Tables**: Roles, Users, UserRoles, RegistrationTokens

```
Services/AuthService/
├── API/
│   ├── Endpoints/
│   │   ├── UserEndpoints.cs
│   │   ├── RoleEndpoints.cs
│   │   ├── UserRoleEndpoints.cs
│   │   └── RegistrationTokenEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateUserRequestValidator.cs
│   │   ├── LoginRequestValidator.cs
│   │   ├── AssignRoleRequestValidator.cs
│   │   └── ResetPasswordRequestValidator.cs
│   ├── AuthService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateUserCommand.cs
│   │   ├── CreateUserCommandHandler.cs
│   │   ├── UpdatePasswordCommand.cs
│   │   ├── UpdatePasswordCommandHandler.cs
│   │   ├── AssignRoleCommand.cs
│   │   ├── AssignRoleCommandHandler.cs
│   │   ├── CreateRegistrationTokenCommand.cs
│   │   └── CreateRegistrationTokenCommandHandler.cs
│   ├── DTOs/
│   │   ├── UserDto.cs
│   │   ├── RoleDto.cs
│   │   ├── UserRoleDto.cs
│   │   └── RegistrationTokenDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── UserCreatedEventHandler.cs
│   │   ├── UserPasswordUpdatedEventHandler.cs
│   │   ├── UserRoleAssignedEventHandler.cs
│   │   └── RegistrationTokenCreatedEventHandler.cs
│   ├── Queries/
│   │   ├── GetUserByIdQuery.cs
│   │   ├── GetUserByIdQueryHandler.cs
│   │   ├── GetUserByEmailQuery.cs
│   │   ├── GetUserByEmailQueryHandler.cs
│   │   ├── GetUsersQuery.cs
│   │   ├── GetUsersQueryHandler.cs
│   │   ├── GetRolesQuery.cs
│   │   └── GetRolesQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── AuthService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   ├── UserRole.cs
│   │   └── RegistrationToken.cs
│   ├── Events/
│   │   ├── UserCreatedEvent.cs
│   │   ├── UserPasswordUpdatedEvent.cs
│   │   ├── UserActivatedEvent.cs
│   │   ├── UserDeactivatedEvent.cs
│   │   ├── UserRoleAssignedEvent.cs
│   │   └── RegistrationTokenCreatedEvent.cs
│   ├── Exceptions/
│   │   ├── UserNotFoundException.cs
│   │   ├── RoleNotFoundException.cs
│   │   └── InvalidTokenException.cs
│   ├── ValueObjects/
│   │   ├── UserId.cs
│   │   ├── RoleId.cs
│   │   ├── TokenId.cs
│   │   ├── Username.cs
│   │   ├── Email.cs
│   │   ├── PasswordHash.cs
│   │   └── TokenType.cs
│   ├── DependencyInjection.cs
│   └── AuthService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── UserConfiguration.cs
│   │   ├── RoleConfiguration.cs
│   │   ├── UserRoleConfiguration.cs
│   │   └── RegistrationTokenConfiguration.cs
│   ├── Persistence/
│   │   └── AuthDbContext.cs
│   ├── Repositories/
│   │   ├── UserRepository.cs
│   │   ├── RoleRepository.cs
│   │   ├── RegistrationTokenRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── AuthService.Infrastructure.csproj
└── AuthService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **4. ContactService** (contact schema)
**Tables**: ContactNumberTypes, AddressTypes

```
Services/ContactService/
├── API/
│   ├── Endpoints/
│   │   ├── ContactNumberTypeEndpoints.cs
│   │   └── AddressTypeEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateContactNumberTypeRequestValidator.cs
│   │   ├── CreateAddressTypeRequestValidator.cs
│   │   ├── GetContactNumberTypesRequestValidator.cs
│   │   └── GetAddressTypesRequestValidator.cs
│   ├── ContactService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateContactNumberTypeCommand.cs
│   │   ├── CreateContactNumberTypeCommandHandler.cs
│   │   ├── CreateAddressTypeCommand.cs
│   │   └── CreateAddressTypeCommandHandler.cs
│   ├── DTOs/
│   │   ├── ContactNumberTypeDto.cs
│   │   └── AddressTypeDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── ContactNumberTypeCreatedEventHandler.cs
│   │   └── AddressTypeCreatedEventHandler.cs
│   ├── Queries/
│   │   ├── GetContactNumberTypeByIdQuery.cs
│   │   ├── GetContactNumberTypeByIdQueryHandler.cs
│   │   ├── GetContactNumberTypesQuery.cs
│   │   ├── GetContactNumberTypesQueryHandler.cs
│   │   ├── GetAddressTypeByIdQuery.cs
│   │   ├── GetAddressTypeByIdQueryHandler.cs
│   │   ├── GetAddressTypesQuery.cs
│   │   └── GetAddressTypesQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── ContactService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── ContactNumberType.cs
│   │   └── AddressType.cs
│   ├── Events/
│   │   ├── ContactNumberTypeCreatedEvent.cs
│   │   └── AddressTypeCreatedEvent.cs
│   ├── Exceptions/
│   │   ├── ContactNumberTypeNotFoundException.cs
│   │   └── AddressTypeNotFoundException.cs
│   ├── ValueObjects/
│   │   ├── ContactNumberTypeId.cs
│   │   ├── AddressTypeId.cs
│   │   ├── ContactTypeName.cs
│   │   └── AddressTypeName.cs
│   ├── DependencyInjection.cs
│   └── ContactService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── ContactNumberTypeConfiguration.cs
│   │   └── AddressTypeConfiguration.cs
│   ├── Persistence/
│   │   └── ContactDbContext.cs
│   ├── Repositories/
│   │   ├── ContactNumberTypeRepository.cs
│   │   ├── AddressTypeRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── ContactService.Infrastructure.csproj
└── ContactService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **5. StoreService** (store schema)
**Tables**: Stores, Registers, Shifts, CashDrawerMovements

```
Services/StoreService/
├── API/
│   ├── Endpoints/
│   │   ├── StoreEndpoints.cs
│   │   ├── RegisterEndpoints.cs
│   │   ├── ShiftEndpoints.cs
│   │   └── CashDrawerMovementEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateStoreRequestValidator.cs
│   │   ├── CreateRegisterRequestValidator.cs
│   │   ├── StartShiftRequestValidator.cs
│   │   └── RecordCashMovementRequestValidator.cs
│   ├── StoreService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateStoreCommand.cs
│   │   ├── CreateStoreCommandHandler.cs
│   │   ├── CreateRegisterCommand.cs
│   │   ├── CreateRegisterCommandHandler.cs
│   │   ├── StartShiftCommand.cs
│   │   ├── StartShiftCommandHandler.cs
│   │   ├── EndShiftCommand.cs
│   │   ├── EndShiftCommandHandler.cs
│   │   ├── RecordCashMovementCommand.cs
│   │   └── RecordCashMovementCommandHandler.cs
│   ├── DTOs/
│   │   ├── StoreDto.cs
│   │   ├── RegisterDto.cs
│   │   ├── ShiftDto.cs
│   │   └── CashDrawerMovementDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── StoreCreatedEventHandler.cs
│   │   ├── RegisterCreatedEventHandler.cs
│   │   ├── ShiftStartedEventHandler.cs
│   │   ├── ShiftEndedEventHandler.cs
│   │   └── CashMovementRecordedEventHandler.cs
│   ├── Queries/
│   │   ├── GetStoreByIdQuery.cs
│   │   ├── GetStoreByIdQueryHandler.cs
│   │   ├── GetStoresQuery.cs
│   │   ├── GetStoresQueryHandler.cs
│   │   ├── GetRegistersByStoreQuery.cs
│   │   ├── GetRegistersByStoreQueryHandler.cs
│   │   ├── GetActiveShiftsQuery.cs
│   │   └── GetActiveShiftsQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── StoreService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── Store.cs
│   │   ├── Register.cs
│   │   ├── Shift.cs
│   │   └── CashDrawerMovement.cs
│   ├── Events/
│   │   ├── StoreCreatedEvent.cs
│   │   ├── RegisterCreatedEvent.cs
│   │   ├── ShiftStartedEvent.cs
│   │   ├── ShiftEndedEvent.cs
│   │   └── CashMovementRecordedEvent.cs
│   ├── Exceptions/
│   │   ├── StoreNotFoundException.cs
│   │   ├── RegisterNotFoundException.cs
│   │   └── ShiftNotFoundException.cs
│   ├── ValueObjects/
│   │   ├── StoreId.cs
│   │   ├── RegisterId.cs
│   │   ├── ShiftId.cs
│   │   ├── MovementId.cs
│   │   ├── StoreName.cs
│   │   ├── RegisterName.cs
│   │   └── MovementType.cs
│   ├── DependencyInjection.cs
│   └── StoreService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── StoreConfiguration.cs
│   │   ├── RegisterConfiguration.cs
│   │   ├── ShiftConfiguration.cs
│   │   └── CashDrawerMovementConfiguration.cs
│   ├── Persistence/
│   │   └── StoreDbContext.cs
│   ├── Repositories/
│   │   ├── StoreRepository.cs
│   │   ├── RegisterRepository.cs
│   │   ├── ShiftRepository.cs
│   │   ├── CashDrawerMovementRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── StoreService.Infrastructure.csproj
└── StoreService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **6. EmployeeService** (employee schema)
**Tables**: Employees, EmployeeContactNumbers, EmployeeAddresses

```
Services/EmployeeService/
├── API/
│   ├── Endpoints/
│   │   ├── EmployeeEndpoints.cs
│   │   ├── EmployeeContactNumberEndpoints.cs
│   │   └── EmployeeAddressEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateEmployeeRequestValidator.cs
│   │   ├── AddEmployeeContactRequestValidator.cs
│   │   ├── AddEmployeeAddressRequestValidator.cs
│   │   └── UpdateEmployeeRequestValidator.cs
│   ├── EmployeeService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateEmployeeCommand.cs
│   │   ├── CreateEmployeeCommandHandler.cs
│   │   ├── UpdateEmployeeCommand.cs
│   │   ├── UpdateEmployeeCommandHandler.cs
│   │   ├── AddEmployeeContactCommand.cs
│   │   ├── AddEmployeeContactCommandHandler.cs
│   │   ├── AddEmployeeAddressCommand.cs
│   │   └── AddEmployeeAddressCommandHandler.cs
│   ├── DTOs/
│   │   ├── EmployeeDto.cs
│   │   ├── EmployeeContactNumberDto.cs
│   │   └── EmployeeAddressDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── EmployeeCreatedEventHandler.cs
│   │   ├── EmployeeUpdatedEventHandler.cs
│   │   ├── EmployeeContactAddedEventHandler.cs
│   │   └── EmployeeAddressAddedEventHandler.cs
│   ├── Queries/
│   │   ├── GetEmployeeByIdQuery.cs
│   │   ├── GetEmployeeByIdQueryHandler.cs
│   │   ├── GetEmployeesQuery.cs
│   │   ├── GetEmployeesQueryHandler.cs
│   │   ├── GetEmployeesByStoreQuery.cs
│   │   └── GetEmployeesByStoreQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── EmployeeService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── Employee.cs
│   │   ├── EmployeeContactNumber.cs
│   │   └── EmployeeAddress.cs
│   ├── Events/
│   │   ├── EmployeeCreatedEvent.cs
│   │   ├── EmployeeUpdatedEvent.cs
│   │   ├── EmployeeContactAddedEvent.cs
│   │   └── EmployeeAddressAddedEvent.cs
│   ├── Exceptions/
│   │   ├── EmployeeNotFoundException.cs
│   │   ├── EmployeeContactNotFoundException.cs
│   │   └── EmployeeAddressNotFoundException.cs
│   ├── ValueObjects/
│   │   ├── EmployeeId.cs
│   │   ├── ContactNumberId.cs
│   │   ├── AddressId.cs
│   │   ├── EmployeeNumber.cs
│   │   └── Position.cs
│   ├── DependencyInjection.cs
│   └── EmployeeService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── EmployeeConfiguration.cs
│   │   ├── EmployeeContactNumberConfiguration.cs
│   │   └── EmployeeAddressConfiguration.cs
│   ├── Persistence/
│   │   └── EmployeeDbContext.cs
│   ├── Repositories/
│   │   ├── EmployeeRepository.cs
│   │   ├── EmployeeContactNumberRepository.cs
│   │   ├── EmployeeAddressRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── EmployeeService.Infrastructure.csproj
└── EmployeeService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **7. CatalogService** (catalog schema)
**Tables**: ProductCategories, Products, ProductBarcodes, CountryPricing, TaxConfigurations

```
Services/CatalogService/
├── API/
│   ├── Endpoints/
│   │   ├── ProductCategoryEndpoints.cs
│   │   ├── ProductEndpoints.cs
│   │   ├── ProductBarcodeEndpoints.cs
│   │   ├── CountryPricingEndpoints.cs
│   │   └── TaxConfigurationEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateProductCategoryRequestValidator.cs
│   │   ├── CreateProductRequestValidator.cs
│   │   ├── AddProductBarcodeRequestValidator.cs
│   │   ├── SetCountryPricingRequestValidator.cs
│   │   └── CreateTaxConfigurationRequestValidator.cs
│   ├── CatalogService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateProductCategoryCommand.cs
│   │   ├── CreateProductCategoryCommandHandler.cs
│   │   ├── CreateProductCommand.cs
│   │   ├── CreateProductCommandHandler.cs
│   │   ├── AddProductBarcodeCommand.cs
│   │   ├── AddProductBarcodeCommandHandler.cs
│   │   ├── SetCountryPricingCommand.cs
│   │   ├── SetCountryPricingCommandHandler.cs
│   │   ├── CreateTaxConfigurationCommand.cs
│   │   └── CreateTaxConfigurationCommandHandler.cs
│   ├── DTOs/
│   │   ├── ProductCategoryDto.cs
│   │   ├── ProductDto.cs
│   │   ├── ProductBarcodeDto.cs
│   │   ├── CountryPricingDto.cs
│   │   └── TaxConfigurationDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── ProductCategoryCreatedEventHandler.cs
│   │   ├── ProductCreatedEventHandler.cs
│   │   ├── ProductBarcodeAddedEventHandler.cs
│   │   ├── CountryPricingSetEventHandler.cs
│   │   └── TaxConfigurationCreatedEventHandler.cs
│   ├── Queries/
│   │   ├── GetProductCategoryByIdQuery.cs
│   │   ├── GetProductCategoryByIdQueryHandler.cs
│   │   ├── GetProductCategoriesQuery.cs
│   │   ├── GetProductCategoriesQueryHandler.cs
│   │   ├── GetProductByIdQuery.cs
│   │   ├── GetProductByIdQueryHandler.cs
│   │   ├── GetProductsQuery.cs
│   │   ├── GetProductsQueryHandler.cs
│   │   ├── GetProductByBarcodeQuery.cs
│   │   └── GetProductByBarcodeQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── CatalogService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── ProductCategory.cs
│   │   ├── Product.cs
│   │   ├── ProductBarcode.cs
│   │   ├── CountryPricing.cs
│   │   └── TaxConfiguration.cs
│   ├── Events/
│   │   ├── ProductCategoryCreatedEvent.cs
│   │   ├── ProductCreatedEvent.cs
│   │   ├── ProductBarcodeAddedEvent.cs
│   │   ├── CountryPricingSetEvent.cs
│   │   └── TaxConfigurationCreatedEvent.cs
│   ├── Exceptions/
│   │   ├── ProductCategoryNotFoundException.cs
│   │   ├── ProductNotFoundException.cs
│   │   ├── ProductBarcodeNotFoundException.cs
│   │   └── TaxConfigurationNotFoundException.cs
│   ├── ValueObjects/
│   │   ├── CategoryId.cs
│   │   ├── ProductId.cs
│   │   ├── BarcodeId.cs
│   │   ├── PricingId.cs
│   │   ├── TaxConfigId.cs
│   │   ├── SKU.cs
│   │   ├── ProductName.cs
│   │   ├── BarcodeValue.cs
│   │   └── TaxRate.cs
│   ├── DependencyInjection.cs
│   └── CatalogService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── ProductCategoryConfiguration.cs
│   │   ├── ProductConfiguration.cs
│   │   ├── ProductBarcodeConfiguration.cs
│   │   ├── CountryPricingConfiguration.cs
│   │   └── TaxConfigurationConfiguration.cs
│   ├── Persistence/
│   │   └── CatalogDbContext.cs
│   ├── Repositories/
│   │   ├── ProductCategoryRepository.cs
│   │   ├── ProductRepository.cs
│   │   ├── ProductBarcodeRepository.cs
│   │   ├── CountryPricingRepository.cs
│   │   ├── TaxConfigurationRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── CatalogService.Infrastructure.csproj
└── CatalogService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **8. InventoryService** (inventory schema)
**Tables**: InventoryItems, StockMovements

```
Services/InventoryService/
├── API/
│   ├── Endpoints/
│   │   ├── InventoryItemEndpoints.cs
│   │   └── StockMovementEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateInventoryItemRequestValidator.cs
│   │   ├── UpdateInventoryQuantityRequestValidator.cs
│   │   ├── RecordStockMovementRequestValidator.cs
│   │   └── GetInventoryRequestValidator.cs
│   ├── InventoryService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateInventoryItemCommand.cs
│   │   ├── CreateInventoryItemCommandHandler.cs
│   │   ├── UpdateInventoryQuantityCommand.cs
│   │   ├── UpdateInventoryQuantityCommandHandler.cs
│   │   ├── RecordStockMovementCommand.cs
│   │   └── RecordStockMovementCommandHandler.cs
│   ├── DTOs/
│   │   ├── InventoryItemDto.cs
│   │   └── StockMovementDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── InventoryItemCreatedEventHandler.cs
│   │   ├── InventoryQuantityUpdatedEventHandler.cs
│   │   └── StockMovementRecordedEventHandler.cs
│   ├── Queries/
│   │   ├── GetInventoryItemByIdQuery.cs
│   │   ├── GetInventoryItemByIdQueryHandler.cs
│   │   ├── GetInventoryByStoreQuery.cs
│   │   ├── GetInventoryByStoreQueryHandler.cs
│   │   ├── GetLowStockItemsQuery.cs
│   │   ├── GetLowStockItemsQueryHandler.cs
│   │   ├── GetStockMovementsQuery.cs
│   │   └── GetStockMovementsQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── InventoryService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── InventoryItem.cs
│   │   └── StockMovement.cs
│   ├── Events/
│   │   ├── InventoryItemCreatedEvent.cs
│   │   ├── InventoryQuantityUpdatedEvent.cs
│   │   └── StockMovementRecordedEvent.cs
│   ├── Exceptions/
│   │   ├── InventoryItemNotFoundException.cs
│   │   ├── StockMovementNotFoundException.cs
│   │   └── InsufficientStockException.cs
│   ├── ValueObjects/
│   │   ├── InventoryItemId.cs
│   │   ├── MovementId.cs
│   │   ├── Quantity.cs
│   │   ├── ReorderLevel.cs
│   │   └── MovementType.cs
│   ├── DependencyInjection.cs
│   └── InventoryService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── InventoryItemConfiguration.cs
│   │   └── StockMovementConfiguration.cs
│   ├── Persistence/
│   │   └── InventoryDbContext.cs
│   ├── Repositories/
│   │   ├── InventoryItemRepository.cs
│   │   ├── StockMovementRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── InventoryService.Infrastructure.csproj
└── InventoryService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **9. SupplierService** (supplier schema)
**Tables**: Suppliers, SupplierContacts, SupplierContactNumbers, SupplierAddresses, PurchaseOrders, PurchaseOrderDetails

```
Services/SupplierService/
├── API/
│   ├── Endpoints/
│   │   ├── SupplierEndpoints.cs
│   │   ├── SupplierContactEndpoints.cs
│   │   ├── SupplierAddressEndpoints.cs
│   │   ├── PurchaseOrderEndpoints.cs
│   │   └── PurchaseOrderDetailEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateSupplierRequestValidator.cs
│   │   ├── AddSupplierContactRequestValidator.cs
│   │   ├── AddSupplierAddressRequestValidator.cs
│   │   ├── CreatePurchaseOrderRequestValidator.cs
│   │   └── AddPurchaseOrderDetailRequestValidator.cs
│   ├── SupplierService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateSupplierCommand.cs
│   │   ├── CreateSupplierCommandHandler.cs
│   │   ├── AddSupplierContactCommand.cs
│   │   ├── AddSupplierContactCommandHandler.cs
│   │   ├── AddSupplierAddressCommand.cs
│   │   ├── AddSupplierAddressCommandHandler.cs
│   │   ├── CreatePurchaseOrderCommand.cs
│   │   ├── CreatePurchaseOrderCommandHandler.cs
│   │   ├── AddPurchaseOrderDetailCommand.cs
│   │   └── AddPurchaseOrderDetailCommandHandler.cs
│   ├── DTOs/
│   │   ├── SupplierDto.cs
│   │   ├── SupplierContactDto.cs
│   │   ├── SupplierContactNumberDto.cs
│   │   ├── SupplierAddressDto.cs
│   │   ├── PurchaseOrderDto.cs
│   │   └── PurchaseOrderDetailDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── SupplierCreatedEventHandler.cs
│   │   ├── SupplierContactAddedEventHandler.cs
│   │   ├── SupplierAddressAddedEventHandler.cs
│   │   ├── PurchaseOrderCreatedEventHandler.cs
│   │   └── PurchaseOrderDetailAddedEventHandler.cs
│   ├── Queries/
│   │   ├── GetSupplierByIdQuery.cs
│   │   ├── GetSupplierByIdQueryHandler.cs
│   │   ├── GetSuppliersQuery.cs
│   │   ├── GetSuppliersQueryHandler.cs
│   │   ├── GetPurchaseOrderByIdQuery.cs
│   │   ├── GetPurchaseOrderByIdQueryHandler.cs
│   │   ├── GetPurchaseOrdersQuery.cs
│   │   └── GetPurchaseOrdersQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── SupplierService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── Supplier.cs
│   │   ├── SupplierContact.cs
│   │   ├── SupplierContactNumber.cs
│   │   ├── SupplierAddress.cs
│   │   ├── PurchaseOrder.cs
│   │   └── PurchaseOrderDetail.cs
│   ├── Events/
│   │   ├── SupplierCreatedEvent.cs
│   │   ├── SupplierContactAddedEvent.cs
│   │   ├── SupplierAddressAddedEvent.cs
│   │   ├── PurchaseOrderCreatedEvent.cs
│   │   └── PurchaseOrderDetailAddedEvent.cs
│   ├── Exceptions/
│   │   ├── SupplierNotFoundException.cs
│   │   ├── SupplierContactNotFoundException.cs
│   │   ├── PurchaseOrderNotFoundException.cs
│   │   └── PurchaseOrderDetailNotFoundException.cs
│   ├── ValueObjects/
│   │   ├── SupplierId.cs
│   │   ├── ContactId.cs
│   │   ├── ContactNumberId.cs
│   │   ├── AddressId.cs
│   │   ├── OrderId.cs
│   │   ├── OrderDetailId.cs
│   │   ├── SupplierName.cs
│   │   ├── TaxIdentificationNumber.cs
│   │   └── OrderStatus.cs
│   ├── DependencyInjection.cs
│   └── SupplierService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── SupplierConfiguration.cs
│   │   ├── SupplierContactConfiguration.cs
│   │   ├── SupplierContactNumberConfiguration.cs
│   │   ├── SupplierAddressConfiguration.cs
│   │   ├── PurchaseOrderConfiguration.cs
│   │   └── PurchaseOrderDetailConfiguration.cs
│   ├── Persistence/
│   │   └── SupplierDbContext.cs
│   ├── Repositories/
│   │   ├── SupplierRepository.cs
│   │   ├── SupplierContactRepository.cs
│   │   ├── SupplierAddressRepository.cs
│   │   ├── PurchaseOrderRepository.cs
│   │   ├── PurchaseOrderDetailRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── SupplierService.Infrastructure.csproj
└── SupplierService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **10. PromotionService** (promotion schema)
**Tables**: DiscountTypes, DiscountCampaigns, DiscountRules, Promotions, PromotionProducts

```
Services/PromotionService/
├── API/
│   ├── Endpoints/
│   │   ├── DiscountTypeEndpoints.cs
│   │   ├── DiscountCampaignEndpoints.cs
│   │   ├── DiscountRuleEndpoints.cs
│   │   ├── PromotionEndpoints.cs
│   │   └── PromotionProductEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateDiscountCampaignRequestValidator.cs
│   │   ├── CreateDiscountRuleRequestValidator.cs
│   │   ├── CreatePromotionRequestValidator.cs
│   │   └── AddPromotionProductRequestValidator.cs
│   ├── PromotionService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateDiscountCampaignCommand.cs
│   │   ├── CreateDiscountCampaignCommandHandler.cs
│   │   ├── CreateDiscountRuleCommand.cs
│   │   ├── CreateDiscountRuleCommandHandler.cs
│   │   ├── CreatePromotionCommand.cs
│   │   ├── CreatePromotionCommandHandler.cs
│   │   ├── AddPromotionProductCommand.cs
│   │   └── AddPromotionProductCommandHandler.cs
│   ├── DTOs/
│   │   ├── DiscountTypeDto.cs
│   │   ├── DiscountCampaignDto.cs
│   │   ├── DiscountRuleDto.cs
│   │   ├── PromotionDto.cs
│   │   └── PromotionProductDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── DiscountCampaignCreatedEventHandler.cs
│   │   ├── DiscountRuleCreatedEventHandler.cs
│   │   ├── PromotionCreatedEventHandler.cs
│   │   └── PromotionProductAddedEventHandler.cs
│   ├── Queries/
│   │   ├── GetDiscountCampaignByIdQuery.cs
│   │   ├── GetDiscountCampaignByIdQueryHandler.cs
│   │   ├── GetActiveDiscountCampaignsQuery.cs
│   │   ├── GetActiveDiscountCampaignsQueryHandler.cs
│   │   ├── GetPromotionByIdQuery.cs
│   │   ├── GetPromotionByIdQueryHandler.cs
│   │   ├── GetActivePromotionsQuery.cs
│   │   └── GetActivePromotionsQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── PromotionService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── DiscountType.cs
│   │   ├── DiscountCampaign.cs
│   │   ├── DiscountRule.cs
│   │   ├── Promotion.cs
│   │   └── PromotionProduct.cs
│   ├── Events/
│   │   ├── DiscountCampaignCreatedEvent.cs
│   │   ├── DiscountRuleCreatedEvent.cs
│   │   ├── PromotionCreatedEvent.cs
│   │   ├── PromotionActivatedEvent.cs
│   │   ├── PromotionDeactivatedEvent.cs
│   │   └── PromotionProductAddedEvent.cs
│   ├── Exceptions/
│   │   ├── DiscountCampaignNotFoundException.cs
│   │   ├── DiscountRuleNotFoundException.cs
│   │   ├── PromotionNotFoundException.cs
│   │   └── PromotionProductNotFoundException.cs
│   ├── ValueObjects/
│   │   ├── DiscountTypeId.cs
│   │   ├── CampaignId.cs
│   │   ├── RuleId.cs
│   │   ├── PromotionId.cs
│   │   ├── PromotionProductId.cs
│   │   ├── DiscountValue.cs
│   │   ├── RuleType.cs
│   │   └── PromotionName.cs
│   ├── DependencyInjection.cs
│   └── PromotionService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── DiscountTypeConfiguration.cs
│   │   ├── DiscountCampaignConfiguration.cs
│   │   ├── DiscountRuleConfiguration.cs
│   │   ├── PromotionConfiguration.cs
│   │   └── PromotionProductConfiguration.cs
│   ├── Persistence/
│   │   └── PromotionDbContext.cs
│   ├── Repositories/
│   │   ├── DiscountTypeRepository.cs
│   │   ├── DiscountCampaignRepository.cs
│   │   ├── DiscountRuleRepository.cs
│   │   ├── PromotionRepository.cs
│   │   ├── PromotionProductRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── PromotionService.Infrastructure.csproj
└── PromotionService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **11. CustomerService** (customer schema)
**Tables**: Customers, CustomerContactNumbers, CustomerAddresses, LoyaltyPrograms, LoyaltyTiers, GiftCards, LoyaltyPointLedger

```
Services/CustomerService/
├── API/
│   ├── Endpoints/
│   │   ├── CustomerEndpoints.cs
│   │   ├── CustomerContactNumberEndpoints.cs
│   │   ├── CustomerAddressEndpoints.cs
│   │   ├── LoyaltyProgramEndpoints.cs
│   │   ├── LoyaltyTierEndpoints.cs
│   │   ├── GiftCardEndpoints.cs
│   │   └── LoyaltyPointLedgerEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateCustomerRequestValidator.cs
│   │   ├── AddCustomerContactRequestValidator.cs
│   │   ├── AddCustomerAddressRequestValidator.cs
│   │   ├── CreateLoyaltyProgramRequestValidator.cs
│   │   ├── CreateGiftCardRequestValidator.cs
│   │   └── AddLoyaltyPointsRequestValidator.cs
│   ├── CustomerService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateCustomerCommand.cs
│   │   ├── CreateCustomerCommandHandler.cs
│   │   ├── AddCustomerContactCommand.cs
│   │   ├── AddCustomerContactCommandHandler.cs
│   │   ├── AddCustomerAddressCommand.cs
│   │   ├── AddCustomerAddressCommandHandler.cs
│   │   ├── CreateLoyaltyProgramCommand.cs
│   │   ├── CreateLoyaltyProgramCommandHandler.cs
│   │   ├── CreateGiftCardCommand.cs
│   │   ├── CreateGiftCardCommandHandler.cs
│   │   ├── AddLoyaltyPointsCommand.cs
│   │   └── AddLoyaltyPointsCommandHandler.cs
│   ├── DTOs/
│   │   ├── CustomerDto.cs
│   │   ├── CustomerContactNumberDto.cs
│   │   ├── CustomerAddressDto.cs
│   │   ├── LoyaltyProgramDto.cs
│   │   ├── LoyaltyTierDto.cs
│   │   ├── GiftCardDto.cs
│   │   └── LoyaltyPointLedgerDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── CustomerCreatedEventHandler.cs
│   │   ├── CustomerContactAddedEventHandler.cs
│   │   ├── CustomerAddressAddedEventHandler.cs
│   │   ├── LoyaltyProgramCreatedEventHandler.cs
│   │   ├── GiftCardCreatedEventHandler.cs
│   │   └── LoyaltyPointsAddedEventHandler.cs
│   ├── Queries/
│   │   ├── GetCustomerByIdQuery.cs
│   │   ├── GetCustomerByIdQueryHandler.cs
│   │   ├── GetCustomersQuery.cs
│   │   ├── GetCustomersQueryHandler.cs
│   │   ├── GetCustomerByMembershipQuery.cs
│   │   ├── GetCustomerByMembershipQueryHandler.cs
│   │   ├── GetLoyaltyProgramsQuery.cs
│   │   ├── GetLoyaltyProgramsQueryHandler.cs
│   │   ├── GetGiftCardByNumberQuery.cs
│   │   └── GetGiftCardByNumberQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── CustomerService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── Customer.cs
│   │   ├── CustomerContactNumber.cs
│   │   ├── CustomerAddress.cs
│   │   ├── LoyaltyProgram.cs
│   │   ├── LoyaltyTier.cs
│   │   ├── GiftCard.cs
│   │   └── LoyaltyPointLedger.cs
│   ├── Events/
│   │   ├── CustomerCreatedEvent.cs
│   │   ├── CustomerContactAddedEvent.cs
│   │   ├── CustomerAddressAddedEvent.cs
│   │   ├── LoyaltyProgramCreatedEvent.cs
│   │   ├── GiftCardCreatedEvent.cs
│   │   ├── GiftCardUsedEvent.cs
│   │   └── LoyaltyPointsAddedEvent.cs
│   ├── Exceptions/
│   │   ├── CustomerNotFoundException.cs
│   │   ├── LoyaltyProgramNotFoundException.cs
│   │   ├── GiftCardNotFoundException.cs
│   │   └── InsufficientGiftCardBalanceException.cs
│   ├── ValueObjects/
│   │   ├── CustomerId.cs
│   │   ├── ContactNumberId.cs
│   │   ├── AddressId.cs
│   │   ├── ProgramId.cs
│   │   ├── TierId.cs
│   │   ├── GiftCardId.cs
│   │   ├── LedgerId.cs
│   │   ├── MembershipNumber.cs
│   │   ├── CardNumber.cs
│   │   └── LoyaltyPoints.cs
│   ├── DependencyInjection.cs
│   └── CustomerService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── CustomerConfiguration.cs
│   │   ├── CustomerContactNumberConfiguration.cs
│   │   ├── CustomerAddressConfiguration.cs
│   │   ├── LoyaltyProgramConfiguration.cs
│   │   ├── LoyaltyTierConfiguration.cs
│   │   ├── GiftCardConfiguration.cs
│   │   └── LoyaltyPointLedgerConfiguration.cs
│   ├── Persistence/
│   │   └── CustomerDbContext.cs
│   ├── Repositories/
│   │   ├── CustomerRepository.cs
│   │   ├── CustomerContactNumberRepository.cs
│   │   ├── CustomerAddressRepository.cs
│   │   ├── LoyaltyProgramRepository.cs
│   │   ├── LoyaltyTierRepository.cs
│   │   ├── GiftCardRepository.cs
│   │   ├── LoyaltyPointLedgerRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── CustomerService.Infrastructure.csproj
└── CustomerService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **12. SalesService** (sales schema)
**Tables**: Sales, SaleDetails, AppliedDiscounts, Returns, ReturnDetails

```
Services/SalesService/
├── API/
│   ├── Endpoints/
│   │   ├── SaleEndpoints.cs
│   │   ├── SaleDetailEndpoints.cs
│   │   ├── AppliedDiscountEndpoints.cs
│   │   ├── ReturnEndpoints.cs
│   │   └── ReturnDetailEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateSaleRequestValidator.cs
│   │   ├── AddSaleDetailRequestValidator.cs
│   │   ├── ApplyDiscountRequestValidator.cs
│   │   ├── ProcessReturnRequestValidator.cs
│   │   └── AddReturnDetailRequestValidator.cs
│   ├── SalesService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateSaleCommand.cs
│   │   ├── CreateSaleCommandHandler.cs
│   │   ├── AddSaleDetailCommand.cs
│   │   ├── AddSaleDetailCommandHandler.cs
│   │   ├── ApplyDiscountCommand.cs
│   │   ├── ApplyDiscountCommandHandler.cs
│   │   ├── ProcessReturnCommand.cs
│   │   ├── ProcessReturnCommandHandler.cs
│   │   ├── AddReturnDetailCommand.cs
│   │   └── AddReturnDetailCommandHandler.cs
│   ├── DTOs/
│   │   ├── SaleDto.cs
│   │   ├── SaleDetailDto.cs
│   │   ├── AppliedDiscountDto.cs
│   │   ├── ReturnDto.cs
│   │   └── ReturnDetailDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── SaleCreatedEventHandler.cs
│   │   ├── SaleDetailAddedEventHandler.cs
│   │   ├── DiscountAppliedEventHandler.cs
│   │   ├── ReturnProcessedEventHandler.cs
│   │   └── ReturnDetailAddedEventHandler.cs
│   ├── Queries/
│   │   ├── GetSaleByIdQuery.cs
│   │   ├── GetSaleByIdQueryHandler.cs
│   │   ├── GetSalesQuery.cs
│   │   ├── GetSalesQueryHandler.cs
│   │   ├── GetSalesByDateRangeQuery.cs
│   │   ├── GetSalesByDateRangeQueryHandler.cs
│   │   ├── GetReturnByIdQuery.cs
│   │   └── GetReturnByIdQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── SalesService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── Sale.cs
│   │   ├── SaleDetail.cs
│   │   ├── AppliedDiscount.cs
│   │   ├── Return.cs
│   │   └── ReturnDetail.cs
│   ├── Events/
│   │   ├── SaleCreatedEvent.cs
│   │   ├── SaleDetailAddedEvent.cs
│   │   ├── DiscountAppliedEvent.cs
│   │   ├── ReturnProcessedEvent.cs
│   │   └── ReturnDetailAddedEvent.cs
│   ├── Exceptions/
│   │   ├── SaleNotFoundException.cs
│   │   ├── SaleDetailNotFoundException.cs
│   │   ├── ReturnNotFoundException.cs
│   │   └── InvalidReturnException.cs
│   ├── ValueObjects/
│   │   ├── SaleId.cs
│   │   ├── SaleDetailId.cs
│   │   ├── AppliedDiscountId.cs
│   │   ├── ReturnId.cs
│   │   ├── ReturnDetailId.cs
│   │   ├── ReceiptNumber.cs
│   │   ├── Money.cs
│   │   └── ReturnReason.cs
│   ├── DependencyInjection.cs
│   └── SalesService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── SaleConfiguration.cs
│   │   ├── SaleDetailConfiguration.cs
│   │   ├── AppliedDiscountConfiguration.cs
│   │   ├── ReturnConfiguration.cs
│   │   └── ReturnDetailConfiguration.cs
│   ├── Persistence/
│   │   └── SalesDbContext.cs
│   ├── Repositories/
│   │   ├── SaleRepository.cs
│   │   ├── SaleDetailRepository.cs
│   │   ├── AppliedDiscountRepository.cs
│   │   ├── ReturnRepository.cs
│   │   ├── ReturnDetailRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── SalesService.Infrastructure.csproj
└── SalesService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **13. PaymentService** (payment schema)
**Tables**: PaymentTypes, PaymentProcessors, PaymentMethods, SalePayments, PaymentDetails, GiftCardTransactions

```
Services/PaymentService/
├── API/
│   ├── Endpoints/
│   │   ├── PaymentTypeEndpoints.cs
│   │   ├── PaymentProcessorEndpoints.cs
│   │   ├── PaymentMethodEndpoints.cs
│   │   ├── SalePaymentEndpoints.cs
│   │   ├── PaymentDetailEndpoints.cs
│   │   └── GiftCardTransactionEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreatePaymentMethodRequestValidator.cs
│   │   ├── ProcessSalePaymentRequestValidator.cs
│   │   ├── ProcessGiftCardTransactionRequestValidator.cs
│   │   └── SettlePaymentRequestValidator.cs
│   ├── PaymentService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreatePaymentMethodCommand.cs
│   │   ├── CreatePaymentMethodCommandHandler.cs
│   │   ├── ProcessSalePaymentCommand.cs
│   │   ├── ProcessSalePaymentCommandHandler.cs
│   │   ├── ProcessGiftCardTransactionCommand.cs
│   │   ├── ProcessGiftCardTransactionCommandHandler.cs
│   │   ├── SettlePaymentCommand.cs
│   │   └── SettlePaymentCommandHandler.cs
│   ├── DTOs/
│   │   ├── PaymentTypeDto.cs
│   │   ├── PaymentProcessorDto.cs
│   │   ├── PaymentMethodDto.cs
│   │   ├── SalePaymentDto.cs
│   │   ├── PaymentDetailDto.cs
│   │   └── GiftCardTransactionDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── PaymentMethodCreatedEventHandler.cs
│   │   ├── SalePaymentProcessedEventHandler.cs
│   │   ├── PaymentSettledEventHandler.cs
│   │   └── GiftCardTransactionProcessedEventHandler.cs
│   ├── Queries/
│   │   ├── GetPaymentMethodByIdQuery.cs
│   │   ├── GetPaymentMethodByIdQueryHandler.cs
│   │   ├── GetPaymentMethodsQuery.cs
│   │   ├── GetPaymentMethodsQueryHandler.cs
│   │   ├── GetSalePaymentByIdQuery.cs
│   │   ├── GetSalePaymentByIdQueryHandler.cs
│   │   ├── GetUnsettledPaymentsQuery.cs
│   │   └── GetUnsettledPaymentsQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── PaymentService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── PaymentType.cs
│   │   ├── PaymentProcessor.cs
│   │   ├── PaymentMethod.cs
│   │   ├── SalePayment.cs
│   │   ├── PaymentDetail.cs
│   │   └── GiftCardTransaction.cs
│   ├── Events/
│   │   ├── PaymentMethodCreatedEvent.cs
│   │   ├── SalePaymentProcessedEvent.cs
│   │   ├── PaymentSettledEvent.cs
│   │   └── GiftCardTransactionProcessedEvent.cs
│   ├── Exceptions/
│   │   ├── PaymentMethodNotFoundException.cs
│   │   ├── SalePaymentNotFoundException.cs
│   │   ├── PaymentProcessingException.cs
│   │   └── InvalidTransactionException.cs
│   ├── ValueObjects/
│   │   ├── PaymentTypeId.cs
│   │   ├── ProcessorId.cs
│   │   ├── MethodId.cs
│   │   ├── PaymentId.cs
│   │   ├── PaymentDetailId.cs
│   │   ├── TransactionId.cs
│   │   ├── TransactionCode.cs
│   │   ├── ApprovalCode.cs
│   │   └── TransactionType.cs
│   ├── DependencyInjection.cs
│   └── PaymentService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── PaymentTypeConfiguration.cs
│   │   ├── PaymentProcessorConfiguration.cs
│   │   ├── PaymentMethodConfiguration.cs
│   │   ├── SalePaymentConfiguration.cs
│   │   ├── PaymentDetailConfiguration.cs
│   │   └── GiftCardTransactionConfiguration.cs
│   ├── Persistence/
│   │   └── PaymentDbContext.cs
│   ├── Repositories/
│   │   ├── PaymentTypeRepository.cs
│   │   ├── PaymentProcessorRepository.cs
│   │   ├── PaymentMethodRepository.cs
│   │   ├── SalePaymentRepository.cs
│   │   ├── PaymentDetailRepository.cs
│   │   ├── GiftCardTransactionRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── PaymentService.Infrastructure.csproj
└── PaymentService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

## **14. ReportingService** (reporting schema)
**Tables**: SalesSnapshots, InventorySnapshots, PromotionEffectiveness

```
Services/ReportingService/
├── API/
│   ├── Endpoints/
│   │   ├── SalesSnapshotEndpoints.cs
│   │   ├── InventorySnapshotEndpoints.cs
│   │   └── PromotionEffectivenessEndpoints.cs
│   ├── Properties/launchSettings.json
│   ├── Validators/
│   │   ├── CreateSalesSnapshotRequestValidator.cs
│   │   ├── CreateInventorySnapshotRequestValidator.cs
│   │   ├── GenerateReportRequestValidator.cs
│   │   └── GetReportDataRequestValidator.cs
│   ├── ReportingService.API.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── Application/
│   ├── Commands/
│   │   ├── CreateSalesSnapshotCommand.cs
│   │   ├── CreateSalesSnapshotCommandHandler.cs
│   │   ├── CreateInventorySnapshotCommand.cs
│   │   ├── CreateInventorySnapshotCommandHandler.cs
│   │   ├── AnalyzePromotionEffectivenessCommand.cs
│   │   └── AnalyzePromotionEffectivenessCommandHandler.cs
│   ├── DTOs/
│   │   ├── SalesSnapshotDto.cs
│   │   ├── InventorySnapshotDto.cs
│   │   └── PromotionEffectivenessDto.cs
│   ├── EventHandlers/
│   │   ├── DomainEventWrapper.cs
│   │   ├── SalesSnapshotCreatedEventHandler.cs
│   │   ├── InventorySnapshotCreatedEventHandler.cs
│   │   └── PromotionEffectivenessAnalyzedEventHandler.cs
│   ├── Queries/
│   │   ├── GetSalesSnapshotsByDateRangeQuery.cs
│   │   ├── GetSalesSnapshotsByDateRangeQueryHandler.cs
│   │   ├── GetInventorySnapshotsByDateQuery.cs
│   │   ├── GetInventorySnapshotsByDateQueryHandler.cs
│   │   ├── GetPromotionEffectivenessQuery.cs
│   │   ├── GetPromotionEffectivenessQueryHandler.cs
│   │   ├── GenerateSalesReportQuery.cs
│   │   └── GenerateSalesReportQueryHandler.cs
│   ├── DependencyInjection.cs
│   └── ReportingService.Application.csproj
├── Domain/
│   ├── Entities/
│   │   ├── SalesSnapshot.cs
│   │   ├── InventorySnapshot.cs
│   │   └── PromotionEffectiveness.cs
│   ├── Events/
│   │   ├── SalesSnapshotCreatedEvent.cs
│   │   ├── InventorySnapshotCreatedEvent.cs
│   │   └── PromotionEffectivenessAnalyzedEvent.cs
│   ├── Exceptions/
│   │   ├── SalesSnapshotNotFoundException.cs
│   │   ├── InventorySnapshotNotFoundException.cs
│   │   └── PromotionEffectivenessNotFoundException.cs
│   ├── ValueObjects/
│   │   ├── SalesSnapshotId.cs
│   │   ├── InventorySnapshotId.cs
│   │   ├── EffectivenessId.cs
│   │   ├── SnapshotDate.cs
│   │   ├── ReportPeriod.cs
│   │   └── RevenueImpact.cs
│   ├── DependencyInjection.cs
│   └── ReportingService.Domain.csproj
├── Infrastructure/
│   ├── Configurations/
│   │   ├── SalesSnapshotConfiguration.cs
│   │   ├── InventorySnapshotConfiguration.cs
│   │   └── PromotionEffectivenessConfiguration.cs
│   ├── Persistence/
│   │   └── ReportingDbContext.cs
│   ├── Repositories/
│   │   ├── SalesSnapshotRepository.cs
│   │   ├── InventorySnapshotRepository.cs
│   │   ├── PromotionEffectivenessRepository.cs
│   │   └── UnitOfWork.cs
│   ├── DependencyInjection.cs
│   └── ReportingService.Infrastructure.csproj
└── ReportingService.Tests/
    └── Integration/
        └── StronglyTypedIdIntegrationTests.cs
```

---

## **Key Patterns and Conventions Summary**

### **Naming Conventions**
- **Service Name**: `[Schema]Service` (e.g., `SharedService`, `AuthService`)
- **Project Files**: `[ServiceName]Service.[Layer].csproj`
- **Namespaces**: `[ServiceName].[Layer].[Sublayer]`
- **Entities**: PascalCase singular (e.g., `Currency`, `User`, `Product`)
- **Value Objects**: Property names (e.g., `CurrencyCode`, `UserId`, `ProductName`)
- **Commands**: `[Action][Entity]Command` (e.g., `CreateCurrencyCommand`)
- **Queries**: `Get[Entity][Criteria]Query` (e.g., `GetCurrencyByCodeQuery`)
- **Events**: `[Entity][Action]Event` (e.g., `CurrencyCreatedEvent`)

### **Architecture Layers**
1. **API Layer**: Minimal APIs, validators, endpoints
2. **Application Layer**: CQRS commands/queries, DTOs, event handlers
3. **Domain Layer**: Entities, events, exceptions, value objects
4. **Infrastructure Layer**: EF configurations, repositories, DbContext
5. **Tests Layer**: Integration tests for strongly typed IDs

### **Features Implemented**
- **CQRS Pattern**: Separate Command and Query handlers
- **Domain Events**: Event-driven architecture
- **Strongly Typed IDs**: Type-safe entity identifiers
- **Value Objects**: Domain modeling with value semantics
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management
- **Minimal APIs**: Endpoint mapping
- **BuildingBlocks Integration**: Consistent cross-cutting concerns

This structure follows Clean Architecture principles with clear separation of concerns, dependency inversion, and consistent patterns across all 14 services.