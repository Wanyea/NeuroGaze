using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
namespace NeuroGaze_Datya_Synthesis
{
    

    public class SelectionResult
    {
        [Name("ID")]
        public int ParticipantID { get; set; }

        [Name("Round Duration")]
        public double RoundDuration { get; set; }

        [Name("Error Count")]
        public int ErrorCount { get; set; }

        [Name("Total Eye Interactables")]
        public int TotalInteractables { get; set; }

        public string SelectionTechnique { get; set; }
    }


}
