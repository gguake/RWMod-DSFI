using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace DSFI.Jobs
{
    public enum ReportStringState
    {
        None,
        A,
        B,
        C,
        D,
        E
    }

    public abstract class IdleJobDriver : JobDriver
    {
        public override string GetReport()
        {
            IdleJobDef def = this.job.def as IdleJobDef;
            if (def != null)
            {
                if (this.reportState == ReportStringState.A)
                {
                    return this.ReportStringProcessed(def.reportStringA);
                }
                else if (this.reportState == ReportStringState.B)
                {
                    return this.ReportStringProcessed(def.reportStringB);
                }
                else if (this.reportState == ReportStringState.C)
                {
                    return this.ReportStringProcessed(def.reportStringC);
                }
                else if (this.reportState == ReportStringState.D)
                {
                    return this.ReportStringProcessed(def.reportStringD);
                }
                else if (this.reportState == ReportStringState.E)
                {
                    return this.ReportStringProcessed(def.reportStringE);
                }
            }

            return base.GetReport();
        }

        protected ReportStringState reportState = ReportStringState.None;
    }
}
