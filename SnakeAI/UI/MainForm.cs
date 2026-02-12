using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnakeAI.Core;
using SnakeAI.Rendering;
using SnakeAI.Simulation;

// Alias to avoid Timer ambiguity
using FormsTimer = System.Windows.Forms.Timer;

namespace SnakeAI.UI;

public sealed class MainForm : Form
{
    private readonly UIConfig _uiConfig = new();
    private readonly GameRenderer _gameRenderer = new();
    private readonly NeuralRenderer _neuralRenderer = new();

    private readonly RenderSurface _gameSurface;
    private readonly RenderSurface _networkSurface;
    private Label _generationLabel = null!;
    private Label _fitnessLabel = null!;
    private Label _foodLabel = null!;

    private Simulator _simulator;
    private readonly object _simLock = new();
    private readonly FormsTimer _timer; // <-- alias used here
    private bool _training;
    private bool _trainingBusy;

    public MainForm()
    {
        Text = "SnakeAI - WinForms";
        ClientSize = new Size(_uiConfig.WindowWidth, _uiConfig.WindowHeight);
        StartPosition = FormStartPosition.CenterScreen;

        var config = new GameConfig();
        _simulator = new Simulator(config, seed: 12345, populationSize: 72, networkShape: [6, 12, 8, 4]);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 88f));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 12f));

        _gameSurface = new RenderSurface { Dock = DockStyle.Fill, BackColor = Color.Black };
        _networkSurface = new RenderSurface { Dock = DockStyle.Fill, BackColor = Color.Black };

        _gameSurface.Paint += (_, e) =>
        {
            lock (_simLock)
            {
                _gameRenderer.Draw(e.Graphics, _gameSurface.ClientRectangle, _simulator.PlaybackState);
            }
        };
        _networkSurface.Paint += (_, e) =>
        {
            lock (_simLock)
            {
                _neuralRenderer.Draw(e.Graphics, _networkSurface.ClientRectangle, _simulator.PlaybackController.Network);
            }
        };

        var controls = BuildControlsPanel();

        root.Controls.Add(_gameSurface, 0, 0);
        root.Controls.Add(_networkSurface, 1, 0);
        root.Controls.Add(controls, 0, 1);
        root.SetColumnSpan(controls, 2);

        Controls.Add(root);

        // Initialize timer using alias
        _timer = new FormsTimer { Interval = _uiConfig.TimerIntervalMs };
        _timer.Tick += OnTick;
        _timer.Start();

        UpdateLabels();
    }

    private FlowLayoutPanel BuildControlsPanel()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(10),
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.FromArgb(28, 28, 32)
        };

        _generationLabel = CreateLabel("Generation: 0");
        _fitnessLabel = CreateLabel("Best Fitness: 0");
        _foodLabel = CreateLabel("Food Eaten: 0");

        var startBtn = new Button { Text = "Start Training", Width = 120, Height = 32 };
        var stopBtn = new Button { Text = "Stop", Width = 80, Height = 32 };
        var resetBtn = new Button { Text = "Reset", Width = 80, Height = 32 };

        startBtn.Click += (_, _) => _training = true;
        stopBtn.Click += (_, _) => _training = false;
        resetBtn.Click += (_, _) => ResetSimulation();

        panel.Controls.Add(_generationLabel);
        panel.Controls.Add(_fitnessLabel);
        panel.Controls.Add(_foodLabel);
        panel.Controls.Add(startBtn);
        panel.Controls.Add(stopBtn);
        panel.Controls.Add(resetBtn);

        return panel;
    }

    private static Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            ForeColor = Color.WhiteSmoke,
            AutoSize = true,
            Margin = new Padding(10, 8, 10, 0),
            Font = new Font("Segoe UI", 10f, FontStyle.Regular)
        };
    }

    private async void OnTick(object? sender, EventArgs e)
    {
        if (_training && !_trainingBusy)
        {
            _trainingBusy = true;
            await Task.Run(() =>
            {
                for (var i = 0; i < _uiConfig.TrainingBurstPerTick; i++)
                {
                    lock (_simLock)
                    {
                        _simulator.EvolveOneGeneration();
                    }
                }
            });
            _trainingBusy = false;
        }

        lock (_simLock)
        {
            _simulator.UpdatePlaybackStep();
            UpdateLabels();
        }
        _gameSurface.Invalidate();
        _networkSurface.Invalidate();
    }

    private void ResetSimulation()
    {
        _training = false;
        var config = new GameConfig();
        lock (_simLock)
        {
            _simulator = new Simulator(config, seed: 12345, populationSize: 72, networkShape: [6, 12, 8, 4]);
            UpdateLabels();
        }
    }

    private void UpdateLabels()
    {
        _generationLabel.Text = $"Generation: {_simulator.Population.Generation}";
        _fitnessLabel.Text = $"Best Fitness: {_simulator.Population.BestFitness:F1}";
        _foodLabel.Text = $"Food Eaten: {_simulator.LastFoodEaten}";
    }
}
