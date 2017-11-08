namespace ExtractionEpeires
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Déclenche l'évènement <see cref="RaisePropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Le nom de de la propriété modifiée.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Evènement déclenché lorsque la valeur d'une propriété de l'objet change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Méthode utilitaire pour implémenter l'interface <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <typeparam name="T">Le type de données du champ à modifier.</typeparam>
        /// <param name="storage">Le champ à modifier.</param>
        /// <param name="value">La nouvelle valeur du champ.</param>
        /// <param name="propertyName">Le nom de la propriété associé au champ.</param>
        /// <returns>true si le champ a été modifié, false autrement.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
