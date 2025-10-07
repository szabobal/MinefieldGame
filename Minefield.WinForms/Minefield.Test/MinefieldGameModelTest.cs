using Minefield.Model;
using Minefield.Persistence;
using Moq;

namespace Minefield.Test
{
    [TestClass]
    public sealed class MinefieldGameModelTest
    {
        private MinefieldModel _model = null!;
        private MinefieldGrid _mockedGrid = null!;
        private Mock<IMinefieldDataAccess> _mock = null!;

        [TestMethod]
        public void Initialize()
        {
            _mock = new Mock<IMinefieldDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<Stream>()))
                .Returns(() => Task.FromResult(_mockedGrid));

            _model = new MinefieldModel(_mock.Object);
        }

        [TestMethod]
        public async Task MinefieldGameModelLoadTest() 
        {

        }
    }
}
