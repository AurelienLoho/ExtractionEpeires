namespace ExtractionEpeires
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Gère les requêtes vers EPEIRES.
    /// </summary>
    public class EpeiresConnexion : IDisposable
    {
        /// <summary>
        /// L'URL d'EPEIRES.
        /// </summary>
        private string _baseUrl;

        /// <summary>
        /// Le nom d'utilisateur.
        /// </summary>
        private string _userName;

        /// <summary>
        /// Le mot de passe.
        /// </summary>
        private string _password;

        /// <summary>
        /// Le gestionnaire de requêtes HTTP.
        /// </summary>
        private readonly HttpRequest http = new HttpRequest("epeires.cdg-lb.aviation");

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="EpeiresConnexion"/>.
        /// </summary>
        /// <param name="baseUrl">L'URL du dépôt.</param>
        /// <param name="username">Le nom d'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        public EpeiresConnexion(string baseUrl, string userName, string password)
        {
            _baseUrl = baseUrl;
            _userName = userName;
            _password = password;

            var succees = Connect();

            if (!succees)
            {
                throw new ArgumentException("Impossible de se connecter à EPEIRES.");
            }
        }

        /// <summary>
        /// Connexion au dépôt.
        /// </summary>
        /// <returns>true si la connexion a réussie, false autrement.</returns>
        private bool Connect()
        {
            try
            {
                var postData = string.Format("identity={0}&credential={1}&redirect=application&submit=", _userName, _password);

                http.SendPostRequest(this._baseUrl + "/user/login?redirect=application", postData).Discard();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Déconnexion du dépôt.
        /// </summary>
        /// <returns>true si la déconnexion a réussie, false autrement.</returns>
        private bool Disconnect()
        {
            try
            {
                http.SendGetRequest(this._baseUrl + "/user/logout?redirect=/").Discard();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Récupère les évènements enregistrés dans le dépôt pendant la période considérée.
        /// </summary>
        /// <param name="periodStart">Le début de la période.</param>
        /// <param name="periodEnd">La fin de la période.</param>
        /// <returns></returns>
        public List<EpeiresEvent> GetEvents(DateTime periodStart, DateTime periodEnd)
        {
            var evts = new List<RawEpeiresEvent>();

            foreach (var day in periodStart.EachDayTo(periodEnd))
            {
                var url = string.Format("{0}/events/getevents?day={1}", this._baseUrl, day.ToString("yyyy-MM-dd"));

                using (var response = http.SendGetRequest(url))
                {
                    var data = response.AsJson<Dictionary<int, RawEpeiresEvent>>();

                    if (data != null)
                    {
                        evts.AddRange(data.Values);
                    }
                }
            }

            return evts.Distinct().Select(x => ToEvent(x)).ToList();
        }

        private static EpeiresEvent ToEvent(RawEpeiresEvent evt)
        {

            var fieldArray = evt.Fields.Values.ToArray();

            return new EpeiresEvent
            {
                StartDate = evt.StartDate,
                EndDate = evt.EndDate,
                Title = fieldArray.Length >= 1 ? fieldArray[0] : string.Empty,
                Description = fieldArray.Length >= 2 ? fieldArray[1] : string.Empty
            };
        }

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés).
                    Disconnect();
                }

                // TODO: libérer les ressources non managées (objets non managés) et remplacer un finaliseur ci-dessous.
                // TODO: définir les champs de grande taille avec la valeur Null.

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~EpeiresRepository() {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
