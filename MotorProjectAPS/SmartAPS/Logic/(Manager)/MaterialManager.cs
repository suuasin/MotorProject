using Mozart.SeePlan.DataModel;
using Mozart.SeePlan.General.DataModel;
using Mozart.SeePlan.Simulation;
using Mozart.Simulation.EnterpriseLibrary;
using Mozart.Task.Execution;
using SmartAPS.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAPS.Logic
{
    [FeatureBind()]
    [FEMyMethod]
    public partial class MaterialManager
    {
        private static MaterialManager _materialManager;

        public static MaterialManager Instance
        {
            get
            {
                if (_materialManager == null)
                    _materialManager = SAPSObjectMapper.Create<MaterialManager>();

                return _materialManager;
            }
        }

        #region Properties

        AoFactory _factory;
        List<IReplenishEvents> _replenishHandler;
        List<IMatPlan> _materials;
        List<IMatBom> _matBoms;
        List<MatFilter> _matFilters;
        AoSchedule _replenishSchedule;

        public IList<IMatPlan> Materials
        {
            get { return this._materials; }
        }

        public IList<IMatBom> MatBoms
        {
            get { return this._matBoms; }
        }

        internal IList<MatFilter> MatFilters
        {
            get { return this._matFilters; }
        }

        /// <summary>
        /// Gets replenish schedule table.
        /// </summary>
        public IList<AoScheduleItem> ScheduleTable
        {
            get { return this._replenishSchedule.ScheduleTable.ToList(); }
        }

        public object Tag
        {
            get { return this._replenishSchedule.Tag; }
        }

        #endregion

        #region Constructors
        public MaterialManager()
        {
            _materials = new List<IMatPlan>();
            _matBoms = new List<IMatBom>();
            _matFilters = new List<MatFilter>();
        }

        #endregion

        #region Methods

        public void Register(IReplenishEvents events)
        {
            this._replenishHandler.Add(events);
        }

        internal void Initialize(AoFactory factory)
        {
            _factory = factory;
            this._replenishSchedule = new AoSchedule(factory);
            _replenishHandler = new List<IReplenishEvents>();

            var replenishEvents = SAPSObjectMapper.CreateReplenishEvents();
            if (replenishEvents != null)
                MaterialManager.Instance.Register(replenishEvents);

            var list = new List<AoScheduleItem>();
            foreach (IReplenishEvents handler in this._replenishHandler)
            {
                var e = this.BuildEvents(handler, this._factory);
                if (e != null)
                    list.AddRange(e);
            }

            this._replenishSchedule.ScheduleTable = list;
            this._replenishSchedule.Notify += this.onReplenishNotify;
        }

        private IEnumerable<AoScheduleItem> BuildEvents(IReplenishEvents handler, AoFactory factory)
        {
            var list = new List<AoScheduleItem>();
            var nowDt = factory.NowDT;

            foreach (var mat in Materials)
            {
                if (mat.ReplenishDate < nowDt)
                    mat.ReplenishDate = nowDt;

                if (mat.IsEmpty)
                    continue;

                if (mat.MatType == MatType.Inv)
                    continue;

                var sch = list.Where(r => r.EventTime == factory.FixTime(mat.ReplenishDate)).FirstOrDefault();

                if (sch == null)
                    list.Add(new AoScheduleItem(factory.FixTime(mat.ReplenishDate), 0, new List<IMatPlan> { mat }));
                else
                    (sch.Tag as List<IMatPlan>).Add(mat);
            }

            return list.OrderBy(x => x.EventTime);
        }

        public IEnumerable<IMatPlan> FindMatPlans(string matType)
        {
            return Materials.Where(r => r.MaterialType == matType);
        }

        public IEnumerable<IMatBom> FindMatBoms(Product prod, Step step)
        {
            return MatBoms.Where(r => r.Product.Equals(prod) && r.Step.Equals(step));
        }

        public void AddMatFilter(IMatBom bom, IHandlingBatch hb, AoEquipment aeqp)
        {
            MatFilter mf = new MatFilter
            {
                Bom = bom,
                Hb = hb,
                Aeqp = aeqp
            };

            this.MatFilters.Add(mf);
        }

        public void RemoveFilter(IHandlingBatch hb, Step step)
        {
            for(int i = this.MatFilters.Count - 1; i >= 0; i--)
            {
                var mf = this.MatFilters[i];

                if (mf.Hb.Equals(hb) && mf.Bom.Step.Equals(step))
                    this.MatFilters.Remove(mf);
            }
        }

        public IEnumerable<MatFilter> FindFilter(IMatPlan plan)
        {
            var mfs = this.MatFilters.Where(r => r.Bom.MaterialType == plan.MaterialType);

            return mfs;
        }

        private void onReplenishNotify(object sendor)
        {
            var factory = this._factory;
            var mats = (IEnumerable<IMatPlan>)this._replenishSchedule.Tag;
            foreach (IReplenishEvents events in this._replenishHandler)
            {
                events.OnEvent(factory, mats);
            }
        }

        #endregion
    }
}
