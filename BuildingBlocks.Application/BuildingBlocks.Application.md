BuildingBlocks.Application/
├── CQRS/
│   ├── Commands/
│   │   ├── ICommand.cs
│   │   ├── ICommandWithResult.cs
│   │   ├── ICommandHandler.cs
│   │   ├── ICommandHandlerWithResult.cs
│   │   └── CommandBase.cs
│   ├── Queries/
│   │   ├── IQuery.cs
│   │   ├── IQueryHandler.cs
│   │   ├── QueryBase.cs
│   │   ├── PagedQuery.cs
│   │   ├── PagedResult.cs
│   │   └── SortingQuery.cs
│   ├── Events/
│   │   ├── IEvent.cs
│   │   ├── IEventHandler.cs
│   │   ├── IIntegrationEvent.cs
│   │   ├── IntegrationEventBase.cs
│   │   └── DomainEventNotification.cs
│   └── Messages/
│       ├── IMessage.cs
│       ├── IStreamMessage.cs
│       ├── MessageBase.cs
│       └── IMessageContext.cs
├── Behaviors/
│   ├── IPipelineBehavior.cs
│   ├── LoggingBehavior.cs
│   ├── ValidationBehavior.cs
│   ├── CachingBehavior.cs
│   ├── TransactionBehavior.cs
│   ├── PerformanceBehavior.cs
│   └── RetryBehavior.cs
├── Services/
│   ├── IApplicationService.cs
│   ├── ApplicationServiceBase.cs
│   ├── IDomainEventService.cs
│   └── DomainEventService.cs
├── Validation/
│   ├── IValidator.cs
│   ├── IValidationRule.cs
│   ├── ValidationResult.cs
│   ├── ValidationError.cs
│   ├── CompositeValidator.cs
│   └── ValidatorBase.cs
├── Caching/
│   ├── ICacheService.cs
│   ├── ICacheKey.cs
│   ├── CacheKey.cs
│   ├── CacheSettings.cs
│   └── CachePolicy.cs
├── Messaging/
│   ├── IMessageBus.cs
│   ├── IEventBus.cs
│   ├── IMessageHandler.cs
│   ├── IMessagePublisher.cs
│   └── MessageMetadata.cs
├── DTOs/
│   ├── BaseDto.cs
│   ├── AuditableDto.cs
│   └── PagedDto.cs
├── Mapping/
│   ├── IMapper.cs
│   ├── IMappingProfile.cs
│   └── MapperBase.cs
├── Security/
│   ├── ICurrentUserService.cs
│   ├── IPermissionService.cs
│   ├── UserContext.cs
│   └── SecurityContext.cs
├── Inbox/
│   ├── IInboxService.cs
│   ├── InboxMessage.cs
│   ├── InboxMessageStatus.cs
│   └── IInboxProcessor.cs
├── Outbox/
│   ├── IOutboxService.cs
│   ├── OutboxMessage.cs
│   ├── OutboxMessageStatus.cs
│   └── IOutboxProcessor.cs
└── Extensions/
    ├── ServiceCollectionExtensions.cs
    ├── ApplicationExtensions.cs
    └── MediatorExtensions.cs