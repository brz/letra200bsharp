using InTheHand.Bluetooth;
using letra200bsharp;

namespace CetraSharp.WinFormsTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await RefreshDevices();

        }

        private async Task RefreshDevices()
        {
            BluetoothDevicesListBox.DataSource = new List<BluetoothDevice>();
            RefreshButton.Enabled = false;
            var options = new RequestDeviceOptions { AcceptAllDevices = false };
            var filter = new BluetoothLEScanFilter { NamePrefix = "Letratag" };
            options.Filters.Add(filter);
            var devices = await Bluetooth.ScanForDevicesAsync(options);
            if (devices.Count == 0)
            {
                MessageBox.Show("Dymo LetraTag 200B not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            BluetoothDevicesListBox.DataSource = devices;
            RefreshButton.Enabled = true;
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await RefreshDevices();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PathLabel.Text = ofd.FileName;
            }
        }

        private async void PrintButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(PathLabel.Text))
            {
                MessageBox.Show("No image file selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (BluetoothDevicesListBox.SelectedIndex == -1)
            {
                MessageBox.Show("No device selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PrintButton.Enabled = false;

            var bluetoothDevice = await BluetoothDevice.FromIdAsync((string)BluetoothDevicesListBox.SelectedValue!);

            if (bluetoothDevice != null)
            {
                var services = await bluetoothDevice.Gatt.GetPrimaryServicesAsync();
                var uuid = services.FirstOrDefault(s => s.Uuid.ToString().Length == 36)?.Uuid;
                if (uuid.HasValue)
                {
                    var serv = await bluetoothDevice.Gatt.GetPrimaryServiceAsync(uuid.Value);

                    var characteristics = await serv.GetCharacteristicsAsync();

                    if (characteristics != null && characteristics.Count > 0)
                    {
                        var imageBytes = await File.ReadAllBytesAsync(PathLabel.Text);
                        var job = LetraHelper.CreateJob(imageBytes);

                        foreach (var jobPart in job)
                        {
                            await characteristics[0].WriteValueWithoutResponseAsync(jobPart);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Unable to determine device characteristics.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Unable to determine UUID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Unable to connect to selected bluetooth device.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            PrintButton.Enabled = true;
        }
    }
}
