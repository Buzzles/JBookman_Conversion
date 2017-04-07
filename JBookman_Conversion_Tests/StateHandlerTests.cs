using FluentAssertions;
using JBookman_Conversion.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static JBookman_Conversion.Engine.StateHandler;

namespace JBookman_Conversion_Tests
{
    [TestClass]
    public class StateHandlerTests
    {
        private StateHandler _stateHandler;

        [TestInitialize]
        public void Initialise()
        {
            _stateHandler = new StateHandler();
        }

        [TestMethod]
        public void Menu_ToWorld_Returns_WorldState()
        { 
            var current = _stateHandler.MoveNext(ProcessAction.ToWorld);
            current.Should().Be(ProcessState.World);
        }
    }
}
