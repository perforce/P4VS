using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perforce.P4VS
{
    public class ActiveChangeListCombo 
    {
        private P4VsProviderService SccService;

        private Dictionary<int, string> ActiveChangeListComboChoicesMap = new Dictionary<int, string>();

        public ActiveChangeListCombo(P4VsProviderService service)
        {
            SccService = service;
            InitActiveChangeListComboChoicesMap();
        }

        public string[] ActiveChangeListComboChoices
        {
            get { return ActiveChangeListComboChoicesMap.Values.ToArray(); }
        }

        public void addActiveChangeListComboChoicesMap(int key, string val)
        {
            ActiveChangeListComboChoicesMap.Add(key, val);
        }

        private void InitActiveChangeListComboChoicesMap()
        {
            bool promptPreferenceSet = Preferences.LocalSettings.GetBool("PromptForChanglist", true);
            int ListSize = 3;
            IList<P4.Changelist> changes = null;
            if ((SccService != null) && (SccService.ScmProvider != null))
            {
                changes = SccService.ScmProvider.GetAvailibleChangelists(10);
            }
            if (changes != null)
            {
                ListSize += changes.Count;
            }
            addActiveChangeListComboChoicesMap(-2, Resources.NoActiveChangelist);
            addActiveChangeListComboChoicesMap(-1, Resources.NewChangelist);
            addActiveChangeListComboChoicesMap(0, Resources.Changelist_Default);

            if (changes != null)
            {
                foreach (P4.Changelist change in changes)
                {
                    string d = change.Description.Replace("\r\n", " ");
                    d = d.Replace('\n', ' ');
                    d = d.Replace('\r', ' ');

                    addActiveChangeListComboChoicesMap(change.Id,
                        string.Format("{0} {1}", change.Id.ToString(), d));
                }
            }
        }

        private string _activeChangeListComboChoice = Resources.Changelist_Default;
        public string ActiveChangeListComboChoice { get { return _activeChangeListComboChoice; } }

        private int _activeChangeList = 0;
        public int ActiveChangeList { 
            get { return _activeChangeList; }
            set { _activeChangeList = value; }
        }

        public void SetActiveChangeList(int id)
        {
            if ((ActiveChangeListComboChoicesMap == null) || (ActiveChangeListComboChoicesMap.Count < 2))
            {
                InitActiveChangeListComboChoicesMap();
            }
            if (ActiveChangeListComboChoicesMap.ContainsKey(id))
            {
                _activeChangeList = id;
                _activeChangeListComboChoice = ActiveChangeListComboChoicesMap[id];
            }
        }

        public void SetActiveChangeList(string newVal)
        {
            if ((ActiveChangeListComboChoicesMap == null) || (ActiveChangeListComboChoicesMap.Count < 2))
            {
                InitActiveChangeListComboChoicesMap();
            }
            int breakIdx = newVal.IndexOf(' ');
            if (breakIdx >= 0)
            {
                int id;
                string changeId = newVal.Substring(0, breakIdx);
                if (int.TryParse(changeId, out id))
                {
                    if (ActiveChangeListComboChoicesMap.ContainsKey(id))
                    {
                        _activeChangeList = id;
                        _activeChangeListComboChoice = newVal;
                        return;
                    }
                }
                if (Preferences.LocalSettings.GetBool("SetEnvironmentVars", true))
                {
                    if (id == 0)
                    {
                        Environment.SetEnvironmentVariable("P4VS_ACTIVE_CHANGELIST", "default");
                    }
                    else
                    {
                        Environment.SetEnvironmentVariable("P4VS_ACTIVE_CHANGELIST", changeId);
                    }
                }
            }
            _activeChangeListComboChoice = Resources.Changelist_Default;
            _activeChangeList = 0;
        }
    }
}
