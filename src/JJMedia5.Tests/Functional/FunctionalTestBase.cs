using JJMedia5.Tests.Helpers;

namespace JJMedia5.Tests.Functional {
    public abstract class FunctionalTestBase {

        protected IServiceProvider _provider {  get; set; }

        protected FunctionalTestBase() {
            _provider = SetupHelper.GetDependencyProvider();
        }
    }
}
