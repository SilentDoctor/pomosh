using System.ComponentModel;
using ATMMobileConnection.Models;

namespace ATMMobileConnection.Forms;

public class HistoryForm : Form
{
    public HistoryForm(List<Operation> operations)
    {
        Text = "История операций";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(760, 360);

        var dgvHistory = new DataGridView
        {
            Dock = DockStyle.Top,
            Height = 300,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Дата",
            DataPropertyName = nameof(Operation.Date),
            Width = 150
        });
        dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Тип",
            DataPropertyName = nameof(Operation.Type),
            Width = 120
        });
        dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Сумма",
            DataPropertyName = nameof(Operation.Amount),
            Width = 110
        });
        dgvHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Описание",
            DataPropertyName = nameof(Operation.Description),
            Width = 220
        });
        dgvHistory.Columns.Add(new DataGridViewCheckBoxColumn
        {
            HeaderText = "Успешно",
            DataPropertyName = nameof(Operation.IsSuccessful),
            Width = 80
        });

        dgvHistory.DataSource = new BindingList<Operation>(operations);

        var btnClose = new Button
        {
            Text = "Закрыть",
            Width = 100,
            Height = 35,
            Location = new Point(320, 312)
        };
        btnClose.Click += (_, _) => Close();

        Controls.Add(dgvHistory);
        Controls.Add(btnClose);
    }
}
