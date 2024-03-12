using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace NeuroGaze_Datya_Synthesis
{

    public sealed class SelectionResultMap : ClassMap<SelectionResult>
    {
        public SelectionResultMap()
        {
            Map(m => m.ParticipantID).Name("ID");
            Map(m => m.RoundDuration).Name("Round Duration");
            Map(m => m.ErrorCount).Name("Error Count");
            Map(m => m.TotalInteractables).Name("Total Eye Interactables");
            // Explicitly ignore the SelectionTechnique property
            Map(m => m.SelectionTechnique).Ignore();
        }
    }

}
