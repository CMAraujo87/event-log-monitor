using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace event_log_monitor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private EventLog eventLog;
    private string logToBeMonitored = "Application"; // Default value
    private int maxMessagesToKeep = 100; // Default value
    private List<LogEntry> logEntries;
    private Regex doubleLine = new Regex(@"(\r\n|\n|\r){2,}", RegexOptions.Compiled);

    public MainWindow()
    {
        try
        {
            InitializeComponent();
            PopulateEventLogComboBox();
            StartEventLogMonitoring();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", "Constructor exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private async void PopulateEventLogComboBox()
    {
        try
        {
                EventLog[] eventLogs = EventLog.GetEventLogs();
                var logNames = eventLogs.Select(log => log.Log).ToList();

                // Bind the list of log names to the ComboBox
                LogToBeMonitoredComboBox.ItemsSource = logNames;

                // Optionally set the initial selected item to the default
                LogToBeMonitoredComboBox.Text = logToBeMonitored;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", "PopulateEventLogComboBox exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void StartEventLogMonitoring()
    {
        
        try
        {
            // Detach the existing event handler if the eventLog is already initialized
            if (eventLog != null)
            {
                eventLog.EntryWritten -= OnEntryWritten;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", "Detach exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        try
        {
            if (logEntries != null)
            {
                logEntries.Clear();
            }
            logEntries = new List<LogEntry>();
            LogDataGrid.ItemsSource = null; // Reset to refresh
            LogDataGrid.ItemsSource = logEntries;
            eventLog = new EventLog(logToBeMonitored)
            {
                EnableRaisingEvents = true,
            };
            eventLog.EntryWritten += OnEntryWritten;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", "New Log Monitor exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    private void OnEntryWritten(object sender, EntryWrittenEventArgs e)
    {
        try
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    var message = doubleLine.Replace(e.Entry.Message, Environment.NewLine);
                    var logEntry = new LogEntry
                    {
                        Level = e.Entry.EntryType.ToString(),
                        TimeGenerated = e.Entry.TimeGenerated,
                        Source = e.Entry.Source,
                        Message = message,
                    };
                    AddLogEntry(logEntry);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", "Create Log Entry exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
    
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", "Dispatcher exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddLogEntry(LogEntry logEntry)
    {
        try
        {
            logEntries.Insert(0, logEntry);
            if (logEntries.Count > maxMessagesToKeep)
            {
                logEntries.RemoveAt(logEntries.Count - 1);
            }

            LogDataGrid.ItemsSource = null; // Reset to refresh
            LogDataGrid.ItemsSource = logEntries;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", "AddLogEntry exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SetMaxMessages_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (int.TryParse(MaxMessagesTextBox.Text, out int newLimit))
            {
                if (newLimit > 0)
                {
                    maxMessagesToKeep = newLimit;
                }
                else
                {
                    MessageBox.Show("Please enter a positive number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", "SetMaxMessages_Click exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SetLogToBeMonitored_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(LogToBeMonitoredComboBox.Text))
            {
                // logToBeMonitored = LogToBeMonitored.Text;
                logToBeMonitored = LogToBeMonitoredComboBox.Text;
                StartEventLogMonitoring();
            }
            else
            {
                MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", "SetLogToBeMonitored_Click exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}