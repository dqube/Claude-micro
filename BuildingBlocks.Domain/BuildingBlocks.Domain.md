PosStore.Libraries.Domain/
├── Entities/
│   ├── Entity.cs
│   ├── AggregateRoot.cs
│   ├── IAuditableEntity.cs
│   └── ISoftDeletable.cs
├── ValueObjects/
│   ├── ValueObject.cs
│   ├── SingleValueObject.cs
│   └── Enumeration.cs
├── StronglyTypedIds/
│   ├── IStronglyTypedId.cs
│   ├── StronglyTypedId.cs
│   ├── IntId.cs
│   ├── LongId.cs
│   ├── GuidId.cs
│   └── StringId.cs
├── DomainEvents/
│   ├── IDomainEvent.cs
│   ├── IDomainEventDispatcher.cs
│   ├── DomainEventDispatcher.cs
│   ├── DomainEventBase.cs
│   └── IDomainEventHandler.cs
├── Repository/
│   ├── IRepository.cs
│   ├── IReadOnlyRepository.cs
│   ├── IUnitOfWork.cs
│   └── RepositoryBase.cs
├── Specifications/
│   ├── ISpecification.cs
│   ├── Specification.cs
│   ├── AndSpecification.cs
│   ├── OrSpecification.cs
│   ├── NotSpecification.cs
│   └── SpecificationEvaluator.cs
├── Exceptions/
│   ├── DomainException.cs
│   ├── BusinessRuleValidationException.cs
│   ├── AggregateNotFoundException.cs
│   ├── ConcurrencyException.cs
│   └── InvalidOperationDomainException.cs
├── BusinessRules/
│   ├── IBusinessRule.cs
│   ├── BusinessRuleBase.cs
│   └── CompositeBusinessRule.cs
├── Common/
│   ├── Money.cs
│   ├── DateRange.cs
│   ├── Address.cs
│   ├── Email.cs
│   └── PhoneNumber.cs
└── Extensions/
    └── DomainExtensions.cs