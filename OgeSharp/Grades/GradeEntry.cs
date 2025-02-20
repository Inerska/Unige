﻿using System.Collections.Generic;

namespace OgeSharp {

    /// <summary>
    /// Represents an entry for the grades<br/>
    /// (This can represent a grade by itself as well as a folder of entries see <see cref="IsFolder"/>)
    /// </summary>
    public class GradeEntry {

        /// <summary>
        /// Determines the normalized max grade value<br/>
        /// (Used for <see cref="NormalizedGrade"/>)
        /// </summary>
        public const double NormalizedValue = 20;

        /// <summary>
        /// Creates a folder of entries with it's name and it's coefficient
        /// </summary>
        internal GradeEntry(string name, double coefficient) {

            // Save the name and coefficient
            Name = name;
            Coefficient = coefficient;

            // Initialize the child entries list
            Entries = new List<GradeEntry>();

        }

        /// <summary>
        /// Creates a grade with it's grade, it's max grade and it's coefficient
        /// </summary>
        internal GradeEntry(double grade, double maxGrade, double coefficient) {

            // Save the grade, maxGrade and coefficient
            Grade = grade;
            MaxGrade = maxGrade;
            Coefficient = coefficient;

        }

        #region Folder

        /// <summary>
        /// Determines if the entry is a folder of entries
        /// </summary>
        public bool IsFolder => Name != null;

        /// <summary>
        /// Name of the grade's folder
        /// </summary>
        public string Name;

        /// <summary>
        /// List containing all the child entries of the current one<br/>
        /// (Equals null if the entry is not a folder)
        /// </summary>
        public List<GradeEntry> Entries { get; }

        #endregion

        /// <summary>
        /// Represents the normalized entry's grade to <see cref="NormalizedValue"/> (its grade is adjusted to have a maximum of <see cref="NormalizedValue"/>)<br/>
        /// (Only available for non-folder entries)
        /// </summary>
        public double NormalizedGrade => _Grade * NormalizedValue / MaxGrade;

        // Represents the cached value of the grade
        // => Not necessarily set if the entry is a folder
        private double _Grade = double.NaN;

        /// <summary>
        /// Gets the actual grade if it's not a folder entry<br/>
        /// Gets the average of all the childs grades if it's a folder (this is done recursively to the bottom of the tree)
        /// </summary>
        public double Grade {

            get {

                // If it's not a folder then simply return the value
                if (!IsFolder) return _Grade;

                // If the folder already have a cached grade
                if (!double.IsNaN(_Grade)) return _Grade;

                // Loop recursively through the entries and return the grade
                return GetGradesRecursively();

            }
            set {

                // If it's a folder then don't do anything
                if (IsFolder) return;

                // Save the grade
                _Grade = value;

            }

        }
        public double MaxGrade { get; }
        public double Coefficient { get; }

        /// <summary>
        /// Returns the normalized grade if it's a grade entry<br/>
        /// Returns the average of all the childs grades if it's a folder 
        /// </summary>
        internal double GetGradesRecursively() {

            // If it's not a folder return the normalized grade
            // => All the grades are normalized for the average, else it will be wrong
            if (!IsFolder) return NormalizedGrade;

            // If the folder do not have any grade
            if (Entries.Count <= 0) return double.NaN;

            // Initialize the grades sum and count (takes in account the coefficients)
            double gradesSum = 0;
            double gradesCount = 0;

            // Loop through the child entries
            foreach (GradeEntry entry in Entries) {

                // Get the child entry grade
                double grade = entry.GetGradesRecursively();

                // If the folder do not have any grade then we don't take it in account
                if (double.IsNaN(grade)) continue;

                // Increase the grades count
                gradesSum += grade * entry.Coefficient;
                gradesCount += entry.Coefficient;

            }

            // Cache the grade's value
            // => To prevent having to run again this whole loop if the user gets back the average
            _Grade = gradesSum / gradesCount;
            return _Grade;

        }

    }

}
