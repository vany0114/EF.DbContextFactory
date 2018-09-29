namespace EF.DbContextFactory.IntegrationTest.StructureMap.DependencyResolution {
    using System.Web;
    using EF.DbContextFactory.IntegrationTest.StructureMap.App_Start;
    using global::StructureMap.Web.Pipeline;

    public class StructureMapScopeModule : IHttpModule {
        #region Public Methods and Operators

        public void Dispose() {
        }

        public void Init(HttpApplication context) {
            context.BeginRequest += (sender, e) => StructuremapMvc.StructureMapDependencyScope.CreateNestedContainer();
            context.EndRequest += (sender, e) => {
                HttpContextLifecycle.DisposeAndClearAll();
                StructuremapMvc.StructureMapDependencyScope.DisposeNestedContainer();
            };
        }

        #endregion
    }
}