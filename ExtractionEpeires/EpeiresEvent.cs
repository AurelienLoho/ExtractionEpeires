using System;

namespace ExtractionEpeires
{
    /// <summary>
    /// Données sur un évènement reporté par le service exploitation (via EPEIRES)
    /// </summary>
    public sealed class EpeiresEvent
    {
        /// <summary>
        /// Date de début de l'évènement.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date de fin de l'évènement (si fini).
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Durée de l'évènement (si applicable)
        /// </summary>
        public TimeSpan? Duration
        {
            get { return EndDate.HasValue ? EndDate - StartDate : null; }
        }

        /// <summary>
        /// Titre de l'évènement.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description de l'évènement.
        /// </summary>
        public string Description { get; set; }
    }
}
