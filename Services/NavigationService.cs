using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitArmLog.Services;

public class NavigationService : INavigationService
{
   
    public Task GoToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        return parameters is not null
            ? Shell.Current.GoToAsync(route, parameters)
            : Shell.Current.GoToAsync(route);
    }

    public Task GoBackAsync() => Shell.Current.GoToAsync("..");
}
