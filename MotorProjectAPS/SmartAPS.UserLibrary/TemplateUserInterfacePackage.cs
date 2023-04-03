using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mozart.Studio.TaskModel;
using Mozart.Studio.TaskModel.Projects;
using Mozart.Studio.Projects;
using SmartAPS.UserLibrary.Utils;

namespace Template.UserInterface
{
    public class TemplateUserInterfacePackage : BaseUserInterfacePackage
    {
        protected override void OnProjectAdded(object sender, ProjectEventArgs e)
        {
            ShopCalendarLoader.InitFactoryTime((IModelProject)this.Project);
            
            base.OnProjectAdded(sender, e);
        }
    }
}
