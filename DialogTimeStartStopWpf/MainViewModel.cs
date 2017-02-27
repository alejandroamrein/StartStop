using DialogTimeStartStopWpf.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DialogTimeStartStopWpf
{
    public class MainViewModel : ViewModelBase
    {
        private Timer _timer;
        private Intervalls _intervalls;
        private string _feedbackText;
        private RelayCommand _startCommand;
        private RelayCommand _startExCommand;
        private RelayCommand _stopCommand;
        private RelayCommand _stopExCommand;
        private RelayCommand _pauseCommand;
        private RelayCommand _resumeCommand;
        private Visibility _startCmdVisibility;
        private Visibility _stopCmdVisibility;
        private Visibility _pauseCmdVisibility;
        private Visibility _resumeCmdVisibility;
        private Brush _feedbackColor;
        private Brush _red = new SolidColorBrush(Colors.IndianRed);
        private Brush _black = new SolidColorBrush(Colors.Black);
        private double _StartExWidth;
        private double _StartExHeight;
        private double _StopExWidth;
        private double _StopExHeight;

        public string Version
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var title = string.Format("Dialog TIME - Start/Stop - Version {0}.{1}", version.Major, version.Minor);
                return title;
            }
        }
        public double StartExWidth
        {
            get
            {
                return _StartExWidth;
            }
            set
            {
                _StartExWidth = value;
                RaisePropertyChanged("StartExWidth");
            }
        }

        public double StopExWidth
        {
            get
            {
                return _StopExWidth;
            }
            set
            {
                _StopExWidth = value;
                RaisePropertyChanged("StopExWidth");
            }
        }

        public double StartExHeight
        {
            get
            {
                return _StartExHeight;
            }
            set
            {
                _StartExHeight = value;
                RaisePropertyChanged("StartExHeight");
            }
        }

        public double StopExHeight
        {
            get
            {
                return _StopExHeight;
            }
            set
            {
                _StopExHeight = value;
                RaisePropertyChanged("StopExHeight");
            }
        }

        internal void UpdateSizes(double width, double height)
        {
            StartExWidth = width / 3;
            StartExHeight = height / 3;
            StopExWidth = width / 6;
            StopExHeight = height / 3;
        }

        private List<Projekte> _projekteListe;
        private List<Lohnkategorien> _lohnkategorienListe;
        private List<Tarifkategorien> _tarifkategorienListe;
        private Guid? _selectedProjekt;
        private string _selectedLohnkategorie;
        private string _selectedTarifkategorie;
        private string _username;
        private string _password;
        private bool _isUsernameEnabled;
        private bool _isCombosEnabled;
        private StartStop _current;
        private InitState _state;
        private Visibility _isAbmeldenVisible;
        private RelayCommand _abmeldenCommand;
        private Personal _user;
        private string _text;

        private enum InitState
        {
            Running,
            NotRunningLogged,
            NotRunningNotLogged,
            Paused
        }

        public MainViewModel()
        {
            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Elapsed += _timer_Elapsed; ;
            _timer.Interval = 30000;
            _intervalls = null;

            // Define command behavior
            StartCommand = new RelayCommand(() => StartAction());
            StartExCommand = new RelayCommand(() => StartExAction());
            StopCommand = new RelayCommand(() => StopAction());
            StopExCommand = new RelayCommand(() => StopExAction());
            PauseCommand = new RelayCommand(() => PauseAction());
            ResumeCommand = new RelayCommand(() => ResumeAction());
            AbmeldenCommand = new RelayCommand(() =>
            {
                Username = "";
                // Password = "";
                _user = null;
                GotoState(InitState.NotRunningNotLogged);
            });
            AbmeldenCommand.IsEnabled = true;

            // Init Comboboxes
            InitComboBoxes();

            // Set initial state
            GotoState(InitState.NotRunningNotLogged);
        }

        private void InitComboBoxes()
        {
            var db = new DialogTimeEntities();
            using (db)
            {
                ProjekteListe = new List<Projekte>();
                if (_user != null)
                {
                    db.Personals.Attach(_user);
                    var q1 = _user.MitarbeiterProjektes;
                    ProjekteListe = q1.ToList();
                }
                LohnkategorienListe = new List<Lohnkategorien>();
                var q3 = db.Tarifkategoriens;
                TarifkategorienListe = q3.ToList();
            }
        }

        private void LoadProjektComboBox()
        {
            var db = new DialogTimeEntities();
            using (db)
            {
                ProjekteListe = new List<Projekte>();
                if (_user != null)
                {
                    db.Personals.Attach(_user);
                    var q1 = _user.MitarbeiterProjektes.OrderBy(pr => pr.Bezeichnung);
                    ProjekteListe = q1.ToList();
                }
            }
        }

        private void BaseStartAction(DateTime time)
        {
            if (!PassValidation())
            {
                FeedbackText = "Bitte, alle nötige Daten eingeben";
                return;
            }
            var entities = new DialogTimeEntities();
            using (entities)
            {
                var entry = new StartStop();
                entry.PersId = _username;
                entry.ProjektId = _selectedProjekt.Value;
                entry.TimeIntervall = string.Format("{0:00}:{1:00}-", time.Hour, time.Minute);
                entry.LohnkategorieKuerzel = _selectedLohnkategorie;
                entry.TarifkategorieId = _selectedTarifkategorie;
                entry.Text = Text;
                entry.Datum = time;
                entities.StartStops.Add(entry);
                entities.SaveChanges();
                _intervalls = new Helpers.Intervalls(entry.TimeIntervall);
                GotoState(InitState.Running);
            }
        }

        private void StartAction()
        {
            BaseStartAction(DateTime.Now);
        }

        private void StartExAction()
        {
            DateTime time = DateTime.Now;
            var vm = new SelectTimeViewModel();
            vm.Datum = time;
            vm.Hour = time.Hour;
            vm.Minute = time.Minute;
            var dlg = new SelectTimeWindow(vm);
            var dr = dlg.ShowDialog();
            if (dr.HasValue && dr.Value)
            {
                time = vm.Datum;
                time = new DateTime(vm.Datum.Year, vm.Datum.Month, vm.Datum.Day, vm.Hour, vm.Minute, 0);
                BaseStartAction(time);
            }
        }

        private void BaseStopAction(DateTime time)
        {
            GotoState(InitState.NotRunningLogged);
            var entities = new DialogTimeEntities();
            using (entities)
            {
                var q = from x in entities.StartStops
                        where (x.TimeIntervall.EndsWith("-") || x.TimeIntervall.EndsWith("p")) && x.PersId == _user.PersId
                        select x;
                if (q.Any())
                {
                    if (q.Count() > 1)
                    {
                        // TODO: Szenario behandeln
                        // I'm stopping, but there are many pending activities
                        return;
                    }
                    var first = q.First();
                    if (first.TimeIntervall.EndsWith("p"))
                    {
                        first.TimeIntervall = first.TimeIntervall.Substring(0, first.TimeIntervall.Length - 1);
                    }
                    else
                    {
                        first.TimeIntervall += string.Format("{0:00}:{1:00}", time.Hour, time.Minute);
                    }
                    var ii = new Intervalls(first.TimeIntervall);
                    var jj = new Intervalls();
                    var q2 = from x in entities.RapportEintraeges
                             where x.PersId == _user.PersId && x.Datum == time
                             select x;
                    foreach (var e in q2)
                    {
                        jj.AddRange(new Intervalls(e.TimeIntervall));
                    }
                    ii.CutWith(jj);
                    RapportEintraege eintrag = new RapportEintraege()
                    {
                        Id = Guid.NewGuid(),
                        AnsatzExtern = 0,
                        AnsatzIntern = 0,
                        ArbeitsRapportNr = 0,
                        Aufwand = Math.Round(ii.EllapsedAsDouble, 1),
                        Datum = time,
                        ErfDatum = DateTime.Now,
                        ErfName = _user.PersId,
                        LohnkategorieKuerzel = first.LohnkategorieKuerzel,
                        LohnKatKontierung = "",
                        MandantId = Guid.Parse("331A58AF-C3F6-42BE-BF55-0AE0C5F26C87"),
                        MutDatum = DateTime.Now,
                        MutName = _user.PersId,
                        PersId = _user.PersId,
                        ProjektId = first.ProjektId,
                        TarifkategorieId = first.TarifkategorieId,
                        Text = first.Text,
                        TimeIntervall = ii.ToString(),
                        Verrechnet = 0,
                        Zuschlag = 0
                    };
                    entities.RapportEintraeges.Add(eintrag);
                    entities.SaveChanges();
                    _intervalls = new Helpers.Intervalls(first.TimeIntervall);
                    _timer_Elapsed(this, null);
                    GotoState(InitState.NotRunningLogged);
                }
            }
        }

        private void StopAction()
        {
            BaseStopAction(DateTime.Now);
        }

        private void StopExAction()
        {
            DateTime time = DateTime.Now;
            var vm = new SelectTimeViewModel();
            vm.Datum = time;
            vm.Hour = time.Hour;
            vm.Minute = time.Minute;
            var dlg = new SelectTimeWindow(vm);
            var dr = dlg.ShowDialog();
            if (dr.HasValue && dr.Value)
            {
                time = vm.Datum;
                time = new DateTime(vm.Datum.Year, vm.Datum.Month, vm.Datum.Day, vm.Hour, vm.Minute, 0);
                BaseStopAction(time);
            }
        }

        private void PauseAction()
        {
            GotoState(InitState.Paused);
            var entities = new DialogTimeEntities();
            using (entities)
            {
                var q = from x in entities.StartStops
                        where x.TimeIntervall.EndsWith("-") && x.PersId == _user.PersId
                        select x;
                if (q.Any())
                {
                    if (q.Count() > 1)
                    {
                        // TODO: Szenario behandeln
                        // I'm pausing, but there are many pending activities
                        return;
                    }
                    var first = q.First();
                    first.TimeIntervall += string.Format("{0:00}:{1:00}p", DateTime.Now.Hour, DateTime.Now.Minute);
                    entities.SaveChanges();
                    _intervalls = new Helpers.Intervalls(first.TimeIntervall);
                    _timer_Elapsed(this, null);
                    GotoState(InitState.Paused);
                }
            }
        }

        private void ResumeAction()
        {
            GotoState(InitState.NotRunningLogged);
            // Save state to database
            var entities = new DialogTimeEntities();
            using (entities)
            {
                var q = from x in entities.StartStops
                        where x.TimeIntervall.EndsWith("p") && x.PersId == _user.PersId
                        select x;
                if (q.Any())
                {
                    if (q.Count() > 1)
                    {
                        // TODO: Szenario behandeln
                        // I'm resuming, but there are many pending activities
                        return;
                    }
                    var first = q.First();
                    first.TimeIntervall = first.TimeIntervall.Substring(0, first.TimeIntervall.Length - 1) +
                        string.Format(",{0:00}:{1:00}-", DateTime.Now.Hour, DateTime.Now.Minute);
                    entities.SaveChanges();
                    _intervalls = new Helpers.Intervalls(first.TimeIntervall);
                    GotoState(InitState.Running);
                }
            }
        }

        private InitState InitAndGetState(DialogTimeEntities db)
        {
            // Restore state from database
            if (_user == null)
            {
                return InitState.NotRunningNotLogged;
            }
            var q = from x in db.StartStops
                    where (x.TimeIntervall.EndsWith("-") || x.TimeIntervall.EndsWith("p")) && x.PersId == _user.PersId
                    select x;
            if (q.Any())
            {
                if (q.Count() > 1)
                {
                    // TODO: Szenarion behandeln
                    // I find more than one pending activity!
                    return InitState.Running;
                }
                _current = q.First();
                if (_current.TimeIntervall.EndsWith("-"))
                {
                    return InitState.Running;
                }
                if (_current.TimeIntervall.EndsWith("p"))
                {
                    return InitState.Paused;
                }
            }
            _current = null;
            return InitState.NotRunningLogged;
        }

        private bool PassValidation()
        {
            if (_selectedProjekt == Guid.Empty ||
                string.IsNullOrEmpty(_selectedLohnkategorie) ||
                string.IsNullOrEmpty(_selectedTarifkategorie) ||
                string.IsNullOrEmpty(_text))
            {
                return false;
            }
            if (_state == InitState.NotRunningNotLogged)
            {
                if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                {
                    return false;
                }
                var db = new DialogTimeEntities();
                using (db)
                {
                    var md5 = MD5(_password);
                    var q = from x in db.Personals
                            where x.PersId == _username && x.Passwort == md5
                            select x;
                    if (q.Any())
                    {
                        // TODO: Set standard values for projekt, lohnkategotie and tarifkategorie based on user
                        _user = q.First();
                        SetLastSelections();
                        // Password = _stars;
                        return true;
                    }
                }
            }
            return true;
        }

        private void SetLastSelections()
        {
            var db = new DialogTimeEntities();
            using (db)
            {
                var r = from x in db.StartStops
                        where x.PersId == _user.PersId
                        orderby x.Id descending
                        select x;
                if (r.Any())
                {
                    var last = r.First();
                    SelectedProjekt = last.ProjektId;
                    // Now the list of Lohnkategorien should be reloaded
                    // then...
                    SelectedLohnkategorie = last.LohnkategorieKuerzel;
                    SelectedTarifkategorie = last.TarifkategorieId;
                    Text = last.Text;
                }
                else
                {
                    SelectedProjekt = _projekteListe.First().Id;
                    SelectedLohnkategorie = _lohnkategorienListe.First().Kuerzel;
                    SelectedTarifkategorie = _tarifkategorienListe.First().Id;
                    Text = "";
                }
            }
        }

        private Personal UserValidation()
        {
            if (string.IsNullOrEmpty(_username) ||
                string.IsNullOrEmpty(_password))
            {
                return null;
            }
            var db = new DialogTimeEntities();
            using (db)
            {
                var md5 = MD5(_password);
                var q = from x in db.Personals
                        where x.PersId == _username && x.Passwort == md5
                        select x;
                if (q.Any())
                {
                    return q.First();
                }
            }
            return null;
        }

        public static string MD5(string password)
        {
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(password);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public Guid? SelectedProjekt
        {
            get
            {
                return _selectedProjekt;
            }
            set
            {
                _selectedProjekt = value;
                LohnkategorienListe = new List<Lohnkategorien>();
                if (value != null || value == Guid.Empty)
                {
                    var db = new DialogTimeEntities();
                    using (db)
                    {
                        var projekt = db.Projektes.Find(value);
                        var q2 = from x in projekt.ProjektLohnkategorieZuordnungs
                                 select x.Lohnkategorien;
                        LohnkategorienListe = q2.OrderBy(lk => lk.Beschreibung).ToList();
                    }
                }
                RaisePropertyChanged("SelectedProjekt");
            }
        }

        public string SelectedLohnkategorie
        {
            get
            {
                return _selectedLohnkategorie;
            }
            set
            {
                _selectedLohnkategorie = value;
                RaisePropertyChanged("SelectedLohnkategorie");
            }
        }

        public string SelectedTarifkategorie
        {
            get
            {
                return _selectedTarifkategorie;
            }
            set
            {
                _selectedTarifkategorie = value;
                RaisePropertyChanged("SelectedTarifkategorie");
            }
        }

        public List<Projekte> ProjekteListe
        {
            get
            {
                return _projekteListe;
            }
            set
            {
                _projekteListe = value;
                RaisePropertyChanged("ProjekteListe");
            }
        }

        public List<Lohnkategorien> LohnkategorienListe
        {
            get
            {
                return _lohnkategorienListe;
            }
            set
            {
                _lohnkategorienListe = value;
                RaisePropertyChanged("LohnkategorienListe");
            }
        }

        public List<Tarifkategorien> TarifkategorienListe
        {
            get
            {
                return _tarifkategorienListe;
            }
            set
            {
                _tarifkategorienListe = value;
                RaisePropertyChanged("TarifkategorienListe");
            }
        }

        public Brush FeedbackColor
        {
            get
            {
                return _feedbackColor;
            }
            set
            {
                _feedbackColor = value;
                RaisePropertyChanged("FeedbackColor");
            }
        }

        public string FeedbackText
        {
            get
            {
                return _feedbackText;
            }

            set
            {
                _feedbackText = value;
                RaisePropertyChanged("FeedbackText");
            }
        }

        public RelayCommand StartCommand
        {
            get
            {
                return _startCommand;
            }

            set
            {
                _startCommand = value;
                RaisePropertyChanged("StartCommand");
            }
        }

        public RelayCommand StartExCommand
        {
            get
            {
                return _startExCommand;
            }

            set
            {
                _startExCommand = value;
                RaisePropertyChanged("StartExCommand");
            }
        }

        public RelayCommand StopCommand
        {
            get
            {
                return _stopCommand;
            }

            set
            {
                _stopCommand = value;
                RaisePropertyChanged("StopCommand");
            }
        }

        public RelayCommand StopExCommand
        {
            get
            {
                return _stopExCommand;
            }

            set
            {
                _stopExCommand = value;
                RaisePropertyChanged("StopExCommand");
            }
        }

        public RelayCommand PauseCommand
        {
            get
            {
                return _pauseCommand;
            }

            set
            {
                _pauseCommand = value;
                RaisePropertyChanged("PauseCommand");
            }
        }

        public RelayCommand ResumeCommand
        {
            get
            {
                return _resumeCommand;
            }

            set
            {
                _resumeCommand = value;
                RaisePropertyChanged("ResumeCommand");
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
                AfterLoginProcess();
                RaisePropertyChanged("Username");
            }
        }

        //public string Password
        //{
        //    get
        //    {
        //        return _password;
        //    }

        //    set
        //    {
        //        _password = value;
        //        AfterLoginProcess();
        //        RaisePropertyChanged("Password");
        //    }
        //}

        public bool IsUsernameEnabled
        {
            get
            {
                return _isUsernameEnabled;
            }

            set
            {
                _isUsernameEnabled = value;
                RaisePropertyChanged("IsUsernameEnabled");
            }
        }

        public bool IsCombosEnabled
        {
            get
            {
                return _isCombosEnabled;
            }

            set
            {
                _isCombosEnabled = value;
                RaisePropertyChanged("IsCombosEnabled");
            }
        }

        public Visibility IsAbmeldenVisible
        {
            get
            {
                return _isAbmeldenVisible;
            }

            set
            {
                _isAbmeldenVisible = value;
                RaisePropertyChanged("IsAbmeldenVisible");
            }
        }

        public RelayCommand AbmeldenCommand
        {
            get
            {
                return _abmeldenCommand;
            }

            set
            {
                _abmeldenCommand = value;
                RaisePropertyChanged("AbmeldenCommand");
            }
        }

        public Visibility StartCmdVisibility
        {
            get
            {
                return _startCmdVisibility;
            }

            set
            {
                _startCmdVisibility = value;
                RaisePropertyChanged("StartCmdVisibility");
            }
        }

        public Visibility StopCmdVisibility
        {
            get
            {
                return _stopCmdVisibility;
            }

            set
            {
                _stopCmdVisibility = value;
                RaisePropertyChanged("StopCmdVisibility");
            }
        }

        public Visibility PauseCmdVisibility
        {
            get
            {
                return _pauseCmdVisibility;
            }

            set
            {
                _pauseCmdVisibility = value;
                RaisePropertyChanged("PauseCmdVisibility");
            }
        }

        public Visibility ResumeCmdVisibility
        {
            get
            {
                return _resumeCmdVisibility;
            }

            set
            {
                _resumeCmdVisibility = value;
                RaisePropertyChanged("ResumeCmdVisibility");
            }
        }

        private void AfterLoginProcess()
        {
            // Validate Username / Password
            _user = UserValidation();
            if (_user == null)
            {
                return;
            }

            // Now we know the user
            LoadProjektComboBox();

            // Restore last selections
            SetLastSelections();

            // Restore status 
            var db = new DialogTimeEntities();
            using (db)
            {
                var state = InitAndGetState(db);
                switch (state)
                {
                    case InitState.Running:
                        _intervalls = new Intervalls(_current.TimeIntervall);
                        //SelectedProjekt = _current.ProjektId;
                        //SelectedLohnkategorie = _current.LohnkategorieKuerzel;
                        //SelectedTarifkategorie = _current.TarifkategorieId;
                        //Text = _current.Text;
                        GotoState(InitState.Running);
                        break;
                    case InitState.Paused:
                        var str = _current.TimeIntervall.Substring(0, _current.TimeIntervall.Length - 1);
                        _intervalls = new Intervalls(str);
                        //SelectedProjekt = _current.ProjektId;
                        //SelectedLohnkategorie = _current.LohnkategorieKuerzel;
                        //SelectedTarifkategorie = _current.TarifkategorieId;
                        //Text = _current.Text;
                        GotoState(InitState.Paused);
                        break;
                    default:
                        GotoState(state);
                        break;
                }
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;

            if (_intervalls == null)
            {
                return;
            }
            switch (_state)
            {
                case InitState.Running:
                    {
                        var msg = "";
                        TimeSpan ts = new TimeSpan(0, 0, 0);
                        for (var i = 0; i < _intervalls.Count - 1; i++)
                        {
                            msg += string.Format("{0}:  {1} Stunden {2} Minuten\n", _intervalls[i], _intervalls[i].Ellapsed.Hours, _intervalls[i].Ellapsed.Minutes);
                            ts = ts.Add(_intervalls[i].Ellapsed);
                        }
                        var last = new Intervall(_intervalls[_intervalls.Count - 1].ToString() + string.Format("{0:00}:{1:00}", now.Hour, now.Minute));
                        ts = ts.Add(last.Ellapsed);
                        msg += string.Format("{0}: {1} Stunden {2} Minuten\n", last, last.Ellapsed.Hours, last.Ellapsed.Minutes);
                        if (_intervalls.Count > 1)
                        {
                            msg += string.Format("TOTAL: {0} Stunden {1} Minuten", ts.Hours, ts.Minutes);
                        }
                        FeedbackText = msg;
                    }
                    break;
                case InitState.Paused:
                    {
                        var msg = "";
                        TimeSpan ts = new TimeSpan(0, 0, 0);
                        for (var i = 0; i < _intervalls.Count; i++)
                        {
                            msg += string.Format("{0}:  {1} Stunden {2} Minuten\n", _intervalls[i], _intervalls[i].Ellapsed.Hours, _intervalls[i].Ellapsed.Minutes);
                            ts = ts.Add(_intervalls[i].Ellapsed);
                        }
                        if (_intervalls.Count > 1)
                        {
                            msg += string.Format("TOTAL: {0} Stunden {1} Minuten", ts.Hours, ts.Minutes);
                        }
                        FeedbackText = msg;
                    }
                    break;
                default:
                    FeedbackText = "";
                    break;
            }
        }

        public string UserData
        {
            get
            {
                return _user == null ? "" : string.Format("{1} {0}", _user.Nachname, _user.Vorname);
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
                RaisePropertyChanged("Text");
            }
        }

        private void GotoState(InitState state)
        {
            _state = state;
            switch (state)
            {
                case InitState.Running:
                    StartCommand.IsEnabled = false;
                    StartExCommand.IsEnabled = false;
                    StopCommand.IsEnabled = true;
                    StopExCommand.IsEnabled = true;
                    PauseCommand.IsEnabled = true;
                    ResumeCommand.IsEnabled = false;
                    StartCmdVisibility = Visibility.Hidden;
                    StopCmdVisibility = Visibility.Visible;
                    PauseCmdVisibility = Visibility.Visible;
                    ResumeCmdVisibility = Visibility.Hidden;
                    IsUsernameEnabled = false;
                    IsCombosEnabled = false;
                    FeedbackColor = _black;
                    FeedbackText = string.Format("Restarting...");
                    _timer.Enabled = true;
                    _timer_Elapsed(this, null);
                    IsAbmeldenVisible = Visibility.Hidden;
                    break;
                case InitState.NotRunningLogged:
                    StartCommand.IsEnabled = true;
                    StartExCommand.IsEnabled = true;
                    StopCommand.IsEnabled = false;
                    StopExCommand.IsEnabled = false;
                    PauseCommand.IsEnabled = false;
                    ResumeCommand.IsEnabled = false;
                    StartCmdVisibility = Visibility.Visible;
                    StopCmdVisibility = Visibility.Hidden;
                    PauseCmdVisibility = Visibility.Hidden;
                    ResumeCmdVisibility = Visibility.Hidden;
                    IsUsernameEnabled = true;
                    IsCombosEnabled = true;
                    FeedbackColor = _red;
                    IsAbmeldenVisible = Visibility.Visible;
                    _timer.Enabled = false;
                    break;
                case InitState.NotRunningNotLogged:
                    StartCommand.IsEnabled = false;
                    StartExCommand.IsEnabled = false;
                    StopCommand.IsEnabled = false;
                    StopExCommand.IsEnabled = false;
                    PauseCommand.IsEnabled = false;
                    ResumeCommand.IsEnabled = false;
                    StartCmdVisibility = Visibility.Hidden;
                    StopCmdVisibility = Visibility.Hidden;
                    PauseCmdVisibility = Visibility.Hidden;
                    ResumeCmdVisibility = Visibility.Hidden;
                    IsUsernameEnabled = true;
                    IsCombosEnabled = false;
                    FeedbackText = "";
                    FeedbackColor = _red;
                    IsAbmeldenVisible = Visibility.Hidden;
                    _timer.Enabled = false;
                    SelectedProjekt = null;
                    SelectedLohnkategorie = null;
                    SelectedTarifkategorie = null;
                    Text = null;
                    break;
                case InitState.Paused:
                    StartCommand.IsEnabled = false;
                    StartExCommand.IsEnabled = false;
                    StopCommand.IsEnabled = true;
                    StopExCommand.IsEnabled = true;
                    PauseCommand.IsEnabled = false;
                    ResumeCommand.IsEnabled = true;
                    StartCmdVisibility = Visibility.Hidden;
                    StopCmdVisibility = Visibility.Visible;
                    PauseCmdVisibility = Visibility.Hidden;
                    ResumeCmdVisibility = Visibility.Visible;
                    IsUsernameEnabled = true;
                    IsCombosEnabled = false;
                    FeedbackColor = _red;
                    IsAbmeldenVisible = Visibility.Hidden;
                    _timer.Enabled = false;
                    //SelectedProjekt = null;
                    //SelectedLohnkategorie = null;
                    //SelectedTarifkategorie = null;
                    //Text = null;
                    _timer_Elapsed(null, null);
                    break;
            }
            RaisePropertyChanged("UserData");
        }

        internal void SetPassword(string password)
        {
            _password = password;
            AfterLoginProcess();
        }
    }
}
