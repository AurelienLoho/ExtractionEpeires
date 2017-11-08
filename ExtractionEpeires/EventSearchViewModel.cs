using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CsvHelper;
using System.IO;

namespace ExtractionEpeires
{
    public sealed class EventSearchViewModel : ViewModelBase
    {  
        /// <summary>
        /// L'URL d'EPEIRES.
        /// </summary>
        private const string EPEIRES_URL = "http://epeires.cdg-lb.aviation";

        /// <summary>
        /// Le nom d'utilisateur pour EPEIRES.
        /// </summary>
        private const string EPEIRES_USERNAME = "supervision";

        /// <summary>
        /// Le mot de passe pour EPEIRES.
        /// </summary>
        private const string EPEIRES_PASSWORD = "epeires";

        private BackgroundWorker _worker;

        public EventSearchViewModel()
        {
            _worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = false };
            _worker.DoWork += DoWork;
            //_worker.ProgressChanged += OnProgressChanged;

            StartSearchCommand = new RelayCommand(_ => _worker.RunWorkerAsync(), _ => !_worker.IsBusy);
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        public ICommand StartSearchCommand { get; private set; }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            List<EpeiresEvent> events = new List<EpeiresEvent>();

            // interrogation d'épeires

            using (var epeiresConnexion = new EpeiresConnexion(EPEIRES_URL, EPEIRES_USERNAME, EPEIRES_PASSWORD))
            {
                events = epeiresConnexion.GetEvents(StartDate, EndDate);                
            }

            // Filtrage

            if(!string.IsNullOrWhiteSpace(_searchText))
            {
                events = events.Where(x => x.Title.Contains(_searchText)).ToList();
            }

            // Sauvegarde dans fichier csv

            using (var outputStream = new StreamWriter("résultat.csv"))
            {
                var csv = new CsvWriter(outputStream);

                csv.WriteRecords(events);
            }
        }
    }
}
