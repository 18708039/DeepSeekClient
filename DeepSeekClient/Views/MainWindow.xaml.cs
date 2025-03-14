using DeepSeekClient.Events;
using Prism.Events;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeepSeekClient.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(IEventAggregator eventAggregator)
    {
        InitializeComponent();

        eventAggregator.GetEvent<CollectionChangedEvent>().Subscribe(ScrollToBottom, ThreadOption.UIThread);
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        if (myListView.Items.Count > 0)
        {
            var lastItem = myListView.Items[myListView.Items.Count - 1];
            myListView.ScrollIntoView(lastItem);
        }
    }
}