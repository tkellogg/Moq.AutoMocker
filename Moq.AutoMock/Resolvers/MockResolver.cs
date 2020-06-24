﻿using System;

namespace Moq.AutoMock.Resolvers
{
    /// <summary>
    /// A resolver that resolves requested types with Mock&lt;T&gt; instances.
    /// </summary>
    public class MockResolver : IMockResolver
    {
        private readonly MockBehavior _mockBehavior;
        private readonly DefaultValue _defaultValue;
        private readonly bool _callBase;

        /// <summary>
        /// Initializes an instance of <c>MockResolver</c>.
        /// </summary>
        /// <param name="mockBehavior">Behavior of created mock.</param>
        /// <param name="defaultValue">Specifies the behavior to use when returning default values for 
        /// unexpected invocations on loose mocks created by this instance.</param>
        /// <param name="callBase">Whether the base member virtual implementation will be called 
        /// for created mocks if no setup is matched.</param>
        public MockResolver(MockBehavior mockBehavior, DefaultValue defaultValue, bool callBase)
        {
            _mockBehavior = mockBehavior;
            _defaultValue = defaultValue;
            _callBase = callBase;
        }

        /// <summary>
        /// Resolves requested types with Mock instances.
        /// </summary>
        /// <param name="context">The resolution context.</param>
        public void Resolve(MockResolutionContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            if (!(context.Value is null)) return;

            var mockType = typeof(Mock<>).MakeGenericType(context.RequestType);

            bool mayHaveDependencies = context.RequestType.IsClass 
                                       && !typeof(Delegate).IsAssignableFrom(context.RequestType);
            
            object?[] constructorArgs;
            if (mayHaveDependencies)
            {
                constructorArgs = context.AutoMocker.CreateArguments(context.RequestType);
            }
            else
            {
#pragma warning disable CA1825
                // Compiler complains about empty array literal, but I can't find an alternative that will compile.
                constructorArgs = new object[0];
#pragma warning restore CA1825
            }

            // Create single params argument to appease the compiler. If we don't do this, the
            // compiler will try to pass an object[] to the constructor, which probably won't work.
            var args = new object[constructorArgs.Length + 1];
            args[0] = _mockBehavior;
            constructorArgs.CopyTo(args, 1);

            if (Activator.CreateInstance(mockType, args) is Mock mock)
            {
                mock.DefaultValue = _defaultValue;
                mock.CallBase = _callBase;
                context.Value = mock;
            }
        }
    }
}
