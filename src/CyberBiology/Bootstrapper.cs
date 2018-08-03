using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Ninject;

namespace CyberBiology
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly IKernel _kernel = new StandardKernel();

        public Bootstrapper() : base(true)
        {
            Initialize();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            _kernel.Dispose();

            base.OnExit(sender, e);
        }

        protected override void Configure()
        {
            _kernel.Bind<IWindowManager, WindowManager>().To<WindowManager>().InSingletonScope();
            _kernel.Bind<IEventAggregator, EventAggregator>().To<EventAggregator>().InSingletonScope();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return _kernel.Get(serviceType);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            _kernel.Inject(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<CyberBiologyViewModel>();
        }
    }
}
