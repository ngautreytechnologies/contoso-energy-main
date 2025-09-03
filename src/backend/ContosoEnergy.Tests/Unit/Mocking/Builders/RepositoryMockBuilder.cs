using Moq;
using Ardalis.Specification;
using ContosoEnergy.Core.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ContosoEnergy.Tests.Unit.Mocking.Builders
{
    /// <summary>
    /// Abstract, generic base builder for mocking repository interfaces in unit tests.
    /// Uses the CRTP pattern to allow fluent methods in the base class to return the derived type.
    /// </summary>
    /// <typeparam name="T">The entity type (e.g., Job, Customer).</typeparam>
    /// <typeparam name="TBuilder">The derived builder type.</typeparam>
    /// <typeparam name="TInterface">The repository interface type being mocked (e.g., IJobRepository).</typeparam>
    public abstract class RepositoryMockBuilderBase<T, TBuilder, TInterface>
        where T : class
        where TBuilder : RepositoryMockBuilderBase<T, TBuilder, TInterface>
        where TInterface : class
    {
        /// <summary>
        /// Internal mock object for the repository.
        /// </summary>
        /// <remarks>
        /// This is the underlying <see cref="Moq.Mock{TInterface}"/> instance used by the builder.
        /// All fluent setup methods operate on this mock.
        /// <para>
        /// Initialized once per builder instance. The <see cref="Build"/> method returns the actual 
        /// <typeparamref name="TInterface"/> object for use in tests.
        /// </para>
        /// <para>
        /// <b>Design note:</b> Supports covariant return types via CRTP pattern, allowing 
        /// fluent methods to return <typeparamref name="TBuilder"/> instead of the base class.
        /// </para>
        /// </remarks>
        protected readonly Mock<IRepositoryBase<T>> _mockRepo = new();

        /// <summary>
        /// Sets up <c>AddAsync</c> to return the entity passed in.
        /// </summary>
        /// <remarks>
        /// Captures the argument to make the mock behave like the real repository.
        /// Covariant return allows fluent chaining of derived builders.
        /// </remarks>
        /// <returns>The derived builder instance for fluent chaining.</returns>
        public TBuilder WithAddAsync()
        {
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((T entity, CancellationToken _) => entity);

            return (TBuilder)this;
        }

        /// <summary>
        /// Sets up <c>SaveChangesAsync</c> to return a number of affected rows.
        /// </summary>
        /// <param name="affectedRows">Number of rows affected (default is 1).</param>
        /// <remarks>
        /// Covariant return allows fluent method chaining in derived builders.
        /// </remarks>
        public TBuilder WithSaveChangesAsync(int affectedRows = 1)
        {
            _mockRepo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(affectedRows);

            return (TBuilder)this;
        }

        /// <summary>
        /// Sets up <c>ListAsync</c> to return a provided list of entities.
        /// </summary>
        /// <param name="items">The list of entities to return.</param>
        /// <remarks>
        /// Useful for injecting predictable test data.
        /// Covariant return ensures fluent chaining.
        /// </remarks>
        public TBuilder WithListAsync(List<T> items)
        {
            _mockRepo.Setup(r => r.ListAsync(It.IsAny<ISpecification<T>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(items);

            return (TBuilder)this;
        }

        /// <summary>
        /// Sets up <c>GetByIdAsync</c> to return a specific entity for a given id.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="entity">The entity to return.</param>
        /// <remarks>
        /// Covariant return supports fluent chaining of builder methods.
        /// </remarks>
        public TBuilder WithGetById(int id, T entity)   
        {   
            _mockRepo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(entity);

            return (TBuilder)this;
        }

        /// <summary>
        /// Returns the underlying mocked repository interface.
        /// </summary>
        /// <remarks>
        /// Use this method after all fluent setup calls to get the <typeparamref name="TInterface"/>
        /// object for dependency injection or direct testing.
        /// </remarks>
        public TInterface Build() => _mockRepo.As<TInterface>().Object;

    }
}
