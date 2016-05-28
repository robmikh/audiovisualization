using AudioVisualization.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioVisualization.ViewModels
{
    /// <summary>
    /// The ViewModel for the AppShell.
    /// </summary>
    class AppShellViewModel : ViewModel
    {
        public AppShellViewModel()
        {
            NavigationBarMenuItems = GetNavigationBarMenuItems();

            BottomNavigationBarMenuItems = GetBottomNavigationBarMenuItems();

            OnPropertyChanged("NavigationBarMenuItems");
            OnPropertyChanged("BottomNavigationBarMenuItems");
        }

        private List<INavigationBarMenuItem> GetNavigationBarMenuItems()
        {
            var result = new List<INavigationBarMenuItem>();

            // Add navigation items here.

            return result;
        }

        private List<INavigationBarMenuItem> GetBottomNavigationBarMenuItems()
        {
            var result = new List<INavigationBarMenuItem>();

            // Add navigation items here.

            return result;
        }

        /// <summary>
        /// The navigation bar items at the bottom.
        /// </summary>
        public List<INavigationBarMenuItem> BottomNavigationBarMenuItems { get; }

        /// <summary>
        /// The navigation bar items at the top.
        /// </summary>
        public List<INavigationBarMenuItem> NavigationBarMenuItems { get; private set; }
    }
}
