using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Durak.Common;

namespace DurakCommon
{
    /// <summary>
    /// Represents a control that can be used to render a game state
    /// </summary>
    public partial class GameStateVisualizer : UserControl
    {
        /// <summary>
        /// The game state to render
        /// </summary>
        private GameState myState;
        
        /// <summary>
        /// Creates a new empty game state visualizer
        /// </summary>
        public GameStateVisualizer()
        {
            InitializeComponent();

            if (dgvMainView != null)
            {
                dgvMainView.Columns.Add("Name", "Name");
                dgvMainView.Columns[0].Width = Width / 2;
                dgvMainView.Columns.Add("Value", "Value");
                dgvMainView.Columns[1].Width = Width / 2;
            }
        }

        /// <summary>
        /// Overrides the OnResize event, this will resize the volumns
        /// </summary>
        /// <param name="e">The blank event arguments for the event</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (dgvMainView.Columns.Count == 2)
            {
                dgvMainView.Columns[0].Width = Width / 2;
                dgvMainView.Columns[1].Width = Width / 2;
            }
        }

        /// <summary>
        /// Sets the game state for this visualizer to render
        /// </summary>
        /// <param name="state">The state to render</param>
        public void SetState(GameState state)
        {
            myState = state;

            StateParameter[] states = state.GetParameterCollection();

            foreach (StateParameter parameter in states)
            {
                dgvMainView.Rows.Add(parameter.Name, parameter.RawValue);
            }

            myState.OnStateChangedUnSilenceable += StateChanged;
            myState.OnCleared += StateCleared;
        }

        /// <summary>
        /// Invoked when the state has been cleared
        /// </summary>
        /// <param name="sender">The object to invoke the event (the GameState)</param>
        /// <param name="e">The empty event argument</param>
        private void StateCleared(object sender, EventArgs e)
        {
            dgvMainView.Rows.Clear();
        }

        /// <summary>
        /// Invoked when a state parameter has changed
        /// </summary>
        /// <param name="sender">The object to invoke the event (the GameState)</param>
        /// <param name="e">The state parameter that has been updated</param>
        private void StateChanged(object sender, StateParameter e)
        {
            bool exists = false;

            for (int index = 0; index < dgvMainView.Rows.Count; index++)
            {
                if (dgvMainView.Rows[index].Cells[0].Value.Equals(e.Name))
                {
                    dgvMainView.Rows[index].Cells[1].Value = e.RawValue;
                    exists = true;
                    break;
                }
            }

            if (!exists)
                dgvMainView.Rows.Add(e.Name, e.RawValue);
        }
    }
}
